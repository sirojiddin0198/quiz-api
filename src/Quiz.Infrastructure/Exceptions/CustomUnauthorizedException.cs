namespace Quiz.Infrastructure.Exceptions;

public class CustomUnauthorizedException(string errorMessage)
: Exception(errorMessage) { } 