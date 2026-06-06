# WHY THIS EXISTS

You explicitly need:
- distributed locks
- idempotency
- caching

This becomes reusable infra.

Idempotency:
The property of an operation that allows it to be performed multiple times without changing the final result beyond the initial execution. 
Whether you run it once or 100 times, the outcome is exactly the same.