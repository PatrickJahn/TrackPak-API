using Microsoft.AspNetCore.Mvc;
using EmployeeService.Api.Dtos;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Models;

namespace EmployeeService.Api.Endpoints;

public static class EmployeeEndpoints
{
  public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapGet("employee/{id}", async (Guid id, IEmployeeService service) =>
    {
      var employee = await service.GetEmployeeByIdAsync(id);
      return Results.Ok(employee);
    });
        
    app.MapPost("employee", async ([FromBody] CreateEmployeeModel request, IEmployeeService service) =>
    {
      await service.CreateEmployeeAsync(request);
      return Results.Created($"/employee/{request.Id}", request);
    });
        
    app.MapPut("employee/{id}", async (Guid id, [FromBody] UpdateEmployeeModel employeeModel, IEmployeeService service) =>
    {
      var updatedEmployee = await service.UpdateEmployeeAsync(id, employeeModel);
      return Results.Ok(updatedEmployee);
    });

    app.MapDelete("employee/{id}", async (Guid id, IEmployeeService service) =>
    {
      await service.DeleteEmployeeAsync(id);
      return Results.NoContent();
    });
  }
}