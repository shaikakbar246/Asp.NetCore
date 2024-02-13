using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Services
{
    public class CounriesService : ICountriesServices
    {
        //private fields
        private readonly List<Countries> _countries;
        public CounriesService(bool initialize = true)
        {
            _countries = new List<Countries>();
            if (initialize)
            {
                _countries.AddRange(new List<Countries>() {
        new Countries() {  CountryId = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "USA" },

        new Countries() { CountryId = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), CountryName = "Canada" },

        new Countries() { CountryId = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "UK" },

        new Countries() { CountryId = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "India" },

        new Countries() { CountryId = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Australia" }
        });
            }
        }
        public CountriesResponse AddCountries(CountryAddRequest? countriesAddRequest)
        {

            //Validation: countryAddRequest parameter can't be null
            if (countriesAddRequest == null)
            {
                throw new ArgumentNullException(nameof
                    (countriesAddRequest));
            }
            //validation :ContryName  can't  be  null
            if (countriesAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof
                    (countriesAddRequest.CountryName));
            }

            //validation :ContryName  can not be duplicate
            if (_countries.Where(temp =>
                temp.CountryName== countriesAddRequest.CountryName).Count()>0)
            {
                throw new ArgumentException("Given Country Name Already Added");

            }
            
            //Convert object from CountryAddRequest to CountryType
            Countries country = countriesAddRequest.ToCuntry();

            //Generate CountryID
            country.CountryId = Guid.NewGuid();

            //Add country object into _Countries
            _countries.Add(country);
            return country.ToCountriesResponse();
        }

        public List<CountriesResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountriesResponse()).ToList();
        }

        public CountriesResponse? GetCountriesByCountriesIDs(Guid? countryID)
        {
            if (countryID == null)
                return null;
            Countries?countries_response_from_list=
                _countries.FirstOrDefault(temp=>temp.CountryId
                ==countryID);
            if (countries_response_from_list == null)
                return null;
            return countries_response_from_list.ToCountriesResponse();
        }
    }
}