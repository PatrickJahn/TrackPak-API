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

        group.MapGet("/{id}", GetEmployeeByIdAsync);
        group.MapGet("/me", GetMyEmployeeProfileAsync);
        group.MapGet("/by-company/{companyId}", GetEmployeesByCompanyIdAsync);
        group.MapGet("/", GetEmployeesAsync);

        group.MapPost("/", CreateEmployeeAsync);
        group.MapPut("/{id}", UpdateEmployeeAsync);
        group.MapDelete("/{id}", DeleteEmployeeAsync);
    }

    private static async Task<IResult> GetEmployeeByIdAsync(Guid id, IEmployeeService service)
    {
        var employee = await service.GetEmployeeByIdAsync(id);
        return Results.Ok(employee);
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
        await service.CreateEmployeeAsync(request);
        return Results.Ok();
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