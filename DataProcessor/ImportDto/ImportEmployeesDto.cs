using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeisterMask.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportEmployeesDto
    {
        [JsonProperty("Username")]
        [Required]
        [StringLength(40, MinimumLength = 3)]
        [RegularExpression(@"^[A-z0-9]+$")] // ??
        public string Username { get; set; }

        [JsonProperty("Email")]
        [Required]
        [EmailAddress]        
        public string Email { get; set; }

        [JsonProperty("Phone")]
        [Required]
        [RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Phone { get; set; }

        public List<int> Tasks { get; set; } = new List<int>();

    }
}
