ONLY reusable persistence mechanics.

NOT repositories.

Repositories belong to services.

## Adicione ao program.cs com:
```
builder.Services.AddPersistence<SchedulingDbContext>(connectionString);
```