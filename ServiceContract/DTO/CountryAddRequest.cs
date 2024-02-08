using Entities;
using System;
using System.Collections.Generic;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class for Adding a new Country
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName {  get; set; }
        public Countries ToCuntry()
        {
            return new Countries()
            {
                CountryName = CountryName
            };

        }
    }
}
