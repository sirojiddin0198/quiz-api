# ABAC Subscription System

## Overview
The Attribute-Based Access Control (ABAC) system provides subscription-based authorization for the quiz API. It checks the `ustoz-membership` claim in JWT tokens to determine user access to different features.

## How It Works

### 1. JWT Token Structure
The system expects JWT tokens from Keycloak with a `ustoz-membership` claim containing a JSON array of subscription types:

```json
{
  "sub": "user123",
  "email": "user@example.com",
  "ustoz-membership": ["csharp-quiz", "premium"],
  "exp": 1640995200
}
```

### 2. Feature Access Mapping
The system maps subscription types to features:

| Subscription | Features Granted |
|--------------|------------------|
| `csharp-quiz` | `csharp-quiz` |
| `premium` | `csharp-quiz`, `advanced-features`, `unlimited-questions` |
| `basic` | `csharp-quiz` |
| `admin` | `csharp-quiz`, `advanced-features`, `unlimited-questions`, `admin-panel` |

### 3. Authorization Levels

#### Controller Level
```csharp
[RequireSubscription("csharp-quiz")]
public sealed class QuestionsController : ControllerBase
{
    // All endpoints require csharp-quiz subscription
}
```

#### Method Level
```csharp
[RequireSubscription("premium")]
public async Task<IActionResult> GetAdvancedFeatures()
{
    // Only premium users can access
}
```

#### Anonymous Access
```csharp
[AllowAnonymous]
public async Task<IActionResult> GetPreviewQuestions()
{
    // No subscription required
}
```

## Usage Examples

### 1. Controller with Subscription Requirement
```csharp
[ApiController]
[Route("api/csharp/questions")]
[RequireSubscription("csharp-quiz")]
public sealed class QuestionsController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetQuestions()
    {
        // Requires authentication + csharp-quiz subscription
    }

    [HttpGet("preview")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPreviewQuestions()
    {
        // No authentication or subscription required
    }
}
```

### 2. Service-Level Authorization
```csharp
public sealed class QuestionService(
    ISubscriptionGuard subscriptionGuard) : IQuestionService
{
    public async Task<Result<Question>> GetAdvancedQuestionAsync(int questionId, CancellationToken cancellationToken)
    {
        // Check subscription at service level
        var accessResult = await subscriptionGuard.EnsureAccessAsync("premium", cancellationToken);
        if (!accessResult.IsSuccess)
            return Result<Question>.Failure(accessResult.ErrorMessage);

        // Proceed with premium feature
        return await GetQuestionAsync(questionId, cancellationToken);
    }
}
```

### 3. Conditional Feature Access
```csharp
public async Task<IActionResult> GetQuestions(int collectionId)
{
    var hasPremium = await subscriptionGuard.HasAccessAsync("premium", cancellationToken);
    
    var questions = hasPremium 
        ? await GetUnlimitedQuestionsAsync(collectionId, cancellationToken)
        : await GetLimitedQuestionsAsync(collectionId, cancellationToken);
        
    return Ok(questions);
}
```

## Subscription Types

### Current Subscriptions
- **`csharp-quiz`**: Basic access to C# quiz features
- **`premium`**: Enhanced features including unlimited questions and advanced features
- **`basic`**: Same as csharp-quiz (for backward compatibility)
- **`admin`**: Full access including admin panel

### Future-Proof Design
The system is designed to easily accommodate new subscription types:

1. **Add new subscription type**:
```csharp
private static readonly Dictionary<string, HashSet<string>> FeatureAccessMap = new()
{
    ["csharp-quiz"] = ["csharp-quiz"],
    ["premium"] = ["csharp-quiz", "advanced-features", "unlimited-questions"],
    ["enterprise"] = ["csharp-quiz", "advanced-features", "unlimited-questions", "team-management", "analytics"],
    ["admin"] = ["csharp-quiz", "advanced-features", "unlimited-questions", "admin-panel"]
};
```

2. **Add new features**:
```csharp
["premium"] = ["csharp-quiz", "advanced-features", "unlimited-questions", "new-feature"]
```

## Error Responses

### 403 Forbidden
When a user doesn't have the required subscription:
```json
{
  "success": false,
  "message": "Access denied. Subscription required for feature: premium"
}
```

### 401 Unauthorized
When user is not authenticated:
```json
{
  "success": false,
  "message": "Authentication required"
}
```

## Testing

### Valid Token Examples

**Basic User:**
```json
{
  "ustoz-membership": ["csharp-quiz"]
}
```

**Premium User:**
```json
{
  "ustoz-membership": ["premium"]
}
```

**Multiple Subscriptions:**
```json
{
  "ustoz-membership": ["csharp-quiz", "premium"]
}
```

### Invalid Token Examples

**No Subscription:**
```json
{
  "ustoz-membership": []
}
```

**Expired Token:**
```json
{
  "ustoz-membership": ["csharp-quiz"],
  "exp": 1640995200  // Past timestamp
}
```

## Configuration

### Keycloak Setup
Ensure your Keycloak realm includes the `ustoz-membership` claim in JWT tokens:

1. Create a custom claim mapper
2. Map user attributes or roles to the `ustoz-membership` claim
3. Include the claim in JWT tokens

### Environment Variables
```json
{
  "Keycloak": {
    "Authority": "http://auth.localhost.uz/realms/ilmhub",
    "Audience": "your-audience",
    "RequireHttpsMetadata": false
  }
}
```

## Security Considerations

1. **Token Validation**: JWT tokens are validated for signature, expiration, and issuer
2. **Claim Verification**: Subscription claims are verified on every request
3. **No Caching**: Subscription status is checked fresh on each request
4. **Audit Trail**: All access attempts are logged for security monitoring

## Future Enhancements

1. **Database Integration**: Store subscription data in database for more complex rules
2. **Usage Tracking**: Track feature usage for billing/analytics
3. **Trial Periods**: Support for temporary access without subscription
4. **Team Management**: Support for team-based subscriptions
5. **Usage Limits**: Implement rate limiting based on subscription tier 