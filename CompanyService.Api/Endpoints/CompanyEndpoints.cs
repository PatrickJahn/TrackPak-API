using Microsoft.AspNetCore.Http.HttpResults;
using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;



namespace CompanyService.Api.Endpoints;

public static class CompanyEndpoints
{
    public static void MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/companies");

        group.MapPost("/", CreateCompanyAsync);
        group.MapGet("/{companyId}", GetCompanyByIdAsync);
        group.MapGet("/", GetCompaniesAsync);
        group.MapPut("/{companyId}", UpdateCompanyAsync);
        group.MapDelete("/{companyId}", DeleteCompanyAsync);
    }

    private static async Task<IResult> CreateCompanyAsync(
        CreateCompanyModel company,
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var createdCompany = await companyService.CreateCompanyAsync(company, cancellationToken);
        return Results.Ok(createdCompany);
    }

    private static async Task<Results<Ok<Company>, NotFound>> GetCompanyByIdAsync(
        Guid companyId,
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var company = await companyService.GetCompanyByIdAsync(companyId, cancellationToken);
        return company != null ? TypedResults.Ok(company) : TypedResults.NotFound();
    }

    private static async Task<Ok<IEnumerable<Company>>> GetCompaniesAsync(
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var companies = await companyService.GetCompaniesAsync(cancellationToken);
        return TypedResults.Ok(companies);
    }

    private static async Task<Results<Ok, NotFound>> UpdateCompanyAsync(
        Guid companyId,
        UpdateCompanyModel updatedCompany,
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var updated = await companyService.UpdateCompanyAsync(companyId, updatedCompany, cancellationToken);
        return updated ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> DeleteCompanyAsync(
        Guid companyId,
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var deleted = await companyService.DeleteCompanyAsync(companyId, cancellationToken);
        return deleted ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
