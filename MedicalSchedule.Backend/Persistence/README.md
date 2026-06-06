ONLY reusable persistence mechanics.

NOT repositories.

Repositories belong to services.

## Adicione ao program.cs com:
```cs
builder.Services.AddPersistence<SchedulingDbContext>(connectionString);
```