using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Services;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using System.Web.Mvc;
using Turbo.Commons;
using System.Linq;
using System;
using System.Web;
using WKEFSERVICE.DataLayers;

namespace WKEFSERVICE.Areas.A3.Models
{
    public class C205MViewModel
    {
        public C205MViewModel() { this.Form = new C205MFormModel(); }

        public C205MFormModel Form { get; set; }

        public C205MDetailModel Detail { get; set; }
    }

    public class C205MFormModel : PagingResultsViewModel
    {
        [Display(Name = "計畫年度")]
        [Control(Mode = Control.Hidden)]
        public string Year { get; set; }

        public IList<SelectListItem> Year_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全顯示", Value = "" };
                var list = model.TransData_Years_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        // 僅用於 SQL 辨別，無實際參與資料過濾
        [Display(Name = "計畫別")]
        [Control(Mode = Control.Hidden)]
        public string PlanTypeCode { get; set; }

        public IList<SelectListItem> PlanTypeCode_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "0" };
                var list = model.TransData_PlanType_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "授課內容所屬分署")]
        public string Units { get; set; }

        public string[] Units_SHOW
        {
            get
            {
                return (this.Units != null) ? this.Units.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.Units = string.Join(",", value.ToList());
            }
        }

        public IList<CheckBoxListItem> Units_SHOW_list
        {
            get
            {
                SessionModel sm = SessionModel.Get();
                List<CheckBoxListItem> Result = new List<CheckBoxListItem>();
                var tmpList = new MyKeyMapDAO().GetCodeMapList(Commons.StaticCodeMap.CodeMap.UNIT_List);
                foreach (var tmpStr in tmpList)
                {
                    Result.Add(new CheckBoxListItem(tmpStr.CODE, tmpStr.TEXT, false));
                }
                return Result;
            }
        }

        [Display(Name = "訓練單位")]
        [Control(Mode = Control.Hidden)]
        public string TrainingUnit { get; set; }

        [Display(Name = "職能類別")]
        [Control(Mode = Control.Hidden)]
        public string JobAbilityCode { get; set; }

        [NotDBField]
        public IList<SelectListItem> JobAbilityCode_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.JobAbilityCode_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "授課單元")]
        [Control(Mode = Control.Hidden)]
        public string CourseUnitCode { get; set; }

        public IList<SelectListItem> CourseUnitCode_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.TeachUnit_list();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "教師姓名")]
        [Control(Mode = Control.Hidden)]
        public string TeacherName { get; set; }

        [Control(Mode = Control.Hidden)]
        public string LoginUnit { get; set; }

        public IList<C205MGridModel> Grid { get; set; }
    }

    public class C205MGridModel
    {
        public long? ROWID { get; set; }
        public string Year { get; set; }
        public string PlanCode { get; set; }
        public string PlanID { get; set; }
        public string TrainID { get; set; }
        public string ClassID { get; set; }
        /// <summary>計畫別代碼 1.(大專就業學程)／2.(小人提)</summary>
        public string PlanTypeCode { get; set; }
        public string PlanTypeText { get; set; }
        public string UnitCode { get; set; }
        public string UnitCode_Text
        {
            get
            {
                if (this.UnitCode.TONotNullString() == "") return "";
                ShareCodeListModel model = new ShareCodeListModel();
                var Obj = model.UNIT_List.Where(x => x.Value == this.UnitCode).FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }
        public string UnitName { get; set; }
        public string TeacherID { get; set; }
        public string TeacherName { get; set; }
        public string ApplyUnit { get; set; }
        public string TrainingUnit { get; set; }
        public string ClassName { get; set; }
        /// <summary>2.(小人提) "授課時間起","TrainingTimeS"</summary>
        public string TrainingDateS { get; set; }
        /// <summary>2.(小人提) "授課時間迄","TrainingTimeE"</summary>
        public string TrainingDateE { get; set; }
        public string ClassDateS { get; set; }
        public string ClassDateE { get; set; }
        public string TrainingType { get; set; }
        public string JobAbilityCode { get; set; }
        public string JobAbilityName
        {
            get
            {
                if (this.JobAbilityCode.TONotNullString() == "") return "";
                ShareCodeListModel model = new ShareCodeListModel();
                var Obj = model.JobAbilityCode_List().Where(x => x.Value == this.JobAbilityCode).FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }
        public string CourseUnitCode { get; set; }
        public string CourseUnitName
        {
            get
            {
                if (this.CourseUnitCode.TONotNullString() == "") return "";
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpList = this.CourseUnitCode.TONotNullString().Split(',').ToList();
                if (!tmpList.Any()) return "";
                for (int i = 0; i <= tmpList.ToCount() - 1; i++)
                {
                    var Obj = model.TeachUnit_list().Where(x => x.Value == tmpList[i]).FirstOrDefault();
                    tmpList[i] = (Obj == null) ? "" : Obj.Text;
                }
                return string.Join(",", tmpList);
            }
        }
        public int? TeachHours { get; set; }
        public int? TeachNum { get; set; }
        public int? FillStudentNum { get; set; }
        public string QuestLink { get; set; }
        /// <summary>問卷填答人數</summary>
        public string QuesSearchLink { get; set; }
        public string UpdatedDatetime { get; set; }
        public string Satisfy { get; set; }
        public string QRCode
        {
            get
            {
                if (this.QuestLink.TONotNullString() == "") return "";
                byte[] data = Turbo.ReportTK.BarCode.BarCodeUtils.GenerateQRCode(this.QuestLink);
                //System.Drawing.Image oImage = null;
                //System.Drawing.Bitmap oBitmap = null;

                System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream(data);
                oMemoryStream.Position = 0;
                //oImage = System.Drawing.Image.FromStream(oMemoryStream);
                //oBitmap = new System.Drawing.Bitmap(oImage);
                return "data:image/png;base64," + Convert.ToBase64String(oMemoryStream.ToArray());
            }
        }
        /// <summary>問卷填答人數-QRCode</summary>
        public string QRCode2
        {
            get
            {
                if (this.QuesSearchLink.TONotNullString() == "") return "";
                byte[] data = Turbo.ReportTK.BarCode.BarCodeUtils.GenerateQRCode(this.QuesSearchLink);
                //System.Drawing.Image oImage = null;
                //System.Drawing.Bitmap oBitmap = null;
                System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream(data);
                oMemoryStream.Position = 0;
                //oImage = System.Drawing.Image.FromStream(oMemoryStream);
                //oBitmap = new System.Drawing.Bitmap(oImage);
                return "data:image/png;base64," + Convert.ToBase64String(oMemoryStream.ToArray());
            }
        }

    }

    public class C205MDetailModel
    {
        /// <summary>記錄查詢頁的 查詢條件 (以 || 分隔)</summary>
        public string FilterString { get; set; }

        /// <summary>計畫別</summary>
        public string PlanTypeCode { get; set; }

        /// <summary>計畫別</summary>
        public string PlanTypeText
        {
            get
            {
                if (this.PlanTypeCode.TONotNullString() == "1") return "補助大專校院辦理就業學程計畫";
                if (this.PlanTypeCode.TONotNullString() == "2") return "小型企業人力提升計畫";
                if (this.PlanTypeCode.TONotNullString() == "3") return "企業人力資源提升計畫";
                return $"{this.PlanTypeCode}";
            }
        }

        /// <summary>授課內容所屬分署</summary>
        public string UnitCode { get; set; }

        /// <summary>授課內容所屬分署</summary>
        public string UnitCode_Text
        {
            get
            {
                if (this.UnitCode.TONotNullString() == "") return "";
                ShareCodeListModel model = new ShareCodeListModel();
                var Obj = model.UNIT_List.Where(x => x.Value == this.UnitCode).FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }

        /// <summary>大專：撈取【申請單位(學校)】欄</summary>
        public string SchoolName { get; set; }

        /// <summary>小人提：撈取【參訓企業】欄</summary>
        public string JoinCompany { get; set; }

        /// <summary>申請單位 (大專：撈取【申請單位(學校)】欄, 小人提：撈取【參訓企業】欄)</summary>
        public string ApplyName
        {
            get
            {
                if (this.PlanTypeCode.TONotNullString() == "1") return this.SchoolName.TONotNullString();
                return $"{this.JoinCompany}";
            }
        }

        /// <summary>小人提：撈取【訓練單位】欄</summary>
        public string TrainingUnit { get; set; }

        /// <summary>訓練單位 (大專：撈取【申請單位(學校)】欄, 	小人提：撈取【訓練單位】欄)</summary>
        public string TrainingUnit_Text
        {
            get
            {
                if (this.PlanTypeCode.TONotNullString() == "1") return this.SchoolName.TONotNullString();
                return this.TrainingUnit.TONotNullString();
            }
        }

        /// <summary>大專：計畫序號(訓練案號)</summary>
        public string PlanID { get; set; }

        /// <summary>小人提：訓練案號</summary>
        public string TrainID { get; set; }

        /// <summary>計畫/訓練案號 (大專：計畫序號(訓練案號), 小人提：訓練案號)</summary>
        public string CombineID
        {
            get
            {
                if (this.PlanTypeCode.TONotNullString() == "1") return this.PlanID.TONotNullString();
                return this.TrainID.TONotNullString();
            }
        }

        /// <summary>學程/課程名稱 (大專：撈取【學程名稱】欄, 小人提：撈取【課程名稱】欄</summary>
        public string ClassName { get; set; }

        public string TrainingDateS { get; set; }

        public string TrainingDateE { get; set; }

        public string ClassDateS { get; set; }

        public string ClassDateE { get; set; }

        public string TrainingTimeS { get; set; }

        public string TrainingTimeE { get; set; }

        /// <summary>開訓日期 (大專：撈取【開訓日期(學年起日)】欄, 小人提：撈取【課程起日】欄)</summary>
        public string Date1S
        {
            get
            {
                if (this.PlanTypeCode.TONotNullString() == "1") return this.TrainingDateS.TONotNullString();
                return this.ClassDateS.TONotNullString();
            }
        }

        /// <summary>結訓日期 (大專：撈取【結訓日期(學年迄日)】欄, 小人提：撈取【課程迄日】欄)</summary>
        public string Date1E
        {
            get
            {
                if (this.PlanTypeCode.TONotNullString() == "1") return this.TrainingDateE.TONotNullString();
                return this.ClassDateE.TONotNullString();
            }
        }

        /// <summary>授課起日</summary>
        public string Date2S
        {
            get
            {
                return this.ClassDateS.TONotNullString();
            }
        }

        /// <summary>授課迄日</summary>
        public string Date2E
        {
            get
            {
                return this.ClassDateE.TONotNullString();
            }
        }

        public string TeacherID { get; set; }

        /// <summary>授課講師姓名</summary>
        public string TeacherName { get; set; }

        /// <summary>內訓/外訓</summary>
        public string TrainingType { get; set; }

        /// <summary>內訓/外訓</summary>
        public string TrainingType_Text
        {
            get
            {
                return string.IsNullOrEmpty(this.TrainingType) ? "" : this.TrainingType == "1" ? "內訓" : this.TrainingType == "2" ? "外訓" : this.TrainingType;
            }
        }

        /// <summary>授課職能類別</summary>
        public string JobAbilityCode { get; set; }

        /// <summary>授課職能類別</summary>
        public string JobAbilityName
        {
            get
            {
                if (this.JobAbilityCode.TONotNullString() == "") return "";
                ShareCodeListModel model = new ShareCodeListModel();
                var Obj = model.JobAbilityCode_List().Where(x => x.Value == this.JobAbilityCode).FirstOrDefault();
                return (Obj == null) ? "" : Obj.Text;
            }
        }

        /// <summary>授課單元</summary>
        public string CourseUnitCode { get; set; }

        [NotDBField]
        public string[] CourseUnitCode_SHOW
        {
            get
            {
                return (this.CourseUnitCode != null) ? this.CourseUnitCode.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.CourseUnitCode = string.Join(",", value.ToList());
            }
        }

        [NotDBField]
        public IList<CheckBoxListItem> CourseUnitCode_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>,轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.TeachUnit_list();
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        /// <summary>授課時數</summary>
        public int? TeachHours { get; set; }
        /// <summary>上課人數</summary>
        public int? TeachNum { get; set; }
        /// <summary>問卷填答人數</summary>
        public int? FillStudentNum { get; set; }
        /// <summary>滿意度問卷連結</summary>
        public string QuestLink { get; set; }
        /// <summary>問卷填答人數</summary>
        public string QuesSearchLink { get; set; }
        public string QRCode
        {
            get
            {
                if (this.QuestLink.TONotNullString() == "") return "";
                byte[] data = Turbo.ReportTK.BarCode.BarCodeUtils.GenerateQRCode(this.QuestLink);
                //System.Drawing.Image oImage = null; //System.Drawing.Bitmap oBitmap = null;

                System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream(data);
                oMemoryStream.Position = 0;
                //oImage = System.Drawing.Image.FromStream(oMemoryStream); //oBitmap = new System.Drawing.Bitmap(oImage);
                return "data:image/png;base64," + Convert.ToBase64String(oMemoryStream.ToArray());
            }
        }
        public string QRCode2
        {
            get
            {
                if (this.QuesSearchLink.TONotNullString() == "") return "";
                byte[] data = Turbo.ReportTK.BarCode.BarCodeUtils.GenerateQRCode(this.QuesSearchLink);
                //System.Drawing.Image oImage = null; //System.Drawing.Bitmap oBitmap = null;
                System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream(data);
                oMemoryStream.Position = 0;
                //oImage = System.Drawing.Image.FromStream(oMemoryStream); //oBitmap = new System.Drawing.Bitmap(oImage);
                return "data:image/png;base64," + Convert.ToBase64String(oMemoryStream.ToArray());
            }
        }

        /// <summary>授課滿意度達70%以上對象表示滿意情形</summary>
        public string Satisfy { get; set; }

        /// <summary>建議事項(意見或建議)</summary>
        public string Suggest { get; set; }

        /// <summary>問卷填答人數</summary>
        public string FillStudentNum_txt
        {
            get
            {
                return FillStudentNum.HasValue ? $"{FillStudentNum.Value}" : "(查無資料)";
            }
        }
        /// <summary>授課滿意度達70%以上對象表示滿意情形</summary>
        public string Satisfy_txt
        {
            get
            {
                return string.IsNullOrEmpty(Satisfy) ? "(查無資料)" : Satisfy;
            }
        }
        /// <summary>建議事項(意見或建議)</summary>
        public string Suggest_txt
        {
            get
            {
                return string.IsNullOrEmpty(Suggest) ? "(查無資料)" : Suggest;
            }
        }
    }
}
