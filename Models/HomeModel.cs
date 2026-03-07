using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using WKEFSERVICE.Areas.A1.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using Turbo.Commons;
using Turbo.DataLayer;

namespace WKEFSERVICE.Models
{
    public class HomeModel : PagingResultsViewModel
    {
        public IList<NewsModel> NewsGrid { get; set; }

        public IList<MeetingsModel> MeetingsGrid { get; set; }

        public IList<TeachersModel> TeachersGrid { get; set; }

        public IList<AuditReportsModel> AuditReportsGrid { get; set; }

        public TeachersAdminModel TeachersAdminModel { get; set; }

        public BranchsAdminModel BranchsAdminModel { get; set; }

        public AdminModel AdminModel { get; set; }
    }

    public class NewsModel : MsgPost
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string PostType_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.PostType_list().Where(x => x.Value == this.PostType.TONotNullString());
                var tmpStr = (tmp.Any()) ? tmp.FirstOrDefault().Text : "";
                return tmpStr;
            }
        }

        [NotDBField]
        public string PostTo_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.GRPID_List.Where(x => x.Value == this.PostTo.TONotNullString());
                var tmpStr = (tmp.Any()) ? tmp.FirstOrDefault().Text : "";
                return tmpStr;
            }
        }

        [NotDBField]
        public string PostDateS_Name
        {
            get
            {
                //if (PostDateS.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(PostDateS) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (PostDateS.TONotNullString() == "") return "";
                string[] tmpList = PostDateS.ToString().Split(' ');
                string tmpDate = tmpList[0]; //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string PostDateE_Name
        {
            get
            {
                //if (PostDateE.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(PostDateE) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (PostDateE.TONotNullString() == "") return "";
                string[] tmpList = PostDateE.ToString().Split(' ');
                string tmpDate = tmpList[0]; //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                //if (UpdatedDatetime.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(UpdatedDatetime) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }

        [NotDBField]
        public string PublishedUnitName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.UNIT_All_List.Where(x => x.Value == this.PublishedUnit.TONotNullString());
                string tmpStr = (tmp.Any()) ? tmp.FirstOrDefault().Text : "";
                return tmpStr;
            }
        }

        /// <summary>是否可以報名</summary>
        [NotDBField]
        public int isCanSignUp { get; set; }
    }

    public class MeetingsModel : Meeting
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string MeetingDate { get; set; }

        [NotDBField]
        public string MeetingTime { get; set; }

        [NotDBField]
        public string SignUpDateTime { get; set; }

        [NotDBField]
        public string MeetingTypeName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.MeetingType_list().Where(x => x.Value == this.MeetingType.TONotNullString());
                var tmpStr = (tmp.Any()) ? tmp.FirstOrDefault().Text : "";
                return tmpStr;
            }
        }

        [NotDBField]
        public bool isCanView
        {
            get
            {
                SessionModel sm = SessionModel.Get();
                return (sm.UserInfo.LoginCharacter == "3") ? (this.CreatedUnit == sm.UserInfo.User.UNITID) : true;
            }
        }
    }

    public class TeachersModel : Teacher
    {
        [NotDBField]
        public string Expertise_Str { get; set; }

        [NotDBField]
        public string[] Expertise_StrArray { get { return this.Expertise_Str.Split(',').ToArray(); } }

        [NotDBField]
        public string Tag1 { get { if (this.Expertise_StrArray.ToCount() < 1) return ""; return this.Expertise_StrArray[0]; } }

        [NotDBField]
        public string Tag2 { get { if (this.Expertise_StrArray.ToCount() < 2) return ""; return this.Expertise_StrArray[1]; } }

        [NotDBField]
        public string Tag3 { get { if (this.Expertise_StrArray.ToCount() < 3) return ""; return this.Expertise_StrArray[2]; } }
    }

    public class AuditReportsModel : SelfReport
    {
        [NotDBField]
        public string YearName { get; set; }

        [NotDBField]
        public string TeacherName { get; set; }

        [NotDBField]
        public string JobAbilityName { get; set; }

        [NotDBField]
        public string FirstTime_Name
        {
            get
            {
                if (string.IsNullOrEmpty(FirstTime) || FirstTime.TONotNullString() == "") return "";
                string[] tmpList = FirstTime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");
            }
        }

        [NotDBField]
        public string AuditStatus_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpObj = model.ReportRecordAuditStatus_List().Where(x => x.Value == this.AuditStatus).FirstOrDefault();
                return tmpObj.Text;
            }
        }
    }

    public class TeachersAdminModel
    {
        public TeachersAdminModel()
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            this.Year = DateTime.Now.ToString("yyyy");
            this.YearShow = (this.Year.TOInt32() - 1911).ToString();
            // 報告繳交期限
            if (sm.UserInfo != null) this.ReportLimit = dao.GetUploadDeadlineStr(this.Year, sm.UserInfo.User.UNITID);
            // 參加回流訓/通過測驗
            Hashtable parms1 = new Hashtable();
            parms1["Year"] = this.Year;
            parms1["TeacherAccount"] = sm.UserInfo.User.USERNO;
            var tmpObj1 = dao.QueryForListAll<MeetingAttend>("A2.GetTeacherAttendsData1", parms1);
            this.Attend = tmpObj1.Where(x => x.Attend == "Y").Any();
            this.AttendShow = this.Attend ? "達成" : "尚未達成";
            this.TestPassed = tmpObj1.Where(x => x.TestPassed == "Y").Any();
            this.TestPassedShow = this.TestPassed ? "達成" : "尚未達成";
            // 參加共識會議
            Hashtable parms2 = new Hashtable();
            parms2["Year"] = this.Year;
            parms2["TeacherAccount"] = sm.UserInfo.User.USERNO;
            parms2["CreatedUnit"] = sm.UserInfo.User.UNITID;
            var tmpObj2 = dao.QueryForListAll<Hashtable>("A2.GetTeacherAttendsData2", parms2);
            this.Meeting2Attend = (tmpObj2.FirstOrDefault()["MeetingCount"].TOInt32() >= 2);
            this.Meeting2AttendShow = this.Meeting2Attend ? "達成" : "尚未達成";
            this.Meeting2AttendCount = tmpObj2.FirstOrDefault()["MeetingCount"].ToString();
            // 繳交教學自我審核報告
            Hashtable parms3 = new Hashtable();
            parms3["Year"] = this.Year;
            parms3["TeacherAccount"] = sm.UserInfo.User.USERNO;
            parms3["UnitCode"] = sm.UserInfo.User.UNITID;
            var tmpObj3 = dao.QueryForListAll<Hashtable>("A2.GetTeacherReportRecord", parms3);
            this.ReportRec = (tmpObj3.ToCount() >= 1);
            this.ReportRecShow = (this.ReportRec) ? "達成" : "尚未達成";
            this.ReportRecCount = tmpObj3.ToCount().ToString();
        }

        /// <summary>年份</summary>
        public string Year { get; set; }

        /// <summary>年份 - 民國年</summary>
        public string YearShow { get; set; }

        /// <summary>繳交期限</summary>
        public string ReportLimit { get; set; }

        /// <summary>參加回流訓/通過測驗 - 是否出席場次</summary>
        public bool Attend { get; set; }

        /// <summary>參加回流訓/通過測驗 - 是否出席場次(顯示文字)</summary>
        public string AttendShow { get; set; }

        /// <summary>參加回流訓/通過測驗 - 是否通過測驗</summary>
        public bool TestPassed { get; set; }

        /// <summary>參加回流訓/通過測驗 - 是否通過測驗(顯示文字)</summary>
        public string TestPassedShow { get; set; }

        /// <summary>參加共識會議 - 是否達成</summary>
        public bool Meeting2Attend { get; set; }

        /// <summary>參加共識會議 - 是否達成(顯示文字)</summary>
        public string Meeting2AttendShow { get; set; }

        /// <summary>參加共識會議 - 達成次數</summary>
        public string Meeting2AttendCount { get; set; }

        /// <summary>繳交教學自我審核報告 - 是否達成</summary>
        public bool ReportRec { get; set; }

        /// <summary>繳交教學自我審核報告 - 是否達成(顯示文字)</summary>
        public string ReportRecShow { get; set; }

        /// <summary>繳交教學自我審核報告 - 次數</summary>
        public string ReportRecCount { get; set; }
    }

    public class BranchsAdminModel
    {
        public BranchsAdminModel()
        {
            SessionModel sm = SessionModel.Get();
            Hashtable byUnitCode = new Hashtable();
            byUnitCode["UnitCode"] = sm.UserInfo.User.UNITID;
            A3DAO dao = new A3DAO();
            ShareCodeListModel model = new ShareCodeListModel();
            // 本月新進教師數量
            var tmpObj1 = dao.QueryForListAll<Hashtable>("A3.GetTeacherThisMonthCount", null);
            this.NewTeacherCount = tmpObj1.FirstOrDefault()["TeacherCount"].TONotNullString();
            // 講師數量
            var tmpObj2 = dao.QueryForListAll<Hashtable>("A3.GetTeacherCount", null);
            this.TeacherCount = tmpObj2.FirstOrDefault()["TeacherCount"].TONotNullString();
            // 講師數量 by 分署別
            var tmpObj2_byUnitCode = dao.QueryForListAll<Hashtable>("A3.GetTeacherCount_byUnitCode", byUnitCode);
            this.TeacherCount_byUnitCode = tmpObj2_byUnitCode.FirstOrDefault()["TeacherCount"].TONotNullString();

            // 動機職能(DC)、行為職能(BC)、知識職能(KC)
            var tmpObj3 = dao.QueryForListAll<Hashtable>("A3.GetTeacherDCBCKCCount", null);
            this.TeacherDCCount = tmpObj3.FirstOrDefault()["DC"].TONotNullString();
            this.TeacherBCCount = tmpObj3.FirstOrDefault()["BC"].TONotNullString();
            this.TeacherKCCount = tmpObj3.FirstOrDefault()["KC"].TONotNullString();

            // 動機職能(DC)、行為職能(BC)、知識職能(KC) by 分署別
            var tmpObj3_byUnitCode = dao.QueryForListAll<Hashtable>("A3.GetTeacherDCBCKCCount_byUnitCode", byUnitCode);
            this.TeacherDCCount_byUnitCode = tmpObj3_byUnitCode.FirstOrDefault()["DC"].TONotNullString();
            this.TeacherBCCount_byUnitCode = tmpObj3_byUnitCode.FirstOrDefault()["BC"].TONotNullString();
            this.TeacherKCCount_byUnitCode = tmpObj3_byUnitCode.FirstOrDefault()["KC"].TONotNullString();

            // 學術界、產業界
            var tmpObj4 = dao.QueryForListAll<Teacher>("A3.GetTeacherIndustryAcademicType", null);
            var tmpList1 = model.IndustryAcademicType_List().Where(x => x.Text.ToLeft(3) == "學術界").Select(s => s.Value).ToArray();
            var tmpList2 = model.IndustryAcademicType_List().Where(x => x.Text.ToLeft(3) == "產業界").Select(s => s.Value).ToArray();
            this.TeacherIndustryAcademicCount1 = tmpObj4.Where(x => tmpList1.Contains(x.IndustryAcademicType)).ToCount().ToString();
            this.TeacherIndustryAcademicCount2 = tmpObj4.Where(x => tmpList2.Contains(x.IndustryAcademicType)).ToCount().ToString();

            // 學術界、產業界  by 分署別
            var tmpObj4_byUnitCode = dao.QueryForListAll<Teacher>("A3.GetTeacherIndustryAcademicType_byUnitCode", byUnitCode);
            var tmpList1_byUnitCode = model.IndustryAcademicType_List().Where(x => x.Text.ToLeft(3) == "學術界").Select(s => s.Value).ToArray();
            var tmpList2_byUnitCode = model.IndustryAcademicType_List().Where(x => x.Text.ToLeft(3) == "產業界").Select(s => s.Value).ToArray();
            this.TeacherIndustryAcademicCount1_byUnitCode = tmpObj4_byUnitCode.Where(x => tmpList1_byUnitCode.Contains(x.IndustryAcademicType)).ToCount().ToString();
            this.TeacherIndustryAcademicCount2_byUnitCode = tmpObj4_byUnitCode.Where(x => tmpList2_byUnitCode.Contains(x.IndustryAcademicType)).ToCount().ToString();

            // 補助大專校院辦理就業學程計畫
            var tmpTransData_D_Class = dao.QueryForListAll<TransData_D_Class>("A3.Get_TransData_D_Class_byUnitCode", byUnitCode);
            var Obj_D_DC = tmpTransData_D_Class.Where(x => x.JobAbilityCode == "DC").FirstOrDefault();
            var Obj_D_BC = tmpTransData_D_Class.Where(x => x.JobAbilityCode == "BC").FirstOrDefault();
            var Obj_D_KC = tmpTransData_D_Class.Where(x => x.JobAbilityCode == "KC").FirstOrDefault();
            this.TransData_D_Class_DC_byUnitCode = (Obj_D_DC == null) ? "0" : Obj_D_DC.TeachHours.ToString();
            this.TransData_D_Class_BC_byUnitCode = (Obj_D_BC == null) ? "0" : Obj_D_BC.TeachHours.ToString();
            this.TransData_D_Class_KC_byUnitCode = (Obj_D_KC == null) ? "0" : Obj_D_KC.TeachHours.ToString();

            // 小型企業人力提升計畫
            var tmpTransData_S_Class = dao.QueryForListAll<TransData_S_Class>("A3.Get_TransData_S_Class_byUnitCode", byUnitCode);
            var Obj_S_DC = tmpTransData_S_Class.Where(x => x.JobAbilityCode == "DC").FirstOrDefault();
            var Obj_S_BC = tmpTransData_S_Class.Where(x => x.JobAbilityCode == "BC").FirstOrDefault();
            var Obj_S_KC = tmpTransData_S_Class.Where(x => x.JobAbilityCode == "KC").FirstOrDefault();
            this.TransData_S_Class_DC_byUnitCode = (Obj_S_DC == null) ? "0" : Obj_S_DC.TeachHours.ToString();
            this.TransData_S_Class_BC_byUnitCode = (Obj_S_BC == null) ? "0" : Obj_S_BC.TeachHours.ToString();
            this.TransData_S_Class_KC_byUnitCode = (Obj_S_KC == null) ? "0" : Obj_S_KC.TeachHours.ToString();
        }

        #region 講師數量
        /// <summary>本月新進教師數量</summary>
        public string NewTeacherCount { get; set; }
        /// <summary>講師數量</summary>
        public string TeacherCount { get; set; }
        /// <summary>講師數量 by 分署別</summary>
        public string TeacherCount_byUnitCode { get; set; }
        /// <summary>動機職能(DC)</summary>
        public string TeacherDCCount { get; set; }
        /// <summary>行為職能(BC)</summary>
        public string TeacherBCCount { get; set; }
        /// <summary>知識職能(KC)</summary>
        public string TeacherKCCount { get; set; }
        /// <summary>動機職能(DC) by 分署別</summary>
        public string TeacherDCCount_byUnitCode { get; set; }
        /// <summary>行為職能(BC) by 分署別</summary>
        public string TeacherBCCount_byUnitCode { get; set; }
        /// <summary>知識職能(KC) by 分署別</summary>
        public string TeacherKCCount_byUnitCode { get; set; }
        /// <summary>學術界</summary>
        public string TeacherIndustryAcademicCount1 { get; set; }
        /// <summary>產業界</summary>
        public string TeacherIndustryAcademicCount2 { get; set; }
        /// <summary>學術界 by 分署別</summary>
        public string TeacherIndustryAcademicCount1_byUnitCode { get; set; }
        /// <summary>產業界 by 分署別</summary>
        public string TeacherIndustryAcademicCount2_byUnitCode { get; set; }
        #endregion

        #region 授課推廣
        /// <summary>補助大專校院辦理就業學程計畫(DC) by 分署別</summary>
        public string TransData_D_Class_DC_byUnitCode { get; set; }
        /// <summary>補助大專校院辦理就業學程計畫(BC) by 分署別</summary>
        public string TransData_D_Class_BC_byUnitCode { get; set; }
        /// <summary>補助大專校院辦理就業學程計畫(KC) by 分署別</summary>
        public string TransData_D_Class_KC_byUnitCode { get; set; }
        /// <summary>小型企業人力提升計畫(DC) by 分署別</summary>
        public string TransData_S_Class_DC_byUnitCode { get; set; }
        /// <summary>小型企業人力提升計畫(BC) by 分署別</summary>
        public string TransData_S_Class_BC_byUnitCode { get; set; }
        /// <summary>小型企業人力提升計畫(KC) by 分署別</summary>
        public string TransData_S_Class_KC_byUnitCode { get; set; }
        #endregion
    }

    public class AdminModel
    {
        public AdminModel()
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            // 本月新進教師數量
            var tmpObj1 = dao.QueryForListAll<Hashtable>("A3.GetTeacherThisMonthCount", null);
            this.NewTeacherCount = tmpObj1.FirstOrDefault()["TeacherCount"].TONotNullString();

            // 講師數量 區分分署 1~5
            var tmpObj2 = dao.QueryForListAll<Hashtable>("A3.GetTeacherCount", null);
            this.TeacherCount = tmpObj2.FirstOrDefault()["TeacherCount"].TONotNullString();
            this.TeacherCountUnit1 = tmpObj2.FirstOrDefault()["TeacherCountUnit1"].TONotNullString();
            this.TeacherCountUnit2 = tmpObj2.FirstOrDefault()["TeacherCountUnit2"].TONotNullString();
            this.TeacherCountUnit3 = tmpObj2.FirstOrDefault()["TeacherCountUnit3"].TONotNullString();
            this.TeacherCountUnit4 = tmpObj2.FirstOrDefault()["TeacherCountUnit4"].TONotNullString();
            this.TeacherCountUnit5 = tmpObj2.FirstOrDefault()["TeacherCountUnit5"].TONotNullString();

            //可授課職能類別 產學類別 //動機職能(DC)、行為職能(BC)、知識職能(KC)
            var tmpObj3 = dao.QueryForListAll<Hashtable>("A3.GetTeacherDCBCKCCount", null);
            this.Teacher_TeachJobAbility_DC = tmpObj3.FirstOrDefault()["DC"].TONotNullString(); //動機職能(DC)人數
            this.Teacher_TeachJobAbility_BC = tmpObj3.FirstOrDefault()["BC"].TONotNullString(); //行為職能(BC)人數
            this.Teacher_TeachJobAbility_KC = tmpObj3.FirstOrDefault()["KC"].TONotNullString(); //知識職能(KC)人數
            this.Teacher_IndustryAcademicType_2345 = tmpObj3.FirstOrDefault()["IAT2345"].TONotNullString(); //學術界人數
            this.Teacher_IndustryAcademicType_1 = tmpObj3.FirstOrDefault()["IAT1"].TONotNullString();//產業界人數

            // 補助大專校院辦理就業學程計畫
            var tmpTransData_D_Class = dao.QueryForListAll<TransData_D_Class>("A3.Get_TransData_D_Class", null);
            var Obj_D_DC = tmpTransData_D_Class.Where(x => x.JobAbilityCode == "DC").FirstOrDefault();
            var Obj_D_BC = tmpTransData_D_Class.Where(x => x.JobAbilityCode == "BC").FirstOrDefault();
            var Obj_D_KC = tmpTransData_D_Class.Where(x => x.JobAbilityCode == "KC").FirstOrDefault();
            this.TransData_D_Class_DC_byUnitCode = (Obj_D_DC == null) ? "0" : Obj_D_DC.TeachHours.ToString();
            this.TransData_D_Class_BC_byUnitCode = (Obj_D_BC == null) ? "0" : Obj_D_BC.TeachHours.ToString();
            this.TransData_D_Class_KC_byUnitCode = (Obj_D_KC == null) ? "0" : Obj_D_KC.TeachHours.ToString();

            // 小型企業人力提升計畫
            var tmpTransData_S_Class = dao.QueryForListAll<TransData_S_Class>("A3.Get_TransData_S_Class", null);
            var Obj_S_DC = tmpTransData_S_Class.Where(x => x.JobAbilityCode == "DC").FirstOrDefault();
            var Obj_S_BC = tmpTransData_S_Class.Where(x => x.JobAbilityCode == "BC").FirstOrDefault();
            var Obj_S_KC = tmpTransData_S_Class.Where(x => x.JobAbilityCode == "KC").FirstOrDefault();
            this.TransData_S_Class_DC_byUnitCode = (Obj_S_DC == null) ? "0" : Obj_S_DC.TeachHours.ToString();
            this.TransData_S_Class_BC_byUnitCode = (Obj_S_BC == null) ? "0" : Obj_S_BC.TeachHours.ToString();
            this.TransData_S_Class_KC_byUnitCode = (Obj_S_KC == null) ? "0" : Obj_S_KC.TeachHours.ToString();
        }

        /// <summary>本月新進教師數量</summary>
        public string NewTeacherCount { get; set; }

        #region 講師數量 區分分署
        /// <summary>講師數量</summary>
        public string TeacherCount { get; set; }

        /// <summary>講師數量 區分分署 1</summary>
        public string TeacherCountUnit1 { get; set; }

        /// <summary>講師數量 區分分署 2</summary>
        public string TeacherCountUnit2 { get; set; }

        /// <summary>講師數量 區分分署 3</summary>
        public string TeacherCountUnit3 { get; set; }

        /// <summary>講師數量 區分分署 4</summary>
        public string TeacherCountUnit4 { get; set; }

        /// <summary>講師數量 區分分署 5</summary>
        public string TeacherCountUnit5 { get; set; }
        #endregion

        #region Total number of courses
        //Total number of courses
        /// <summary>課程總數 北分署</summary>
        public string CoursesCountUnit1 { get; set; }
        /// <summary>課程總數 桃分署</summary>
        public string CoursesCountUnit2 { get; set; }
        /// <summary>課程總數 中分署</summary>
        public string CoursesCountUnit3 { get; set; }
        /// <summary>課程總數 高分署</summary>
        public string CoursesCountUnit4 { get; set; }
        /// <summary>課程總數 南分署</summary>
        public string CoursesCountUnit5 { get; set; }
        #endregion

        #region 授課推廣
        /// <summary>補助大專校院辦理就業學程計畫(DC) by 分署別</summary>
        public string TransData_D_Class_DC_byUnitCode { get; set; }
        /// <summary>補助大專校院辦理就業學程計畫(BC) by 分署別</summary>
        public string TransData_D_Class_BC_byUnitCode { get; set; }
        /// <summary>補助大專校院辦理就業學程計畫(KC) by 分署別</summary>
        public string TransData_D_Class_KC_byUnitCode { get; set; }
        /// <summary>小型企業人力提升計畫(DC) by 分署別</summary>
        public string TransData_S_Class_DC_byUnitCode { get; set; }
        /// <summary>小型企業人力提升計畫(BC) by 分署別</summary>
        public string TransData_S_Class_BC_byUnitCode { get; set; }
        /// <summary>小型企業人力提升計畫(KC) by 分署別</summary>
        public string TransData_S_Class_KC_byUnitCode { get; set; }

        public string DISTIDCodeName1
        {
            get
            {
                //ShareCodeListModel model = new ShareCodeListModel();
                var Obj = new ShareCodeListModel().DISTIDCode_list("1").FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }
        public string DISTIDCodeName2
        {
            get
            {
                //ShareCodeListModel model = new ShareCodeListModel();
                var Obj = new ShareCodeListModel().DISTIDCode_list("2").FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }

        public string DISTIDCodeName3
        {
            get
            {
                //ShareCodeListModel model = new ShareCodeListModel();
                var Obj = new ShareCodeListModel().DISTIDCode_list("3").FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }
        public string DISTIDCodeName4
        {
            get
            {
                //ShareCodeListModel model = new ShareCodeListModel();
                var Obj = new ShareCodeListModel().DISTIDCode_list("4").FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }

        public string DISTIDCodeName5
        {
            get
            {
                //ShareCodeListModel model = new ShareCodeListModel();
                var Obj = new ShareCodeListModel().DISTIDCode_list("5").FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }

        #endregion

        #region 可授課職能類別 產學類別
        //動機職能(DC)人數 //計算加總「Teacher」表中【TeachJobAbilityDC】=Y的師資筆數
        /// <summary>動機職能(DC)人數</summary>
        public string Teacher_TeachJobAbility_DC { get; set; }
        //行為職能(BC)人數：//計算加總「Teacher」表中【TeachJobAbilityBC】=Y的師資筆數
        /// <summary>行為職能(BC)人數</summary>
        public string Teacher_TeachJobAbility_BC { get; set; }
        //知識職能(KC)人數： //計算加總「Teacher」表中【TeachJobAbilityKC】=Y的師資筆數
        /// <summary>知識職能(KC)人數</summary>
        public string Teacher_TeachJobAbility_KC { get; set; }
        //學術界人數： //計算加總「Teacher」表中【IndustryAcademicType】in 2、3、4、5的師資筆數
        /// <summary>學術界人數</summary>
        public string Teacher_IndustryAcademicType_2345 { get; set; }
        //產業界人數 ：//計算加總「Teacher」表中【IndustryAcademicType】=1的師資筆數
        /// <summary>產業界人數</summary>
        public string Teacher_IndustryAcademicType_1 { get; set; }

        #endregion

    }

    public class Api_TeachersModel
    {
        /// <summary>姓名</summary>
        public string plkt_name { get; set; }

        /// <summary>身份證字號</summary>
        public string plkt_idno { get; set; }

        /// <summary>服務機構</summary>
        public string plkt_company { get; set; }

        /// <summary>職稱</summary>
        public string plkt_co { get; set; }

        /// <summary>服務機構2</summary>
        public string plkt_company2 { get; set; }

        /// <summary>職稱2</summary>
        public string plkt_co2 { get; set; }

        /// <summary>所屬區域 (2:北分/3:桃分/4:中分/5:南分/6:高分)</summary>
        public string plkt_dis_id { get; set; }

        /// <summary>成為師資年度</summary>
        public string plkt_year { get; set; }

        /// <summary>動機職能 (DC)</summary>
        public string plkt_dc { get; set; }

        /// <summary>行為職能 (BC)</summary>
        public string plkt_bc { get; set; }

        /// <summary>知識職能 (KC)</summary>
        public string plkt_kc { get; set; }

        /// <summary>是否在線</summary>
        public string Online { get; set; }

        /// <summary>下線時間</summary>
        public string OfflineDate { get; set; }
    }
}