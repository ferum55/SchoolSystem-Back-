FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-preview AS build
WORKDIR /src
COPY SchoolSystem.Grades/SchoolSystem.Grades.csproj SchoolSystem.Grades/
COPY SchoolSystem.Shared/SchoolSystem.Shared.csproj SchoolSystem.Shared/
RUN dotnet restore SchoolSystem.Grades/SchoolSystem.Grades.csproj
COPY . .
RUN dotnet publish SchoolSystem.Grades/SchoolSystem.Grades.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "SchoolSystem.Grades.dll"]
