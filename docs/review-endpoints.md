# Review Endpoints Documentation

## Overview
These endpoints allow users to review their answers and get detailed results after completing questions. They provide access to correct answers, explanations, and performance metrics.

## Endpoints

### 1. Get Detailed Answer Review
**GET** `/api/csharp/results/collections/{collectionId}/review?includeUnanswered=false`

Returns detailed review of all answered questions with correct answers and explanations.

**Authentication:** Required

**Query Parameters:**
- `includeUnanswered` (optional): Include questions that haven't been answered (default: false)

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "questionId": 1,
      "questionType": "MCQ",
      "prompt": "What is the correct answer?",
      "content": {
        "codeBefore": "```csharp\n// Code here\n```",
        "codeAfter": "",
        "codeWithBlank": null,
        "codeWithError": null,
        "snippet": null
      },
      "userAnswer": {
        "answer": "A",
        "isCorrect": true,
        "submittedAt": "2024-01-15T10:25:00Z",
        "timeSpentSeconds": 45
      },
      "correctAnswer": {
        "options": [
          {
            "id": "A",
            "text": "Correct option",
            "isCorrect": true
          },
          {
            "id": "B",
            "text": "Wrong option",
            "isCorrect": false
          }
        ],
        "booleanAnswer": null,
        "textAnswer": null,
        "sampleSolution": null,
        "testCaseResults": null
      },
      "explanation": "This is the correct answer because...",
      "hints": ["Hint 1", "Hint 2"]
    }
  ]
}
```

### 2. Complete Session (for Preview/Anonymous Users)
**POST** `/api/csharp/results/sessions/{sessionId}/complete`

Allows anonymous users to submit all answers at once and get immediate results.

**Authentication:** Not required

**Request Body:**
```json
{
  "answers": [
    {
      "questionId": 1,
      "answer": "A",
      "timeSpentSeconds": 45
    },
    {
      "questionId": 2,
      "answer": "true",
      "timeSpentSeconds": 30
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "sessionId": "preview-session-123",
    "totalQuestions": 2,
    "correctAnswers": 1,
    "scorePercentage": 50.0,
    "reviewItems": [
      {
        "questionId": 1,
        "questionType": "MCQ",
        "prompt": "What is the correct answer?",
        "content": {
          "codeBefore": "```csharp\n// Code here\n```",
          "codeAfter": "",
          "codeWithBlank": null,
          "codeWithError": null,
          "snippet": null
        },
        "userAnswer": {
          "answer": "A",
          "isCorrect": true,
          "submittedAt": "2024-01-15T10:30:00Z",
          "timeSpentSeconds": 45
        },
        "correctAnswer": {
          "options": [
            {
              "id": "A",
              "text": "Correct option",
              "isCorrect": true
            }
          ]
        },
        "explanation": "This is the correct answer because...",
        "hints": ["Hint 1"]
      }
    ]
  }
}
```

## Question Types and Correct Answer Structure

### MCQ Questions
```json
{
  "correctAnswer": {
    "options": [
      {
        "id": "A",
        "text": "Option A",
        "isCorrect": true
      },
      {
        "id": "B", 
        "text": "Option B",
        "isCorrect": false
      }
    ]
  }
}
```

### True/False Questions
```json
{
  "correctAnswer": {
    "booleanAnswer": true
  }
}
```

### Fill Questions
```json
{
  "correctAnswer": {
    "textAnswer": "public static T AddAll<T>(T[] values)\n    where T : INumber<T>, IAdditionOperators<T, T, T>"
  }
}
```

### Error Spotting Questions
```json
{
  "correctAnswer": {
    "textAnswer": "public class Calculator<T> : ICalc<T>\n    where T : IAdditionOperators<T, T, T>"
  }
}
```

### Output Prediction Questions
```json
{
  "correctAnswer": {
    "textAnswer": "AB"
  }
}
```

### Code Writing Questions
```json
{
  "correctAnswer": {
    "sampleSolution": "public abstract class Shape\n{\n    public abstract double GetArea();\n}",
    "testCaseResults": [
      {
        "input": "Circle with radius 2.5",
        "expectedOutput": "19.63",
        "userOutput": null,
        "passed": false
      }
    ]
  }
}
```

## Usage Scenarios

### 1. Authenticated Users
- Use endpoints 1 and 2 to review completed collections
- Answers are stored in the database
- Can review progress over time

### 2. Preview/Anonymous Users
- Use endpoint 3 to submit all answers at once
- Get immediate results without storing data
- Perfect for previewing question types

### 3. Results Page Implementation
1. After completing questions, redirect to results page
2. Call the review endpoint to get detailed feedback
3. Display correct answers, explanations, and performance metrics
4. Allow users to review each question individually

## Error Handling

All endpoints return consistent error responses:
```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

Common error scenarios:
- User not authenticated (for protected endpoints)
- Collection not found
- No progress found for collection
- Invalid session ID
- Malformed request data 