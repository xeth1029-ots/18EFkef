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

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C102MViewModel
    {
        public C102MViewModel() { this.Form = new C102MFormModel(); }

        public C102MFormModel Form { get; set; }

        public C102MSetAuthModel SetAuth { get; set; }
    }

    public class C102MFormModel : PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        [Display(Name = "代號")]
        public string GRPID { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "名稱")]
        public string GRPNAME { get; set; }

        public IList<C102MGridModel> Grid { get; set; }
    }

    public class C102MGridModel : AMGRP
    {
        [NotDBField]
        public long? ROWID { get; set; }
    }

    public class C102MSetAuthModel : AMGRP
    {
        [Display(Name = "代號")]
        public string GRPID { get; set; }

        [Display(Name = "名稱")]
        public string GRPNAME { get; set; }

        [NotDBField]
        public string SYSID
        {
            get
            {
                if (this.GRPID.TONotNullString() == "1") return "A1";
                if (this.GRPID.TONotNullString() == "2") return "A2";
                if (this.GRPID.TONotNullString() == "3") return "A3";
                if (this.GRPID.TONotNullString() == "4") return "AM";
                return "";
            }
        }

        [Display(Name = "模組名稱")]
        [Control(Mode = Control.DropDownList)]
        public string MODULES { get; set; }

        [NotDBField]
        public IList<SelectListItem> MODULES_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.MODULES_list(this.SYSID);
            }
        }

        public IList<C102MSetAuthGridModel> SetAuthGrid { get; set; }
    }

    public class C102MSetAuthGridModel : AMGMAPM
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string PRGNAME { get; set; }

        [NotDBField]
        public bool PRG_Q_CHECK
        {
            get { return "1".Equals(this.PRG_Q) ? true : false; }
            set { this.PRG_Q = value ? "1" : "0"; }
        }

        [NotDBField]
        public bool PRG_I_CHECK
        {
            get { return "1".Equals(this.PRG_I) ? true : false; }
            set { this.PRG_I = value ? "1" : "0"; }
        }

        [NotDBField]
        public bool PRG_U_CHECK
        {
            get { return "1".Equals(this.PRG_U) ? true : false; }
            set { this.PRG_U = value ? "1" : "0"; }
        }

        [NotDBField]
        public bool PRG_D_CHECK
        {
            get { return "1".Equals(this.PRG_D) ? true : false; }
            set { this.PRG_D = value ? "1" : "0"; }
        }

        [NotDBField]
        public bool PRG_P_CHECK
        {
            get { return "1".Equals(this.PRG_P) ? true : false; }
            set { this.PRG_P = value ? "1" : "0"; }
        }
    }
}