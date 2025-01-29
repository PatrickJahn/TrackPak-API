using Microsoft.AspNetCore.Mvc;
using UserService.Api.Dtos;
using UserService.Application.Interfaces;
using UserService.Application.Models;

namespace UserService.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("user/{id}", async (Guid id, IUserService service) =>
        {

            var user = await service.GetUserByIdAsync(id);
            return Results.Ok(user);
        });
        
        app.MapPost("user", async ([FromBody] CreateUserModel request,IUserService service) =>
        {
            
            await service.CreateUser(request);
        });
        
        app.MapPut("user/{id}", async (Guid id, [FromBody] UpdateUserModel userModel, IUserService service) =>
        {
            var updatedUser = await service.UpdateUserAsync(id, userModel);
            return Results.Ok(updatedUser);
        });

        // Delete a user
        app.MapDelete("user/{id}", async (Guid id, IUserService service) =>
        {
            await service.DeleteUserAsync(id);
            return Results.Ok();
        });
    }
    
}