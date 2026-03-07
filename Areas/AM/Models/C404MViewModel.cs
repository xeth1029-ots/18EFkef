using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Turbo.Commons;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C404MViewModel
    {
        public C404MViewModel() { this.Form = new C404MFormModel(); }

        public C404MFormModel Form { get; set; }
    }

    public class C404MFormModel
    {
        public C404MFormModel() { this.ExportFormat = "0"; }

        [Display(Name = "計畫年度")]
        [Control(Mode = Control.Hidden)]
        public string Year { get; set; }

        public IList<SelectListItem> Year_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var list = model.TransData_Years_List();
                list.Insert(0, new SelectListItem() { Text = "全顯示", Value = "" });
                return list;
            }
        }

        //用於 SQL 辨別，無實際參與資料過濾
        /// <summary>教師姓名：文字框，輸入關鍵字，針對姓名欄位做模糊查詢</summary>
        [Display(Name = "教師姓名")]
        [Control(Mode = Control.Hidden)]
        public string TeacherName { get; set; }

        // 僅用於 SQL 辨別，無實際參與資料過濾
        [Display(Name = "計畫別")]
        [Control(Mode = Control.Hidden)]
        public string PlanTypeCode { get; set; }

        [Display(Name = "所屬轄區")]
        [Control(Mode = Control.Hidden)]
        public string TeacherUnit { get; set; }

        public IList<SelectListItem> TeacherUnit_list
        {
            get
            {
                SessionModel sm = SessionModel.Get();
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> list = model.UNIT_List;
                list.Insert(0, new SelectListItem() { Text = "全部", Value = "" });
                return list;
            }
        }

        [Display(Name = "匯出檔案格式")]
        [Control(Mode = Control.Hidden)]
        public string ExportFormat { get; set; }

        /// <summary>服務單位屬性(工會、協會、管顧)</summary>
        [Display(Name = "服務單位屬性")]
        [Control(Mode = Control.Hidden)]
        public string ServiceUnitProperties { get; set; }

        [NotDBField]
        public string ServiceUnitProperties_Name
        {
            get
            {
                if (ServiceUnitProperties == null) { return ""; }
                ShareCodeListModel model = new ShareCodeListModel();
                var AllList = model.ServiceUnitProperties_List();
                var Result = this.ServiceUnitProperties.TONotNullString().Split(',').ToArray();
                for (int i = 0; i <= Result.Length - 1; i++)
                {
                    var tmpStr = AllList.Where(x => x.Value == Result[i]);
                    if (!tmpStr.Any()) continue;
                    Result[i] = tmpStr.FirstOrDefault().Text;
                }
                return string.Join(", ", Result);
            }
        }

    }

}