# Answers API

The Answers API provides endpoints for submitting and managing user answers to quiz questions. The API supports real-time validation and progress tracking.

## Overview

The Answers API enables users to:
- Submit answers to questions
- Receive immediate validation feedback
- Track answer history
- Monitor progress across collections

## Base URL

```
http://localhost:5138/api/csharp/answers
```

## Endpoints

### Submit Answer

Submits a user's answer to a question and returns validation results.

**Endpoint:** `POST /api/csharp/answers`

**Authentication:** Required

**Request Body:**
```json
{
  "questionId": 1,
  "answer": "[\"a\", \"c\"]",
  "timeSpentSeconds": 45
}
```

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 123,
    "questionId": 1,
    "answer": "[\"a\", \"c\"]",
    "isCorrect": true,
    "timeSpentSeconds": 45,
    "submittedAt": "2024-01-01T10:00:00Z",
    "attemptNumber": 1,
    "explanation": "In C#, you can use either 'var' for implicit typing or explicit type declaration."
  }
}
```

**Status Codes:**
- `200` - Success
- `400` - Bad Request
- `401` - Unauthorized
- `404` - Question not found
- `500` - Internal Server Error

### Get Answer History

Retrieves a user's answer history for a specific question.

**Endpoint:** `GET /api/csharp/answers/{questionId}`

**Parameters:**
- `questionId` (int, required): Question ID

**Authentication:** Required

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 123,
      "questionId": 1,
      "answer": "[\"a\"]",
      "isCorrect": true,
      "timeSpentSeconds": 30,
      "submittedAt": "2024-01-01T09:30:00Z",
      "attemptNumber": 1
    },
    {
      "id": 124,
      "questionId": 1,
      "answer": "[\"a\", \"c\"]",
      "isCorrect": true,
      "timeSpentSeconds": 45,
      "submittedAt": "2024-01-01T10:00:00Z",
      "attemptNumber": 2
    }
  ]
}
```

**Status Codes:**
- `200` - Success
- `401` - Unauthorized
- `404` - Question not found
- `500` - Internal Server Error

## Answer Formats by Question Type

### MCQ (Multiple Choice)

**Answer Format:** JSON array of option IDs
```json
{
  "questionId": 1,
  "answer": "[\"a\", \"c\"]",
  "timeSpentSeconds": 45
}
```

**Validation:** All correct options must be selected

### TrueFalse

**Answer Format:** Boolean string
```json
{
  "questionId": 2,
  "answer": "true",
  "timeSpentSeconds": 10
}
```

**Validation:** Must match the correct boolean value

### Fill

**Answer Format:** String (code snippet)
```json
{
  "questionId": 3,
  "answer": "string name = \"John\";",
  "timeSpentSeconds": 60
}
```

**Validation:** Normalized string comparison (ignores code formatting)

### ErrorSpotting

**Answer Format:** String (error description or code)
```json
{
  "questionId": 4,
  "answer": "Cannot implicitly convert type 'int' to 'string'",
  "timeSpentSeconds": 90
}
```

**Validation:** Normalized string comparison

### OutputPrediction

**Answer Format:** String (expected output)
```json
{
  "questionId": 5,
  "answer": "8",
  "timeSpentSeconds": 15
}
```

**Validation:** Case-insensitive string comparison

### CodeWriting

**Answer Format:** String (code solution)
```json
{
  "questionId": 6,
  "answer": "public static int Factorial(int n) { return n <= 1 ? 1 : n * Factorial(n - 1); }",
  "timeSpentSeconds": 300
}
```

**Validation:** Non-empty string (manual review required)

## Data Models

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

## Error Responses

### Invalid Answer Format (400)
```json
{
  "success": false,
  "message": "Invalid answer format for question type",
  "data": null
}
```

### Question Not Found (404)
```json
{
  "success": false,
  "message": "Question not found",
  "data": null
}
```

### Validation Error (400)
```json
{
  "success": false,
  "message": "Answer validation failed",
  "data": null
}
```

## Usage Examples

### Submit MCQ Answer

```bash
curl -X POST \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 1,
    "answer": "[\"a\", \"c\"]",
    "timeSpentSeconds": 45
  }' \
  http://localhost:5138/api/csharp/answers
```

### Submit TrueFalse Answer

```bash
curl -X POST \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 2,
    "answer": "true",
    "timeSpentSeconds": 10
  }' \
  http://localhost:5138/api/csharp/answers
```

### Get Answer History

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/answers/1
```

### JavaScript Example

```javascript
const submitAnswer = async (questionId, answer, timeSpent) => {
  const response = await fetch('http://localhost:5138/api/csharp/answers', {
    method: 'POST',
    headers: {
      'Authorization': 'Bearer ' + token,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      questionId: questionId,
      answer: answer,
      timeSpentSeconds: timeSpent
    })
  });

  const data = await response.json();
  return data.data;
};

// Example usage
const result = await submitAnswer(1, '["a", "c"]', 45);
console.log(result.isCorrect); // true/false
```

### C# Example

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var request = new SubmitAnswerRequest
{
    QuestionId = 1,
    Answer = "[\"a\", \"c\"]",
    TimeSpentSeconds = 45
};

var json = JsonSerializer.Serialize(request);
var content = new StringContent(json, Encoding.UTF8, "application/json");

var response = await client.PostAsync("http://localhost:5138/api/csharp/answers", content);
var responseContent = await response.Content.ReadAsStringAsync();
var result = JsonSerializer.Deserialize<ApiResponse<SubmitAnswerResponse>>(responseContent);
```

## Validation Rules

### MCQ Questions
- Answer must be a valid JSON array
- All correct options must be selected
- No incorrect options should be selected

### TrueFalse Questions
- Answer must be "true" or "false" (case-insensitive)
- Must match the correct boolean value

### Fill Questions
- Answer is normalized (removes code formatting)
- Case-sensitive comparison with correct answer

### ErrorSpotting Questions
- Answer is normalized (removes code formatting)
- Case-sensitive comparison with correct answer

### OutputPrediction Questions
- Answer is trimmed and compared case-insensitively
- Must match expected output exactly

### CodeWriting Questions
- Answer must not be empty
- Manual review may be required for complex solutions

## Best Practices

1. **Time Tracking**: Always include accurate time spent
2. **Answer Format**: Follow the correct format for each question type
3. **Error Handling**: Handle validation errors gracefully
4. **Retry Logic**: Implement retry logic for network issues
5. **Progress Tracking**: Use answer history for progress indicators

## Rate Limiting

- **Answer Submission**: 30 requests per minute per user
- **Answer History**: 50 requests per minute per user

## Notes

- Answers are immutable once submitted
- Attempt numbers are automatically incremented
- Time tracking helps with analytics and difficulty assessment
- Explanations are provided for incorrect answers
- Answer history is user-specific and private 