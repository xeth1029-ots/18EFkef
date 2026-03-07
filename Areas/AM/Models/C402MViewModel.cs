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

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C402MViewModel
    {
        public C402MViewModel() { this.Form = new C402MFormModel(); }

        public C402MFormModel Form { get; set; }
    }

    public class C402MFormModel
    {
        public C402MFormModel() { this.ExportFormat = "0"; }

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

        [Display(Name = "所屬轄區")]
        [Control(Mode = Control.Hidden)]
        public string TeacherUnit { get; set; }

        public IList<SelectListItem> TeacherUnit_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.UNIT_List;
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "匯出檔案格式")]
        [Control(Mode = Control.Hidden)]
        public string ExportFormat { get; set; }
    }
}