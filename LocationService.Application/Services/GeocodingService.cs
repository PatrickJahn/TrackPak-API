using System.Globalization;
using LocationService.Application.Interfaces;
using LocationService.Domain.Entities;

namespace LocationService.Application.Services;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GeoLocation?> GetGeoLocationAsync(string address)
    {
        var encodedAddress = Uri.EscapeDataString(address);
        var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

        // Add proper User-Agent header as per OpenStreetMap's terms of use
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("trackpak/1.0 (trackpak@gmail.com)");

        var response = await _httpClient.GetStringAsync(url);

        if (response.Contains("error"))
        {
            return null; // Address not found or error
        }

        var json = JsonDocument.Parse(response);

        if (json.RootElement.GetArrayLength() == 0)
            return null; // Address not found

        var location = json.RootElement[0];
        
        
        if (location.TryGetProperty("lat", out var latProperty) &&
            location.TryGetProperty("lon", out var lonProperty))
        {
            if (double.TryParse(latProperty.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude) &&
                double.TryParse(lonProperty.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude))
            {
                return new GeoLocation
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
            }
        }

        return null; // If parsing fails
    
    }
}
