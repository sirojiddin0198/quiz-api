# Authentication

The Quiz API uses JWT (JSON Web Token) Bearer token authentication with Keycloak integration.

## Overview

Authentication is required for most API endpoints. The API validates JWT tokens issued by Keycloak and extracts user information for authorization and personalization.

## JWT Token Structure

The API expects JWT tokens with the following claims:

```json
{
  "sub": "user-id",
  "preferred_username": "username",
  "email": "user@example.com",
  "name": "Full Name",
  "roles": ["user", "admin"],
  "exp": 1640995200,
  "iat": 1640908800
}
```

### Required Claims

- `sub`: Unique user identifier
- `preferred_username`: Username for display
- `email`: User's email address
- `name`: Full name of the user
- `roles`: Array of user roles
- `exp`: Token expiration timestamp
- `iat`: Token issued timestamp

## Authentication Flow

1. **User Login**: User authenticates with Keycloak
2. **Token Issuance**: Keycloak issues JWT token
3. **API Request**: Client includes token in Authorization header
4. **Token Validation**: API validates token signature and claims
5. **User Context**: API extracts user information for request processing

## Authorization Header

Include the JWT token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Protected Endpoints

The following endpoints require authentication:

### Collections
- `GET /api/csharp/collections` - List collections

### Questions
- `GET /api/csharp/questions` - Get questions by collection
- `GET /api/csharp/questions/preview` - Get preview questions

### Answers
- `POST /api/csharp/answers` - Submit answer
- `GET /api/csharp/answers/{questionId}/latest` - Get latest answer for question

### Progress
- `GET /api/csharp/progress/{collectionId}` - Get user progress
- `GET /api/csharp/progress` - Get all user progress

## Public Endpoints

The following endpoints are publicly accessible:

- `GET /health` - Health check
- `GET /swagger` - API documentation
- `GET /api/csharp/questions/preview` - Preview questions (limited)

## Error Responses

### Unauthorized (401)
```json
{
  "success": false,
  "message": "Unauthorized",
  "data": null
}
```

### Invalid Token (401)
```json
{
  "success": false,
  "message": "Invalid token",
  "data": null
}
```

### Expired Token (401)
```json
{
  "success": false,
  "message": "Token expired",
  "data": null
}
```

## Token Configuration

The API is configured to validate tokens with the following settings:

```json
{
  "Authentication": {
    "JwtBearer": {
      "Authority": "https://your-keycloak-url/auth/realms/your-realm",
      "Audience": "quiz-api",
      "RequireHttpsMetadata": false,
      "ValidateIssuer": true,
      "ValidateAudience": true,
      "ValidateLifetime": true
    }
  }
}
```

## User Context

Once authenticated, the API provides user context through the `ICurrentUser` service:

```csharp
public interface ICurrentUser
{
    string? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
```

## Role-Based Access

The API supports role-based access control:

- **user**: Standard user access
- **admin**: Administrative access (future use)
- **moderator**: Content moderation access (future use)

## Security Best Practices

1. **Token Storage**: Store tokens securely (not in localStorage)
2. **Token Refresh**: Implement automatic token refresh
3. **HTTPS**: Always use HTTPS in production
4. **Token Expiration**: Handle token expiration gracefully
5. **Logout**: Clear tokens on logout

## Testing Authentication

### Using Swagger UI

1. Navigate to `http://localhost:5138/swagger`
2. Click the "Authorize" button
3. Enter your JWT token: `Bearer <your-token>`
4. Click "Authorize"

### Using curl

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/collections
```

### Using Postman

1. Set the Authorization type to "Bearer Token"
2. Enter your JWT token
3. Make API requests

## Troubleshooting

### Common Issues

1. **Invalid Token Format**: Ensure token starts with "Bearer "
2. **Expired Token**: Refresh your token
3. **Wrong Audience**: Verify token audience matches API configuration
4. **Network Issues**: Check Keycloak connectivity

### Debug Information

Enable debug logging to troubleshoot authentication issues:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
``` 