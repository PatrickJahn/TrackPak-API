using Microsoft.Extensions.DependencyInjection;
using EmployeeService.Application.Interfaces;

namespace EmployeeService.Application;

public static class DependencyInjection
{
  public static void AddApplication(this IServiceCollection services)
  {
    services.AddScoped<IEmployeeService, Services.EmployeeService>();
        
        
  }
    

}