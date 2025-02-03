using Shared.Models;

namespace CompanyService.Domain.Entities;

public class Company : BaseModel
{
  public string Name { get; set; }
  public string Country { get; set; }
  public string City { get; set; }
  public string AddressLine { get; set; }
  public string PostalCode { get; set; }
  public CompanyDetails? Details { get; set; }
}

public class CompanyDetails
{
  public string Industry { get; set; }
  public int NumberOfEmployees { get; set; }
  public double AnnualRevenue { get; set; }
}