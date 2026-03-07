using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using Turbo.Commons;
using Turbo.DataLayer;

namespace WKEFSERVICE.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
            this.form = new LoginFormModel();
            this.PwdChange = new PwdChangeFormModel();
            this.ForgetPasswd = new ForgetPasswdModel();
            this.ForgetPasswdChange = new ForgetPasswdChangeModel();
        }
        public LoginFormModel form { get; set; }
        public PwdChangeFormModel PwdChange { get; set; }
        public ForgetPasswdModel ForgetPasswd { get; set; }
        public ForgetPasswdChangeModel ForgetPasswdChange { get; set; }

        /// <summary>登入失敗的錯誤訊息</summary>
        public string ErrorMessage { get; set; }

    }

    /// <summary>
    /// 登入表單 Model
    /// </summary>
    public class LoginFormModel
    {
        /// <summary>電子郵件或帳號</summary>
        [Display(Name = "電子郵件或帳號")]
        [Required]
        public string UserNo_EMAIL { get; set; }
        //public string UserNo { get; set; }

        /// <summary>密碼</summary>
        [Display(Name = "密碼")]
        [Required]
        public string UserPwd { get; set; }

        /// <summary>驗證碼</summary>
        [Display(Name = "驗證碼")]
        [Required]
        public string ValidateCode { get; set; }
    }

    public class PwdChangeFormModel
    {
        public PwdChangeFormModel() { this.IsNew = true; }

        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        /// <summary>電子郵件</summary>
        [Display(Name = "電子郵件")]
        [Control(Mode = Control.TextBox, size = "50", maxlength = "50", IsReadOnly = true)]
        [Required]
        public string EMAIL { get; set; }

        /// <summary>帳號</summary>
        [Display(Name = "帳號")]
        [Control(Mode = Control.TextBox, size = "20", maxlength = "20", IsReadOnly = true)]
        [Required]
        public string UserNo { get; set; }

        /// <summary>密碼</summary>
        [Display(Name = "密碼")]
        [Control(Mode = Control.PxssWxrd, size = "20", maxlength = "20", IsOpenNew = false)]
        [NotDBField]
        [Required]
        public string UserPWD { get; set; }

        /// <summary>重複密碼</summary>
        [Display(Name = "重複使用者密碼")]
        [NotDBField]
        [Required]
        public string UserPWD_REPEAT { get; set; }
    }

    public class ForgetPasswdModel
    {
        /// <summary>身分證號</summary>
        [Display(Name = "身分證號")]
        [Required]
        public string IDNO { get; set; }

        /// <summary>電子郵件</summary>
        [Display(Name = "電子郵件")]
        [Required]
        public string EMAIL { get; set; }

        /// <summary>生日</summary>
        public string BIRTHDAY { get; set; }

        /// <summary>出生年月日</summary>
        [Required]
        [NotDBField]
        [Display(Name = "生日")]
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

        /// <summary>驗證碼</summary>
        [Display(Name = "驗證碼")]
        [Required]
        public string ValidateCode { get; set; }
    }

    public class ForgetPasswdChangeModel : AMDBURM
    {
        public ForgetPasswdChangeModel() { }

        /// <summary>GUID</summary>
        [Control(Mode = Control.Hidden)]
        [Required]
        public string GUID { get; set; }

        /// <summary>電子郵件</summary>
        [Display(Name = "電子郵件")]
        [Control(Mode = Control.TextBox, size = "50", maxlength = "50", IsReadOnly = true)]
        [Required]
        public string EMAIL { get; set; }

        /// <summary>帳號</summary>
        [Display(Name = "帳號")]
        [Control(Mode = Control.TextBox, size = "20", maxlength = "20", IsReadOnly = true)]
        [Required]
        public string USERNO { get; set; }

        /// <summary>姓名</summary>
        [Display(Name = "姓名")]
        [Control(Mode = Control.TextBox, size = "20", maxlength = "20", IsReadOnly = true)]
        [Required]
        public string USERNAME { get; set; }

        /// <summary>密碼</summary>
        [Display(Name = "密碼")]
        [Control(Mode = Control.PxssWxrd, size = "20", maxlength = "20")]
        [NotDBField]
        [Required]
        public string PWD { get; set; }

        /// <summary>重複密碼</summary>
        [Display(Name = "重複使用者密碼")]
        [NotDBField]
        [Required]
        public string PWD_REPEAT { get; set; }
    }
}