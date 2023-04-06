namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var sb = new StringBuilder();
            var root = new XmlRootAttribute("Projects");
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(ExportProjectDto[]), root);

            using (var writer = new StringWriter(sb))
            {
                var result = context.Projects
                        .Include(x => x.Tasks)
                        .ToArray()
                        .Where(x => x.Tasks.Count > 0)
                        //.Select(x => new ExportProjectDto
                        //{
                        //    ProjectName = x.Name,
                        //    TasksCount = x.Tasks.Count.ToString(),
                        //    HasEndDate = x.DueDate == null ? "No" : "Yes",
                        //    Tasks = x.Tasks.Select(a => new ExportProjectTaskDto
                        //    {
                        //        Name = a.Name,
                        //        Label = a.LabelType.ToString()

                        //    })
                        //    .OrderBy(x => x.Name)
                        //    .ToArray()
                        //})
                        .Select(x => AutoMapper.Mapper.Map<ExportProjectDto>(x))
                        .OrderByDescending(x => x.Tasks.Length)
                        .ThenBy(x => x.Name)
                        .ToArray();

                xmlSerializer.Serialize(writer, result, namespaces);

                return sb.ToString().Trim();
            }

        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var result = context.Employees
                 .Include(x => x.EmployeesTasks)
                 .ThenInclude(x => x.Task)
                 .ToArray()
                 .Where(x => x.EmployeesTasks.Any(a => a.Task.OpenDate >= date))
                 .Select(x => new Employee
                 {
                     Username = x.Username,
                     Email = x.Email,
                     Id = x.Id,
                     Phone = x.Phone,
                     EmployeesTasks = x.EmployeesTasks.Where(a => a.Task.OpenDate >= date).ToHashSet(),

                 }) //It is just for exercise

                  //.Select(x => new
                  //{
                  //    Username = x.Username,
                  //    Tasks = x.EmployeesTasks
                  //    .Where(a => a.Task.OpenDate >= date)
                  //    .Select(b => new
                  //    {
                  //        TaskName = b.Task.Name,
                  //        OpenDate = b.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                  //        DueDate = b.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                  //        LabelType = b.Task.LabelType.ToString(),
                  //        ExecutionType = b.Task.ExecutionType.ToString()

                  //    })
                  //    .OrderByDescending(x => DateTime.Parse(x.DueDate))
                  //    .ThenBy(x => x.TaskName)
                  //    .ToList()

                  //})
                  .Select(x => AutoMapper.Mapper.Map<ExportEmployeeDto>(x))   //It is much beteer withou mapper in this case
                  .OrderByDescending(x => x.Tasks.Count)
                  .ThenBy(x => x.Username)
                  .Take(10)
                  .ToArray();



            return JsonConvert.SerializeObject(result, Formatting.Indented);

        }
    }
}