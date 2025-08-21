namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Api.Contracts.Reviews;
using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.Repositories.Abstractions;
using Quiz.Shared.Authentication;
using Quiz.Shared.Common;
using System.Text.Json;
using Quiz.CSharp.Api.Services.Abstractions;
using Quiz.Infrastructure.Exceptions;

public sealed class ResultsService(
    IAnswerRepository answerRepository,
    IQuestionRepository questionRepository,
    ICurrentUser currentUser,
    IAnswerValidator answerValidator) : IResultsService
{
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
        public string? CorrectAnswer { get; set; }
    }

    private class ErrorSpottingMetadata : QuestionMetadataBase
    {
        public string? CodeWithError { get; set; }
        public string? CorrectAnswer { get; set; }
    }

    private class OutputPredictionMetadata : QuestionMetadataBase
    {
        public string? Snippet { get; set; }
        public string? ExpectedOutput { get; set; }
    }

    private class CodeWritingMetadata : QuestionMetadataBase
    {
        public string? Solution { get; set; }
        public List<string> Examples { get; set; } = [];
        public List<TestCaseData> TestCases { get; set; } = [];
    }

    private class MCQOptionData
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    private class QuestionHintData
    {
        public string? Hint { get; set; }
        public int OrderIndex { get; set; }
    }

    private class TestCaseData
    {
        public string? Input { get; set; }
        public string? ExpectedOutput { get; set; }
    }

    public async Task<List<QuestionReviewResponse>> GetAnswerReviewAsync(
        int collectionId,
        bool includeUnanswered = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(currentUser.UserId))
            throw new CustomUnauthorizedException("User is not authenticated");

        var questions = await questionRepository.GetQuestionsByCollectionAsync(
            collectionId,
            1,
            int.MaxValue,
            cancellationToken);
        var reviewResponses = new List<QuestionReviewResponse>();

        foreach (var question in questions.Items)
        {
            var userAnswer = await answerRepository.GetLatestAnswerOrDefaultAsync(
                currentUser.UserId,
                question.Id,
                cancellationToken);
            
            if (userAnswer == null && !includeUnanswered)
                continue;

            var reviewResponse = BuildQuestionReview(question, userAnswer);
            reviewResponses.Add(reviewResponse);
        }

        return reviewResponses;
    }

    public async Task<SessionResultsResponse> CompleteSessionAsync(
        string sessionId,
        CompleteSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new CustomBadRequestException("Session ID is required");

        if (request?.Answers == null || request.Answers.Count == 0)
            throw new CustomBadRequestException("Answers are required to complete the session");

        var reviewResponses = new List<QuestionReviewResponse>();
        var correctCount = 0;

        foreach (var answer in request.Answers)
        {
            var question = await questionRepository.GetSingleOrDefaultAsync(answer.QuestionId, cancellationToken);
            if (question == null) continue;

            var isCorrect = await answerValidator.ValidateAnswerAsync(
                question,
                answer.Answer,
                cancellationToken);
            if (isCorrect) correctCount++;

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

        return response;
    }

    private QuestionReviewResponse BuildQuestionReview(Question question, UserAnswer? userAnswer)
    {
        UserAnswerReview? userAnswerReview = null;
        if (userAnswer is not null)
            userAnswerReview = new UserAnswerReview
            {
                Answer = userAnswer.Answer,
                IsCorrect = userAnswer.IsCorrect,
                SubmittedAt = userAnswer.SubmittedAt,
                TimeSpentSeconds = userAnswer.TimeSpentSeconds
            };

        return BuildQuestionReview(question, userAnswerReview);
    }

    private QuestionReviewResponse BuildQuestionReview(Question question, UserAnswerReview? userAnswerReview)
    {
        var metadata = JsonSerializer.Deserialize<QuestionMetadataBase>(question.Metadata);
        
        var content = new QuestionContentReview
        {
            CodeBefore = string.IsNullOrWhiteSpace(metadata?.CodeBefore) ? null : metadata.CodeBefore,
            CodeAfter = string.IsNullOrWhiteSpace(metadata?.CodeAfter) ? null : metadata.CodeAfter,
            CodeWithBlank = GetCodeWithBlank(question),
            CodeWithError = GetCodeWithError(question),
            Snippet = GetSnippet(question)
        };

        var correctAnswer = BuildCorrectAnswer(question);
        
        var hints = metadata?.Hints?
            .Where(h => !string.IsNullOrWhiteSpace(h.Hint))
            .OrderBy(h => h.OrderIndex)
            .Select(h => h.Hint!)
            .ToList();

        return new QuestionReviewResponse
        {
            QuestionId = question.Id,
            QuestionType = GetQuestionType(question),
            Prompt = question.Prompt,
            Content = content,
            UserAnswer = userAnswerReview,
            CorrectAnswer = correctAnswer,
            Explanation = string.IsNullOrWhiteSpace(metadata?.Explanation) ? null : metadata.Explanation,
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
        var options = metadata?.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.Id) && !string.IsNullOrWhiteSpace(o.Text))
            .Select(o => new MCQCorrectOption
            {
                Id = o.Id ?? string.Empty,
                Text = o.Text ?? string.Empty,
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
        var testCaseResults = metadata?.TestCases
            .Where(tc => !string.IsNullOrWhiteSpace(tc.Input) && !string.IsNullOrWhiteSpace(tc.ExpectedOutput))
            .Select(tc => new TestCaseResult
            {
                Input = tc.Input ?? string.Empty,
                ExpectedOutput = tc.ExpectedOutput ?? string.Empty,
                UserOutput = null, 
                Passed = false
            }).ToList();

        return new CorrectAnswerReview
        {
            SampleSolution = string.IsNullOrWhiteSpace(metadata?.Solution) ? null : metadata.Solution,
            TestCaseResults = testCaseResults
        };
    }

    private string GetQuestionType(Question question)
        => question switch
        {
            MCQQuestion => "MCQ",
            TrueFalseQuestion => "TrueFalse",
            FillQuestion => "Fill",
            ErrorSpottingQuestion => "ErrorSpotting",
            OutputPredictionQuestion => "OutputPrediction",
            CodeWritingQuestion => "CodeWriting",
            _ => "Unknown"
        };

    private string? GetCodeWithBlank(Question question)
    {
        if (question is not FillQuestion) return null;

        var metadata = JsonSerializer.Deserialize<FillMetadata>(question.Metadata);
        var codeWithBlank = metadata?.CodeWithBlank;

        return string.IsNullOrWhiteSpace(codeWithBlank) ? null : codeWithBlank;
    }

    private string? GetCodeWithError(Question question)
    {
        if (question is not ErrorSpottingQuestion) return null;

        var metadata = JsonSerializer.Deserialize<ErrorSpottingMetadata>(question.Metadata);
        var codeWithError = metadata?.CodeWithError;

        return string.IsNullOrWhiteSpace(codeWithError) ? null : codeWithError;
    }

    private string? GetSnippet(Question question)
    {
        if (question is not OutputPredictionQuestion) return null;

        var metadata = JsonSerializer.Deserialize<OutputPredictionMetadata>(question.Metadata);
        var snippet = metadata?.Snippet;
        
        return string.IsNullOrWhiteSpace(snippet) ? null : snippet;
    }
} 