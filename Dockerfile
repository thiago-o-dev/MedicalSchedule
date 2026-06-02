FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MedicalSchedule.WebApi/MedicalSchedule.WebApi.csproj", "MedicalSchedule.WebApi/"]
COPY ["MedicalSchedule.Application/MedicalSchedule.Application.csproj", "MedicalSchedule.Application/"]
COPY ["MedicalSchedule.Infrastructure/MedicalSchedule.Infrastructure.csproj", "MedicalSchedule.Infrastructure/"]
COPY ["MedicalSchedule.Domain/MedicalSchedule.Domain.csproj", "MedicalSchedule.Domain/"]
RUN dotnet restore "MedicalSchedule.WebApi/MedicalSchedule.WebApi.csproj"
COPY . .
RUN dotnet publish "MedicalSchedule.WebApi/MedicalSchedule.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MedicalSchedule.WebApi.dll"]
