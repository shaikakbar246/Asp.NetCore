using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Xunit.Sdk;
using Moq;
using AutoFixture;
using FluentAssertions;

namespace CrudTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesServices _countriesServices;
        private readonly IFixture _fixture;
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            var countriesInitialData = new List<Countries>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
              new DbContextOptionsBuilder<ApplicationDbContext>().Options
             );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesServices = new CounriesService(null);
        }
        #region Add Countries

        //When CountryAddRequest is null,it should 
        //through ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;
            //Act
            var action = async () =>
            {
                await _countriesServices.AddCountries(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When CountryName  is null,it should through
        //ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
    .With(temp => temp.CountryName, null as string)
    .Create();

            //Act
            var action = async () =>
            {
                await _countriesServices.AddCountries(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }
        //When CountryName  is duplicate,it should through
        //ArgumentException
        [Fact]
        public async Task Duplicate_Countryname()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
     .Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
             .With(temp => temp.CountryName, request1.CountryName)
             .Create();
            //Act
            var action = async () =>
            {
                await _countriesServices.AddCountries(request1);
                await _countriesServices.AddCountries(request2);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //When you supply proper CountryName  it should insert (add) the country
        // the existing list of countris

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
    .Create();

            //Act
            CountriesResponse response =await _countriesServices.AddCountries(request);
            List<CountriesResponse> countries_from_GetAllCountries =
               await _countriesServices.GetAllCountries();
            //Assert
            response.CountryId.Should().NotBe(Guid.Empty);
            countries_from_GetAllCountries.Should().Contain(response);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        //The list of countries should be empty by defalult
        //(before adding countries)
        public async Task GetAllCoutries_EmptyList()
        {
            //Act
            List<CountriesResponse> actual_countries_response_list =
               await _countriesServices.GetAllCountries();

            //Assert
            actual_countries_response_list.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>() {
      _fixture.Build<CountryAddRequest>()
       .Create(),
      _fixture.Build<CountryAddRequest>()
       .Create()
      };
            //Act
            List<CountriesResponse> countries_list_from_add_country = new
                List<CountriesResponse>();
            foreach (CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add
                (await _countriesServices.AddCountries(country_request));
            }
            List<CountriesResponse> actualCountriesResponseList =
                await _countriesServices.GetAllCountries();
            //read each element from countries_list_from_add_country
            //Assert
            actualCountriesResponseList.Should().BeEquivalentTo(countries_list_from_add_country);

        }
        #endregion

        #region GetCountriesByCountriesIDs



        [Fact]
        //if we supply null as countryId,It should be return 
        //null as CountriesResponse
        public async Task GetCountriesByCountriesIDs_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Act
            CountriesResponse? country_response_from_get_method =
              await _countriesServices.GetCountriesByCountriesIDs(countryID);

            //Assert
            country_response_from_get_method.Should().BeNull();
        }

        [Fact]
        //if we supply the valid country id,it should return 
        //the matching country details as CountryResponse object

        public async Task GetCountriesByCountriesIDs_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = _fixture.Build<CountryAddRequest>()
    .Create();
            CountriesResponse country_response_from_add =
                await _countriesServices.AddCountries(country_add_request);
            //Act
            CountriesResponse? country_response_from_get =
                await _countriesServices.GetCountriesByCountriesIDs
                (country_response_from_add.CountryId);

            //Assert
            country_response_from_get.Should().Be(country_response_from_add);

        }
        #endregion
    }
}
