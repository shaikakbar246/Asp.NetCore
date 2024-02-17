using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain Model for Country
    /// </summary>
    public class Countries
    {
        [Key]
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

    }
}