using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Services
{
    public class CounriesService : ICountriesServices
    {
        //private fields
        private readonly ICountriesRepository _countriesRepository;
        public CounriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
            
        }
        public async Task<CountriesResponse> AddCountries(CountryAddRequest? countriesAddRequest)
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
            if (await _countriesRepository.GetCountryByCountryName(countriesAddRequest.CountryName)!=null)
            {
                throw new ArgumentException("Given Country Name Already Added");
            }
            
            //Convert object from CountryAddRequest to CountryType
            Countries country = countriesAddRequest.ToCuntry();

            //Generate CountryID
            country.CountryId = Guid.NewGuid();

            //Add country object into _Countries
           await _countriesRepository.AddCountries(country);
            return country.ToCountriesResponse();
        }

        public async Task<List<CountriesResponse>> GetAllCountries()
        {
            List<Countries> countries = await _countriesRepository.GetAllCountries();
            return countries
              .Select(country => country.ToCountriesResponse()).ToList();
        }

        public async Task<CountriesResponse?> GetCountriesByCountriesIDs(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Countries? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountriesResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if (await _countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Countries country = new Countries() { CountryName = countryName };
                            await _countriesRepository.AddCountries(country);

                            countriesInserted++;
                        }
                    }
                }
            }

            return countriesInserted;
        }
    }
}