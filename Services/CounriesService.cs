using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Services
{
    public class CounriesService : ICountriesServices
    {
        //private fields
        private readonly ApplicationDbContext _db;
        public CounriesService(ApplicationDbContext personsDbContext)
        {
            _db = personsDbContext;
            
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
            if (await _db.Countries.CountAsync(temp =>
                temp.CountryName== countriesAddRequest.CountryName)>0)
            {
                throw new ArgumentException("Given Country Name Already Added");

            }
            
            //Convert object from CountryAddRequest to CountryType
            Countries country = countriesAddRequest.ToCuntry();

            //Generate CountryID
            country.CountryId = Guid.NewGuid();

            //Add country object into _Countries
            _db.Countries.Add(country);
           await _db.SaveChangesAsync();
            return country.ToCountriesResponse();
        }

        public async Task<List<CountriesResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountriesResponse()).ToListAsync();
        }

        public async Task<CountriesResponse?> GetCountriesByCountriesIDs(Guid? countryID)
        {
            if (countryID == null)
                return null;
            Countries?countries_response_from_list=
              await  _db.Countries.FirstOrDefaultAsync(temp=>temp.CountryId
                ==countryID);
            if (countries_response_from_list == null)
                return null;
            return countries_response_from_list.ToCountriesResponse();
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

                        if (_db.Countries.Where(temp => temp.CountryName == countryName).Count() == 0)
                        {
                            Countries country = new Countries() { CountryName = countryName };
                            _db.Countries.Add(country);
                            await _db.SaveChangesAsync();

                            countriesInserted++;
                        }
                    }
                }
            }

            return countriesInserted;
        }
    }
}