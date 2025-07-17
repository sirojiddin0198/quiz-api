# API Examples

This document provides comprehensive examples and usage scenarios for the Quiz API.

## Getting Started

### 1. Authentication

First, obtain a JWT token from your authentication provider (Keycloak):

```bash
# Example: Get token from Keycloak
curl -X POST \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&client_id=quiz-api&username=your-username&password=your-password" \
  https://your-keycloak-url/auth/realms/your-realm/protocol/openid-connect/token
```

### 2. Set Authorization Header

```bash
# Set your token for subsequent requests
TOKEN="eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## Complete Workflow Examples

### Scenario 1: Complete Quiz Session

This example shows a complete workflow from browsing collections to submitting answers.

#### Step 1: Get Collections

```bash
curl -H "Authorization: Bearer $TOKEN" \
     http://localhost:5138/api/csharp/collections
```

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": 1,
      "code": "csharp-fundamentals",
      "title": "C# Fundamentals",
      "description": "Learn the basics of C# programming language",
      "icon": "code",
      "sortOrder": 1,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

#### Step 2: Get Preview Questions

```bash
curl "http://localhost:5138/api/csharp/questions/preview?collectionId=1"
```

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
  ]
}
```

#### Step 3: Get Questions (Paginated)

```bash
curl -H "Authorization: Bearer $TOKEN" \
     "http://localhost:5138/api/csharp/questions?collectionId=1&page=1&pageSize=5"
```

#### Step 4: Submit Answer

```bash
curl -X POST \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 1,
    "answer": "[\"a\"]",
    "timeSpentSeconds": 45
  }' \
  http://localhost:5138/api/csharp/answers
```

**Response:**
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 123,
    "questionId": 1,
    "answer": "[\"a\"]",
    "isCorrect": true,
    "timeSpentSeconds": 45,
    "submittedAt": "2024-01-01T10:00:00Z",
    "attemptNumber": 1,
    "explanation": "In C#, you can use either 'var' for implicit typing or explicit type declaration."
  }
}
```

### Scenario 2: Different Question Types

#### MCQ Question

```bash
# Get MCQ question
curl -H "Authorization: Bearer $TOKEN" \
     http://localhost:5138/api/csharp/questions/1

# Submit MCQ answer
curl -X POST \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 1,
    "answer": "[\"a\", \"c\"]",
    "timeSpentSeconds": 30
  }' \
  http://localhost:5138/api/csharp/answers
```

#### TrueFalse Question

```bash
# Submit TrueFalse answer
curl -X POST \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 2,
    "answer": "true",
    "timeSpentSeconds": 10
  }' \
  http://localhost:5138/api/csharp/answers
```

#### Fill Question

```bash
# Submit Fill answer
curl -X POST \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 3,
    "answer": "string name = \"John\";",
    "timeSpentSeconds": 60
  }' \
  http://localhost:5138/api/csharp/answers
```

#### CodeWriting Question

```bash
# Submit CodeWriting answer
curl -X POST \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "questionId": 6,
    "answer": "public static int Factorial(int n) { return n <= 1 ? 1 : n * Factorial(n - 1); }",
    "timeSpentSeconds": 300
  }' \
  http://localhost:5138/api/csharp/answers
```

## JavaScript Examples

### Complete Quiz Application

```javascript
class QuizAPI {
  constructor(baseUrl, token) {
    this.baseUrl = baseUrl;
    this.token = token;
  }

  async getCollections() {
    const response = await fetch(`${this.baseUrl}/api/csharp/collections`, {
      headers: {
        'Authorization': `Bearer ${this.token}`,
        'Content-Type': 'application/json'
      }
    });
    return response.json();
  }

  async getQuestions(collectionId, page = 1, pageSize = 10) {
    const response = await fetch(
      `${this.baseUrl}/api/csharp/questions?collectionId=${collectionId}&page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          'Authorization': `Bearer ${this.token}`,
          'Content-Type': 'application/json'
        }
      }
    );
    return response.json();
  }

  async submitAnswer(questionId, answer, timeSpent) {
    const response = await fetch(`${this.baseUrl}/api/csharp/answers`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${this.token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        questionId: questionId,
        answer: answer,
        timeSpentSeconds: timeSpent
      })
    });
    return response.json();
  }

  async getAnswerHistory(questionId) {
    const response = await fetch(`${this.baseUrl}/api/csharp/answers/${questionId}`, {
      headers: {
        'Authorization': `Bearer ${this.token}`,
        'Content-Type': 'application/json'
      }
    });
    return response.json();
  }
}

// Usage example
const quiz = new QuizAPI('http://localhost:5138', 'your-token');

// Get collections
const collections = await quiz.getCollections();
console.log('Collections:', collections.data);

// Get questions for first collection
const questions = await quiz.getQuestions(1);
console.log('Questions:', questions.data.items);

// Submit answer
const result = await quiz.submitAnswer(1, '["a"]', 45);
console.log('Answer result:', result.data);
```

### React Hook Example

```javascript
import { useState, useEffect } from 'react';

const useQuizAPI = (token) => {
  const [collections, setCollections] = useState([]);
  const [questions, setQuestions] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const baseUrl = 'http://localhost:5138';

  const fetchCollections = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${baseUrl}/api/csharp/collections`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      const data = await response.json();
      if (data.success) {
        setCollections(data.data);
      } else {
        setError(data.message);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchQuestions = async (collectionId, page = 1) => {
    try {
      setLoading(true);
      const response = await fetch(
        `${baseUrl}/api/csharp/questions?collectionId=${collectionId}&page=${page}`,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        }
      );
      const data = await response.json();
      if (data.success) {
        setQuestions(data.data.items);
      } else {
        setError(data.message);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const submitAnswer = async (questionId, answer, timeSpent) => {
    try {
      const response = await fetch(`${baseUrl}/api/csharp/answers`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          questionId: questionId,
          answer: answer,
          timeSpentSeconds: timeSpent
        })
      });
      const data = await response.json();
      return data;
    } catch (err) {
      setError(err.message);
      return null;
    }
  };

  return {
    collections,
    questions,
    loading,
    error,
    fetchCollections,
    fetchQuestions,
    submitAnswer
  };
};

// Usage in React component
const QuizComponent = ({ token }) => {
  const {
    collections,
    questions,
    loading,
    error,
    fetchCollections,
    fetchQuestions,
    submitAnswer
  } = useQuizAPI(token);

  useEffect(() => {
    fetchCollections();
  }, []);

  const handleAnswerSubmit = async (questionId, answer, timeSpent) => {
    const result = await submitAnswer(questionId, answer, timeSpent);
    if (result?.success) {
      console.log('Answer submitted successfully:', result.data);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      <h2>Collections</h2>
      {collections.map(collection => (
        <div key={collection.id}>
          <h3>{collection.title}</h3>
          <p>{collection.description}</p>
          <button onClick={() => fetchQuestions(collection.id)}>
            View Questions
          </button>
        </div>
      ))}

      <h2>Questions</h2>
      {questions.map(question => (
        <div key={question.id}>
          <h3>{question.content.prompt}</h3>
          {question.type === 'MCQ' && question.options && (
            <div>
              {question.options.map(option => (
                <label key={option.id}>
                  <input type="checkbox" value={option.id} />
                  {option.option}
                </label>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
};
```

## C# Examples

### HttpClient Wrapper

```csharp
public class QuizApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public QuizApiClient(string baseUrl, string token)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<ApiResponse<List<CollectionResponse>>> GetCollectionsAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/csharp/collections");
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<List<CollectionResponse>>>(content);
    }

    public async Task<PaginatedApiResponse<QuestionResponse>> GetQuestionsAsync(
        int collectionId, int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync(
            $"{_baseUrl}/api/csharp/questions?collectionId={collectionId}&page={page}&pageSize={pageSize}");
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaginatedApiResponse<QuestionResponse>>(content);
    }

    public async Task<ApiResponse<SubmitAnswerResponse>> SubmitAnswerAsync(
        int questionId, string answer, int timeSpentSeconds)
    {
        var request = new SubmitAnswerRequest
        {
            QuestionId = questionId,
            Answer = answer,
            TimeSpentSeconds = timeSpentSeconds
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/csharp/answers", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<SubmitAnswerResponse>>(responseContent);
    }

    public async Task<ApiResponse<List<UserAnswerResponse>>> GetAnswerHistoryAsync(int questionId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/csharp/answers/{questionId}");
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<List<UserAnswerResponse>>>(content);
    }
}

// Usage example
var client = new QuizApiClient("http://localhost:5138", "your-token");

// Get collections
var collections = await client.GetCollectionsAsync();
if (collections.Success)
{
    foreach (var collection in collections.Data)
    {
        Console.WriteLine($"Collection: {collection.Title}");
    }
}

// Get questions
var questions = await client.GetQuestionsAsync(1);
if (questions.Success)
{
    foreach (var question in questions.Data.Items)
    {
        Console.WriteLine($"Question: {question.Content.Prompt}");
    }
}

// Submit answer
var result = await client.SubmitAnswerAsync(1, "[\"a\"]", 45);
if (result.Success)
{
    Console.WriteLine($"Answer correct: {result.Data.IsCorrect}");
}
```

## Error Handling Examples

### Handling API Errors

```javascript
const handleApiError = (response) => {
  if (!response.ok) {
    switch (response.status) {
      case 401:
        throw new Error('Unauthorized - Please login again');
      case 404:
        throw new Error('Resource not found');
      case 429:
        throw new Error('Too many requests - Please wait');
      case 500:
        throw new Error('Server error - Please try again later');
      default:
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
  }
  return response.json();
};

// Usage
try {
  const response = await fetch('/api/csharp/collections', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  const data = await handleApiError(response);
  console.log(data);
} catch (error) {
  console.error('API Error:', error.message);
}
```

### Retry Logic

```javascript
const retryRequest = async (fn, maxRetries = 3, delay = 1000) => {
  for (let i = 0; i < maxRetries; i++) {
    try {
      return await fn();
    } catch (error) {
      if (i === maxRetries - 1) throw error;
      await new Promise(resolve => setTimeout(resolve, delay * (i + 1)));
    }
  }
};

// Usage
const submitAnswerWithRetry = async (questionId, answer, timeSpent) => {
  return retryRequest(async () => {
    const response = await fetch('/api/csharp/answers', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ questionId, answer, timeSpentSeconds: timeSpent })
    });
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }
    
    return response.json();
  });
};
```

## Testing Examples

### Unit Tests

```csharp
[Test]
public async Task SubmitAnswer_ValidMCQAnswer_ReturnsSuccess()
{
    // Arrange
    var client = new QuizApiClient("http://localhost:5138", "test-token");
    var questionId = 1;
    var answer = "[\"a\"]";
    var timeSpent = 45;

    // Act
    var result = await client.SubmitAnswerAsync(questionId, answer, timeSpent);

    // Assert
    Assert.IsTrue(result.Success);
    Assert.IsTrue(result.Data.IsCorrect);
    Assert.AreEqual(questionId, result.Data.QuestionId);
    Assert.AreEqual(answer, result.Data.Answer);
}
```

### Integration Tests

```csharp
[Test]
public async Task CompleteQuizWorkflow_ShouldWorkEndToEnd()
{
    // Arrange
    var client = new QuizApiClient("http://localhost:5138", "test-token");

    // Act & Assert
    // 1. Get collections
    var collections = await client.GetCollectionsAsync();
    Assert.IsTrue(collections.Success);
    Assert.IsNotEmpty(collections.Data);

    // 2. Get questions for first collection
    var questions = await client.GetQuestionsAsync(collections.Data[0].Id);
    Assert.IsTrue(questions.Success);
    Assert.IsNotEmpty(questions.Data.Items);

    // 3. Submit answer for first question
    var question = questions.Data.Items[0];
    var answer = question.Type == "MCQ" ? "[\"a\"]" : "true";
    var result = await client.SubmitAnswerAsync(question.Id, answer, 30);
    Assert.IsTrue(result.Success);
}
``` 