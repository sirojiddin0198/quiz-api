namespace Quiz.CSharp.Data.Models;

using System.Text.Json.Serialization;

public class SeedCollectionMetadata
{
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class SeedQuestionMetadata
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
    
    [JsonPropertyName("subcategory")]
    public string Subcategory { get; set; } = string.Empty;
}

public class SeedQuestionOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("option")]
    public string Option { get; set; } = string.Empty;
}

public class SeedQuestion
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("metadata")]
    public SeedQuestionMetadata Metadata { get; set; } = new();
    
    [JsonPropertyName("codeBefore")]
    public string CodeBefore { get; set; } = string.Empty;
    
    [JsonPropertyName("codeAfter")]
    public string CodeAfter { get; set; } = string.Empty;
    
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;
    
    [JsonPropertyName("options")]
    public List<SeedQuestionOption> Options { get; set; } = [];
    
    [JsonPropertyName("answer")]
    public List<string> Answer { get; set; } = [];
    
    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;
    
    [JsonPropertyName("codeWithBlank")]
    public string CodeWithBlank { get; set; } = string.Empty;
    
    [JsonPropertyName("codeWithError")]
    public string CodeWithError { get; set; } = string.Empty;
    
    [JsonPropertyName("snippet")]
    public string Snippet { get; set; } = string.Empty;
    
    [JsonPropertyName("solution")]
    public string Solution { get; set; } = string.Empty;
    
    [JsonPropertyName("testCases")]
    public List<SeedTestCase> TestCases { get; set; } = [];
    
    [JsonPropertyName("examples")]
    public List<string> Examples { get; set; } = [];
    
    [JsonPropertyName("rubric")]
    public List<string> Rubric { get; set; } = [];
}

public class SeedTestCase
{
    [JsonPropertyName("input")]
    public string Input { get; set; } = string.Empty;
    
    [JsonPropertyName("expectedOutput")]
    public string ExpectedOutput { get; set; } = string.Empty;
}

public class SeedQuestionFile
{
    [JsonPropertyName("metadata")]
    public SeedCollectionMetadata Metadata { get; set; } = new();
    
    [JsonPropertyName("questions")]
    public List<SeedQuestion> Questions { get; set; } = [];
} 