using SharedKernel.Exceptions;

namespace Registry.Domain.Exceptions;

public class DuplicateVetException(string message) : ConflictException(message);
