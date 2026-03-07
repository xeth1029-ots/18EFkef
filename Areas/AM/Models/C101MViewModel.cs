using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using System.Web.Mvc;
using Turbo.Commons;
using System.Linq;

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C101MViewModel
    {
        public C101MViewModel() { this.Form = new C101MFormModel(); }

        public C101MFormModel Form { get; set; }

        public C101MDetailModel Detail { get; set; }
    }

    public class C101MFormModel : PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        [Display(Name = "帳號")]
        public string USERNO { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "姓名")]
        public string USERNAME { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "單位")]
        public string UNITID { get; set; }

        public IList<SelectListItem> UNITID_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.UNIT_All_List;
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "角色")]
        public string GRPID { get; set; }

        public IList<SelectListItem> GRPID_list
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
        [Display(Name = "狀態")]
        public string AUTHSTATUS { get; set; }

        public IList<SelectListItem> AUTHSTATUS_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.AUTHSTATUS_list();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [NotDBField]
        [Control(Mode = Control.Hidden)]
        [Display(Name = "最後登入(起)")]
        public string LASTLOGIN_S { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string LASTLOGIN_S_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(LASTLOGIN_S))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(LASTLOGIN_S, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                LASTLOGIN_S = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [NotDBField]
        [Control(Mode = Control.Hidden)]
        [Display(Name = "最後登入(迄)")]
        public string LASTLOGIN_E { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string LASTLOGIN_E_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(LASTLOGIN_E))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(LASTLOGIN_E, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                LASTLOGIN_E = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        /// <summary>瀏覽頁 - 批次審核用欄位 狀態</summary>
        public string TheAuthStatus { get; set; }

        /// <summary>瀏覽頁 - 批次審核用欄位 日期</summary>
        public string TheAuthDate { get; set; }

        public IList<C101MGridModel> Grid { get; set; }
    }

    public class C101MGridModel : AMDBURM
    {
        [NotDBField]
        public bool IsCheck { get; set; }

        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string UNIT_NAME { get; set; }

        [NotDBField]
        public string GRP_NAME { get; set; }

        [NotDBField]
        public string AUTHSTATUS_NAME
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var list = model.AUTHSTATUS_list().ToList().Where(x => x.Value == this.AUTHSTATUS).FirstOrDefault();
                return (list == null) ? "-- -- --" : list.Text;
            }
        }
    }

    public class C101MDetailModel : AMDBURM
    {
        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        [Required]
        [Display(Name = "帳號")]
        public string USERNO { get; set; }

        [Required]
        [Display(Name = "姓名")]
        public string USERNAME { get; set; }

        [Required]
        [Display(Name = "身分證號")]
        public string IDNO { get; set; }

        [Required]
        [Display(Name = "生日")]
        public string BIRTHDAY { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string BIRTHDAY_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(BIRTHDAY))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(BIRTHDAY, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                BIRTHDAY = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Required]
        [Display(Name = "電子郵件")]
        public string EMAIL { get; set; }

        [Required]
        [Display(Name = "單位")]
        public string UNITID { get; set; }

        [Required]
        [NotDBField]
        public IList<SelectListItem> UNITID_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.UNIT_All_List;
            }
        }

        [Required]
        [Display(Name = "角色")]
        public string GRPID { get; set; }

        [Required]
        [NotDBField]
        public IList<SelectListItem> GRPID_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var rtn = model.GRPID_List.Where(x => x.Value != "1").ToList();  // 過濾掉 "一般使用者" 新建時不可選 (此選項非帳號種類之一)
                return rtn;
            }
        }

        [Required]
        [Display(Name = "狀態")]
        public string AUTHSTATUS { get; set; }

        [Required]
        [NotDBField]
        public IList<SelectListItem> AUTHSTATUS_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.AUTHSTATUS_list(true);
            }
        }

        [Required]
        [Display(Name = "申請日期")]
        public string APPDATE { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string APPDATE_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(APPDATE))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(APPDATE, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                APPDATE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Required]
        [Display(Name = "啟用日期")]
        public string AUTHDATES { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string AUTHDATES_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(AUTHDATES))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(AUTHDATES, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                AUTHDATES = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        //[Required]
        [Display(Name = "停用日期")]
        public string AUTHDATEE { get; set; }

        [NotDBField]
        [Control(Mode = Control.DatePicker)]
        public string AUTHDATEE_AD
        {
            get
            {
                if (!string.IsNullOrEmpty(AUTHDATEE))
                {
                    return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(AUTHDATEE, ""));  // YYYYMMDD 回傳給系統
                }
                return null;
            }
            set
            {
                AUTHDATEE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }
    }
}