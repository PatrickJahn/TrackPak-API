namespace Shared.Models;

public class LocationResponseModel
{
    public string Country { get; set; }
    public string City { get; set; }
    public string AddressLine { get; set; }
    public string PostalCode { get; set; }
    public GeoLocation? GeoLocation { get; set; }
}


public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}