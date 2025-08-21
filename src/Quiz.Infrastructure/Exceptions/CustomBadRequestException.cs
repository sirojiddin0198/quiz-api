namespace Quiz.Infrastructure.Exceptions;

public class CustomBadRequestException(string errorMessage)
: Exception(errorMessage) { } 