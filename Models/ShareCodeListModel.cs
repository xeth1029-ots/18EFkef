using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Turbo.Commons;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models.Entities;
using System.Collections;
using WKEFSERVICE.Services;

namespace WKEFSERVICE.Models
{
    /// <summary>
    /// 共用代碼統一模型(所有下拉選單放置)
    /// </summary>
    public class ShareCodeListModel
    {
        /// <summary>
        /// 所屬單位清單
        /// </summary>
        public IList<SelectListItem> UNIT_List
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.UNIT_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary>
        /// 所屬單位清單 (含本署)
        /// </summary>
        public IList<SelectListItem> UNIT_All_List
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.UNIT_All_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary>
        /// 所屬單位清單(短) (含本署)
        /// </summary>
        public IList<SelectListItem> UNIT_Shot_List
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.UNIT_Shot_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary>
        /// 帳號角色清單
        /// </summary>
        public IList<SelectListItem> GRPID_List
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.GRPID_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary>
        /// 帳號使用狀態_清單來源
        /// </summary>
        public IList<SelectListItem> AUTHSTATUS_list(bool Only_Use_Unuse = false)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("1", "使用中");
            dictionary.Add("2", "停用");
            if (!Only_Use_Unuse) dictionary.Add("0", "申請中");
            if (!Only_Use_Unuse) dictionary.Add("8", "帳號鎖定");
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 模組名稱清單
        /// </summary>
        public IList<SelectListItem> MODULES_list(string SYSID)
        {
            MyKeyMapDAO dao = new MyKeyMapDAO();
            Hashtable parms = new Hashtable();
            parms["SYSID"] = SYSID;
            IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.MODULES_list, parms);
            return MyCommonUtil.ConvertSelItems(list);
        }

        /// <summary>
        /// 訊息公告 - 類別
        /// </summary>
        /// <param name="Only_Use_Unuse"></param>
        /// <returns></returns>
        public IList<SelectListItem> PostType_list()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "訊息公告" },
                { "2", "會議報名公告" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 訊息公告 - 選擇會議的下拉清單
        /// </summary>
        public IList<SelectListItem> MeetingSeq_list
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                Hashtable parms = new Hashtable();
                SessionModel sm = SessionModel.Get();
                if (sm.UserInfo.LoginCharacter == "3")
                {
                    parms["SelfSelect"] = " AND a.CreatedUnit='" + sm.UserInfo.User.UNITID + "'";
                }
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.MeetingSeq_List, parms);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary>會議類別／1:回流訓／2:共識會議</summary>
        /// <returns></returns>
        public IList<SelectListItem> MeetingType_list()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "回流訓" },
                { "2", "共識會議" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 縣市<br />CodeIsText = 是否編碼等於顯示名
        /// </summary>
        /// <param name="CodeIsText"></param>
        /// <returns></returns>
        public IList<SelectListItem> MeetLocated_List(bool CodeIsText = false)
        {
            if (CodeIsText)
            {
                var dictionary = new Dictionary<string, string> {
                    { "基隆市", "基隆市" },
                    { "嘉義市", "嘉義市" },
                    { "台北市", "台北市" },
                    { "嘉義縣", "嘉義縣" },
                    { "新北市", "新北市" },
                    { "台南市", "台南市" },
                    { "桃園縣", "桃園縣" },
                    { "高雄市", "高雄市" },
                    { "新竹市", "新竹市" },
                    { "屏東縣", "屏東縣" },
                    { "新竹縣", "新竹縣" },
                    { "台東縣", "台東縣" },
                    { "苗栗縣", "苗栗縣" },
                    { "花蓮縣", "花蓮縣" },
                    { "台中市", "台中市" },
                    { "宜蘭縣", "宜蘭縣" },
                    { "彰化縣", "彰化縣" },
                    { "澎湖縣", "澎湖縣" },
                    { "南投縣", "南投縣" },
                    { "金門縣", "金門縣" },
                    { "雲林縣", "雲林縣" },
                    { "連江縣", "連江縣" },
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
            else
            {
                var dictionary = new Dictionary<string, string> {
                    { "01", "基隆市" },
                    { "12", "嘉義市" },
                    { "02", "台北市" },
                    { "13", "嘉義縣" },
                    { "03", "新北市" },
                    { "14", "台南市" },
                    { "04", "桃園縣" },
                    { "15", "高雄市" },
                    { "05", "新竹市" },
                    { "16", "屏東縣" },
                    { "06", "新竹縣" },
                    { "17", "台東縣" },
                    { "07", "苗栗縣" },
                    { "18", "花蓮縣" },
                    { "08", "台中市" },
                    { "19", "宜蘭縣" },
                    { "09", "彰化縣" },
                    { "20", "澎湖縣" },
                    { "10", "南投縣" },
                    { "21", "金門縣" },
                    { "11", "雲林縣" },
                    { "22", "連江縣" },
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
        }

        /// <summary>
        /// 圖檔位置<br />R：右、L：左
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> PicAlign_list()
        {
            var dictionary = new Dictionary<string, string> {
                { "R", "右" },
                { "L", "左" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 會議報名管道<br />1：自行(教師自行登入報名)<br />2：管理者(管理者由後台存入)
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> SignUpType_list()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "自行　(教師自行登入報名)" },
                { "2", "管理者(管理者由後台存入)" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>授課單元 下拉清單</summary>
        public IList<SelectListItem> TeachUnit_list(bool getUnitCode = true)
        {
            MyKeyMapDAO dao = new MyKeyMapDAO();
            IList<KeyMapModel> list = dao.GetCodeMapList(
                (getUnitCode) ?
                Commons.StaticCodeMap.CodeMap.TeachUnit_List_UnitCode :
                Commons.StaticCodeMap.CodeMap.TeachUnit_List_JobACode
            );
            return MyCommonUtil.ConvertSelItems(list);
        }

        /// <summary> 產業別(大類) 下拉清單</summary>
        public IList<SelectListItem> Industry_list
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.Industry_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary> 地區類別選單 下拉清單</summary>
        public IList<SelectListItem> City_list1
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.City_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary> 地區類別選單 下拉清單</summary>
        public IList<SelectListItem> City_list2
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.City_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }
        /// <summary> 學歷 下拉清單 </summary>
        public IList<SelectListItem> EduLevel_List
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.EduLevel_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary> 專長類別標籤 下拉清單</summary>
        public IList<SelectListItem> Expertise_List
        {
            get
            {
                MyKeyMapDAO dao = new MyKeyMapDAO();
                IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.Expertise_List);
                return MyCommonUtil.ConvertSelItems(list);
            }
        }

        /// <summary> 下線原因</summary>
        /// <returns></returns>
        public IList<SelectListItem> OfflineReason_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "未出席回流訓或未通過測驗。" },
                { "2", "提供不實資料、回流訓由他人代為出席或未親自簽到退等，經查證屬實。" },
                { "3", "涉及刑事案件經判刑確定。" },
                { "4", "有其他損害本署聲譽之行為。" },
                { "5", "其他" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 產學類別 (單選)<br />
        /// 1. 產業界<br />
        /// 2. 學術界-講師<br />
        /// 3. 學術界-助理教授<br />
        /// 4. 學術界-副教授<br />
        /// 5. 學術界-教授<br />
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> IndustryAcademicType_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "產業界" },
                { "2", "學術界-講師" },
                { "3", "學術界-助理教授" },
                { "4", "學術界-副教授" },
                { "5", "學術界-教授" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>報告管理的 年度 下拉清單</summary>
        /// <returns></returns>
        public IList<SelectListItem> ReportRecordYears_List()
        {
            MyKeyMapDAO dao = new MyKeyMapDAO();
            // 這邊抓回來，應該只會有一筆，資料庫裡最舊的那個年度
            IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.ReportRecordYears_List);
            IList<KeyMapModel> listNew = new List<KeyMapModel>();
            // 取年度最小值與最大值
            int Low = DateTime.Now.Year;       // 預設值
            int High = DateTime.Now.Year + 1;  // 預設值
            if (list.ToCount() > 0)
            {
                Low = list.FirstOrDefault().CODE.TOInt32();
                High = list.LastOrDefault().CODE.TOInt32();
                if (Low > DateTime.Now.Year) Low = DateTime.Now.Year;
                if (High < DateTime.Now.Year + 1) High = DateTime.Now.Year + 1;
            }
            // 程式跑迴圈，產生到 DateTime.Now 明年為止的清單
            for (int i = Low; i <= High; i++)
            {
                listNew.Insert(0, new KeyMapModel()
                {
                    CODE = (i).ToString(),
                    TEXT = (i - 1911).ToString()
                });
            }
            return MyCommonUtil.ConvertSelItems(listNew);
        }

        /// <summary>
        /// 報告管理的 月份 下拉清單
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> ReportRecordMonth_List()
        {
            IList<KeyMapModel> listNew = new List<KeyMapModel>();
            // 取最小值與最大值
            int Low = 1; // 預設值
            int High = 12; // 預設值
            // 程式跑迴圈，產生清單
            for (int i = Low; i <= High; i++)
            {
                listNew.Insert(0, new KeyMapModel() { CODE = $"{i}", TEXT = $"{i}" });
            }
            return MyCommonUtil.ConvertSelItems(listNew);
        }

        /// <summary>
        /// 報告管理的 審核狀態 清單<br />Field: AuditStatus
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> ReportRecordAuditStatus_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "",  "未送出" },
                { " ", "未送出" },
                { "Y", "審核通過" },
                { "N", "審核不通過" },
                { "R", "退件修正" },
                { "S", "送件審核中" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 報告管理的 審核狀態 清單<br />Field: AuditStatus<br />
        /// 注意：供分署審核用，非全部狀態清單。
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> ReportRecordAuditStatus_forAudit_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "Y", "審核通過" },
                { "N", "審核不通過" },
                { "R", "退件修正" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 報告管理的 報告類別 清單<br />Field: ReportType
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> ReportRecordReportType_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "教學自我審核報告" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 職能類別 清單
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> JobAbilityCode_List()
        {
            //var dictionary = new Dictionary<string, string> {
            //    { "DC", "DC 動機職能" },
            //    { "BC", "BC 行為職能" },
            //    { "KC", "KC 知識職能" }
            //};
            MyKeyMapDAO dao = new MyKeyMapDAO();
            IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.JobAbilityCode_List);
            return MyCommonUtil.ConvertSelItems(list);
        }

        /// <summary>
        /// AM/C105M 登入記錄查詢頁 類型
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> C105MFilterType_Tab1_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "LOGIN", "登入" },
                { "LOGOUT", "登出" },
                { "LOGINFAIL", "登入失敗" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// AM/C105M 檔案上傳查詢頁 類型
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> C105MFilterType_Tab2_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "UPLOAD SUCCESS", "上傳" },
                { "DOWNLOAD SUCCESS", "下載" },
                { "UPLOAD FAIL", "上傳失敗" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// AM/C105M 使用歷程查詢頁 操作項目
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> C105MFilterType_Tab3_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "CREATE", "新增" },
                { "UPDATE", "修改" },
                { "DELETE", "刪除" },
                { "PRINT",  "匯出" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>AM/C105M 使用歷程查詢頁 功能項目</summary>
        /// <returns></returns>
        public IList<SelectListItem> C105MFilterFunc_Tab3_List()
        {
            // 分類各程式
            string[] func1 = { "A3/C101M", "A3/C102M", "AM/C101M", "AM/C102M", "AM/C103M", "AM/C104M", "AM/C105M", "AM/C106M" };
            string[] func2 = { "A2/C201M", "A2/C501M", "A3/C201M", "A3/C202M", "A3/C203M", "AM/C201M", "AM/C401M", "A3/C501M", "AM/C902M", "A3/C204M", "A2/C502M", "A2/C202M", "A3/C205M", "AM/C202M", "A3/C502M", "AM/C402M", "A3/C503M", "AM/C403M" };
            string[] func3 = { "A2/C301M", "A2/C302M", "A3/C401M", "A3/C402M", "AM/C301M", "AM/C302M" };
            string funcStr1 = string.Join(",", func1);
            string funcStr2 = string.Join(",", func2);
            string funcStr3 = string.Join(",", func3);
            var dictionary = new Dictionary<string, string> {
                { funcStr1, "系統管理" },  // func1
                { funcStr2, "師資管理" },  // func2
                { funcStr3, "會議管理" }   // func3
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>服務單位屬性</summary>
        /// <returns></returns>
        public IList<SelectListItem> ServiceUnitProperties_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "工會" },
                { "2", "協會" },
                { "3", "管顧" }
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>服務單位屬性</summary>
        public IList<SelectListItem> ServiceUnitPropertiesName_List
        {
            get
            {
                return this.ServiceUnitProperties_List();
            }
        }

        /// <summary>文字-服務單位屬性(工會、協會、管顧)</summary>
        /// <param name="rowValue"></param>
        /// <returns></returns>
        public string ServiceUnitPropertiesName(string rowValue)
        {
            var s_TMP1 = "";
            if (string.IsNullOrEmpty(rowValue)) { return s_TMP1; }
            if (rowValue.IndexOf(",") > -1)
            {
                foreach (var xUnitlst in rowValue.Split(','))
                {
                    foreach (var sclmVal in ServiceUnitPropertiesName_List)
                    {
                        if (sclmVal.Value == xUnitlst)
                        {
                            s_TMP1 += string.Concat((s_TMP1 != "" ? "," : ""), sclmVal.Text);
                        }
                    }
                }
            }
            else
            {
                foreach (var sclmVal in ServiceUnitPropertiesName_List)
                {
                    if (sclmVal.Value == rowValue)
                    {
                        s_TMP1 += string.Concat((s_TMP1 != "" ? "," : ""), sclmVal.Text);
                        break;
                    }
                }
            }
            return s_TMP1;
        }


        /// <summary>
        /// 時間下拉 (小時)
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> TimeHour_List()
        {
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i <= 23; i++) dictionary.Add(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0'));
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 時間下拉 (分鐘)
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> TimeMin_List()
        {
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i <= 59; i++) dictionary.Add(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0'));
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>
        /// 授課對象 (下拉選單) 選項為：1：大專校院學生、2：企業員工
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> OBJECT_TYPE_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "大專校院學生" },
                { "2", "企業員工" },
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        /// <summary>個人授課資料(介接) - 年度 下拉清單</summary>
        /// <returns></returns>
        public IList<SelectListItem> TransData_Years_List()
        {
            IList<KeyMapModel> list = new List<KeyMapModel>();
            int YearA = 112;
            int YearB = DateTime.Now.Year - 1911;
            for (int i = YearA; i <= YearB; i++)
            {
                list.Add(new KeyMapModel() { CODE = (i + 1911).ToString(), TEXT = i.ToString() });
            }
            list = list.OrderByDescending(x => x.CODE).ToList();
            return MyCommonUtil.ConvertSelItems(list);
        }

        /// <summary>新增【年度定版數據下載】功能-從113年開始,不含當年</summary>
        /// <returns></returns>
        public IList<SelectListItem> TransData_Years_List_113()
        {
            IList<KeyMapModel> list = new List<KeyMapModel>();
            int YearA = 113;
            int YearB = DateTime.Now.Year - 1 - 1911 ;
            for (int i = YearA; i <= YearB; i++)
            {
                list.Add(new KeyMapModel() { CODE = (i + 1911).ToString(), TEXT = i.ToString() });
            }
            list = list.OrderByDescending(x => x.CODE).ToList();
            return MyCommonUtil.ConvertSelItems(list);
        }

        /// <summary>
        /// 個人授課資料(介接) - 計畫別 下拉清單 1.補助大專校院辦理就業學程計畫 2.小型企業人力提升計畫
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> TransData_PlanType_List()
        {
            var dictionary = new Dictionary<string, string> {
                { "1", "補助大專校院辦理就業學程計畫" },
                { "2", "小型企業人力提升計畫" },
                { "3", "企業人力資源提升計畫" },
            };
            return MyCommonUtil.ConvertSelItems(dictionary);
        }

        public IList<SelectListItem> DISTIDCode_list(string distid)
        {
            MyKeyMapDAO dao = new MyKeyMapDAO();
            Hashtable parms = new Hashtable();
            parms["DISTID"] = distid;
            IList<KeyMapModel> list = dao.GetCodeMapList(Commons.StaticCodeMap.CodeMap.DISTIDCode_List, parms);
            return MyCommonUtil.ConvertSelItems(list);
        }
    }
}