using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    /// <summary>企業人力資源提升計畫(大人提)</summary>
    public class TransData_K_Class : IDBRow
    {
        public string ADSysName { get; set; }
        public int? Year { get; set; }
        public string PlanCode { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string TrainID { get; set; }
        public string JoinCompany { get; set; }
        public string TrainingUnit { get; set; }
        public int? ClassID { get; set; }
        public string ClassName { get; set; }
        public string ClassDateS { get; set; }
        public string ClassDateE { get; set; }
        public string TrainingTimeS { get; set; }
        public string TrainingTimeE { get; set; }
        public string TeacherID { get; set; }
        public string TeacherName { get; set; }
        public string TrainingType { get; set; }
        public string JobAbilityCode { get; set; }
        public string CourseUnitCode { get; set; }
        public int? TeachHours { get; set; }
        public int? TeachHours_X { get; set; }
        public int? TeachNum { get; set; }
        public string QuestLink { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.TransData_K_Class;
        }
    }
}