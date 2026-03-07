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

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C104MViewModel
    {
        public C104MViewModel() { this.Form = new C104MFormModel(); }

        public C104MFormModel Form { get; set; }

        public C104MDetailModel Detail { get; set; }
    }

    public class C104MFormModel : PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        [Display(Name = "類別")]
        public string PostType { get; set; }

        public IList<SelectListItem> PostType_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.PostType_list();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "公告對象")]
        public string PostTo { get; set; }

        public IList<SelectListItem> PostTo_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.GRPID_List;
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "關鍵字")]
        public string KeyWord { get; set; }

        public string PostDateS_A { get; set; }

        [Display(Name = "公告日期起")]
        [Control(Mode = Control.DatePicker)]
        public string PostDateS_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(PostDateS_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDateS_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string PostDateS_B { get; set; }

        [Display(Name = "公告日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string PostDateS_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(PostDateS_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDateS_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string UpdatedDatetime_A { get; set; }

        [Display(Name = "異動日期起")]
        [Control(Mode = Control.DatePicker)]
        public string UpdatedDatetime_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(UpdatedDatetime_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                UpdatedDatetime_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string UpdatedDatetime_B { get; set; }

        [Display(Name = "異動日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string UpdatedDatetime_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(UpdatedDatetime_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                UpdatedDatetime_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string PostDateE_A { get; set; }

        [Display(Name = "停用日期起")]
        [Control(Mode = Control.DatePicker)]
        public string PostDateE_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(PostDateE_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDateE_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string PostDateE_B { get; set; }

        [Display(Name = "停用日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string PostDateE_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(PostDateE_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDateE_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "優先排序")]
        public string OrderField { get; set; }

        public IList<SelectListItem> OrderField_list
        {
            get
            {
                var dictionary = new Dictionary<string, string> {
                    { "a.UpdatedDatetime", "異動日期" },
                    { "a.PostDateS",       "公告日期" },
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "優先排序")]
        public string OrderField_ASC_DESC { get; set; }
        public IList<SelectListItem> OrderField_ASC_DESC_list
        {
            get
            {
                var dictionary = new Dictionary<string, string> {
                    { "ASC",  "正排序" },
                    { "DESC", "倒排序" },
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
        }

        [Control(Mode = Control.Hidden)]
        public string Seq_forDel { get; set; }

        public IList<C104MGridModel> Grid { get; set; }
    }

    public class C104MGridModel : MsgPost
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string PostType_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.PostType_list().Where(x => x.Value == this.PostType.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [NotDBField]
        public string PostTo_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.GRPID_List.Where(x => x.Value == this.PostTo.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
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
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
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
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
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
    }

    public class C104MDetailModel : MsgPost
    {
        public C104MDetailModel() { this.IsNew = false; }

        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        [Required]
        [Display(Name = "類別")]
        public int PostType { get; set; }

        public IList<SelectListItem> PostType_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.PostType_list();
            }
        }

        [Required]
        [Display(Name = "公告對象")]
        public int PostTo { get; set; }

        public IList<SelectListItem> PostTo_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.GRPID_List;
            }
        }

        public string PostDateS { get; set; }

        [Required]
        [Display(Name = "公告日期")]
        [Control(Mode = Control.DatePicker)]
        public string PostDateS_AD
        {
            get
            {
                if (PostDateS.TONotNullString() == "") return "";
                string[] tmpList = PostDateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string PostDateE { get; set; }

        [Required]
        [Display(Name = "停用日期")]
        [Control(Mode = Control.DatePicker)]
        public string PostDateE_AD
        {
            get
            {
                if (PostDateE.TONotNullString() == "") return "";
                string[] tmpList = PostDateE.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Required]
        [Display(Name = "標題")]
        public string PostCaption { get; set; }

        /// <summary>公告內容</summary>
        [Required]
        [Display(Name = "公告內容")]
        public string PostContent { get; set; }

        [Required]
        [Display(Name = "置頂")]
        public bool Is_OnTop
        {
            get { return IsOnTop == "1"; }
            set { IsOnTop = value ? "1" : "0"; }
        }

        [Required]
        [Display(Name = "是否要於公告開放我要報名")]
        public string IsShowSignUp { get; set; }

        [Display(Name = "會議")]
        public Int64? MeetingSeq { get; set; }

        [NotDBField]
        public string MeetingSeq_Name { get; set; }

        public IList<SelectListItem> MeetingSeq_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "<無>", Value = "" };
                var list = model.MeetingSeq_list;
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "發布單位")]
        public string PublishedUnitName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                string tmpStr = "";
                var tmp = model.UNIT_All_List.Where(x => x.Value == this.PublishedUnit.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        /// <summary>檔案上傳 - 附件檔案資料表 (dbo.MsgPostAttached)</summary>
        public IList<C104MAttachedsModel> Attacheds { get; set; }

        /// <summary>檔案上傳 - 新增附件檔案用元件</summary>
        [NotDBField]
        public HttpPostedFileBase _UPLOAD_FILE { get; set; }
    }

    public class C104MAttachedsModel : MsgPostAttached
    {
        public C104MAttachedsModel()
        {
            this.isActive = true;
            //this.isNewFile = false;
        }

        [NotDBField]
        public bool isActive { get; set; }

        //[NotDBField]
        //public bool isNewFile { get; set; }

        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }

        /// <summary>檔案上傳</summary>
        //[NotDBField]
        //public HttpPostedFileBase FILE { get; set; }
    }
}