# Collection Management API Documentation

## Overview

The Collection Management endpoint allows administrators to create new quiz collections with questions in a single API call. This endpoint is designed for bulk creation of educational content and requires administrative privileges.

**Endpoint:** `POST /api/csharp/management/collections`  
**Authorization:** Requires `Admin:Write` resource role  
**Content-Type:** `application/json`

## Request Structure

### Collection Properties

| Field | Type | Required | Max Length | Description |
|-------|------|----------|------------|-------------|
| `code` | string | Yes | 50 | Unique identifier for the collection. Must contain only lowercase letters, numbers, and hyphens (regex: `^[a-z0-9-]+$`) |
| `title` | string | Yes | 200 | Display name of the collection |
| `description` | string | Yes | 1000 | Detailed description of the collection content |
| `icon` | string | Yes | 50 | Icon identifier for UI display |
| `sortOrder` | integer | No | - | Display order (≥ 0, default: 0) |
| `questions` | array | Yes | - | Array of questions (minimum 1 required) |

### Question Properties

| Field | Type | Required | Max Length | Description |
|-------|------|----------|------------|-------------|
| `type` | string | Yes | - | Question type (see [Question Types](#question-types)) |
| `subcategory` | string | Yes | 200 | Topic/subcategory within the collection |
| `difficulty` | string | Yes | - | `Beginner`, `Intermediate`, or `Advanced` |
| `prompt` | string | Yes | 2000 | Question text (see [Prompt Guidelines](#prompt-guidelines)) |
| `estimatedTimeMinutes` | integer | Yes | - | Expected completion time (1-120 minutes) |
| `metadata` | string | Yes | - | JSON string containing question-specific data |

## Question Types

### Question Structure Overview

All questions share these common fields:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `type` | string | Yes | Question type identifier |
| `subcategory` | string | Yes | Topic/subcategory within the collection |
| `difficulty` | string | Yes | `Beginner`, `Intermediate`, or `Advanced` |
| `prompt` | string | Yes | Question text in Uzbek with markdown formatting |
| `estimatedTimeMinutes` | integer | Yes | Expected completion time (1-120 minutes) |
| `metadata` | string | Yes | JSON string containing complete question data structure |

**Critical:** The `metadata` field must contain a JSON string with the exact structure used in the database, including all required fields for each question type.

### Common Metadata Fields (All Question Types)

Every question metadata JSON must include these fields:

```json
{
  "Hints": [
    {
      "Hint": "Detailed hint text in Uzbek",
      "OrderIndex": 1
    }
  ],
  "CodeAfter": "",
  "CodeBefore": "```csharp\n// Code snippet if needed\n```",
  "Explanation": "Detailed explanation in Uzbek"
}
```

### 1. Multiple Choice Questions (MCQ)
**Type:** `"mcq"`

**Complete Metadata JSON Structure:**
```json
{
  "Hints": [
    {
      "Hint": "Explanation hint in Uzbek",
      "OrderIndex": 1
    }
  ],
  "Options": [
    {
      "Id": "A",
      "Text": "First option text",
      "IsCorrect": true
    },
    {
      "Id": "B",
      "Text": "Second option text", 
      "IsCorrect": false
    }
  ],
  "CodeAfter": "",
  "CodeBefore": "```csharp\n// Code snippet\n```",
  "Explanation": "Detailed explanation in Uzbek",
  "CorrectAnswerIds": ["A"]
}
```

**Required Fields:**
- `Options`: Array of answer choices with `Id`, `Text`, `IsCorrect`
- `CorrectAnswerIds`: Array of correct option IDs

### 2. True/False Questions
**Type:** `"true_false"`

**Complete Metadata JSON Structure:**
```json
{
  "Hints": [
    {
      "Hint": "Explanation hint in Uzbek",
      "OrderIndex": 1
    }
  ],
  "CodeAfter": "",
  "CodeBefore": "",
  "Explanation": "Detailed explanation in Uzbek",
  "CorrectAnswer": true
}
```

**Required Fields:**
- `CorrectAnswer`: Boolean value (`true` or `false`)

### 3. Fill-in-the-Blank Questions
**Type:** `"fill"`

**Complete Metadata JSON Structure:**
```json
{
  "Hints": [
    {
      "Hint": "Explanation hint in Uzbek",
      "OrderIndex": 1
    }
  ],
  "CodeAfter": "",
  "FillHints": [],
  "CodeBefore": "",
  "Explanation": "Detailed explanation in Uzbek",
  "CodeWithBlank": "Code with ______ blanks to fill",
  "CorrectAnswer": "Complete code with blanks filled correctly"
}
```

**Required Fields:**
- `CodeWithBlank`: Code with blanks marked as `______`
- `CorrectAnswer`: Complete correct code/text
- `FillHints`: Empty array (reserved for future use)

### 4. Error Spotting Questions
**Type:** `"error_spotting"`

**Complete Metadata JSON Structure:**
```json
{
  "Hints": [
    {
      "Hint": "Explanation hint in Uzbek", 
      "OrderIndex": 1
    }
  ],
  "CodeAfter": "",
  "CodeBefore": "",
  "Explanation": "Detailed explanation of errors and fixes",
  "CodeWithError": "Code containing errors to identify",
  "CorrectAnswer": "Corrected version of the code"
}
```

**Required Fields:**
- `CodeWithError`: Code snippet containing errors
- `CorrectAnswer`: Corrected version of the code

### 5. Output Prediction Questions
**Type:** `"output_prediction"`

**Complete Metadata JSON Structure:**
```json
{
  "Hints": [
    {
      "Hint": "Explanation hint in Uzbek",
      "OrderIndex": 1
    }
  ],
  "Snippet": "Code to analyze for output prediction",
  "CodeAfter": "",
  "CodeBefore": "",
  "Explanation": "Detailed explanation of the output",
  "ExpectedOutput": "Expected program output"
}
```

**Required Fields:**
- `Snippet`: Code to analyze
- `ExpectedOutput`: Expected program output

### 6. Code Writing Questions
**Type:** `"code_writing"`

**Complete Metadata JSON Structure:**
```json
{
  "Hints": [],
  "Rubric": [],
  "Examples": [
    "Example input/output scenario 1",
    "Example input/output scenario 2"
  ],
  "Solution": "",
  "CodeAfter": "",
  "TestCases": [],
  "CodeBefore": "",
  "Explanation": ""
}
```

**Required Fields:**
- `Examples`: Array of example scenarios
- `Solution`: Empty string (for future use)
- `TestCases`: Empty array (for future use)
- `Rubric`: Empty array (for future use)

## Important Implementation Notes

### Metadata JSON String Format
The `metadata` field in the API request must be a **properly escaped JSON string** containing the complete metadata object. All quotes must be properly escaped.

### Example Metadata String for MCQ:
```json
"{\"Hints\":[{\"Hint\":\"Detailed explanation\",\"OrderIndex\":1}],\"Options\":[{\"Id\":\"A\",\"Text\":\"Option A\",\"IsCorrect\":true},{\"Id\":\"B\",\"Text\":\"Option B\",\"IsCorrect\":false}],\"CodeAfter\":\"\",\"CodeBefore\":\"\",\"Explanation\":\"Detailed explanation\",\"CorrectAnswerIds\":[\"A\"]}"
```

### Validation Rules
- All metadata must include `Hints`, `CodeAfter`, `CodeBefore`, `Explanation` fields
- `Hints` array must have at least one hint with `OrderIndex: 1`
- Code snippets must use proper C# syntax with triple backticks
- All text content must be in Uzbek with English technical terms
- Proper JSON escaping is critical for nested quotes

### Field Requirements by Type
- **MCQ**: Must have `Options` array and `CorrectAnswerIds` array
- **True/False**: Must have `CorrectAnswer` boolean
- **Fill**: Must have `CodeWithBlank`, `CorrectAnswer`, and empty `FillHints` array
- **Error Spotting**: Must have `CodeWithError` and `CorrectAnswer`
- **Output Prediction**: Must have `Snippet` and `ExpectedOutput`
- **Code Writing**: Must have `Examples` array and empty arrays for `Rubric`, `TestCases`

## Content Guidelines

### Prompt Guidelines

#### General Rules
1. **Language**: All prompts must be written in **Uzbek language**
2. **Clarity**: Use clear, concise language appropriate for the target difficulty level
3. **Context**: Provide sufficient context without being verbose
4. **Technical Terms**: Use English technical terms with Uzbek explanations when necessary

#### Markdown Formatting
- Use `**bold**` for emphasis on key concepts
- Use `*italic*` for variable names and parameters
- Use backticks for inline code: `` `Console.WriteLine()` ``
- Use code blocks for multi-line code:
  ```csharp
  public class Example
  {
      // code here
  }
  ```
- Use bullet points for lists of requirements or steps
- Use numbered lists for sequential instructions

#### Difficulty Specifications

**Beginner Level:**
- Focus on basic syntax and fundamental concepts
- Provide more context and explanation in the prompt
- Use simple, straightforward examples
- Avoid complex nested structures or advanced patterns

**Intermediate Level:**
- Assume familiarity with basic concepts
- Introduce more complex scenarios and patterns
- May require understanding of multiple concepts together
- Include practical, real-world examples

**Advanced Level:**
- Assume deep knowledge of the language and frameworks
- Focus on optimization, best practices, and edge cases
- May involve complex architectural decisions
- Require understanding of performance implications

### Uzbek Language Specifications

#### Technical Translation Guidelines
- **Class** → "Sinf" yoki "Class"
- **Method** → "Metod" yoki "Method"
- **Property** → "Property"
- **Interface** → "Interfeys" yoki "Interface"
- **Inheritance** → "Meros olish"
- **Polymorphism** → "Polimorfizm"
- **Encapsulation** → "Inkapsulyatsiya"
- **Dependency Injection** → "Dependency Injection"

#### Writing Style
1. Use formal Uzbek language appropriate for educational content
2. Maintain consistency in technical term usage
3. Provide brief explanations for complex English terms
4. Use active voice when possible
5. Structure sentences clearly with proper punctuation

### Code-Related Question Guidelines

#### Code Formatting
- Always use proper indentation (4 spaces for C#)
- Include necessary using statements
- Use meaningful variable and method names
- Follow C# naming conventions (PascalCase, camelCase)

#### Code Examples
- Keep examples concise but complete
- Ensure code compiles and runs correctly
- Include relevant comments in English
- Use realistic scenarios when possible

#### Error Examples
- Create subtle, educational errors
- Focus on common mistakes developers make
- Ensure errors are at appropriate difficulty level
- Provide clear explanations of why code is incorrect

## Response Format

### Success Response (201 Created)
```json
{
  "success": true,
  "message": "Collection created successfully",
  "data": {
    "id": 123,
    "code": "collection-code",
    "title": "Collection Title",
    "description": "Collection Description",
    "icon": "icon-name",
    "sortOrder": 10,
    "questionsCreated": 12,
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

### Error Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    {
      "field": "code",
      "message": "Collection code must contain only lowercase letters, numbers, and hyphens"
    }
  ]
}
```

## Common Validation Errors

| Error | Description | Solution |
|-------|-------------|----------|
| Collection code already exists | Duplicate collection code | Use a unique code identifier |
| Invalid question type | Unsupported question type | Use one of the six supported types |
| Invalid JSON metadata | Malformed JSON in metadata field | Validate JSON syntax |
| Missing required fields | Required fields not provided | Ensure all required fields are included |
| Invalid difficulty level | Difficulty not in allowed values | Use "Beginner", "Intermediate", or "Advanced" |

## Best Practices

1. **Test metadata JSON** before submitting requests
2. **Use meaningful collection codes** that reflect the content
3. **Balance question types** within a collection
4. **Provide clear explanations** in metadata when possible
5. **Follow difficulty progression** from beginner to advanced
6. **Use consistent terminology** throughout the collection
7. **Include realistic code examples** that students might encounter
8. **Validate all code snippets** to ensure they compile correctly 