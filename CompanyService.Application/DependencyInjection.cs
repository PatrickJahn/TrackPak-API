using CompanyService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyService.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICompanyService, Services.CompanyService>();
        
        
    }
    
}