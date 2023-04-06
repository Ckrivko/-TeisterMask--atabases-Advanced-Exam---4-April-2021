namespace TeisterMask
{
    using AutoMapper;
    using System;
    using System.Globalization;
    using System.Linq;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using TeisterMask.DataProcessor.ImportDto;

    public class TeisterMaskProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
        public TeisterMaskProfile()
        {
            //Import Dto  projects
            CreateMap<ImportProjectsDto, Project>()
                .ForMember(x => x.DueDate, mo => mo.MapFrom
                (so =>String.IsNullOrEmpty(so.DueDate)?(DateTime?)null: DateTime.ParseExact(so.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(x => x.OpenDate, mo => mo.MapFrom
                (so => DateTime.ParseExact(so.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));


            CreateMap<ImportProjectTasksDto, Task>()
               .ForMember(x => x.DueDate, mo => mo.MapFrom
                (so => DateTime.ParseExact(so.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(x => x.OpenDate, mo => mo.MapFrom
                (so => DateTime.ParseExact(so.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(x=>x.ExecutionType,mo=>mo.MapFrom(a=>(ExecutionType)a.ExecutionType))
                 .ForMember(x => x.LabelType, mo => mo.MapFrom(a => (LabelType)a.LabelType));

            //Employees
            CreateMap<ImportEmployeesDto, Employee>();

            //Export Dto

            //Export Projects 
            //Inner map
            CreateMap<Task, ExportProjectTaskDto>()
                .ForMember(x => x.Label, mo => mo.MapFrom(so => so.LabelType.ToString()));              
            //Main map
            CreateMap<Project, ExportProjectDto>()
                .ForMember(x => x.TasksCount, mo => mo.MapFrom(so => so.Tasks.Count.ToString()))
                .ForMember(x => x.HasEndDate, mo => mo.MapFrom(so => so.DueDate == null ? "No" : "Yes"))
                .ForMember(x => x.Tasks, mo => mo.MapFrom(so => so.Tasks.OrderBy(a=>a.Name)));

            //Export Employees
            //Inner map
            CreateMap<Task, ExportEmployeeTaskDto>()
                .ForMember(x => x.OpenDate, mo => mo.MapFrom
                (so => so.OpenDate.ToString("d", CultureInfo.InvariantCulture)))
                .ForMember(x => x.DueDate, mo => mo.MapFrom
                (so => so.DueDate.ToString("d", CultureInfo.InvariantCulture)))
                .ForMember(x => x.ExecutionType, mo => mo.MapFrom
                (so => so.ExecutionType.ToString()))
                .ForMember(x => x.LabelType, mo => mo.MapFrom
                (so => so.LabelType.ToString()));
            //Main map
            CreateMap<Employee, ExportEmployeeDto>()
                .ForMember(x=>x.Tasks,mo=>mo.MapFrom(so=>so.EmployeesTasks
                .Select(a=>a.Task)
                .OrderByDescending(a=>a.DueDate)
                .ThenBy(a=>a.Name)));

        }
                        
    }
}
