using Shared.Models;

namespace CompanyService.Application.Models;

public class CreateCompanyModel
{
    public string Cvr { get; set; }
    public string BrandId { get; set; }
    public string Name { get; set; }
    public CreateLocationRequestModel Location { get; set; }
    
}