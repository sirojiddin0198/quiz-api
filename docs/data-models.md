# Data Models

This document describes all the data models and schemas used in the Quiz API.

## Common Response Format

All API responses follow a consistent format:

```json
{
  "success": "boolean",
  "message": "string",
  "data": "any"
}
```

**Properties:**
- `success` (boolean): Indicates if the request was successful
- `message` (string): Optional message or error description
- `data` (any): The actual response data

## Pagination Format

Paginated responses include additional metadata:

```json
{
  "success": true,
  "message": null,
  "data": {
    "items": "array",
    "totalCount": "integer",
    "page": "integer",
    "pageSize": "integer"
  }
}
```

## Collections

### CollectionResponse

```json
{
  "id": "integer",
  "code": "string",
  "title": "string",
  "description": "string",
  "icon": "string",
  "sortOrder": "integer",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

**Properties:**
- `id` (int): Unique identifier
- `code` (string): URL-friendly identifier
- `title` (string): Display name
- `description` (string): Detailed description
- `icon` (string): Icon identifier
- `sortOrder` (int): Display order
- `createdAt` (datetime): Creation timestamp
- `updatedAt` (datetime): Last update timestamp

## Questions

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

**Properties:**
- `id` (int): Unique identifier
- `type` (string): Question type (MCQ, TrueFalse, Fill, ErrorSpotting, OutputPrediction, CodeWriting)
- `metadata` (QuestionMetadata): Question metadata
- `content` (QuestionContent): Question content
- `options` (MCQOptionResponse[]): MCQ options (only for MCQ questions)
- `hints` (string[]): Array of hints
- `explanation` (string): Explanation of the answer
- `previousAnswer` (PreviousAnswerResponse): User's previous answer (if authenticated)

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

**Properties:**
- `collectionId` (int): Collection identifier
- `collectionCode` (string): Collection code
- `subcategory` (string): Question subcategory
- `difficulty` (string): Difficulty level (Beginner, Intermediate, Advanced)
- `estimatedTime` (int): Estimated time in minutes

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

**Properties:**
- `prompt` (string): Question prompt
- `codeBefore` (string): Code snippet before the question
- `codeAfter` (string): Code snippet after the question
- `codeWithBlank` (string): Code with blank spaces to fill
- `codeWithError` (string): Code with errors to identify
- `snippet` (string): Code snippet for output prediction
- `examples` (string[]): Example inputs/outputs
- `testCases` (TestCaseResponse[]): Test cases for code writing

### MCQOptionResponse

```json
{
  "id": "string",
  "option": "string"
}
```

**Properties:**
- `id` (string): Option identifier
- `option` (string): Option text

### TestCaseResponse

```json
{
  "input": "string",
  "expectedOutput": "string"
}
```

**Properties:**
- `input` (string): Test input
- `expectedOutput` (string): Expected output

### PreviousAnswerResponse

```json
{
  "answer": "string",
  "submittedAt": "datetime",
  "isCorrect": "boolean"
}
```

**Properties:**
- `answer` (string): User's answer
- `submittedAt` (datetime): Submission timestamp
- `isCorrect` (boolean): Whether the answer was correct

## Answers

### SubmitAnswerRequest

```json
{
  "questionId": "integer",
  "answer": "string",
  "timeSpentSeconds": "integer"
}
```

**Properties:**
- `questionId` (int, required): Question identifier
- `answer` (string, required): User's answer
- `timeSpentSeconds` (int, required): Time spent on question

### SubmitAnswerResponse

```json
{
  "id": "integer",
  "questionId": "integer",
  "answer": "string",
  "isCorrect": "boolean",
  "timeSpentSeconds": "integer",
  "submittedAt": "datetime",
  "attemptNumber": "integer",
  "explanation": "string"
}
```

**Properties:**
- `id` (int): Answer identifier
- `questionId` (int): Question identifier
- `answer` (string): Submitted answer
- `isCorrect` (bool): Validation result
- `timeSpentSeconds` (int): Time spent
- `submittedAt` (datetime): Submission timestamp
- `attemptNumber` (int): Attempt number for this question
- `explanation` (string): Explanation of the answer

### UserAnswerResponse

```json
{
  "id": "integer",
  "questionId": "integer",
  "answer": "string",
  "isCorrect": "boolean",
  "timeSpentSeconds": "integer",
  "submittedAt": "datetime",
  "attemptNumber": "integer"
}
```

**Properties:**
- `id` (int): Answer identifier
- `questionId` (int): Question identifier
- `answer` (string): Submitted answer
- `isCorrect` (bool): Whether the answer was correct
- `timeSpentSeconds` (int): Time spent on question
- `submittedAt` (datetime): Submission timestamp
- `attemptNumber` (int): Attempt number

## Progress

### UserProgressResponse

```json
{
  "collectionId": "integer",
  "collectionCode": "string",
  "totalQuestions": "integer",
  "answeredQuestions": "integer",
  "correctAnswers": "integer",
  "successRate": "decimal",
  "lastAnsweredAt": "datetime"
}
```

**Properties:**
- `collectionId` (int): Collection identifier
- `collectionCode` (string): Collection code
- `totalQuestions` (int): Total questions in collection
- `answeredQuestions` (int): Number of answered questions
- `correctAnswers` (int): Number of correct answers
- `successRate` (decimal): Success rate as percentage
- `lastAnsweredAt` (datetime): Last answer timestamp

## Error Responses

### Validation Error

```json
{
  "success": false,
  "message": "Validation error description",
  "data": null
}
```

### Not Found Error

```json
{
  "success": false,
  "message": "Resource not found",
  "data": null
}
```

### Unauthorized Error

```json
{
  "success": false,
  "message": "Unauthorized",
  "data": null
}
```

### Internal Server Error

```json
{
  "success": false,
  "message": "An error occurred while processing your request",
  "data": null
}
```

## Question Type-Specific Properties

### MCQ Questions
- `options`: Array of MCQOptionResponse
- `answer`: JSON array of option IDs

### TrueFalse Questions
- `answer`: Boolean string ("true" or "false")

### Fill Questions
- `codeWithBlank`: Code with blank spaces
- `answer`: String (code snippet)

### ErrorSpotting Questions
- `codeWithError`: Code with errors
- `answer`: String (error description)

### OutputPrediction Questions
- `snippet`: Code snippet
- `answer`: String (expected output)

### CodeWriting Questions
- `examples`: Array of example inputs/outputs
- `testCases`: Array of TestCaseResponse
- `answer`: String (code solution)

## Data Types

### Integer
- Whole numbers (e.g., 1, 42, -5)
- Used for IDs, counts, time values

### String
- Text values
- UTF-8 encoded
- Can contain special characters

### Boolean
- `true` or `false`
- Used for flags and conditions

### Decimal
- Floating-point numbers with precision
- Used for percentages and calculations
- Format: `0.00`

### DateTime
- ISO 8601 format: `YYYY-MM-DDTHH:mm:ssZ`
- UTC timezone
- Example: `2024-01-01T10:00:00Z`

### Array
- Ordered collections
- Can contain mixed types
- JSON array format: `[item1, item2, ...]`

## Validation Rules

### Required Fields
- All required fields must be present
- Required fields cannot be null or empty

### String Validation
- Maximum length: 1000 characters (unless specified)
- Cannot contain control characters
- UTF-8 encoding required

### Integer Validation
- Range: -2,147,483,648 to 2,147,483,647
- Must be whole numbers

### Decimal Validation
- Precision: 5 digits total, 2 decimal places
- Range: 0.00 to 999.99

### DateTime Validation
- Must be valid ISO 8601 format
- Must be in UTC timezone
- Cannot be in the future

## Examples

### Complete MCQ Question Response

```json
{
  "success": true,
  "message": null,
  "data": {
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
}
```

### Paginated Questions Response

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
        ],
        "hints": null,
        "explanation": null,
        "previousAnswer": null
      }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 10
  }
}
``` 