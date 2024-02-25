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
using RepositoryContracts;

namespace CrudTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesServices _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        private readonly IFixture _fixture;

        //constructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CounriesService(_countriesRepository);
        }


        #region AddCountry

        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;

            Countries country = _fixture.Build<Countries>()
                 .With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountries(It.IsAny<Countries>()))
             .ReturnsAsync(country);


            //Act
            var action = async () =>
            {
                await _countriesService.AddCountries(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
             .With(temp => temp.CountryName, null as string)
             .Create();

            Countries country = _fixture.Build<Countries>()
                 .With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountries(It.IsAny<Countries>()))
             .ReturnsAsync(country);

            //Act
            var action = async () =>
            {
                await _countriesService.AddCountries(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest first_country_request = _fixture.Build<CountryAddRequest>()
                 .With(temp => temp.CountryName, "Test name").Create();
            CountryAddRequest second_country_request = _fixture.Build<CountryAddRequest>()
              .With(temp => temp.CountryName, "Test name").Create();

            Countries first_country = first_country_request.ToCuntry();
            Countries second_country = second_country_request.ToCuntry();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountries(It.IsAny<Countries>()))
             .ReturnsAsync(first_country);

            //Return null when GetCountryByCountryName is called
            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Countries);

            CountriesResponse first_country_from_add_country = await _countriesService.AddCountries(first_country_request);

            //Act
            var action = async () =>
            {
                //Return first country when GetCountryByCountryName is called
                _countriesRepositoryMock.Setup(temp => temp.AddCountries(It.IsAny<Countries>())).ReturnsAsync(first_country);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(first_country);

                await _countriesService.AddCountries(second_country_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_FullCountry_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            Countries country = country_request.ToCuntry();
            CountriesResponse country_response = country.ToCountriesResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountries(It.IsAny<Countries>()))
             .ReturnsAsync(country);

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Countries);


            //Act
            CountriesResponse country_from_add_country = await _countriesService.AddCountries(country_request);

            country.CountryId = country_from_add_country.CountryId;
            country_response.CountryId = country_from_add_country.CountryId;

            //Assert
            country_from_add_country.CountryId.Should().NotBe(Guid.Empty);
            country_from_add_country.Should().BeEquivalentTo(country_response);
        }

        #endregion


        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public async Task GetAllCountries_ToBeEmptyList()
        {
            //Arrange
            List<Countries> country_empty_list = new List<Countries>();
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

            //Act
            List<CountriesResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            actual_country_response_list.Should().BeEmpty();
        }


        [Fact]
        public async Task GetAllCountries_ShouldHaveFewCountries()
        {
            //Arrange
            List<Countries> country_list = new List<Countries>() {
        _fixture.Build<Countries>()
        .With(temp => temp.Persons, null as List<Person>).Create(),
        _fixture.Build<Countries>()
        .With(temp => temp.Persons, null as List<Person>).Create()
      };

            List<CountriesResponse> country_response_list = country_list.Select(temp => temp.ToCountriesResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            //Act
            List<CountriesResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            //Assert
            actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
        }
        #endregion


        #region GetCountryByCountryID

        [Fact]
        //If we supply null as CountryID, it should return null as CountryResponse
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Arrange
            Guid? countryID = null;

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
             .ReturnsAsync(null as Countries);

            //Act
            CountriesResponse? country_response_from_get_method = await _countriesService.GetCountriesByCountriesIDs(countryID);


            //Assert
            country_response_from_get_method.Should().BeNull();
        }


        [Fact]
        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Countries country = _fixture.Build<Countries>()
              .With(temp => temp.Persons, null as List<Person>)
              .Create();
            CountriesResponse country_response = country.ToCountriesResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
             .ReturnsAsync(country);

            //Act
            CountriesResponse? country_response_from_get = await _countriesService.GetCountriesByCountriesIDs(country.CountryId);


            //Assert
            country_response_from_get.Should().Be(country_response);
        }
        #endregion
    }
}
