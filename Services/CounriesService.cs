using Entities;
using ServiceContract;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class CounriesService : ICountriesServices
    {
        //private fields
        private readonly List<Countries> _countries;
        public CounriesService()
        {
            _countries = new List<Countries>();
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