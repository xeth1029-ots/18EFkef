using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    /// <summary>小型企業人力提升計畫</summary>
    public class TransData_S_Class : IDBRow
    {
        public string Year { get; set; }
        public string PlanCode { get; set; }
        /// <summary>1:北基宜花金馬分署,2:桃竹苗分署,3:中彰投分署,4:雲嘉南分署,5:高屏澎東分署</summary>
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string TrainID { get; set; }
        public string ClassID { get; set; }
        public string JoinCompany { get; set; }
        public string TrainingUnit { get; set; }
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
        public int? TeachNum { get; set; }
        public string QuestLink { get; set; }
        /// <summary>問卷填答人數</summary>
        public string QuesSearchLink { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.TransData_S_Class;
        }
    }
}