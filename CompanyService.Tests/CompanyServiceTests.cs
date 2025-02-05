using Moq;
using System.Threading;
using System.Threading.Tasks;
using CompanyService.Application.Interfaces;
using CompanyService.Domain.Entities;
using Xunit;
using System.Collections.Generic;

namespace CompanyService.Tests
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyService> _companyServiceMock;

        public CompanyServiceTests()
        {
            _companyServiceMock = new Mock<ICompanyService>();
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnCompany_WhenSuccess()
        {
            // Arrange
            var newCompany = new Company { CompanyId = "1", Name = "Test Company" };
            _companyServiceMock.Setup(x => x.CreateCompanyAsync(It.IsAny<Company>(), CancellationToken.None))
                .ReturnsAsync(newCompany);

            // Act
            var result = await _companyServiceMock.Object.CreateCompanyAsync(newCompany, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Company", result.Name);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ShouldReturnCompany_WhenFound()
        {
            // Arrange
            var company = new Company { CompanyId = "1", Name = "Test Company" };
            _companyServiceMock.Setup(x => x.GetCompanyByIdAsync("1", CancellationToken.None))
                .ReturnsAsync(company);

            // Act
            var result = await _companyServiceMock.Object.GetCompanyByIdAsync("1", CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Company", result.Name);
        }

        [Fact]
        public async Task GetCompaniesAsync_ShouldReturnListOfCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = "1", Name = "Company 1" },
                new Company { CompanyId = "2", Name = "Company 2" }
            };

            _companyServiceMock.Setup(x => x.GetCompaniesAsync(CancellationToken.None))
                .ReturnsAsync(companies);

            // Act
            var result = await _companyServiceMock.Object.GetCompaniesAsync(CancellationToken.None);

            // Assert
            var companyList = result.ToList(); // Ensures the result is treated as a List
            Assert.NotEmpty(companyList);
            Assert.Equal(2, companyList.Count);
        }


        [Fact]
        public async Task UpdateCompanyAsync_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            var updatedCompany = new Company { CompanyId = "1", Name = "Updated Company" };
            _companyServiceMock.Setup(x => x.UpdateCompanyAsync("1", updatedCompany, CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _companyServiceMock.Object.UpdateCompanyAsync("1", updatedCompany, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCompanyAsync_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            _companyServiceMock.Setup(x => x.DeleteCompanyAsync("1", CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _companyServiceMock.Object.DeleteCompanyAsync("1", CancellationToken.None);

            // Assert
            Assert.True(result);
        }
    }
}
