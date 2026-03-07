using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using System.Net.Mail;
using WKEFSERVICE.Areas.A2.Models;
using Omu.ValueInjecter;
using System.Web;
using System.IO;
using WKEFSERVICE.Commons;
using Turbo.DataLayer;

namespace WKEFSERVICE.DataLayers
{
    public class A2DAO : BaseDAO
    {
        public C201MDetailModel DetailC201M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C201MDetailModel>("A2.detailC201M", parms);
        }

        public void SaveC201M(C201MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            // 準備儲存物件
            Teacher where = new Teacher { Seq = model.Detail.Seq };
            Teacher update = new Teacher
            {
                UpdatedAccount = sm.UserInfo.UserNo,
                UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
            };
            // 清空需設為 null 欄位
            ClearFieldMap cfmModel = new ClearFieldMap();  //cfmModel.Add((Teacher x) => x.);
            // 依區塊儲存
            string Idx = model.Detail.EditAreaIdx.TONotNullString();
            if (Idx == "1")
            {
                #region Area 1
                // 此為教師登入 不可編輯項目： update.TeacherName = model.Detail.TeacherName;
                // 此為教師登入 不可編輯項目： update.TeacherEName = model.Detail.TeacherEName;
                update.Sex = model.Detail.Sex;
                update.Tel = model.Detail.Tel;
                update.Phone = model.Detail.Phone;
                update.Birthday = model.Detail.Birthday;
                update.Email = model.Detail.Email;
                update.EmailWork = model.Detail.EmailWork;
                update.PostalCode1 = model.Detail.PostalCode1;
                update.PostalCode2 = model.Detail.PostalCode2;
                update.Address = model.Detail.Address;
                // 前台顯示
                update.IsShowTel = model.Detail.IsShowTel;
                update.IsShowPhone = model.Detail.IsShowPhone;
                update.IsShowBirthday = model.Detail.IsShowBirthday;
                update.IsShowEmail = model.Detail.IsShowEmail;
                update.IsShowEmailWork = model.Detail.IsShowEmailWork;
                update.IsShowAddress = model.Detail.IsShowAddress;
                // 圖檔上傳
                var file = model.Detail.Pic_FILE;
                if (file != null && file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    if (!Directory.Exists(model.Detail._tmpPicPath)) Directory.CreateDirectory(model.Detail._tmpPicPath);  // 不存在則建立
                    Random tmpNum = new Random();
                    string tmpFileName =
                        DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                        tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                        Path.GetExtension(file.FileName);                   // 副檔名
                    file.SaveAs(model.Detail._tmpPicPath + "/" + tmpFileName);
                    FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A2/C201M", file.FileName, tmpFileName);
                    // 變更 model 欄位 - 檔名
                    update.SelfPicPath = tmpFileName;
                }
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.Sex);
                cfmModel.Add((Teacher x) => x.Tel);
                cfmModel.Add((Teacher x) => x.Phone);
                cfmModel.Add((Teacher x) => x.Birthday);
                cfmModel.Add((Teacher x) => x.Email);
                cfmModel.Add((Teacher x) => x.EmailWork);
                cfmModel.Add((Teacher x) => x.PostalCode1);
                cfmModel.Add((Teacher x) => x.PostalCode2);
                cfmModel.Add((Teacher x) => x.Address);
                cfmModel.Add((Teacher x) => x.IsShowTel);
                cfmModel.Add((Teacher x) => x.IsShowPhone);
                cfmModel.Add((Teacher x) => x.IsShowBirthday);
                cfmModel.Add((Teacher x) => x.IsShowEmail);
                cfmModel.Add((Teacher x) => x.IsShowEmailWork);
                cfmModel.Add((Teacher x) => x.IsShowAddress);
                //cfmModel.Add((Teacher x) => x.SelfPicPath);
                #endregion
            }
            else if (Idx == "2")
            {
                #region Area 2
                // 此為教師登入 不可編輯項目： update.EduLevelHighest = model.Detail.EduLevelHighest;
                #endregion
            }
            else if (Idx == "3")
            {
                #region Area 3
                // 此為教師登入 不可編輯項目： update.EduSchool1 = model.Detail.EduSchool1;
                // 此為教師登入 不可編輯項目： update.EduSchool2 = model.Detail.EduSchool2;
                // 此為教師登入 不可編輯項目： update.EduSchool3 = model.Detail.EduSchool3;
                // 此為教師登入 不可編輯項目： update.EduDept1 = model.Detail.EduDept1;
                // 此為教師登入 不可編輯項目： update.EduDept2 = model.Detail.EduDept2;
                // 此為教師登入 不可編輯項目： update.EduDept3 = model.Detail.EduDept3;
                // 此為教師登入 不可編輯項目： update.EduLevel1 = model.Detail.EduLevel1;
                // 此為教師登入 不可編輯項目： update.EduLevel2 = model.Detail.EduLevel2;
                // 此為教師登入 不可編輯項目： update.EduLevel3 = model.Detail.EduLevel3;
                #endregion
            }
            else if (Idx == "4")
            {
                #region Area 4
                update.ServiceUnit1 = model.Detail.ServiceUnit1;
                update.JobTitle1 = model.Detail.JobTitle1;
                update.ServiceUnit2 = model.Detail.ServiceUnit2;
                update.JobTitle2 = model.Detail.JobTitle2;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.ServiceUnit1);
                cfmModel.Add((Teacher x) => x.JobTitle1);
                cfmModel.Add((Teacher x) => x.ServiceUnit2);
                cfmModel.Add((Teacher x) => x.JobTitle2);
                #endregion
            }
            else if (Idx == "5")
            {
                #region Area 5
                // 此為教師登入 不可編輯項目： update.UnitCode = model.Detail.UnitCode;
                #endregion
            }
            else if (Idx == "6")
            {
                #region Area 6
                update.ExpertiseDesc = model.Detail.ExpertiseDesc;
                update.ExpertiseCode = model.Detail.ExpertiseCode;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.ExpertiseDesc);
                cfmModel.Add((Teacher x) => x.ExpertiseCode);
                #endregion
            }
            else if (Idx == "7")
            {
                #region Area 7
                // 此為教師登入 不可編輯項目： update.TeachJobAbilityDC = model.Detail.TeachJobAbilityDC;
                // 此為教師登入 不可編輯項目： update.TeachJobAbilityBC = model.Detail.TeachJobAbilityBC;
                // 此為教師登入 不可編輯項目： update.TeachJobAbilityKC = model.Detail.TeachJobAbilityKC;
                #endregion
            }
            else if (Idx == "8")
            {
                #region Area 8
                //int i_TeachArea_MaxLength = 2000;
                //string s_TeachArea = model.Detail.TeachArea;
                //if (!string.IsNullOrEmpty(s_TeachArea) && s_TeachArea.Length > i_TeachArea_MaxLength)
                //{
                //    if (s_TeachArea.IndexOf(",") == -1) {
                //        model.Detail.TeachArea = "";
                //    }
                //    else {
                //        string[] TeachAreaSp = s_TeachArea.Split(',');
                //        foreach (string sTmp in TeachAreaSp)
                //        {
                //            if (sTmp.Length > 10) { }
                //        }
                //    }
                //    //model.Detail.TeachArea = s_TeachArea.Substring(0, i_TeachArea_MaxLength);
                //}
                update.TeachArea = model.Detail.TeachArea;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeachArea);
                #endregion
            }
            else if (Idx == "9")
            {
                #region Area 9
                update.TeachIndustryDetCode = model.Detail.TeachIndustryDetCode;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeachIndustryDetCode);
                #endregion
            }
            else if (Idx == "10")
            {
                #region Area 10
                update.WorkHistory = model.Detail.WorkHistory;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.WorkHistory);
                #endregion
            }
            else if (Idx == "11")
            {
                #region Area 11
                update.ProLicense = model.Detail.ProLicense;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.ProLicense);
                #endregion
            }
            else if (Idx == "12")
            {
                #region Area 12
                update.SelfIntroduction = model.Detail.SelfIntroduction;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.SelfIntroduction);
                #endregion
            }
            else if (Idx == "13")
            {
                #region Area 13
                // 此為教師登入 不可編輯項目： update.JoinYear = model.Detail.JoinYear;
                // 此為教師登入 不可編輯項目： update.Online = model.Detail.Online;
                // 此為教師登入 不可編輯項目： update.OfflineDate = model.Detail.OfflineDate;
                // 此為教師登入 不可編輯項目： update.OfflineReason = model.Detail.OfflineReason;
                // 此為教師登入 不可編輯項目： update.OfflineReasonRemark = model.Detail.OfflineReasonRemark;
                // 此為教師登入 不可編輯項目： update.HomePageCarousel = model.Detail.HomePageCarousel;
                #endregion
            }
            else if (Idx == "14")
            {
                #region Area 14
                update.IndustryAcademicType = model.Detail.IndustryAcademicType;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.IndustryAcademicType);
                #endregion
            }

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A2/C201M", base.GetRow(where), where, "[Teacher]");
            int rtn = base.Update(update, where, cfmModel);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A2/C201M", update, where, "[Teacher]");
        }

        public IList<JsonIndustry> GetJsonIndustry()
        {
            return base.QueryForListAll<JsonIndustry>("A2.getJsonIndustry", null);
        }

        /// <summary>A2/C202M - 教師首頁>教師資料>個人授課資料：匯出師資個人授課總表</summary>
        /// <param name="parms"></param>
        /// <param name="PlanDataType"></param>
        /// <param name="GetAll"></param>
        /// <returns></returns>
        public IList<C202MGridModel> QueryC202M(C202MFormModel parms, string PlanDataType, bool GetAll)
        {
            // 1.補助大專校院辦理就業學程計畫 2.小型企業人力提升計畫
            if (PlanDataType.TONotNullString() == "") PlanDataType = "0";
            string statementId = string.Concat("A2.queryC202M_", PlanDataType);
            if (GetAll)
                return base.QueryForListAll<C202MGridModel>(statementId, parms);
            else
                return base.QueryForList<C202MGridModel>(statementId, parms);
        }

        /// <summary>2.小型企業人力提升計畫 Suggest</summary>
        /// <param name="parms"></param>
        /// <param name="PlanDataType"></param>
        /// <returns></returns>
        public IList<TransData_S_Satisfy> QueryC202M_Suggest(Hashtable parms)
        {
            //1.補助大專校院辦理就業學程計畫 2.小型企業人力提升計畫, 3:企業人力資源提升計畫(大人提)
            string statementId = "A2.queryC202M_Suggest_2";
            return base.QueryForListAll<TransData_S_Satisfy>(statementId, parms);
        }

        /// <summary>3:企業人力資源提升計畫(大人提)</summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<TransData_K_Satisfy> QueryC202M_Suggest_K(Hashtable parms)
        {
            //1.補助大專校院辦理就業學程計畫 2.小型企業人力提升計畫, 3:企業人力資源提升計畫(大人提)
            string statementId = "A2.queryC202M_Suggest_K";
            return base.QueryForListAll<TransData_K_Satisfy>(statementId, parms);
        }

        /// <summary>
        /// A2/C301M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C301MGridModel> QueryC301M(C301MFormModel parms)
        {
            return base.QueryForList<C301MGridModel>("A2.queryC301M", parms);
        }

        public C301MDetailModel DetailC301M(string Seq, string LoginAccount)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable
            {
                ["Seq"] = i_Seq,
                ["LoginAccount"] = LoginAccount
            };
            return base.QueryForObject<C301MDetailModel>("A2.detailC301M", parms);
        }

        public IList<C301MAttachedsModel> DetailAttachedsC301M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable
            {
                ["Seq"] = i_Seq
            };
            return base.QueryForListAll<C301MAttachedsModel>("A2.detailAttachedsC301M", parms);
        }

        /// <summary>
        /// A2/C302M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C302MGridModel> QueryC302M(C302MFormModel parms)
        {
            return base.QueryForList<C302MGridModel>("A2.queryC302M", parms);
        }

        public void UnSigninC302M(Hashtable parms)
        {
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A2/C302M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingSignUp", parms), parms, "A2.UnSigninC302M_1");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A2/C302M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend", parms), parms, "A2.UnSigninC302M_2");

            int rtn1 = base.Delete("A2.UnSigninC302M_1", parms);
            int rtn2 = base.Delete("A2.UnSigninC302M_2", parms);

            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (rtn1 == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A2/C302M", null, parms, "A2.UnSigninC302M_1");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (rtn2 == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A2/C302M", null, parms, "A2.UnSigninC302M_2");
        }

        public C302MDetailModel DetailC302M(string Seq, string LoginAccount)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable
            {
                ["Seq"] = i_Seq,
                ["LoginAccount"] = LoginAccount
            };
            return base.QueryForObject<C302MDetailModel>("A2.detailC302M", parms);
        }

        public IList<C302MAttachedsModel> DetailAttachedsC302M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable
            {
                ["Seq"] = i_Seq
            };
            return base.QueryForListAll<C302MAttachedsModel>("A2.detailAttachedsC302M", parms);
        }

        /// <summary>A2/C501M Query</summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C501MGridModel> QueryC501M(C501MFormModel parms)
        {
            return base.QueryForList<C501MGridModel>("A2.queryC501M", parms);
        }

        /// <summary>
        /// 取得 繳交期限 文字
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public string GetUploadDeadlineStr(string Year, string UnitID)
        {
            Hashtable parms = new Hashtable
            {
                ["Year"] = Year,
                ["UnitID"] = UnitID
            };
            Hashtable rtn = base.QueryForObject<Hashtable>("A2.GetUploadDeadlineStr", parms);
            string Result = "";
            if (rtn != null && rtn.Count > 0) Result = rtn["Result"].TONotNullString();
            return Result;
        }

        /// <summary>
        /// 取得 繳交期限 程式用
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public Hashtable GetUploadDeadline(string Year, string UnitID)
        {
            Hashtable parms = new Hashtable
            {
                ["Year"] = Year,
                ["UnitID"] = UnitID
            };
            return base.QueryForObject<Hashtable>("A2.GetUploadDeadline", parms);
        }

        /// <summary>
        /// A2/C502M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C502MGridModel> QueryC502M(C502MFormModel parms)
        {
            return base.QueryForList<C502MGridModel>("A2.queryC502M", parms);
        }

        /// <summary>
        /// A2/C601M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C601MGridModel> QueryC601M(C601MFormModel parms)
        {
            return base.QueryForList<C601MGridModel>("A2.queryC601M", parms);
        }

        public IList<C601MGridAttachedModel> QueryC601MAttached(C601MGridModel rowData)
        {
            return base.QueryForListAll<C601MGridAttachedModel>("A2.queryC601MAttached", rowData);
        }
    }
}