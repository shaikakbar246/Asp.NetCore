using System;
using Entities;
namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most
    /// of CountriesService methods
    /// </summary>
    public class CountriesResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        //it compares the current object to another object
        //of  CountriesResponse type and returns true, if both values
        //are same other wise return false

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(CountriesResponse))
            {
                return false;
            }
            CountriesResponse countries_to_compare = (CountriesResponse)obj;
            return CountryId == countries_to_compare.CountryId &&
                CountryName == countries_to_compare.CountryName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public static class ContriesExtensions
    {
        public static CountriesResponse ToCountriesResponse
            (this Countries countries)
        {
            return new CountriesResponse
            {
                CountryId = countries.CountryId,
                CountryName = countries.CountryName
            };
        }
    }
}
