using Microsoft.AspNetCore.Mvc;
using EmployeeService.Api.Dtos;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Models;
using Shared.Extensions;
using Shared.Security;

namespace EmployeeService.Api.Endpoints;
public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/employee");
        group.MapGet("/{id}", GetEmployeeByIdAsync)
            .RequireAuthorization(PolicyRoles.CompanyAdmin);

        group.MapGet("/me", GetMyEmployeeProfileAsync)
            .RequireAuthorization(PolicyRoles.Driver);

        group.MapGet("/by-company/{companyId}", GetEmployeesByCompanyIdAsync)
            .RequireAuthorization(PolicyRoles.CompanyAdmin);

        group.MapGet("/", GetEmployeesAsync)
            .RequireAuthorization(PolicyRoles.CompanyAdmin);

        group.MapGet("/by-email/{email}", GetEmployeeByEmailAsync)
            .RequireAuthorization("RequireWriteReadEmployees");

        group.MapPost("/", CreateEmployeeAsync)
            .RequireAuthorization("RequireWriteReadEmployees");

        group.MapPut("/{id}", UpdateEmployeeAsync)
            .RequireAuthorization(PolicyRoles.CompanyAdmin);

        group.MapDelete("/{id}", DeleteEmployeeAsync)
            .RequireAuthorization(PolicyRoles.SystemAdmin);

        group.MapPost("/{id}/check-in", CheckIn)
            .RequireAuthorization(PolicyRoles.Driver);

        group.MapPost("/{id}/check-out", CheckOut)
            .RequireAuthorization(PolicyRoles.Driver);

    }

    private static async Task<IResult> GetEmployeeByIdAsync(Guid id, IEmployeeService service)
    {
        var employee = await service.GetEmployeeByIdAsync(id);
        return Results.Ok(employee);
    }
    
    private static async Task<IResult> GetEmployeeByEmailAsync(string email, IEmployeeService service)
    {
        var employee = await service.GetEmployeeByEmailAsync(email);
        return Results.Ok(employee);
    }
    
    private static async Task<IResult> CheckIn(Guid id, IEmployeeService service)
    {
         await service.CheckIn(id);
        return Results.Ok();
    }
    
    private static async Task<IResult> CheckOut(Guid id, IEmployeeService service)
    {
        await service.CheckOut(id);
        return Results.Ok();
    }
    
    private static async Task<IResult> GetMyEmployeeProfileAsync(HttpContext httpContext, IEmployeeService service)
    {
        var userId = httpContext.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var employee = await service.GetEmployeeByIdAsync((Guid)userId);
        return Results.Ok(employee);
    }

    private static async Task<IResult> GetEmployeesByCompanyIdAsync(Guid companyId, IEmployeeService service)
    {
        var employees = await service.GetEmployeeByCompanyIdAsync(companyId);
        return Results.Ok(employees);
    }

    private static async Task<IResult> GetEmployeesAsync(HttpContext httpContext, IEmployeeService service)
    {
        var companyId = httpContext.GetCompanyId();

        if (companyId == null)
            throw new Exception("Invalid companyId");

        var employees = await service.GetEmployeeByCompanyIdAsync((Guid)companyId);
        return Results.Ok(employees);
    }

    private static async Task<IResult> CreateEmployeeAsync([FromBody] CreateEmployeeModel request, IEmployeeService service)
    {
        var id = await service.CreateEmployeeAsync(request);
        return Results.Ok(id);
    }

    private static async Task<IResult> UpdateEmployeeAsync(Guid id, [FromBody] UpdateEmployeeModel employeeModel, IEmployeeService service)
    {
        var updatedEmployee = await service.UpdateEmployeeAsync(id, employeeModel);
        return Results.Ok(updatedEmployee);
    }

    private static async Task<IResult> DeleteEmployeeAsync(Guid id, IEmployeeService service)
    {
        await service.DeleteEmployeeAsync(id);
        return Results.NoContent();
    }
}