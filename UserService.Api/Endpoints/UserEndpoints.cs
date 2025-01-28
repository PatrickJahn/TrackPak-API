using Microsoft.AspNetCore.Mvc;
using UserService.Api.Dtos;
using UserService.Application.Models;

namespace UserService.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("user/{id}", async (Guid id, Application.Services.UserService service) =>
        {

            var user = await service.GetUserByIdAsync(id);
            return Results.Ok(user);
        });
        
        app.MapPost("post", async ([FromBody] CreateUserRequestModel request, Application.Services.UserService service) =>
        {
            
            await service.CreateUser(new CreateUserModel()
            {
                Address = request.Address,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            });
        });
    }
}