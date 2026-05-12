namespace MedicalSchedule.Domain.Exceptions;

public class DomainValidationException(string message) : Exception(message);
