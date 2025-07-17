# Quiz API Documentation

Welcome to the Quiz API documentation. This API provides a comprehensive platform for managing and interacting with programming quizzes, specifically focused on C# programming language.

## Overview

The Quiz API is a .NET 8 modular monolith that provides:
- **Collection Management**: Organize questions into themed collections
- **Question Types**: Support for multiple question types (MCQ, True/False, Fill-in-the-blank, etc.)
- **User Progress Tracking**: Monitor user performance and progress
- **Authentication**: JWT-based authentication with Keycloak integration
- **Real-time Validation**: Instant answer validation and feedback

## Architecture

The API follows a modular monolith architecture with:
- **Quiz.CSharp.Api**: Main API layer with controllers and services
- **Quiz.CSharp.Data**: Data access layer with Entity Framework Core
- **Quiz.Infrastructure**: Cross-cutting concerns (authentication, logging, etc.)
- **Quiz.Shared**: Shared contracts and utilities

## Quick Start

1. **Base URL**: `http://localhost:5138`
2. **Swagger UI**: `http://localhost:5138/swagger`
3. **Health Check**: `http://localhost:5138/health`

## Authentication

The API uses JWT Bearer token authentication. Include your token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Documentation Sections

- [Authentication](./authentication.md) - JWT authentication details
- [Collections](./collections.md) - Collection management endpoints
- [Questions](./questions.md) - Question retrieval and interaction
- [Answers](./answers.md) - Answer submission and validation
- [Data Models](./data-models.md) - API request/response schemas
- [Examples](./examples.md) - Usage examples and scenarios

## Question Types

The API supports the following question types:

| Type | Description | Properties |
|------|-------------|------------|
| MCQ | Multiple Choice Questions | Options with correct answers |
| TrueFalse | True/False Questions | Boolean correct answer |
| Fill | Fill-in-the-blank | String correct answer |
| ErrorSpotting | Code error detection | String correct answer |
| OutputPrediction | Predict code output | Expected output string |
| CodeWriting | Write code solutions | Test cases, examples, rubric |

## Rate Limiting

- **Collections**: 100 requests per minute
- **Questions**: 50 requests per minute
- **Answers**: 30 requests per minute

## Error Handling

The API returns consistent error responses:
```json
{
  "success": false,
  "message": "Error description",
  "data": null
}
```

Common HTTP status codes:
- `200` - Success
- `400` - Bad Request
- `401` - Unauthorized
- `404` - Not Found
- `429` - Too Many Requests
- `500` - Internal Server Error 