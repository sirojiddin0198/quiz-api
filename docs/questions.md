# Questions API

The Questions API provides endpoints for retrieving and interacting with quiz questions. The API supports multiple question types through Table-Per-Hierarchy (TPH) inheritance.

## Overview

Questions are organized by collections and support various types:
- **MCQ**: Multiple Choice Questions with options
- **TrueFalse**: True/False questions
- **Fill**: Fill-in-the-blank questions
- **ErrorSpotting**: Code error detection
- **OutputPrediction**: Predict code output
- **CodeWriting**: Write code solutions

## Base URL

```
http://localhost:5138/api/csharp/questions
```

## Endpoints

### Get Questions by Collection

Retrieves paginated questions for a specific collection.

**Endpoint:** `GET /api/csharp/questions`

**Query Parameters:**
- `collectionId` (int, required): Collection ID
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10, max: 50)

**Authentication:** Required

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [
      {
        "id": 1,
        "type": "MCQ",
        "metadata": {
          "collectionId": 1,
          "collectionCode": "csharp-fundamentals",
          "subcategory": "Variables",
          "difficulty": "Beginner",
          "estimatedTime": 2
        },
        "content": {
          "prompt": "What is the correct way to declare a variable in C#?",
          "codeBefore": null,
          "codeAfter": null,
          "codeWithBlank": null,
          "codeWithError": null,
          "snippet": null,
          "examples": null,
          "testCases": null
        },
        "options": [
          {
            "id": "a",
            "option": "var name = \"John\";"
          },
          {
            "id": "b",
            "option": "string name = \"John\";"
          },
          {
            "id": "c",
            "option": "name = \"John\";"
          }
        ],
        "hints": [
          "Think about type declaration",
          "Consider explicit vs implicit typing"
        ],
        "explanation": "In C#, you can use either 'var' for implicit typing or explicit type declaration.",
        "previousAnswer": {
          "answer": "[\"a\"]",
          "submittedAt": "2024-01-01T10:00:00Z",
          "isCorrect": true
        }
      }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 10
  }
}
```

**Status Codes:**
- `200` - Success
- `400` - Bad Request
- `401` - Unauthorized
- `404` - Collection not found
- `500` - Internal Server Error

### Get Preview Questions

Retrieves a preview of questions for a collection (limited to 2 questions).

**Endpoint:** `GET /api/csharp/questions/preview`

**Query Parameters:**
- `collectionId` (int, required): Collection ID

**Authentication:** Not required

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "type": "MCQ",
      "metadata": {
        "collectionId": 1,
        "collectionCode": "csharp-fundamentals",
        "subcategory": "Variables",
        "difficulty": "Beginner",
        "estimatedTime": 2
      },
      "content": {
        "prompt": "What is the correct way to declare a variable in C#?",
        "codeBefore": null,
        "codeAfter": null,
        "codeWithBlank": null,
        "codeWithError": null,
        "snippet": null,
        "examples": null,
        "testCases": null
      },
      "options": [
        {
          "id": "a",
          "option": "var name = \"John\";"
        },
        {
          "id": "b",
          "option": "string name = \"John\";"
        }
      ],
      "hints": null,
      "explanation": null,
      "previousAnswer": null
    }
  ]
}
```

**Status Codes:**
- `200` - Success
- `400` - Bad Request
- `404` - Collection not found
- `500` - Internal Server Error




## Question Types

### MCQ (Multiple Choice)

```json
{
  "id": 1,
  "type": "MCQ",
  "content": {
    "prompt": "What is the correct way to declare a variable in C#?"
  },
  "options": [
    {
      "id": "a",
      "option": "var name = \"John\";"
    },
    {
      "id": "b",
      "option": "string name = \"John\";"
    }
  ]
}
```

### TrueFalse

```json
{
  "id": 2,
  "type": "TrueFalse",
  "content": {
    "prompt": "C# is a strongly-typed language."
  }
}
```

### Fill

```json
{
  "id": 3,
  "type": "Fill",
  "content": {
    "prompt": "Complete the code to declare a string variable:",
    "codeWithBlank": "string name = ___;"
  }
}
```

### ErrorSpotting

```json
{
  "id": 4,
  "type": "ErrorSpotting",
  "content": {
    "prompt": "Find the error in the following code:",
    "codeWithError": "int x = 10;\nstring y = x;"
  }
}
```

### OutputPrediction

```json
{
  "id": 5,
  "type": "OutputPrediction",
  "content": {
    "prompt": "What will be the output of this code?",
    "snippet": "Console.WriteLine(5 + 3);"
  }
}
```

### CodeWriting

```json
{
  "id": 6,
  "type": "CodeWriting",
  "content": {
    "prompt": "Write a function that calculates the factorial of a number.",
    "examples": [
      "Input: 5, Output: 120",
      "Input: 0, Output: 1"
    ],
    "testCases": [
      {
        "input": "5",
        "expectedOutput": "120"
      },
      {
        "input": "0",
        "expectedOutput": "1"
      }
    ]
  }
}
```

## Data Models

### QuestionResponse

```json
{
  "id": "integer",
  "type": "string",
  "metadata": "QuestionMetadata",
  "content": "QuestionContent",
  "options": "MCQOptionResponse[]",
  "hints": "string[]",
  "explanation": "string",
  "previousAnswer": "PreviousAnswerResponse"
}
```

### QuestionMetadata

```json
{
  "collectionId": "integer",
  "collectionCode": "string",
  "subcategory": "string",
  "difficulty": "string",
  "estimatedTime": "integer"
}
```

### QuestionContent

```json
{
  "prompt": "string",
  "codeBefore": "string",
  "codeAfter": "string",
  "codeWithBlank": "string",
  "codeWithError": "string",
  "snippet": "string",
  "examples": "string[]",
  "testCases": "TestCaseResponse[]"
}
```

### MCQOptionResponse

```json
{
  "id": "string",
  "option": "string"
}
```

### TestCaseResponse

```json
{
  "input": "string",
  "expectedOutput": "string"
}
```

### PreviousAnswerResponse

```json
{
  "answer": "string",
  "submittedAt": "datetime",
  "isCorrect": "boolean"
}
```

## Error Responses

### Question Not Found (404)
```json
{
  "success": false,
  "message": "Question not found",
  "data": null
}
```

### Collection Not Found (404)
```json
{
  "success": false,
  "message": "Collection not found",
  "data": null
}
```

### Bad Request (400)
```json
{
  "success": false,
  "message": "Invalid collection ID",
  "data": null
}
```

## Usage Examples

### Get Questions by Collection

```bash
curl -H "Authorization: Bearer <your-token>" \
     "http://localhost:5138/api/csharp/questions?collectionId=1&page=1&pageSize=10"
```

### Get Preview Questions

```bash
curl "http://localhost:5138/api/csharp/questions/preview?collectionId=1"
```

### Get Specific Question

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/questions/1
```

### JavaScript Example

```javascript
const response = await fetch('http://localhost:5138/api/csharp/questions?collectionId=1', {
  headers: {
    'Authorization': 'Bearer ' + token,
    'Content-Type': 'application/json'
  }
});

const data = await response.json();
console.log(data.data.items); // Array of questions
```

## Best Practices

1. **Pagination**: Use pagination for large collections
2. **Caching**: Cache questions on the client side
3. **Type Handling**: Handle different question types appropriately
4. **Previous Answers**: Use previous answer data for UI state
5. **Error Handling**: Always check the `success` field

## Rate Limiting

- **Questions**: 50 requests per minute per user
- **Preview Questions**: 20 requests per minute per user

## Notes

- Questions are read-only in the current API version
- Preview questions are limited to 2 per request
- Previous answer data is included when user is authenticated
- Question types determine the available properties
- Hints are ordered by `orderIndex` 