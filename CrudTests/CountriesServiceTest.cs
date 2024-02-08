using ServiceContract;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using Xunit.Sdk;

namespace CrudTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesServices _countriesServices;

        public CountriesServiceTest()
        {
            _countriesServices = new CounriesService();
        }
        #region Add Countries

        //When CountryAddRequest is null,it should 
        //through ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesServices.AddCountries(request);
            });
        }

        //When CountryName  is null,it should through
        //ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            { CountryName = null };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesServices.AddCountries(request);
            });

        }
        //When CountryName  is duplicate,it should through
        //ArgumentException
        [Fact]
        public void Duplicate_Countryname()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest()
            { CountryName = "USA" };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesServices.AddCountries(request1);
                _countriesServices.AddCountries(request2);
            });

        }

        //When you supply proper CountryName  it should insert (add) the country
        // the existing list of countris

        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            { CountryName = "Japan" };

            //Act
            CountriesResponse response = _countriesServices.AddCountries(request);
            List<CountriesResponse> countries_from_GetAllCountries =
                _countriesServices.GetAllCountries();
            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        //The list of countries should be empty by defalult
        //(before adding countries)
        public void GetAllCoutries_EmptyList()
        {
            //Act
            List<CountriesResponse> actual_countries_response_list =
                _countriesServices.GetAllCountries();

            //Asert
            Assert.Empty(actual_countries_response_list);
        }
        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_List = new List<CountryAddRequest>()
            {
                new CountryAddRequest(){CountryName="USA"},
                new CountryAddRequest(){CountryName="UK"}
            };
            //Act
            List<CountriesResponse> countries_list_from_add_country = new
                List<CountriesResponse>();
            foreach (CountryAddRequest country_request in country_request_List)
            {
                countries_list_from_add_country.Add
                (_countriesServices.AddCountries(country_request));
            }
            List<CountriesResponse> actualCountriesResponseList =
                _countriesServices.GetAllCountries();
            //read each element from countries_list_from_add_country
            foreach (CountriesResponse expected_country in
                countries_list_from_add_country)
            {
                Assert.Contains(expected_country,
                    actualCountriesResponseList);

            }

        }
        #endregion

        #region GetCountriesByCountriesIDs



        [Fact]
        //if we supply null as countryId,It should be return 
        //null as CountriesResponse
        public void GetCountriesByCountriesIDs_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;
            //Act
            CountriesResponse? country_response_from_get_method =
               _countriesServices.GetCountriesByCountriesIDs(countryID);

            //Assert 
            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        //if we supply the valid country id,it should return 
        //the matching country details as CountryResponse object

        public void GetCountriesByCountriesIDs_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = new
                CountryAddRequest() { CountryName = "IND" };
            CountriesResponse country_response_from_add =
                _countriesServices.AddCountries(country_add_request);
            //Act
            CountriesResponse? country_response_from_get =
                _countriesServices.GetCountriesByCountriesIDs
                (country_response_from_add.CountryId);
            //Assert
            Assert.Equal(country_response_from_add,
                country_response_from_get);

        }
        #endregion
    }
}
