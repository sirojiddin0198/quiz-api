namespace Quiz.Infrastructure.Exceptions;

public class CustomConflictException(string errorMessage)
: Exception(errorMessage) { }