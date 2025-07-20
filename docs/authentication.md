# Authentication

The Quiz API uses JWT (JSON Web Token) Bearer token authentication with Keycloak integration and realm-based role authorization.

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
  "realm_access": {
    "roles": ["user", "admin"]
  },
  "exp": 1640995200,
  "iat": 1640908800
}
```

### Required Claims

- `sub`: Unique user identifier
- `preferred_username`: Username for display
- `email`: User's email address
- `name`: Full name of the user
- `realm_access.roles`: Array of realm roles assigned to the user
- `exp`: Token expiration timestamp
- `iat`: Token issued timestamp

## Authentication Flow

1. **User Login**: User authenticates with Keycloak
2. **Token Issuance**: Keycloak issues JWT token with realm roles
3. **API Request**: Client includes token in Authorization header
4. **Token Validation**: API validates token signature and claims
5. **Role Authorization**: API checks realm roles for endpoint access
6. **User Context**: API extracts user information for request processing

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

### Management Endpoints (Admin Only)
- `GET /api/management/user-progresses` - Get all user progress (requires `quiz-admin:read` realm role)
- `POST /api/management/collections` - Create new collection (requires `quiz-admin:write` realm role)

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

### Forbidden (403)
```json
{
  "success": false,
  "message": "Insufficient privileges. Quiz admin role required.",
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
  "Keycloak": {
    "realm": "ilmhub",
    "auth-server-url": "http://auth.localhost.uz/",
    "ssl-required": "none",
    "resource": "quiz-api",
    "verify-token-audience": false
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
    IReadOnlyList<string> RealmRoles { get; }
    bool HasRealmRole(string role);
}
```

## Realm Role-Based Access

The API supports realm role-based access control with the following roles:

### Realm Roles
- **`user`**: Standard user access to quiz features
- **`quiz-admin:read`**: Read access to management endpoints
- **`quiz-admin:write`**: Write access to create/modify management data

### Authorization Policies
- **`Admin:Read`**: Requires `quiz-admin:read` realm role - Read access to management data
- **`Admin:Write`**: Requires `quiz-admin:write` realm role - Write access to create/modify data  
- **`Admin:Manage`**: Requires both `quiz-admin:read` and `quiz-admin:write` realm roles - Full management access

### Usage Examples

```csharp
// Controller level authorization
[Authorize(Policy = "Admin:Read")]
public class UserProgressManagementController : ControllerBase
{
    // All endpoints require quiz-admin:read realm role
}

// Method level authorization
[Authorize(Policy = "Admin:Write")]
[HttpPost]
public async Task<IActionResult> CreateCollection(CreateCollectionRequest request)
{
    // Only users with quiz-admin:write can create collections
}

// Check roles in code
public async Task<IActionResult> SomeAction()
{
    if (!currentUser.HasRealmRole("quiz-admin:read"))
    {
        return Forbid("Quiz admin read role required");
    }
    // Admin-only logic
}
```

## Security Best Practices

1. **Token Storage**: Store tokens securely (not in localStorage)
2. **Token Refresh**: Implement automatic token refresh
3. **HTTPS**: Always use HTTPS in production
4. **Token Expiration**: Handle token expiration gracefully
5. **Logout**: Clear tokens on logout
6. **Role Validation**: Always validate realm roles on sensitive operations

## Testing Authentication

### Using Swagger UI

1. Navigate to `http://localhost:5138/swagger`
2. Click the "Authorize" button
3. Use OAuth2 flow to authenticate with Keycloak
4. Or manually enter JWT token: `Bearer <your-token>`

### Using curl

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/collections
```

### Testing Admin Endpoints

```bash
# Get user progress (requires quiz-admin:read role)
curl -H "Authorization: Bearer <admin-token>" \
     http://localhost:5138/api/management/user-progresses

# Create collection (requires quiz-admin:write role)  
curl -X POST \
     -H "Authorization: Bearer <admin-token>" \
     -H "Content-Type: application/json" \
     -d '{"code":"test","title":"Test Collection","description":"Test"}' \
     http://localhost:5138/api/management/collections
```

## Keycloak Configuration

### Realm Setup
1. Create or configure the `ilmhub` realm in Keycloak
2. Create realm roles: `user`, `quiz-admin:read`, `quiz-admin:write`
3. Assign roles to users as needed

### Client Configuration
1. Create a client with ID `quiz-api`
2. Enable "Standard Flow" and "Implicit Flow" for Swagger UI
3. Set valid redirect URIs for your application
4. Ensure realm roles are included in JWT tokens

### Token Claims
Ensure your Keycloak configuration includes these claims in JWT tokens:
- `realm_access.roles` - Array of assigned realm roles
- `preferred_username` - User's username
- `name` - User's full name
- `email` - User's email address

## Troubleshooting

### Common Issues

1. **403 Forbidden**: User doesn't have required realm role (`quiz-admin:read` or `quiz-admin:write`)
2. **Invalid Token Format**: Ensure token starts with "Bearer "
3. **Expired Token**: Refresh your token
4. **Missing Roles**: Check Keycloak role assignments for quiz admin roles
5. **Wrong Realm**: Verify token is from correct Keycloak realm

### Debug Information

Enable debug logging to troubleshoot authentication issues:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug"
    }
  }
}
``` 