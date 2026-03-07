using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using System.Net.Mail;
using WKEFSERVICE.Areas.A3.Models;
using Omu.ValueInjecter;
using System.Web;
using System.IO;
using WKEFSERVICE.Commons;
using Turbo.DataLayer;

namespace WKEFSERVICE.DataLayers
{
    public class A3DAO : BaseDAO
    {
        /// <summary>
        /// A3/C101M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C101MGridModel> QueryC101M(C101MFormModel parms)
        {
            return base.QueryForList<C101MGridModel>("A3.queryC101M", parms);
        }

        public C101MDetailModel DetailC101M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C101MDetailModel>("A3.detailC101M", parms);
        }

        public IList<C101MAttachedsModel> DetailAttachedsC101M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForListAll<C101MAttachedsModel>("A3.detailAttachedsC101M", parms);
        }

        public void InsertOrUpdateC101M(C101MDetailModel model)
        {
            base.BeginTransaction();
            try
            {
                if (model.IsNew)
                {
                    // 主檔 MsgPost
                    MsgPost Ins = new MsgPost();
                    Ins.InjectFrom(model);
                    int MstKey = base.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (MstKey >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C101M", Ins, null, "[MsgPost]");
                    // 明細 MsgPostAttached
                    if (model.Attacheds.ToCount() > 0)
                    {
                        foreach (var row in model.Attacheds)
                        {
                            MsgPostAttached where = new MsgPostAttached() { Seq = row.Seq };
                            MsgPostAttached Upd = new MsgPostAttached();
                            Upd.InjectFrom(row);
                            Upd.MsgPostSeq = MstKey;
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C101M", base.GetRow(where), where, "[MsgPostAttached]");
                            int UpdInt = base.Update(Upd, where);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C101M", Upd, where, "[MsgPostAttached]");
                        }
                    }
                }
                else
                {
                    // 主檔 MsgPost
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C101M", base.QueryForListAll<Hashtable>("AM.LogGet__MsgPost", model), model, "A3.updateC101M_MsgPost");
                    int UpdInt = base.Update("A3.updateC101M_MsgPost", model);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C101M", model, model, "A3.updateC101M_MsgPost");
                    // 明細 MsgPostAttached
                    // => UPDATE 不需要，已經即時異動了
                }
                // 明細 MsgPostAttached 移除 isActive = False
                if (model.Attacheds.ToCount() > 0)
                {
                    foreach (var row in model.Attacheds.Where(x => x.isActive == false).ToList())
                    {
                        MsgPostAttached where2 = new MsgPostAttached() { Seq = row.Seq };
                        FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C101M", base.GetRow(where2), where2, "[MsgPostAttached]");
                        int DelInt = base.Delete<MsgPostAttached>(where2);
                        FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (DelInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C101M", null, where2, "[MsgPostAttached]");
                        // 移除實體檔案
                        string UploadPath = HttpContext.Current.Server.MapPath(new AMDAO().UploadPathC104M);
                        string FullFileName = UploadPath + row.FileNameNew;
                        if (System.IO.File.Exists(FullFileName))
                        {
                            System.IO.File.Delete(FullFileName);
                            FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "A3/C101M", row.FileNameOrg.TONotNullString(), row.FileNameNew.TONotNullString());
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                base.RollBackTransaction();
                throw new Exception("Insert or Update C101M failed: " + ex.Message, ex);
            }
        }

        public void DeleteC101M(string Seq, string UserNo)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return; }  /*throw new ArgumentNullException("不可為空。");*/

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq, ["UpdatedAccount"] = UserNo };
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C101M", base.QueryForListAll<Hashtable>("AM.LogGet__MsgPost", parms), parms, "A3.deleteC101M");
            var i = base.Delete("A3.deleteC101M", parms);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (i == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C101M", null, parms, "A3.deleteC101M");
        }

        /// <summary>
        /// A3/C102M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C102MGridModel> QueryC102M(C102MFormModel parms)
        {
            return base.QueryForListAll<C102MGridModel>("A3.queryC102M", parms);
        }

        public C102MDetailModel DetailC102M(string Year, string Unit)
        {
            C102MDetailModel Result = new C102MDetailModel
            {
                Year = Year,
                UnitCode = Unit
            };
            return base.QueryForObject<C102MDetailModel>("A3.detailC102M", Result);
        }

        public void SaveC102M(C102MViewModel model)
        {
            if (model.Detail.IsNew)
            {
                Report Ins = new Report();
                Ins.InjectFrom(model.Detail);
                int rtn = base.Insert("A3.saveC102M", Ins);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "A3/C102M", Ins, null, "[Report]");
            }
            else
            {
                Report upd = new Report();
                upd.InjectFrom(model.Detail);
                Report where = new Report
                {
                    Year = model.Detail.Year,
                    UnitCode = model.Detail.UnitCode
                };
                FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C102M", base.QueryForObject<Hashtable>("AM.LogGet__Report", where), where, "[Report]");
                int rtn = base.Update(upd, where);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C102M", upd, where, "[Report]");
            }
        }

        /// <summary>
        /// A3/C201M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C201MGridModel> QueryC201M(C201MFormModel form)
        {
            return base.QueryForList<C201MGridModel>("A3.queryC201M", form);
        }

        public C201MDetailModel DetailC201M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C201MDetailModel>("A3.detailC201M", parms);
        }

        public void SaveC201M(C201MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            // 準備儲存物件
            Teacher where = new Teacher
            {
                Seq = model.Detail.Seq
            };
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
                update.TeacherName = model.Detail.TeacherName;
                update.TeacherEName = model.Detail.TeacherEName;
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
                    FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A3/C201M", file.FileName, tmpFileName);
                    // 變更 model 欄位 - 檔名
                    update.SelfPicPath = tmpFileName;
                }
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeacherName);
                cfmModel.Add((Teacher x) => x.TeacherEName);
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
                // 此為分署登入 不可編輯項目： update.EduLevelHighest = model.Detail.EduLevelHighest;
                #endregion
            }
            else if (Idx == "3")
            {
                #region Area 3
                // 此為分署登入 不可編輯項目： update.EduSchool1 = model.Detail.EduSchool1;
                // 此為分署登入 不可編輯項目： update.EduSchool2 = model.Detail.EduSchool2;
                // 此為分署登入 不可編輯項目： update.EduSchool3 = model.Detail.EduSchool3;
                // 此為分署登入 不可編輯項目： update.EduDept1 = model.Detail.EduDept1;
                // 此為分署登入 不可編輯項目： update.EduDept2 = model.Detail.EduDept2;
                // 此為分署登入 不可編輯項目： update.EduDept3 = model.Detail.EduDept3;
                // 此為分署登入 不可編輯項目： update.EduLevel1 = model.Detail.EduLevel1;
                // 此為分署登入 不可編輯項目： update.EduLevel2 = model.Detail.EduLevel2;
                // 此為分署登入 不可編輯項目： update.EduLevel3 = model.Detail.EduLevel3;
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
                // 此為分署登入 不可編輯項目： update.UnitCode = model.Detail.UnitCode;
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
                // 此為分署登入 不可編輯項目： update.TeachJobAbilityDC = model.Detail.TeachJobAbilityDC;
                // 此為分署登入 不可編輯項目： update.TeachJobAbilityBC = model.Detail.TeachJobAbilityBC;
                // 此為分署登入 不可編輯項目： update.TeachJobAbilityKC = model.Detail.TeachJobAbilityKC;
                #endregion
            }
            else if (Idx == "8")
            {
                #region Area 8
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
                update.JoinYear = model.Detail.JoinYear;
                // 此為分署登入 不可編輯項目： update.ServiceUnitProperties = model.Detail.ServiceUnitProperties;
                // 此為分署登入 不可編輯項目： update.Online = model.Detail.Online;
                // 此為分署登入 不可編輯項目： update.OfflineDate = model.Detail.OfflineDate;
                // 此為分署登入 不可編輯項目： update.OfflineReason = model.Detail.OfflineReason;
                // 此為分署登入 不可編輯項目： update.OfflineReasonRemark = model.Detail.OfflineReasonRemark;
                update.HomePageCarousel = model.Detail.HomePageCarousel;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.JoinYear);
                //cfmModel.Add((Teacher x) => x.ServiceUnitProperties);
                //cfmModel.Add((Teacher x) => x.Online);
                //cfmModel.Add((Teacher x) => x.OfflineDate);
                //cfmModel.Add((Teacher x) => x.OfflineReason);
                //cfmModel.Add((Teacher x) => x.OfflineReasonRemark);
                cfmModel.Add((Teacher x) => x.HomePageCarousel);
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

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C201M", base.GetRow(where), where, "[Teacher]");
            int rtn = base.Update(update, where, cfmModel);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C201M", update, where, "[Teacher]");
        }

        public IList<JsonIndustry> GetJsonIndustry()
        {
            return base.QueryForListAll<JsonIndustry>("A3.getJsonIndustry", null);
        }

        /// <summary>A3/C202M Query</summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C202MGridModel> QueryC202M(C202MFormModel form)
        {
            return base.QueryForList<C202MGridModel>("A3.queryC202M", form);
        }

        public C202MDetailModel DetailC202M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C202MDetailModel>("A3.detailC202M", parms);
        }

        /// <summary>
        /// A3/C204M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C204MGridModel> QueryC204M(C204MFormModel form)
        {
            return base.QueryForList<C204MGridModel>("A3.queryC204M", form);
        }

        public C204MDetailModel DetailC204M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C204MDetailModel>("A3.detailC204M", parms);
        }

        /// <summary>A3/C205M -分署首頁>師資管理>教師授課資料：匯出師資個人授課總表 </summary>
        /// <param name="parms"></param>
        /// <param name="PlanDataType"></param>
        /// <param name="GetAll"></param>
        /// <returns></returns>
        public IList<C205MGridModel> QueryC205M(C205MFormModel parms, string PlanDataType, bool GetAll)
        {
            if (PlanDataType.TONotNullString() == "") PlanDataType = "0";
            string statementId = string.Concat("A3.queryC205M_", PlanDataType);
            if (GetAll)
                return base.QueryForListAll<C205MGridModel>(statementId, parms);
            else
                return base.QueryForList<C205MGridModel>(statementId, parms);
        }

        /// <summary>
        /// A3/C401M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C401MGridModel> QueryC401M(C401MFormModel parms)
        {
            return base.QueryForList<C401MGridModel>("A3.queryC401M", parms);
        }

        public C401MDetailModel DetailC401M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C401MDetailModel>("A3.detailC401M", parms);
        }

        public IList<C401MAttachedsModel> DetailAttachedsC401M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForListAll<C401MAttachedsModel>("A3.detailAttachedsC401M", parms);
        }

        public C401MDetailSignUpModel DetailSignUpC401M_Mst(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C401MDetailSignUpModel>("A3.detailSignUpC401M_Mst", parms);
        }

        public IList<C401MDetailSignUpListModel> DetailSignUpC401M_Det(int iSeq, int iMINROW, int iMAXROW)
        {
            Hashtable parms = new Hashtable { ["Seq"] = iSeq, ["MINROW"] = iMINROW, ["MAXROW"] = iMAXROW };
            return base.QueryForListAll<C401MDetailSignUpListModel>("A3.detailSignUpC401M_Det", parms);
        }

        private void DoMstUploadC401M(C401MDetailModel model)
        {
            var file = model.PicName_FILE;
            if (file != null && file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
            {
                // 上傳檔案
                if (!Directory.Exists(model._tmpPicPath)) Directory.CreateDirectory(model._tmpPicPath);  // 不存在則建立
                Random tmpNum = new Random();
                string tmpFileName =
                    DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                    tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                    Path.GetExtension(file.FileName);                   // 副檔名
                file.SaveAs(model._tmpPicPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "A3/C401M", file.FileName, tmpFileName);
                // 變更 model 欄位 - 檔名
                model.PicName = tmpFileName;
            }
        }

        public void InsertOrUpdateC401M(C401MDetailModel model)
        {
            base.BeginTransaction();
            try
            {
                if (model.IsNew)
                {
                    // 主檔 Meeting 的 圖檔上傳
                    DoMstUploadC401M(model);
                    // 主檔 Meeting
                    Meeting Ins = new Meeting();
                    Ins.InjectFrom(model);
                    int MstKey = base.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (MstKey >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", Ins, null, "[Meeting]");
                    // 明細 MeetingAttached
                    if (model.Attacheds.ToCount() > 0)
                    {
                        foreach (var row in model.Attacheds)
                        {
                            MeetingAttached where = new MeetingAttached() { Seq = row.Seq };
                            MeetingAttached Upd = new MeetingAttached();
                            Upd.InjectFrom(row);
                            Upd.MeetingSeq = MstKey;
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C401M", base.GetRow(where), where, "[MeetingAttached]");
                            int UpdInt = base.Update(Upd, where);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", Upd, where, "[MeetingAttached]");
                        }
                    }
                }
                else
                {
                    // 主檔 Meeting 的 圖檔上傳
                    DoMstUploadC401M(model);
                    // 主檔 Meeting
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__Meeting_All", model), model, "A3.updateC401M_Meeting");
                    int UpdInt = base.Update("A3.updateC401M_Meeting", model);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", model, model, "A3.updateC401M_Meeting");
                    // 明細 MeetingAttached
                    // => UPDATE 不需要，已經即時異動了
                }
                // 明細 MeetingAttached 移除 isActive = False
                if (model.Attacheds.ToCount() > 0)
                {
                    foreach (var row in model.Attacheds.Where(x => x.isActive == false).ToList())
                    {
                        MeetingAttached where2 = new MeetingAttached() { Seq = row.Seq };
                        FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.GetRow(where2), where2, "[MeetingAttached]");
                        int DelInt = base.Delete<MeetingAttached>(where2);
                        FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (DelInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", null, where2, "[MeetingAttached]");
                        // 移除實體檔案
                        string UploadPath = HttpContext.Current.Server.MapPath(new AMDAO().UploadPathC301M);
                        string FullFileName = UploadPath + row.FileNameNew;
                        if (System.IO.File.Exists(FullFileName))
                        {
                            System.IO.File.Delete(FullFileName);
                            FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", row.FileNameOrg.TONotNullString(), row.FileNameNew.TONotNullString());
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                base.RollBackTransaction();
                throw new Exception("Insert or Update C401M failed: " + ex.Message, ex);
            }
        }

        public void DeleteC401M(string Seq, string UserNo)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            var i = 0;

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend_All", parms), parms, "A3.deleteC401M_MeetingAttend_All");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingSignUp_All", parms), parms, "A3.deleteC401M_MeetingSignUp_All");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttached_All", parms), parms, "A3.deleteC401M_MeetingAttached_All");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__Meeting_All", parms), parms, "A3.deleteC401M_Meeting_All");

            i = base.Delete("A3.deleteC401M_MeetingAttend_All", parms);
            i = base.Delete("A3.deleteC401M_MeetingSignUp_All", parms);
            i = base.Delete("A3.deleteC401M_MeetingAttached_All", parms);
            i = base.Delete("A3.deleteC401M_Meeting_All", parms);

            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", null, parms, "A3.deleteC401M_MeetingAttend_All");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", null, parms, "A3.deleteC401M_MeetingSignUp_All");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", null, parms, "A3.deleteC401M_MeetingAttached_All");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", null, parms, "A3.deleteC401M_Meeting_All");
        }

        public IList<C401MTeacherPopupListModel> GetTeacherListC401M(string TeacherSearch, string UnitCode)
        {
            Hashtable parms = new Hashtable { ["TeacherName"] = TeacherSearch, ["UnitCode"] = UnitCode };
            var model = base.QueryForListAll<C401MTeacherPopupListModel>("A3.getTeacherListC401M", parms);
            return model;
            //base.PageSize = 5;
            //base.m_default_pagesize = 5;
            //return base.PagingList("A3.getTeacherListC401M", model);
        }

        public void InsertOrUpdateC401M_SignUp(C401MDetailSignUpModel model)
        {
            base.BeginTransaction();
            try
            {
                foreach (var item in model.SignUpList)
                {
                    if (item.isActive)
                    {
                        if (item.Seq.TONotNullString() == "")
                        {
                            // Insert - MeetingSignUp
                            MeetingSignUp ins = new MeetingSignUp();
                            ins.InjectFrom(item);
                            ins.MeetingSeq = model.Seq;
                            int res = base.Insert(ins);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (res >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", ins, null, "[MeetingSignUp]");
                            // Insert - MeetingAttend
                            MeetingAttend ins2 = new MeetingAttend();
                            ins2.InjectFrom(ins);
                            ins2.Attend = "N";
                            ins2.TestPassed = "N";
                            int res2 = base.Insert(ins2);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (res2 >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", ins2, null, "[MeetingAttend]");
                        }
                    }
                    else
                    {
                        if (item.Seq.TONotNullString() != "")
                        {
                            // Delete - MeetingSignUp
                            Hashtable del = new Hashtable { ["Seq"] = item.Seq };
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingSignUp2", del), del, "A3.deleteC401M_MeetingSignUp");
                            int res = base.Delete("A3.deleteC401M_MeetingSignUp", del);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (res == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", null, del, "A3.deleteC401M_MeetingSignUp");
                            // Delete - MeetingAttend
                            Hashtable del2 = new Hashtable { ["MeetingSeq"] = item.MeetingSeq, ["TeacherAccount"] = item.TeacherAccount };
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "A3/C401M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend", del2), del2, "A3.deleteC401M_MeetingAttend");
                            int res2 = base.Delete("A3.deleteC401M_MeetingAttend", del2);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (res2 == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C401M", null, del2, "A3.deleteC401M_MeetingAttend");
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                base.RollBackTransaction();
                throw new Exception("Insert or Update C401M SignUpList failed: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// A3/C402M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C402MGridModel> QueryC402M(C402MFormModel parms)
        {
            return base.QueryForList<C402MGridModel>("A3.queryC402M", parms);
        }

        public C402MDetailModel DetailC402M_Mst(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C402MDetailModel>("A3.detailC402M_Mst", parms);
        }

        public IList<C402MDetailRowsModel> DetailC402M_Det(int iSeq, int iMINROW, int iMAXROW)
        {
            Hashtable parms = new Hashtable { ["Seq"] = iSeq, ["MINROW"] = iMINROW, ["MAXROW"] = iMAXROW };
            return base.QueryForListAll<C402MDetailRowsModel>("A3.detailC402M_Det", parms);
        }

        public void SaveC402M(C402MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            if (model.Detail.DetailRows != null)
            {
                foreach (var item in model.Detail.DetailRows)
                {
                    item.UpdatedAccount = sm.UserInfo.UserNo;
                    item.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    Hashtable tmpHash = new Hashtable { ["Seq"] = item.Seq };
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "A3/C402M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend2", tmpHash), tmpHash, "A3.updateC402M_MeetingAttend");
                    int rtn = base.Update("A3.updateC402M_MeetingAttend", item);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "A3/C402M", item, tmpHash, "A3.updateC402M_MeetingAttend");
                }
            }
        }

        /// <summary>
        /// A3/C501M Query for Export
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<C501MGridModel> QueryC501M(C501MFormModel model)
        {
            Hashtable parms = new Hashtable { ["Unit"] = model.Unit, ["Online"] = model.Online };
            return base.QueryForListAll<C501MGridModel>("A3.queryC501M", parms);
        }

        /// <summary>
        /// A3/C601M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C601MGridModel> QueryC601M(C601MFormModel parms)
        {
            return base.QueryForList<C601MGridModel>("A3.queryC601M", parms);
        }

        public IList<C601MGridAttachedModel> QueryC601MAttached(C601MGridModel rowData)
        {
            return base.QueryForListAll<C601MGridAttachedModel>("A3.queryC601MAttached", rowData);
        }

        public IList<C503MGridModel1> QueryC503M1(string Year)
        {
            Hashtable parms = new Hashtable { ["ACTUALYEAR"] = Year };
            return base.QueryForListAll<C503MGridModel1>("A3.QueryC503M1", parms);
        }
        public IList<C503MGridModel2> QueryC503M2(string Year)
        {
            Hashtable parms = new Hashtable { ["Year"] = Year };
            return base.QueryForListAll<C503MGridModel2>("A3.QueryC503M2", parms);
        }
        public IList<C503MGridModel3> QueryC503M3(string Year)
        {
            Hashtable parms = new Hashtable { ["Year"] = Year };
            return base.QueryForListAll<C503MGridModel3>("A3.QueryC503M3", parms);
        }

    }
}
