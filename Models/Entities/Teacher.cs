using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using System.ComponentModel.DataAnnotations;

namespace WKEFSERVICE.Models.Entities
{
    public class Teacher : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public string ACCOUNT { get; set; }
        public string IDNO { get; set; }
        /// <summary>1:北基宜花金馬分署,2:桃竹苗分署,3:中彰投分署,4:雲嘉南分署,5:高屏澎東分署</summary>
        public string UnitCode { get; set; }
        public string Birthday { get; set; }
        public string IsShowBirthday { get; set; }
        public string TeacherName { get; set; }
        public string TeacherEName { get; set; }
        public string Sex { get; set; }
        public string Tel { get; set; }
        public string IsShowTel { get; set; }
        public string Phone { get; set; }
        public string IsShowPhone { get; set; }
        public string PostalCode1 { get; set; }
        public string PostalCode2 { get; set; }
        public string Address { get; set; }
        public string IsShowAddress { get; set; }
        public string Email { get; set; }
        public string IsShowEmail { get; set; }
        public string EmailWork { get; set; }
        public string IsShowEmailWork { get; set; }
        public string EduLevelHighest { get; set; }
        public string EduSchool1 { get; set; }
        public string EduDept1 { get; set; }
        public string EduLevel1 { get; set; }
        public string EduSchool2 { get; set; }
        public string EduDept2 { get; set; }
        public string EduLevel2 { get; set; }
        public string EduSchool3 { get; set; }
        public string EduDept3 { get; set; }
        public string EduLevel3 { get; set; }
        public string ServiceUnit1 { get; set; }
        public string JobTitle1 { get; set; }
        public string ServiceUnit2 { get; set; }
        public string JobTitle2 { get; set; }
        public string ExpertiseDesc { get; set; }
        public string ExpertiseCode { get; set; }
        public string TeachJobAbilityDC { get; set; }
        public string TeachJobAbilityBC { get; set; }
        public string TeachJobAbilityKC { get; set; }
        public string TeachArea { get; set; }
        public string TeachIndustryDetCode { get; set; }
        public string IndustryAcademicType { get; set; }
        public string WorkHistory { get; set; }
        public string ProLicense { get; set; }
        public string SelfIntroduction { get; set; }
        public string SelfPicPath { get; set; }
        /// <summary>「其他」大項-共通核心職能老師:N:否Y:是</summary>
        [Display(Name = "共通核心職能老師")]
        public string PublicCore  { get; set; }
        public string Online { get; set; }
        public string JoinYear { get; set; }
        public string OfflineDate { get; set; }
        public string OfflineReason { get; set; }
        public string OfflineReasonRemark { get; set; }
        public string HomePageCarousel { get; set; }
        public string CreatedAccount { get; set; }
        public string CreatedDatetime { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public string ServiceUnitProperties { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.Teacher;
        }
    }
}