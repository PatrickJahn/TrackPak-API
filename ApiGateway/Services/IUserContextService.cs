namespace ApiGateway.Services;

public interface IUserContextService
{
    Guid GetUserId();
    Role GetRole();
    bool IsSystemAdministrator();
    string GetAuthId();
    string GetEmail();
    string GetCompanyId();
    bool IsClientCredentials();

}