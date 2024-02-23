﻿using System.Diagnostics.Metrics;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository:ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Countries> AddCountries(Countries country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country;
        }

        public async Task<List<Countries>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Countries?> GetCountryByCountryID(Guid countryID)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryId == countryID);
        }

        public Task<Countries?> GetCountryByCountryName(string countryName)
        {
            throw new NotImplementedException();
        }
    }
}