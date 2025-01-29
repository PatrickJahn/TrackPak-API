using System.Text;
using System.Text.Json;
using Shared.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Services;



public class LocationServiceClient : ILocationServiceClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://locationservice-api"; // Replace with actual URL

    public LocationServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LocationResponseModel?> GetLocationByIdAsync(Guid locationId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/location/{locationId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<LocationResponseModel?>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<Guid?> CreateLocationAsync(CreateLocationRequestModel request)
    {
        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BaseUrl}/location", content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Guid?>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}


