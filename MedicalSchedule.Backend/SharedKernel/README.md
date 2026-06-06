# WHAT SHAREDKERNEL ACTUALLY IS

Universal primitives.

Things EVERY service can safely use.

Examples:
- Result<T>
- Entity
- AggregateRoot
- ValueObject

NOT:
- Appointment
- Owner
- Subscription
- Enums from domains

Otherwise services become coupled forever.