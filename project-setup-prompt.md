# 🚀 **Simplified Master Prompt for .NET 8 Modular Monolith Quiz API**

## **Project Architecture Overview**

Create a **modular monolith** .NET 8 Web API application with the following structure:

```
QuizPlatform/
├── src/
│   ├── Quiz.Api/                          # Main API host (collects all modules)
│   ├── Quiz.CSharp.Api/                   # C# Quiz module API
│   ├── Quiz.CSharp.Data/                  # C# Quiz module data layer
│   ├── Quiz.Shared/                       # Shared contracts and utilities
│   └── Quiz.Infrastructure/               # Shared infrastructure (auth, etc.)
└── docs/
    └── api-documentation.md
```

## **Technical Requirements**

### **Core Technologies**
- **.NET 8** Web API with minimal APIs where appropriate
- **C# 12+** features and best practices
- **Entity Framework Core 8** with SQL Server
- **Keycloak JWT Bearer** authentication (token validation only)
- **Swagger/OpenAPI** documentation
- **Clean Architecture** within each module
- **AutoMapper** for object mapping
- **FluentValidation** for input validation
- **Built-in logging** (ILogger)

### **C# 12+ Requirements & Best Practices**
- Use **file-scoped namespaces** throughout
- Implement **primary constructors** for dependency injection
- Utilize **collection expressions** `[item1, item2]` instead of `new[]`
- Apply **required members** and **init-only setters**
- Use **raw string literals** for SQL queries and long strings
- Implement **generic attributes** where applicable
- Use **ref readonly** for large structs
- Apply **pattern matching** and **switch expressions**
- Use **global using** statements for common imports
- Implement **nullable reference types** with proper annotations
- Use **record types** for DTOs and value objects
- Apply **interpolated string handlers** for performance
- Use **CallerArgumentExpression** for better error messages
- Implement **UTF-8 string literals** for API responses
- Use **checked** operators for overflow detection
- Apply **list patterns** for collection matching

### **Authentication Requirements**
- **Keycloak SSO** integration (token validation only)
- **JWT Bearer** token authentication
- **No user management** in backend (handled by Keycloak)
- **Claims-based** authorization
- **Stateless** authentication
- **Token validation** middleware

### **API Design Guidelines**
- **Modular routing**: `/api/csharp/<endpoints>`
- **RESTful** design principles
- **Consistent** naming conventions (PascalCase for properties)
- **Proper HTTP status codes** and error responses
- **Pagination** for large datasets
- **Versioning** strategy (header-based)
- **CORS** configuration
- **Health checks** and monitoring
- **Graceful error handling**

## **Implementation Instructions**

### **1. Quiz.Shared Project**

```csharp
// Quiz.Shared/Common/BaseEntity.cs
namespace Quiz.Shared.Common;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; init; } = true;
}

// Quiz.Shared/Common/PaginatedResult.cs
namespace Quiz.Shared.Common;

public sealed record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

// Quiz.Shared/Common/Result.cs
namespace Quiz.Shared.Common;

public readonly record struct Result<T>
{
    public T? Value { get; init; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static Result<T> Failure(IReadOnlyList<string> errors) => new() { IsSuccess = false, Errors = errors };
}

// Quiz.Shared/Authentication/ICurrentUser.cs
namespace Quiz.Shared.Authentication;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
}

// Quiz.Shared/Contracts/ApiResponse.cs
namespace Quiz.Shared.Contracts;

public sealed record ApiResponse<T>(
    T? Data = default,
    bool Success = true,
    string? Message = null,
    IReadOnlyList<string>? Errors = null);

public sealed record PaginatedApiResponse<T>(
    IReadOnlyList<T> Data,
    int TotalCount,
    int Page,
    int PageSize,
    bool Success = true,
    string? Message = null) : ApiResponse<IReadOnlyList<T>>(Data, Success, Message);
```

### **2. Quiz.Infrastructure Project**

```csharp
// Quiz.Infrastructure/Authentication/CurrentUser.cs
namespace Quiz.Infrastructure.Authentication;

using Quiz.Shared.Authentication;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly ClaimsPrincipal? _principal = httpContextAccessor.HttpContext?.User;

    public string? UserId => _principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? Email => _principal?.FindFirst(ClaimTypes.Email)?.Value;
    public string? FirstName => _principal?.FindFirst(ClaimTypes.GivenName)?.Value;
    public string? LastName => _principal?.FindFirst(ClaimTypes.Surname)?.Value;
    public bool IsAuthenticated => _principal?.Identity?.IsAuthenticated ?? false;
    public IReadOnlyList<string> Roles => _principal?.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList() ?? [];
}

// Quiz.Infrastructure/Extensions/ServiceCollectionExtensions.cs
namespace Quiz.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Keycloak:Authority"];
                options.Audience = configuration["Keycloak:Audience"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Current User
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
```

### **3. Quiz.CSharp.Data Project**

```csharp
// Quiz.CSharp.Data/Entities/Category.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class Category : BaseEntity
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int SortOrder { get; init; }
    
    public ICollection<Question> Questions { get; init; } = [];
    public ICollection<UserProgress> UserProgress { get; init; } = [];
}

// Quiz.CSharp.Data/Entities/Question.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class Question : BaseEntity
{
    public required int Id { get; init; }
    public required QuestionType Type { get; init; }
    public required string CategoryId { get; init; }
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; }
    public required string Prompt { get; init; }
    public string? CodeBefore { get; init; }
    public string? CodeAfter { get; init; }
    public string? CodeWithBlank { get; init; }
    public string? CodeWithError { get; init; }
    public string? Snippet { get; init; }
    public string? Explanation { get; init; }
    public int EstimatedTimeMinutes { get; init; }
    
    public Category Category { get; init; } = null!;
    public ICollection<MCQOption> Options { get; init; } = [];
    public ICollection<UserAnswer> UserAnswers { get; init; } = [];
    public ICollection<QuestionHint> Hints { get; init; } = [];
    public ICollection<TestCase> TestCases { get; init; } = [];
}

// Quiz.CSharp.Data/Entities/MCQOption.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class MCQOption : BaseEntity
{
    public required string Id { get; init; }
    public required int QuestionId { get; init; }
    public required string Option { get; init; }
    public required bool IsCorrect { get; init; }
    
    public Question Question { get; init; } = null!;
}

// Quiz.CSharp.Data/Entities/UserAnswer.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class UserAnswer : BaseEntity
{
    public required int Id { get; init; }
    public required string UserId { get; init; }
    public required int QuestionId { get; init; }
    public required string Answer { get; init; }
    public required bool IsCorrect { get; init; }
    public required int TimeSpentSeconds { get; init; }
    public DateTime SubmittedAt { get; init; } = DateTime.UtcNow;
    public int AttemptNumber { get; init; }
    
    public Question Question { get; init; } = null!;
}

// Quiz.CSharp.Data/Entities/UserProgress.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class UserProgress : BaseEntity
{
    public required string UserId { get; init; }
    public required string CategoryId { get; init; }
    public int TotalQuestions { get; init; }
    public int AnsweredQuestions { get; init; }
    public int CorrectAnswers { get; init; }
    public decimal SuccessRate { get; init; }
    public DateTime LastAnsweredAt { get; init; }
    
    public Category Category { get; init; } = null!;
}

// Quiz.CSharp.Data/Entities/QuestionHint.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class QuestionHint : BaseEntity
{
    public required int Id { get; init; }
    public required int QuestionId { get; init; }
    public required string Hint { get; init; }
    public int OrderIndex { get; init; }
    
    public Question Question { get; init; } = null!;
}

// Quiz.CSharp.Data/Entities/TestCase.cs
namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class TestCase : BaseEntity
{
    public required int Id { get; init; }
    public required int QuestionId { get; init; }
    public required string Input { get; init; }
    public required string ExpectedOutput { get; init; }
    
    public Question Question { get; init; } = null!;
}

// Quiz.CSharp.Data/ValueObjects/QuestionType.cs
namespace Quiz.CSharp.Data.ValueObjects;

public enum QuestionType
{
    MCQ,
    TrueFalse,
    Fill,
    ErrorSpotting,
    OutputPrediction,
    CodeWriting
}

// Quiz.CSharp.Data/CSharpDbContext.cs
namespace Quiz.CSharp.Data;

public sealed class CSharpDbContext(DbContextOptions<CSharpDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();
    public DbSet<MCQOption> MCQOptions => Set<MCQOption>();
    public DbSet<QuestionHint> QuestionHints => Set<QuestionHint>();
    public DbSet<TestCase> TestCases => Set<TestCase>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("csharp");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

// Quiz.CSharp.Data/Configurations/CategoryConfiguration.cs
namespace Quiz.CSharp.Data.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasMaxLength(100);
        builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.Icon).HasMaxLength(50);
        
        builder.HasMany(c => c.Questions)
            .WithOne(q => q.Category)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => c.SortOrder);
        builder.HasIndex(c => c.IsActive);
    }
}

// Quiz.CSharp.Data/Configurations/QuestionConfiguration.cs
namespace Quiz.CSharp.Data.Configurations;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();
        builder.Property(q => q.Type).HasConversion<string>();
        builder.Property(q => q.CategoryId).HasMaxLength(100);
        builder.Property(q => q.Subcategory).HasMaxLength(200);
        builder.Property(q => q.Difficulty).HasMaxLength(50);
        builder.Property(q => q.Prompt).HasMaxLength(2000);
        
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(q => q.UserAnswers)
            .WithOne(ua => ua.Question)
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(q => q.Hints)
            .WithOne(h => h.Question)
            .HasForeignKey(h => h.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(q => q.TestCases)
            .WithOne(tc => tc.Question)
            .HasForeignKey(tc => tc.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(q => q.CategoryId);
        builder.HasIndex(q => q.IsActive);
    }
}

// Quiz.CSharp.Data/Configurations/MCQOptionConfiguration.cs
namespace Quiz.CSharp.Data.Configurations;

public sealed class MCQOptionConfiguration : IEntityTypeConfiguration<MCQOption>
{
    public void Configure(EntityTypeBuilder<MCQOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(10);
        builder.Property(o => o.Option).HasMaxLength(1000);
        
        builder.HasIndex(o => o.QuestionId);
    }
}

// Quiz.CSharp.Data/Configurations/UserAnswerConfiguration.cs
namespace Quiz.CSharp.Data.Configurations;

public sealed class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id).ValueGeneratedOnAdd();
        builder.Property(ua => ua.UserId).HasMaxLength(100);
        builder.Property(ua => ua.Answer).HasMaxLength(5000);
        
        builder.HasIndex(ua => ua.UserId);
        builder.HasIndex(ua => ua.QuestionId);
        builder.HasIndex(ua => new { ua.UserId, ua.QuestionId });
    }
}

// Quiz.CSharp.Data/Services/ICSharpRepository.cs
namespace Quiz.CSharp.Data.Services;

public interface ICSharpRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Question>> GetQuestionsByCategoryAsync(
        string categoryId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Question>> GetPreviewQuestionsAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<Question?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default);
    Task<UserAnswer?> GetLatestAnswerAsync(string userId, int questionId, CancellationToken cancellationToken = default);
    Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default);
    Task<UserProgress?> GetUserProgressAsync(string userId, string categoryId, CancellationToken cancellationToken = default);
    Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default);
}

// Quiz.CSharp.Data/Services/CSharpRepository.cs
namespace Quiz.CSharp.Data.Services;

public sealed class CSharpRepository(
    CSharpDbContext context,
    ILogger<CSharpRepository> logger) : ICSharpRepository
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive, cancellationToken);
    }

    public async Task<PaginatedResult<Question>> GetQuestionsByCategoryAsync(
        string categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Questions
            .Where(q => q.CategoryId == categoryId && q.IsActive)
            .Include(q => q.Options)
            .Include(q => q.Hints)
            .Include(q => q.TestCases);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Question>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<Question>> GetPreviewQuestionsAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Questions
            .Where(q => q.CategoryId == categoryId && q.IsActive)
            .Include(q => q.Options)
            .OrderBy(q => Guid.NewGuid())
            .Take(2)
            .ToListAsync(cancellationToken);
    }

    public async Task<Question?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        return await context.Questions
            .Include(q => q.Options)
            .Include(q => q.Hints)
            .Include(q => q.TestCases)
            .FirstOrDefaultAsync(q => q.Id == questionId && q.IsActive, cancellationToken);
    }

    public async Task<UserAnswer?> GetLatestAnswerAsync(string userId, int questionId, CancellationToken cancellationToken = default)
    {
        return await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveAnswerAsync(UserAnswer answer, CancellationToken cancellationToken = default)
    {
        context.UserAnswers.Add(answer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserProgress?> GetUserProgressAsync(string userId, string categoryId, CancellationToken cancellationToken = default)
    {
        return await context.UserProgress
            .FirstOrDefaultAsync(up => up.UserId == userId && up.CategoryId == categoryId, cancellationToken);
    }

    public async Task UpdateUserProgressAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        context.UserProgress.Update(progress);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetNextAttemptNumberAsync(string userId, int questionId, CancellationToken cancellationToken = default)
    {
        var lastAttempt = await context.UserAnswers
            .Where(ua => ua.UserId == userId && ua.QuestionId == questionId)
            .OrderByDescending(ua => ua.AttemptNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return (lastAttempt?.AttemptNumber ?? 0) + 1;
    }
}

// Quiz.CSharp.Data/Extensions/ServiceCollectionExtensions.cs
namespace Quiz.CSharp.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CSharpDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICSharpRepository, CSharpRepository>();

        return services;
    }
}
```

### **4. Quiz.CSharp.Api Project**

```csharp
// Quiz.CSharp.Api/Services/ICategoryService.cs
namespace Quiz.CSharp.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default);
}

// Quiz.CSharp.Api/Services/CategoryService.cs
namespace Quiz.CSharp.Api.Services;

public sealed class CategoryService(
    ICSharpRepository repository,
    IMapper mapper) : ICategoryService
{
    public async Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetCategoriesAsync(cancellationToken);
        return mapper.Map<List<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse?> GetCategoryByIdAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        var category = await repository.GetCategoryByIdAsync(categoryId, cancellationToken);
        return category is not null ? mapper.Map<CategoryResponse>(category) : null;
    }
}

// Quiz.CSharp.Api/Services/IQuestionService.cs
namespace Quiz.CSharp.Api.Services;

public interface IQuestionService
{
    Task<PaginatedResult<QuestionResponse>> GetQuestionsByCategoryAsync(
        string categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<List<QuestionResponse>> GetPreviewQuestionsAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default);
}

// Quiz.CSharp.Api/Services/QuestionService.cs
namespace Quiz.CSharp.Api.Services;

public sealed class QuestionService(
    ICSharpRepository repository,
    IMapper mapper,
    ICurrentUser currentUser) : IQuestionService
{
    public async Task<PaginatedResult<QuestionResponse>> GetQuestionsByCategoryAsync(
        string categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var questions = await repository.GetQuestionsByCategoryAsync(categoryId, page, pageSize, cancellationToken);
        var responses = new List<QuestionResponse>();

        foreach (var question in questions.Items)
        {
            var response = mapper.Map<QuestionResponse>(question);

            // Get previous answer if user is authenticated
            if (currentUser.IsAuthenticated && currentUser.UserId is not null)
            {
                var previousAnswer = await repository.GetLatestAnswerAsync(
                    currentUser.UserId,
                    question.Id,
                    cancellationToken);

                if (previousAnswer is not null)
                {
                    response = response with
                    {
                        PreviousAnswer = new PreviousAnswerResponse
                        {
                            Answer = previousAnswer.Answer,
                            SubmittedAt = previousAnswer.SubmittedAt,
                            IsCorrect = previousAnswer.IsCorrect
                        }
                    };
                }
            }

            responses.Add(response);
        }

        return new PaginatedResult<QuestionResponse>(responses, questions.TotalCount, questions.Page, questions.PageSize);
    }

    public async Task<List<QuestionResponse>> GetPreviewQuestionsAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        var questions = await repository.GetPreviewQuestionsAsync(categoryId, cancellationToken);
        return mapper.Map<List<QuestionResponse>>(questions);
    }

    public async Task<QuestionResponse?> GetQuestionByIdAsync(int questionId, CancellationToken cancellationToken = default)
    {
        var question = await repository.GetQuestionByIdAsync(questionId, cancellationToken);
        return question is not null ? mapper.Map<QuestionResponse>(question) : null;
    }
}

// Quiz.CSharp.Api/Services/IAnswerService.cs
namespace Quiz.CSharp.Api.Services;

public interface IAnswerService
{
    Task<Result<AnswerSubmissionResponse>> SubmitAnswerAsync(
        int questionId,
        string answer,
        int timeSpentSeconds,
        CancellationToken cancellationToken = default);
    Task<UserAnswerResponse?> GetLatestAnswerAsync(int questionId, CancellationToken cancellationToken = default);
}

// Quiz.CSharp.Api/Services/AnswerService.cs
namespace Quiz.CSharp.Api.Services;

public sealed class AnswerService(
    ICSharpRepository repository,
    IAnswerValidator validator,
    ICurrentUser currentUser,
    ILogger<AnswerService> logger) : IAnswerService
{
    public async Task<Result<AnswerSubmissionResponse>> SubmitAnswerAsync(
        int questionId,
        string answer,
        int timeSpentSeconds,
        CancellationToken cancellationToken = default)
    {
        var question = await repository.GetQuestionByIdAsync(questionId, cancellationToken);
        if (question is null)
            return Result<AnswerSubmissionResponse>.Failure("Question not found");

        var isCorrect = await validator.ValidateAnswerAsync(question, answer, cancellationToken);

        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var attemptNumber = await repository.GetNextAttemptNumberAsync(
                currentUser.UserId,
                questionId,
                cancellationToken);

            var userAnswer = new UserAnswer
            {
                UserId = currentUser.UserId,
                QuestionId = questionId,
                Answer = answer,
                IsCorrect = isCorrect,
                TimeSpentSeconds = timeSpentSeconds,
                AttemptNumber = attemptNumber
            };

            await repository.SaveAnswerAsync(userAnswer, cancellationToken);
        }

        logger.LogInformation("Answer submitted for question {QuestionId}, correct: {IsCorrect}",
            questionId, isCorrect);

        return Result<AnswerSubmissionResponse>.Success(new AnswerSubmissionResponse
        {
            Success = true,
            IsCorrect = isCorrect
        });
    }

    public async Task<UserAnswerResponse?> GetLatestAnswerAsync(int questionId, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return null;

        var answer = await repository.GetLatestAnswerAsync(currentUser.UserId, questionId, cancellationToken);
        return answer is not null ? new UserAnswerResponse
        {
            Answer = answer.Answer,
            SubmittedAt = answer.SubmittedAt,
            IsCorrect = answer.IsCorrect
        } : null;
    }
}

// Quiz.CSharp.Api/Services/IAnswerValidator.cs
namespace Quiz.CSharp.Api.Services;

public interface IAnswerValidator
{
    Task<bool> ValidateAnswerAsync(Question question, string userAnswer, CancellationToken cancellationToken = default);
}

// Quiz.CSharp.Api/Services/AnswerValidator.cs
namespace Quiz.CSharp.Api.Services;

public sealed class AnswerValidator : IAnswerValidator
{
    public async Task<bool> ValidateAnswerAsync(Question question, string userAnswer, CancellationToken cancellationToken = default)
    {
        return question.Type switch
        {
            QuestionType.MCQ => ValidateMCQAnswer(question, userAnswer),
            QuestionType.TrueFalse => ValidateTrueFalseAnswer(question, userAnswer),
            QuestionType.Fill => ValidateFillAnswer(question, userAnswer),
            QuestionType.ErrorSpotting => ValidateErrorSpottingAnswer(question, userAnswer),
            QuestionType.OutputPrediction => ValidateOutputPredictionAnswer(question, userAnswer),
            QuestionType.CodeWriting => ValidateCodeWritingAnswer(question, userAnswer),
            _ => false
        };
    }

    private static bool ValidateMCQAnswer(Question question, string userAnswer)
    {
        try
        {
            var userAnswers = JsonSerializer.Deserialize<string[]>(userAnswer);
            var correctAnswers = question.Options
                .Where(o => o.IsCorrect)
                .Select(o => o.Id)
                .ToArray();

            return userAnswers?.Length == correctAnswers.Length &&
                   userAnswers.All(ua => correctAnswers.Contains(ua));
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateTrueFalseAnswer(Question question, string userAnswer)
    {
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               string.Equals(userAnswer, correctAnswer.Id, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateFillAnswer(Question question, string userAnswer)
    {
        var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
        
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               normalizeCode(correctAnswer.Option) == normalizeCode(userAnswer);
    }

    private static bool ValidateErrorSpottingAnswer(Question question, string userAnswer)
    {
        var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
        
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               normalizeCode(correctAnswer.Option) == normalizeCode(userAnswer);
    }

    private static bool ValidateOutputPredictionAnswer(Question question, string userAnswer)
    {
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               string.Equals(correctAnswer.Option.Trim(), userAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateCodeWritingAnswer(Question question, string userAnswer)
    {
        // For code writing questions, consider it correct if user provided any non-empty answer
        return !string.IsNullOrWhiteSpace(userAnswer);
    }
}

// Quiz.CSharp.Api/Contracts/CategoryResponse.cs
namespace Quiz.CSharp.Api.Contracts;

public sealed record CategoryResponse
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int TotalQuestions { get; init; }
    public UserProgressResponse? UserProgress { get; init; }
}

// Quiz.CSharp.Api/Contracts/QuestionResponse.cs
namespace Quiz.CSharp.Api.Contracts;

public sealed record QuestionResponse
{
    public required int Id { get; init; }
    public required string Type { get; init; }
    public required QuestionMetadata Metadata { get; init; }
    public required QuestionContent Content { get; init; }
    public IReadOnlyList<MCQOptionResponse>? Options { get; init; }
    public IReadOnlyList<string>? Hints { get; init; }
    public string? Explanation { get; init; }
    public PreviousAnswerResponse? PreviousAnswer { get; init; }
}

public sealed record QuestionMetadata
{
    public required string Category { get; init; }
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; }
    public int EstimatedTime { get; init; }
}

public sealed record QuestionContent
{
    public required string Prompt { get; init; }
    public string? CodeBefore { get; init; }
    public string? CodeAfter { get; init; }
    public string? CodeWithBlank { get; init; }
    public string? CodeWithError { get; init; }
    public string? Snippet { get; init; }
    public IReadOnlyList<string>? Examples { get; init; }
    public IReadOnlyList<TestCaseResponse>? TestCases { get; init; }
}

public sealed record MCQOptionResponse
{
    public required string Id { get; init; }
    public required string Option { get; init; }
}

public sealed record TestCaseResponse
{
    public required string Input { get; init; }
    public required string ExpectedOutput { get; init; }
}

public sealed record PreviousAnswerResponse
{
    public required string Answer { get; init; }
    public required DateTime SubmittedAt { get; init; }
    public required bool IsCorrect { get; init; }
}

// Quiz.CSharp.Api/Contracts/AnswerSubmissionResponse.cs
namespace Quiz.CSharp.Api.Contracts;

public sealed record AnswerSubmissionResponse
{
    public bool Success { get; init; }
    public bool IsCorrect { get; init; }
    public string? Message { get; init; }
}

// Quiz.CSharp.Api/Contracts/UserAnswerResponse.cs
namespace Quiz.CSharp.Api.Contracts;

public sealed record UserAnswerResponse
{
    public required string Answer { get; init; }
    public required DateTime SubmittedAt { get; init; }
    public required bool IsCorrect { get; init; }
}

// Quiz.CSharp.Api/Contracts/UserProgressResponse.cs
namespace Quiz.CSharp.Api.Contracts;

public sealed record UserProgressResponse
{
    public int AnsweredQuestions { get; init; }
    public int CorrectAnswers { get; init; }
    public decimal SuccessRate { get; init; }
    public decimal CompletionRate { get; init; }
}

// Quiz.CSharp.Api/Contracts/Requests/SubmitAnswerRequest.cs
namespace Quiz.CSharp.Api.Contracts.Requests;

public sealed record SubmitAnswerRequest
{
    public required int QuestionId { get; init; }
    public required string Answer { get; init; }
    public required int TimeSpentSeconds { get; init; }
}

// Quiz.CSharp.Api/Mapping/CategoryProfile.cs
namespace Quiz.CSharp.Api.Mapping;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Questions.Count(q => q.IsActive)));
    }
}

// Quiz.CSharp.Api/Mapping/QuestionProfile.cs
namespace Quiz.CSharp.Api.Mapping;

public sealed class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
            .ForMember(dest => dest.Hints, opt => opt.MapFrom(src => src.Hints.OrderBy(h => h.OrderIndex).Select(h => h.Hint)));

        CreateMap<Question, QuestionMetadata>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.EstimatedTime, opt => opt.MapFrom(src => src.EstimatedTimeMinutes));

        CreateMap<Question, QuestionContent>()
            .ForMember(dest => dest.Examples, opt => opt.Ignore())
            .ForMember(dest => dest.TestCases, opt => opt.MapFrom(src => src.TestCases));

        CreateMap<MCQOption, MCQOptionResponse>();
        CreateMap<TestCase, TestCaseResponse>();
    }
}

// Quiz.CSharp.Api/Validators/SubmitAnswerRequestValidator.cs
namespace Quiz.CSharp.Api.Validators;

public sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
    public SubmitAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.Answer)
            .NotEmpty()
            .WithMessage("Answer cannot be empty")
            .MaximumLength(5000)
            .WithMessage("Answer cannot exceed 5000 characters");

        RuleFor(x => x.TimeSpentSeconds)
            .GreaterThan(0)
            .WithMessage("Time spent must be greater than 0");
    }
}

// Quiz.CSharp.Api/Controllers/CategoriesController.cs
namespace Quiz.CSharp.Api.Controllers;

[ApiController]
[Route("api/csharp/categories")]
[Produces("application/json")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ApiResponse<List<CategoryResponse>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetCategoriesAsync(cancellationToken);
        return Ok(new ApiResponse<List<CategoryResponse>>(categories));
    }

    [HttpGet("{categoryId}")]
    [ProducesResponseType<ApiResponse<CategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<CategoryResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(string categoryId, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetCategoryByIdAsync(categoryId, cancellationToken);
        return category is not null
            ? Ok(new ApiResponse<CategoryResponse>(category))
            : NotFound(new ApiResponse<CategoryResponse>(Success: false, Message: "Category not found"));
    }
}

// Quiz.CSharp.Api/Controllers/QuestionsController.cs
namespace Quiz.CSharp.Api.Controllers;

[ApiController]
[Route("api/csharp/questions")]
[Produces("application/json")]
public sealed class QuestionsController(IQuestionService questionService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType<PaginatedApiResponse<QuestionResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] string categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var questions = await questionService.GetQuestionsByCategoryAsync(categoryId, page, pageSize, cancellationToken);
        return Ok(new PaginatedApiResponse<QuestionResponse>(
            questions.Items.ToList(),
            questions.TotalCount,
            questions.Page,
            questions.PageSize));
    }

    [HttpGet("preview")]
    [ProducesResponseType<ApiResponse<List<QuestionResponse>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreviewQuestions(
        [FromQuery] string categoryId,
        CancellationToken cancellationToken)
    {
        var questions = await questionService.GetPreviewQuestionsAsync(categoryId, cancellationToken);
        return Ok(new ApiResponse<List<QuestionResponse>>(questions));
    }

    [HttpGet("{questionId:int}")]
    [Authorize]
    [ProducesResponseType<ApiResponse<QuestionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<QuestionResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestion(int questionId, CancellationToken cancellationToken)
    {
        var question = await questionService.GetQuestionByIdAsync(questionId, cancellationToken);
        return question is not null
            ? Ok(new ApiResponse<QuestionResponse>(question))
            : NotFound(new ApiResponse<QuestionResponse>(Success: false, Message: "Question not found"));
    }
}

// Quiz.CSharp.Api/Controllers/AnswersController.cs
namespace Quiz.CSharp.Api.Controllers;

[ApiController]
[Route("api/csharp/answers")]
[Produces("application/json")]
public sealed class AnswersController(IAnswerService answerService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ApiResponse<AnswerSubmissionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<AnswerSubmissionResponse>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitAnswer(
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await answerService.SubmitAnswerAsync(
            request.QuestionId,
            request.Answer,
            request.TimeSpentSeconds,
            cancellationToken);

        return result.IsSuccess
            ? Ok(new ApiResponse<AnswerSubmissionResponse>(result.Value))
            : BadRequest(new ApiResponse<AnswerSubmissionResponse>(
                Success: false,
                Message: result.ErrorMessage,
                Errors: result.Errors));
    }

    [HttpGet("{questionId:int}/latest")]
    [Authorize]
    [ProducesResponseType<ApiResponse<UserAnswerResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<UserAnswerResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestAnswer(int questionId, CancellationToken cancellationToken)
    {
        var answer = await answerService.GetLatestAnswerAsync(questionId, cancellationToken);
        return answer is not null
            ? Ok(new ApiResponse<UserAnswerResponse>(answer))
            : NotFound(new ApiResponse<UserAnswerResponse>(Success: false, Message: "Answer not found"));
    }
}

// Quiz.CSharp.Api/Extensions/ServiceCollectionExtensions.cs
namespace Quiz.CSharp.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpApi(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(CategoryProfile).Assembly);

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(SubmitAnswerRequestValidator).Assembly);

        // Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IAnswerValidator, AnswerValidator>();

        return services;
    }
}
```

### **5. Quiz.Api Project (Main Host)**

```csharp
// Quiz.Api/Program.cs
using Quiz.CSharp.Api.Extensions;
using Quiz.CSharp.Data.Extensions;
using Quiz.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz Platform API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Add modules
builder.Services.AddCSharpData(builder.Configuration);
builder.Services.AddCSharpApi();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContext<CSharpDbContext>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

### **6. Configuration Files**

```json
// Quiz.Api/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=QuizPlatformDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Keycloak": {
    "Authority": "https://your-keycloak-server/auth/realms/quiz-platform",
    "Audience": "quiz-platform-api",
    "RequireHttpsMetadata": true
  },
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://your-frontend-domain.com"
  ],
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### **7. Global Using Statements**

```csharp
// Quiz.Shared/GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;

// Quiz.CSharp.Data/GlobalUsings.cs
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Quiz.CSharp.Data.Entities;
global using Quiz.CSharp.Data.ValueObjects;
global using Quiz.Shared.Common;
global using System.Reflection;

// Quiz.CSharp.Api/GlobalUsings.cs
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using AutoMapper;
global using FluentValidation;
global using Quiz.CSharp.Api.Contracts;
global using Quiz.CSharp.Api.Contracts.Requests;
global using Quiz.CSharp.Api.Services;
global using Quiz.CSharp.Data.Services;
global using Quiz.Shared.Authentication;
global using Quiz.Shared.Common;
global using Quiz.Shared.Contracts;
global using System.Text.Json;

// Quiz.Api/GlobalUsings.cs
global using Microsoft.OpenApi.Models;
global using Quiz.CSharp.Data;
```

## **Key Implementation Notes**

1. **Simplified Architecture**: Removed Redis, MediatR, Serilog, and testing frameworks
2. **Direct Service Calls**: Services communicate directly without CQRS pattern
3. **Built-in Logging**: Use ILogger<T> for logging
4. **Modular Structure**: Each quiz type has its own Api and Data modules
5. **Authentication**: JWT token validation only - no user management
6. **Database**: Each module has its own schema (e.g., `csharp`)
7. **API Routing**: `/api/csharp/` for C# module
8. **Error Handling**: Proper HTTP status codes and error responses
9. **Documentation**: Automatic OpenAPI documentation generation
10. **Scalability**: Designed for future module additions

**Generate the complete implementation following this simplified modular monolith architecture with all specified C# 12+ features and best practices.**