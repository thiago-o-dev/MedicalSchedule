namespace Registry.Domain.Exceptions;

public class DuplicateOwnerException(string message) : Exception(message);