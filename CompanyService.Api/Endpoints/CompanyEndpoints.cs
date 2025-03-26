using Microsoft.AspNetCore.Http.HttpResults;
using CompanyService.Application.Interfaces;
using CompanyService.Application.Models;
using CompanyService.Domain.Entities;
using Shared.Extensions;
using Shared.Security;


namespace CompanyService.Api.Endpoints;

public static class CompanyEndpoints
{
    public static void MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/company");

        // Only SystemAdmin can create companies
        group.MapPost("/", CreateCompanyAsync)
            .RequireAuthorization("RequireWriteReadCompanies");

        // CompanyAdmin or SystemAdmin can fetch company by ID
        group.MapGet("/{companyId:guid}", GetCompanyByIdAsync)
            .RequireAuthorization(PolicyRoles.CompanyAdmin);

        // CompanyAdmin or SystemAdmin can fetch their own company
        group.MapGet("/me", GetMeAsync)
            .RequireAuthorization(PolicyRoles.SystemAdmin);

        // Only SystemAdmin can fetch all companies
        group.MapGet("/", GetCompaniesAsync)
            .RequireAuthorization(PolicyRoles.SystemAdmin);

        // CompanyAdmin can update their company
        group.MapPut("/{companyId:guid}", UpdateCompanyAsync)
            .RequireAuthorization(PolicyRoles.CompanyAdmin);

        // Only SystemAdmin can delete companies
        group.MapDelete("/{companyId:guid}", DeleteCompanyAsync)
            .RequireAuthorization(PolicyRoles.SystemAdmin);
    }

    private static async Task<IResult> CreateCompanyAsync(
        CreateCompanyModel company,
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var createdCompany = await companyService.CreateCompanyAsync(company, cancellationToken);
        return Results.Ok(createdCompany);
    }
    private static async Task<IResult> GetMeAsync(HttpContext httpContext, ICompanyService service, CancellationToken cancellationToken)
    {
        var companyId = httpContext.GetCompanyId();
        
        //if (companyId is null)
          //  return Results.Unauthorized();
        
        var company = await service.GetCompanyByIdAsync((Guid) companyId, cancellationToken);
        return Results.Ok(company);
    }
    private static async Task<Results<Ok<Company>, NotFound>> GetCompanyByIdAsync(
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

    private static async Task<Ok<IEnumerable<Company>>> GetCompaniesAsync(
        ICompanyService companyService,
        CancellationToken cancellationToken)
    {
        var companyEntities = await companyService.GetCompaniesAsync(cancellationToken);
        var companiesDto = companyEntities.Select(MapToDto);
        return TypedResults.Ok(companiesDto);
    }

    private static Company MapToDto(Company company)
    {
        return new Company
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
