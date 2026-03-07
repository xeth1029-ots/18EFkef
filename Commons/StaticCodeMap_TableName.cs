using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Turbo.DataLayer;

namespace WKEFSERVICE.Commons
{
    /// <summary>系統代碼及表格名稱列舉</summary>
    public partial class StaticCodeMap
    {
        /// <summary>系統表格名稱列舉</summary>
        public class TableName
        {
            public static DBRowTableName AMCHANGEPWD_GUID = DBRowTableName.Instance("AMCHANGEPWD_GUID");
            public static DBRowTableName AMDBURM = DBRowTableName.Instance("AMDBURM");
            public static DBRowTableName AMDBURM_LOG = DBRowTableName.Instance("AMDBURM_LOG");
            public static DBRowTableName AMCHANGEPWD_LOG = DBRowTableName.Instance("AMCHANGEPWD_LOG");
            public static DBRowTableName AMFUNCM = DBRowTableName.Instance("AMFUNCM");
            public static DBRowTableName AMGMAPM = DBRowTableName.Instance("AMGMAPM");
            public static DBRowTableName AMGRP = DBRowTableName.Instance("AMGRP");
            public static DBRowTableName AMLOGIN = DBRowTableName.Instance("AMLOGIN");
            public static DBRowTableName MsgPost = DBRowTableName.Instance("MsgPost");
            public static DBRowTableName MsgPostAttached = DBRowTableName.Instance("MsgPostAttached");
            public static DBRowTableName Meeting = DBRowTableName.Instance("Meeting");
            public static DBRowTableName MeetingAttached = DBRowTableName.Instance("MeetingAttached");
            public static DBRowTableName MeetingSignUp = DBRowTableName.Instance("MeetingSignUp");
            public static DBRowTableName MeetingAttend = DBRowTableName.Instance("MeetingAttend");
            public static DBRowTableName Teacher = DBRowTableName.Instance("Teacher");
            public static DBRowTableName FinalReport = DBRowTableName.Instance("FinalReport");
            public static DBRowTableName Report = DBRowTableName.Instance("Report");
            public static DBRowTableName ReportRecord = DBRowTableName.Instance("ReportRecord");
            public static DBRowTableName ReviewReport = DBRowTableName.Instance("ReviewReport");
            public static DBRowTableName SelfReport = DBRowTableName.Instance("SelfReport");
            public static DBRowTableName loginlog = DBRowTableName.Instance("loginlog");
            public static DBRowTableName sys_login_log = DBRowTableName.Instance("sys_login_log");
            public static DBRowTableName funclog = DBRowTableName.Instance("funclog");
            public static DBRowTableName filelog = DBRowTableName.Instance("filelog");
            public static DBRowTableName Notices = DBRowTableName.Instance("Notices");
            public static DBRowTableName NoticesAttached = DBRowTableName.Instance("NoticesAttached");
            // 介接用
            public static DBRowTableName TransData_D_Class = DBRowTableName.Instance("TransData_D_Class");
            public static DBRowTableName TransData_D_Satisfy = DBRowTableName.Instance("TransData_D_Satisfy");
            public static DBRowTableName TransData_S_Class = DBRowTableName.Instance("TransData_S_Class");
            public static DBRowTableName TransData_S_Satisfy = DBRowTableName.Instance("TransData_S_Satisfy");
            public static DBRowTableName TransData_K_Class = DBRowTableName.Instance("TransData_K_Class");
            public static DBRowTableName TransData_K_Satisfy = DBRowTableName.Instance("TransData_K_Satisfy");
        }
    }

}