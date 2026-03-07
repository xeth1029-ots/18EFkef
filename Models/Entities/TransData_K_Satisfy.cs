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
    public class TransData_K_Satisfy : IDBRow
    {
        /// <summary>Year	nvarchar	4</summary> 
        public string Year { get; set; }
        /// <summary>PlanCode	varchar	10</summary> 
        public string PlanCode { get; set; }
        /// <summary>UnitCode	int </summary> 
        public string UnitCode { get; set; }
        /// <summary>TrainID	varchar	30</summary> 
        public string TrainID { get; set; }
        /// <summary>ClassID int </summary> 
        public int? ClassID { get; set; }
        /// <summary>ClassName	nvarchar,333</summary> 
        public string ClassName { get; set; }
        /// <summary>CourseUnitCode	varchar,10</summary> 
        public string CourseUnitCode { get; set; }
        /// <summary>TeacherID	varchar,20</summary> 
        public string TeacherID { get; set; }
        /// <summary>TeacherName	nvarchar,300</summary> 
        public string TeacherName { get; set; }
        /// <summary>StudentID	varchar,20</summary> 
        public string StudentID { get; set; }
        /// <summary>StudentName	nvarchar,300</summary> 
        public string StudentName { get; set; }
        /// <summary>PersonWorkExp	nvarchar,66</summary> 
        public string PersonWorkExp { get; set; }
        /// <summary>DutyType	nvarchar,66</summary> 
        public string DutyType { get; set; }
        /// <summary>FillDate	datetime</summary> 
        public DateTime? FillDate { get; set; }
        /// <summary>Q1	int </summary> 
        public int? Q1 { get; set; }
        /// <summary>Q2	int </summary> 
        public int? Q2 { get; set; }
        /// <summary>Q3	int </summary> 
        public int? Q3 { get; set; }
        /// <summary>Q4	int </summary> 
        public int? Q4 { get; set; }
        /// <summary>Q5	int </summary> 
        public int? Q5 { get; set; }
        /// <summary>Q6	int </summary> 
        public int? Q6 { get; set; }
        /// <summary>Q7	int </summary> 
        public int? Q7 { get; set; }
        /// <summary>Q8	int </summary> 
        public int? Q8 { get; set; }
        /// <summary>Q9	int </summary> 
        public int? Q9 { get; set; }
        /// <summary>Q10,int </summary> 
        public int? Q10 { get; set; }
        /// <summary>Q11,int </summary> 
        public int? Q11 { get; set; }
        /// <summary>Suggest,nvarchar,3000</summary> 
        public string Suggest { get; set; }
        /// <summary>UpdatedDatetime,datetime</summary> 
        public string UpdatedDatetime { get; set; }

        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.TransData_K_Satisfy;
        }
    }
}
