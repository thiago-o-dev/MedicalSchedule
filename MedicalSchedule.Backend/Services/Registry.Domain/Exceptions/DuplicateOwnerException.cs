using SharedKernel.Exceptions;

namespace Registry.Domain.Exceptions;

public class DuplicateOwnerException(string message) : ConflictException(message);
