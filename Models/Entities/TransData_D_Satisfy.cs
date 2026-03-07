using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    /// <summary>補助大專校院辦理就業學程計畫</summary>
    public class TransData_D_Satisfy : IDBRow
    {
        public string Year { get; set; }
        public string PlanCode { get; set; }
        /// <summary>1:北基宜花金馬分署,2:桃竹苗分署,3:中彰投分署,4:雲嘉南分署,5:高屏澎東分署</summary>
        public string UnitCode { get; set; }
        public string PlanID { get; set; }
        public string ClassName { get; set; }
        public string CourseUnitCode { get; set; }
        public string TeacherID { get; set; }
        public string TeacherName { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Dept { get; set; }
        public string Identity { get; set; }
        public string FillDate { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }
        public string Q6 { get; set; }
        public string Q7 { get; set; }
        public string Q8 { get; set; }
        public string Q9 { get; set; }
        public string Q10 { get; set; }
        public string Q11 { get; set; }
        public string Suggest { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.TransData_D_Satisfy;
        }
    }
}