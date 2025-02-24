using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using UserService.Api.Dtos;
using UserService.Application.Interfaces;
using UserService.Application.Models;

namespace UserService.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");
        
        group.MapPost("/", CreateUserAsync);
        group.MapGet("/{id}", GetUserByIdAsync);
        group.MapPut("/{id}", UpdateUserAsync);
        group.MapDelete("/{id}", UpdateUserAsync);
        group.MapPut("/{id}", UpdateUserAsync);

    }

    private static async Task<IResult> CreateUserAsync([FromBody] CreateUserModel request, IUserService service, CancellationToken cancellationToken)
    {
        await service.CreateUser(request, cancellationToken);
        return Results.Ok();
    }
    
    private static async Task<IResult> GetUserByIdAsync(Guid id, IUserService service, CancellationToken cancellationToken)
    {
        var user = await service.GetUserByIdAsync(id, cancellationToken);
        return Results.Ok(user);
    }
    
    private static async Task<IResult> UpdateUserAsync(Guid id, [FromBody] UpdateUserModel userModel,IUserService service, CancellationToken cancellationToken)
    {
        var updatedUser = await service.UpdateUserAsync(id, userModel, cancellationToken);
        return Results.Ok(updatedUser);
    }
    
    private static async Task<IResult> DeleteUserAsync(Guid id, IUserService service, CancellationToken cancellationToken)
    {
        await service.DeleteUserAsync(id, cancellationToken);
        return Results.Ok();
    }
    
    
    private static async Task<IResult> UpdateUserLocation(Guid id, [FromBody] CreateLocationRequestModel request, IUserService service, CancellationToken cancellationToken)
    {
        await service.UpdateUserLocationAsync(id, request, cancellationToken);
        return Results.Ok();
    }
    
}