using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Company : BaseModel
{
    public string Cvr { get; set; }
    public string BrandId { get; set; }
    public string Name { get; set; }
    public Guid? LocationId { get; set; }
}