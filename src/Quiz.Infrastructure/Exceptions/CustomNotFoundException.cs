namespace Quiz.Infrastructure.Exceptions;

public class CustomNotFoundException(string errorMessage)
: Exception(errorMessage) { }