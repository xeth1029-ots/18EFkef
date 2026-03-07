using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Turbo.Commons;

namespace WKEFSERVICE.Commons
{
    /// <summary>系統代碼及表格名稱列舉</summary>
    public partial class StaticCodeMap
    {
        /// <summary>代碼表類別列舉清單, 叫用 KeyMapDAO.GetCodeMapList() 所需的參數</summary>
        public class CodeMap : CodeMapType
        {
            #region 私有(隱藏) CodeMap 建構式

            private CodeMap(string codeName) : base(codeName)
            { }

            /// <summary>
            ///
            /// </summary>
            /// <param name="codeName"></param>
            /// <param name="sqlStatementId"></param>
            private CodeMap(string codeName, string sqlStatementId) :
                base(codeName, sqlStatementId)
            { }

            #endregion 私有(隱藏) CodeMap 建構式

            // 前台申辦案件縣市別清單
            //public static CodeMapType E_SRV_CITY = new CodeMapType("E_SRV_CITY", "KeyMap.getE_SRV_CITY");
            // 系統名稱清單
            //public static CodeMapType sysid = new CodeMapType("sysid", "KeyMap.getSYSID");
            // 模組名稱清單
            //public static CodeMapType modules = new CodeMapType("modules", "KeyMap.getMODULES");
            // 單位資料
            //public static CodeMapType unit = new CodeMapType("unit", "KeyMap.getUNIT");
            // 單位資料
            //public static CodeMapType unit1 = new CodeMapType("unit1", "KeyMap.getUNIT1");
            // 功能群組
            //public static CodeMapType amgrp = new CodeMapType("amgrp", "KeyMap.getAMGRP");
            // 申辦項目
            //public static CodeMapType apply = new CodeMapType("apply", "KeyMap.getApply");
            // 申辦縣市
            //public static CodeMapType srv_city = new CodeMapType("srv_city", "KeyMap.getSrv_City");
            // 申辦縣市
            //public static CodeMapType srv_city_unit = new CodeMapType("srv_city_unit", "KeyMap.getSrv_City_Unit");
            // 申辦縣市
            //public static CodeMapType srv_city_parm = new CodeMapType("srv_city_parm", "KeyMap.getSrv_City_Parm");
            // 申辦縣市轄區
            //public static CodeMapType srv_city_s_parm = new CodeMapType("srv_city_s_parm", "KeyMap.getsrv_city_s_parm");
            // 狀態所屬種類
            //public static CodeMapType applyhardtype = new CodeMapType("applyhardtype", "KeyMap.getApplyhardtype");
            // 處理狀態
            //public static CodeMapType srv_status = new CodeMapType("srv_status", "KeyMap.getSrv_status");
            // 處理狀態
            //public static CodeMapType srv_status1 = new CodeMapType("srv_status1", "KeyMap.getSrv_status1");
            // 承辦人
            //public static CodeMapType apy_undertaker = new CodeMapType("apy_undertaker", "KeyMap.getApy_undertaker");
            // 公告類型
            //public static CodeMapType enews = new CodeMapType("enews", "KeyMap.getEnews");
            // 常見問題類型
            //public static CodeMapType code_name = new CodeMapType("code_name", "KeyMap.getfaq");
            // 常見問題類型(不顯示停用)
            //public static CodeMapType code_name1 = new CodeMapType("code_name1", "KeyMap.getfaq1");
            // 最新公告上架狀態
            //public static CodeMapType status = new CodeMapType("status", "KeyMap.getEnews");
            // 機構類別
            //public static CodeMapType Type_id = new CodeMapType("Type_id", "KeyMap.getType_id");
            // 機構類別
            //public static CodeMapType StatuList = new CodeMapType("StatuList", "KeyMap.getStatuList");
            // 登記事項
            //public static CodeMapType Apy_change = new CodeMapType("Apy_change", "KeyMap.getApy_change");
            // 病床類型
            //public static CodeMapType Bed_type = new CodeMapType("Bed_type", "KeyMap.getBed_type");
            //病床類型
            //public static CodeMapType Bed_kind = new CodeMapType("Bed_kind", "KeyMap.getBed_kind");
            //執業科別(限院所)
            //public static CodeMapType PrtDept = new CodeMapType("PrtDept", "KeyMap.getPrtDept");
            //執業科別(全部)
            //public static CodeMapType DcdDept = new CodeMapType("DcdDept", "KeyMap.getDcdDept");
            //執業科別(全部)
            //public static CodeMapType NDcdDept = new CodeMapType("NDcdDept", "KeyMap.getNDcdDept");
            //醫事證書類別(全部)
            //public static CodeMapType RefID = new CodeMapType("RefID", "KeyMap.getRefID");
            //權屬別(全部)
            //public static CodeMapType AUTHOR = new CodeMapType("AUTHOR", "KeyMap.getAUTHOR");
            //縣市清單
            //public static CodeMapType Zip_City = new CodeMapType("Zip_City", "KeyMap.getZip_City");
            //鄉鎮區清單
            //public static CodeMapType Zip_Town = new CodeMapType("Zip_Town", "KeyMap.getZip_Town");
            //街道清單
            //public static CodeMapType Zip_Road = new CodeMapType("Zip_Road", "KeyMap.getZip_Road");
            //領取地址清單(申辦查詢)
            //public static CodeMapType Get_Body_p = new CodeMapType("Get_Body_p", "KeyMap.getBody_p");
            //領取地址清單(填寫申辦)
            //public static CodeMapType Get_Body_w = new CodeMapType("Get_Body_w", "KeyMap.getBody_w");

            ///<summary>所屬單位清單 </summary>
            public static CodeMapType UNIT_List = new CodeMapType("UNIT_List", "KeyMap.getUNIT_List");
            /// <summary>所屬單位清單 (含本署)</summary>
            public static CodeMapType UNIT_All_List = new CodeMapType("UNIT_All_List", "KeyMap.getUNIT_All_List");
            /// <summary>所屬單位清單(短) (含本署)</summary>
            public static CodeMapType UNIT_Shot_List = new CodeMapType("UNIT_Shot_List", "KeyMap.getUNIT_Shot_List");
            /// <summary>帳號角色清單</summary>
            public static CodeMapType GRPID_List = new CodeMapType("GRPID_List", "KeyMap.getGRPID_List");
            /// <summary>模組名稱清單</summary>
            public static CodeMapType MODULES_list = new CodeMapType("MODULES_list", "KeyMap.getMODULES_list");
            /// <summary>選擇會議的下拉清單</summary>
            public static CodeMapType MeetingSeq_List = new CodeMapType("MeetingSeq_List", "KeyMap.getMeetingSeq_List");
            /// <summary>授課單元 下拉清單<br />CODE field is: CourseUnitCode</summary>
            public static CodeMapType TeachUnit_List_UnitCode = new CodeMapType("TeachUnit_List_UnitCode", "KeyMap.getTeachUnit_List_UnitCode");
            /// <summary>授課單元 下拉清單<br />CODE field is: JobAbilityCode</summary>
            public static CodeMapType TeachUnit_List_JobACode = new CodeMapType("TeachUnit_List_JobACode", "KeyMap.getTeachUnit_List_JobACode");
            /// <summary>產業別(大類) 下拉清單</summary>
            public static CodeMapType Industry_List = new CodeMapType("Industry_List", "KeyMap.getIndustry_List");
            /// <summary>學歷 下拉清單</summary>
            public static CodeMapType EduLevel_List = new CodeMapType("EduLevel_List", "KeyMap.getEduLevel_List");
            /// <summary>專長類別標籤 下拉清單</summary>
            public static CodeMapType Expertise_List = new CodeMapType("Expertise_List", "KeyMap.getExpertise_List");
            /// <summary>報告管理的 年度 下拉清單</summary>
            public static CodeMapType ReportRecordYears_List = new CodeMapType("ReportRecordYears_List", "KeyMap.getReportRecordYears_List");
            /// <summary>職能類別 清單</summary>
            public static CodeMapType JobAbilityCode_List = new CodeMapType("JobAbilityCode_List", "KeyMap.getJobAbilityCode_List");
            /// <summary>分署資訊 清單</summary>
            public static CodeMapType DISTIDCode_List = new CodeMapType("DISTIDCode_List", "KeyMap.getDISTIDCode_List");
            /// <summary>地區類別選單</summary>
            public static CodeMapType City_List = new CodeMapType("City_List", "KeyMap.getCity_List");
        }
    }
}