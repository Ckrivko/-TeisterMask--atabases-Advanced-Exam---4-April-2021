using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeisterMask.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportEmployeeDto
    {
        //"Username": 
        // "Tasks": 
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Tasks")]
        public ICollection<ExportEmployeeTaskDto> Tasks { get; set; } = new List<ExportEmployeeTaskDto>();

    }
}
