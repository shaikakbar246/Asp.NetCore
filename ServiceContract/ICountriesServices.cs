﻿using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents Business logic for manipulating
    /// Countries Entity
    /// </summary>
    public interface ICountriesServices
    {
        /// <summary>
        /// Add a Country object to the list of 
        /// Countries
        /// </summary>
        /// <param name="countriesAddRequest">Countries 
        /// object to add </param>
        /// <returns>Returns the country object after adding it 
        /// (including newly generated country id </returns>
       CountriesResponse AddCountries(CountryAddRequest? 
           countriesAddRequest);

        /// <summary>
        /// Returns All Countris from the List
        /// </summary>
        /// <returns>All coutries from the list as list
        /// of CountryResponse</CountriesReponse</returns>
        List<CountriesResponse> GetAllCountries();

        /// <summary>
        /// returns a country based on the given country id
        /// </summary>
        /// <param name="countryID">CountriesID (guid) to search</param>
        /// <returns>Matching counry as CountriesResponse object</returns>
        CountriesResponse GetCountriesByCountriesIDs(Guid? countryID);
    }
}