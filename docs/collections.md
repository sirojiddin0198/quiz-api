# Collections API

The Collections API provides endpoints for managing and retrieving quiz collections. Collections organize questions into themed groups for better user experience and content management.

## Overview

Collections represent themed groups of questions, such as:
- C# Fundamentals
- Object-Oriented Programming
- LINQ and Collections
- Async Programming
- Design Patterns

## Base URL

```
http://localhost:5138/api/csharp/collections
```

## Endpoints

### Get All Collections

Retrieves a list of all available collections.

**Endpoint:** `GET /api/csharp/collections`

**Authentication:** Required

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
      "updatedAt": "2024-01-01T00:00:00Z",
      "totalQuestions": 15
    },
    {
      "id": 2,
      "code": "oop-concepts",
      "title": "Object-Oriented Programming",
      "description": "Master OOP concepts in C#",
      "icon": "class",
      "sortOrder": 2,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z",
      "totalQuestions": 12
    }
  ]
}
```

**Status Codes:**
- `200` - Success
- `401` - Unauthorized
- `500` - Internal Server Error



## Data Models

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
  "updatedAt": "datetime",
  "totalQuestions": "integer"
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
- `totalQuestions` (int): Total number of questions in the collection

## Error Responses

### Collection Not Found (404)
```json
{
  "success": false,
  "message": "Collection not found",
  "data": null
}
```

### Unauthorized (401)
```json
{
  "success": false,
  "message": "Unauthorized",
  "data": null
}
```

### Internal Server Error (500)
```json
{
  "success": false,
  "message": "An error occurred while processing your request",
  "data": null
}
```

## Usage Examples

### Get All Collections

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/collections
```

### Get Collection by ID

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/collections/1
```

### Get Collection by Code

```bash
curl -H "Authorization: Bearer <your-token>" \
     http://localhost:5138/api/csharp/collections/code/csharp-fundamentals
```

### JavaScript Example

```javascript
const response = await fetch('http://localhost:5138/api/csharp/collections', {
  headers: {
    'Authorization': 'Bearer ' + token,
    'Content-Type': 'application/json'
  }
});

const data = await response.json();
console.log(data.data); // Array of collections
```

### C# Example

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var response = await client.GetAsync("http://localhost:5138/api/csharp/collections");
var content = await response.Content.ReadAsStringAsync();
var result = JsonSerializer.Deserialize<ApiResponse<List<CollectionResponse>>>(content);
```

## Best Practices

1. **Caching**: Cache collection data on the client side
2. **Error Handling**: Always check the `success` field in responses
3. **Pagination**: Collections are typically small, so pagination is not implemented
4. **Sorting**: Collections are returned in `sortOrder` sequence
5. **Icons**: Use consistent icon identifiers across your application

## Rate Limiting

- **Collections**: 100 requests per minute per user
- **Individual Collection**: 50 requests per minute per user

## Notes

- Collections are read-only in the current API version
- All collections are active by default
- Collection codes are URL-friendly and unique
- Icons are referenced by identifier (implement icon mapping in your frontend) 