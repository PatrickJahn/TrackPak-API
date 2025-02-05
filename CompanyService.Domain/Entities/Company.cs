using Shared.Models;

namespace CompanyService.Domain.Entities;

public class Company : BaseModel
{
  public string CompanyId { get; set; }
  public string Cvr { get; set; }
  public string BrandId { get; set; }
  public string Name { get; set; }
  public string LocationId { get; set; }
  public string Created_at { get; set; }
  public string Last_Updated_At { get; set; }
  public string Deleted { get; set; }
  public string Deleted_at { get; set; }
}
