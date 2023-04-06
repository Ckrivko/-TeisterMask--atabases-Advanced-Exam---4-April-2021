namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var root = new XmlRootAttribute("Projects");

            var xmlSerializerer = new XmlSerializer(typeof(ImportProjectsDto[]), root);

            using (var reader = new StringReader(xmlString))
            {
                var projectsDto = (ImportProjectsDto[])xmlSerializerer.Deserialize(reader);

                var result = new List<Project>();


                foreach (var projDto in projectsDto)
                {
                    if (!IsValid(projDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }

                    var projectOpenDateIsValid = DateTime.TryParseExact
                        (projDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime projectOpenDate);

                    var isProjectDueDateNull = String.IsNullOrEmpty(projDto.DueDate);

                    var projectDueDateIsValid = DateTime.TryParseExact
                        (projDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime projectDueDate);



                    if (!isProjectDueDateNull)
                    {
                        if (!projectDueDateIsValid
                           || !projectOpenDateIsValid
                            || projectOpenDate > projectDueDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                    }
                    else
                    {
                        if (!projectOpenDateIsValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                    }

                    var currProject = AutoMapper.Mapper.Map<Project>(projDto);
                    currProject.Tasks.Clear();

                    foreach (var taskDto in projDto.Tasks)
                    {

                        if (!IsValid(taskDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;

                        }

                        var taskOpenDateIsValid = DateTime.TryParseExact
                        (taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime taskOpenDate); ;

                        var tasktDueDateIsValid = DateTime.TryParseExact
                            (taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out DateTime taskDueDate);

                        if (!taskOpenDateIsValid ||
                            !tasktDueDateIsValid ||
                            taskOpenDate > taskDueDate ||
                           taskOpenDate < currProject.OpenDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }


                        if (!isProjectDueDateNull)
                        {

                            if (taskDueDate > currProject.DueDate)
                            {
                                sb.AppendLine(ErrorMessage);
                                continue;

                            }
                        }

                        var currTask = AutoMapper.Mapper.Map<Task>(taskDto);
                        currProject.Tasks.Add(currTask);

                    }

                    result.Add(currProject);
                    sb.AppendLine(String.Format
                        (SuccessfullyImportedProject, currProject.Name, currProject.Tasks.Count));
                }

                context.Projects.AddRange(result);
                context.SaveChanges();
            }

            return sb.ToString().Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importEmployeesDto = JsonConvert.DeserializeObject<ImportEmployeesDto[]>(jsonString);
            var result = new List<Employee>();
            var tasksId = context.Tasks.Select(x => x.Id).ToHashSet<int>();


            foreach (var employeeDto in importEmployeesDto)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var currEmployee = AutoMapper.Mapper.Map<Employee>(employeeDto);

                foreach (var id in employeeDto.Tasks.Distinct())
                {
                    if (!tasksId.Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var currEmployeeTask = new EmployeeTask
                    {
                        TaskId = id,
                        EmployeeId = currEmployee.Id
                    };

                    currEmployee.EmployeesTasks.Add(currEmployeeTask);
                }

                result.Add(currEmployee);
                sb.AppendLine(String.Format
                    (SuccessfullyImportedEmployee, currEmployee.Username, currEmployee.EmployeesTasks.Count));
            }
            context.Employees.AddRange(result);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}