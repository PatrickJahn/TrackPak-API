FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EmployeeService/EmployeeService.Api/EmployeeService.Api.csproj", "EmployeeService/EmployeeService.Api/"]
COPY ["EmployeeService/EmployeeService.Application/EmployeeService.Application.csproj", "EmployeeService/EmployeeService.Application/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["EmployeeService/EmployeeService.Domain/EmployeeService.Domain.csproj", "EmployeeService/EmployeeService.Domain/"]
COPY ["EmployeeService/EmployeeService.Infrastructure/EmployeeService.Infrastructure.csproj", "EmployeeService/EmployeeService.Infrastructure/"]
RUN dotnet restore "EmployeeService/EmployeeService.Api/EmployeeService.Api.csproj"
COPY . .
WORKDIR "/src/EmployeeService/EmployeeService.Api"
RUN dotnet build "EmployeeService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EmployeeService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeService.Api.dll"]
