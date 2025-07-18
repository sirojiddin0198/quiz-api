namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Api.Contracts.Reviews;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Services;
using Quiz.Shared.Authentication;
using Quiz.Shared.Common;
using System.Text.Json;

public sealed class ResultsService(
    ICSharpRepository repository,
    ICurrentUser currentUser,
    IAnswerValidator answerValidator) : IResultsService
{
    // Internal metadata classes for deserialization
    private class QuestionMetadataBase
    {
        public string? CodeBefore { get; set; }
        public string? CodeAfter { get; set; }
        public List<QuestionHintData> Hints { get; set; } = [];
        public string? Explanation { get; set; }
    }

    private class MCQMetadata : QuestionMetadataBase
    {
        public List<MCQOptionData> Options { get; set; } = [];
        public List<string> CorrectAnswerIds { get; set; } = [];
    }

    private class TrueFalseMetadata : QuestionMetadataBase
    {
        public bool CorrectAnswer { get; set; }
    }

    private class FillMetadata : QuestionMetadataBase
    {
        public string? CodeWithBlank { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    private class ErrorSpottingMetadata : QuestionMetadataBase
    {
        public string? CodeWithError { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    private class OutputPredictionMetadata : QuestionMetadataBase
    {
        public string? Snippet { get; set; }
        public string ExpectedOutput { get; set; } = string.Empty;
    }

    private class CodeWritingMetadata : QuestionMetadataBase
    {
        public string? Solution { get; set; }
        public List<string> Examples { get; set; } = [];
        public List<TestCaseData> TestCases { get; set; } = [];
    }

    private class MCQOptionData
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    private class QuestionHintData
    {
        public string Hint { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }

    private class TestCaseData
    {
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
    }

    public async Task<Result<CollectionResultsResponse>> GetCollectionResultsAsync(
        int collectionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(currentUser.UserId))
            return Result<CollectionResultsResponse>.Failure("User is not authenticated");

        var collection = await repository.GetCollectionByIdAsync(collectionId, cancellationToken);
        if (collection == null)
            return Result<CollectionResultsResponse>.Failure("Collection not found");

        var progress = await repository.GetUserProgressAsync(currentUser.UserId, collectionId, cancellationToken);
        if (progress == null)
            return Result<CollectionResultsResponse>.Failure("No progress found for this collection");

        // Get all user answers for time calculation
        var questions = await repository.GetQuestionsByCollectionAsync(collectionId, 1, int.MaxValue, cancellationToken);
        var totalTimeSpent = TimeSpan.Zero;
        
        foreach (var question in questions.Items)
        {
            var answer = await repository.GetLatestAnswerAsync(currentUser.UserId, question.Id, cancellationToken);
            if (answer != null)
            {
                totalTimeSpent = totalTimeSpent.Add(TimeSpan.FromSeconds(answer.TimeSpentSeconds));
            }
        }

        var response = new CollectionResultsResponse
        {
            CollectionId = collectionId,
            CollectionName = collection.Title,
            TotalQuestions = progress.TotalQuestions,
            AnsweredQuestions = progress.AnsweredQuestions,
            CorrectAnswers = progress.CorrectAnswers,
            ScorePercentage = progress.SuccessRate,
            TotalTimeSpent = totalTimeSpent,
            CompletedAt = progress.LastAnsweredAt
        };

        return Result<CollectionResultsResponse>.Success(response);
    }

    public async Task<Result<List<QuestionReviewResponse>>> GetAnswerReviewAsync(
        int collectionId,
        bool includeUnanswered = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(currentUser.UserId))
            return Result<List<QuestionReviewResponse>>.Failure("User is not authenticated");

        var questions = await repository.GetQuestionsByCollectionAsync(collectionId, 1, int.MaxValue, cancellationToken);
        var reviewResponses = new List<QuestionReviewResponse>();

        foreach (var question in questions.Items)
        {
            var userAnswer = await repository.GetLatestAnswerAsync(currentUser.UserId, question.Id, cancellationToken);
            
            if (userAnswer == null && !includeUnanswered)
                continue;

            var reviewResponse = BuildQuestionReview(question, userAnswer);
            reviewResponses.Add(reviewResponse);
        }

        return Result<List<QuestionReviewResponse>>.Success(reviewResponses);
    }

    public async Task<Result<SessionResultsResponse>> CompleteSessionAsync(
        string sessionId,
        CompleteSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var reviewResponses = new List<QuestionReviewResponse>();
        var correctCount = 0;

        foreach (var answer in request.Answers)
        {
            var question = await repository.GetQuestionByIdAsync(answer.QuestionId, cancellationToken);
            if (question == null)
                continue;

            var isCorrect = await answerValidator.ValidateAnswerAsync(question, answer.Answer, cancellationToken);
            if (isCorrect)
                correctCount++;

            var userAnswerReview = new UserAnswerReview
            {
                Answer = answer.Answer,
                IsCorrect = isCorrect,
                SubmittedAt = DateTime.UtcNow,
                TimeSpentSeconds = answer.TimeSpentSeconds
            };

            var reviewResponse = BuildQuestionReview(question, userAnswerReview);
            reviewResponses.Add(reviewResponse);
        }

        var scorePercentage = request.Answers.Count > 0 
            ? (decimal)correctCount / request.Answers.Count * 100 
            : 0;

        var response = new SessionResultsResponse
        {
            SessionId = sessionId,
            TotalQuestions = request.Answers.Count,
            CorrectAnswers = correctCount,
            ScorePercentage = Math.Round(scorePercentage, 2),
            ReviewItems = reviewResponses
        };

        return Result<SessionResultsResponse>.Success(response);
    }

    private QuestionReviewResponse BuildQuestionReview(Question question, UserAnswer? userAnswer)
    {
        UserAnswerReview? userAnswerReview = null;
        if (userAnswer != null)
        {
            userAnswerReview = new UserAnswerReview
            {
                Answer = userAnswer.Answer,
                IsCorrect = userAnswer.IsCorrect,
                SubmittedAt = userAnswer.SubmittedAt,
                TimeSpentSeconds = userAnswer.TimeSpentSeconds
            };
        }

        return BuildQuestionReview(question, userAnswerReview);
    }

    private QuestionReviewResponse BuildQuestionReview(Question question, UserAnswerReview? userAnswerReview)
    {
        var metadata = JsonSerializer.Deserialize<QuestionMetadataBase>(question.Metadata);
        
        var content = new QuestionContentReview
        {
            CodeBefore = metadata?.CodeBefore,
            CodeAfter = metadata?.CodeAfter,
            CodeWithBlank = GetCodeWithBlank(question),
            CodeWithError = GetCodeWithError(question),
            Snippet = GetSnippet(question)
        };

        var correctAnswer = BuildCorrectAnswer(question);
        
        var hints = metadata?.Hints?
            .OrderBy(h => h.OrderIndex)
            .Select(h => h.Hint)
            .ToList();

        return new QuestionReviewResponse
        {
            QuestionId = question.Id,
            QuestionType = GetQuestionType(question),
            Prompt = question.Prompt,
            Content = content,
            UserAnswer = userAnswerReview,
            CorrectAnswer = correctAnswer,
            Explanation = metadata?.Explanation,
            Hints = hints
        };
    }

    private CorrectAnswerReview BuildCorrectAnswer(Question question)
    {
        return question switch
        {
            MCQQuestion => BuildMCQCorrectAnswer(question),
            TrueFalseQuestion => BuildTrueFalseCorrectAnswer(question),
            FillQuestion => BuildFillCorrectAnswer(question),
            ErrorSpottingQuestion => BuildErrorSpottingCorrectAnswer(question),
            OutputPredictionQuestion => BuildOutputPredictionCorrectAnswer(question),
            CodeWritingQuestion => BuildCodeWritingCorrectAnswer(question),
            _ => new CorrectAnswerReview()
        };
    }

    private CorrectAnswerReview BuildMCQCorrectAnswer(Question question)
    {
        var metadata = JsonSerializer.Deserialize<MCQMetadata>(question.Metadata);
        var options = metadata?.Options.Select(o => new MCQCorrectOption
        {
            Id = o.Id,
            Text = o.Text,
            IsCorrect = o.IsCorrect
        }).ToList();

        return new CorrectAnswerReview { Options = options };
    }

    private CorrectAnswerReview BuildTrueFalseCorrectAnswer(Question question)
    {
        var metadata = JsonSerializer.Deserialize<TrueFalseMetadata>(question.Metadata);
        return new CorrectAnswerReview { BooleanAnswer = metadata?.CorrectAnswer };
    }

    private CorrectAnswerReview BuildFillCorrectAnswer(Question question)
    {
        var metadata = JsonSerializer.Deserialize<FillMetadata>(question.Metadata);
        return new CorrectAnswerReview { TextAnswer = metadata?.CorrectAnswer };
    }

    private CorrectAnswerReview BuildErrorSpottingCorrectAnswer(Question question)
    {
        var metadata = JsonSerializer.Deserialize<ErrorSpottingMetadata>(question.Metadata);
        return new CorrectAnswerReview { TextAnswer = metadata?.CorrectAnswer };
    }

    private CorrectAnswerReview BuildOutputPredictionCorrectAnswer(Question question)
    {
        var metadata = JsonSerializer.Deserialize<OutputPredictionMetadata>(question.Metadata);
        return new CorrectAnswerReview { TextAnswer = metadata?.ExpectedOutput };
    }

    private CorrectAnswerReview BuildCodeWritingCorrectAnswer(Question question)
    {
        var metadata = JsonSerializer.Deserialize<CodeWritingMetadata>(question.Metadata);
        var testCaseResults = metadata?.TestCases.Select(tc => new TestCaseResult
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput,
            UserOutput = null, // Would need to run tests to get this
            Passed = false
        }).ToList();

        return new CorrectAnswerReview
        {
            SampleSolution = metadata?.Solution,
            TestCaseResults = testCaseResults
        };
    }

    private string GetQuestionType(Question question)
    {
        return question switch
        {
            MCQQuestion => "MCQ",
            TrueFalseQuestion => "TrueFalse",
            FillQuestion => "Fill",
            ErrorSpottingQuestion => "ErrorSpotting",
            OutputPredictionQuestion => "OutputPrediction",
            CodeWritingQuestion => "CodeWriting",
            _ => "Unknown"
        };
    }

    private string? GetCodeWithBlank(Question question)
    {
        if (question is not FillQuestion) return null;
        var metadata = JsonSerializer.Deserialize<FillMetadata>(question.Metadata);
        return metadata?.CodeWithBlank;
    }

    private string? GetCodeWithError(Question question)
    {
        if (question is not ErrorSpottingQuestion) return null;
        var metadata = JsonSerializer.Deserialize<ErrorSpottingMetadata>(question.Metadata);
        return metadata?.CodeWithError;
    }

    private string? GetSnippet(Question question)
    {
        if (question is not OutputPredictionQuestion) return null;
        var metadata = JsonSerializer.Deserialize<OutputPredictionMetadata>(question.Metadata);
        return metadata?.Snippet;
    }
} 