using Microsoft.AspNetCore.Http.HttpResults;
using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using Company = Shared.Models.Company;


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

    private static async Task<Results<Ok<Shared.Models.Company>, NotFound>> GetCompanyByIdAsync(
        Guid companyId,
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var companyEntity = await companyService.GetCompanyByIdAsync(companyId, cancellationToken);

        if (companyEntity != null)
        {
            var companyDto = MapToDto(companyEntity);
            return TypedResults.Ok(companyDto);
        }

        return TypedResults.NotFound();
    }

    private static async Task<Ok<IEnumerable<Shared.Models.Company>>> GetCompaniesAsync(
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var companyEntities = await companyService.GetCompaniesAsync(cancellationToken);
        var companiesDto = companyEntities.Select(MapToDto);
        return TypedResults.Ok(companiesDto);
    }

    private static Shared.Models.Company MapToDto(CompanyService.Domain.Entities.Company company)
    {
        return new Shared.Models.Company
        {
            Id = company.Id,
            Cvr = company.Cvr,
            BrandId = company.BrandId,
            Name = company.Name,
            LocationId = company.LocationId
        };
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
