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

namespace WKEFSERVICE.Areas.A3.Models
{
    public class C701MViewModel
    {
        public C701MViewModel() { this.Form = new C701MFormModel(); }

        public C701MFormModel Form { get; set; }
    }

    public class C701MFormModel
    {
        public C701MFormModel() { this.ExportFormat = "0"; }

        [Display(Name = "計畫年度")]
        [Control(Mode = Control.Hidden)]
        public string Year { get; set; }

        public IList<SelectListItem> Year_list
        {
            get
            {
                var model = new ShareCodeListModel();
                var list = model.TransData_Years_List_113();
                list.Insert(0, new SelectListItem() { Text = "請選擇", Value = "" });
                return list;
            }
        }

        /// <summary>
        /// 署可以選全部，分署鎖定自己的分署
        /// 所屬轄區：(0,發展署,1,北基宜花金馬分署,2,桃竹苗分署,3,中彰投分署,4,雲嘉南分署,5,高屏澎東分署)
        /// </summary>
        [Display(Name = "所屬轄區")]
        [Control(Mode = Control.Hidden)]
        public string TeacherUnit { get; set; }
        /// <summary>所屬轄區</summary>
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

        /// <summary>
        /// 定版報表(1,師資個人授課總表、2,師資授課情形統計表、3,滿意度調查表、4,年度評核結果總表)
        /// </summary>
        [Display(Name = "定版報表")]
        public string FinalReportCode { get; set; }
        public IList<SelectListItem> FinalReportCode_list
        {
            get
            {
                IList<SelectListItem> list = new List<SelectListItem>
                {
                    new SelectListItem() { Value = "1", Text = "師資個人授課總表" },
                    new SelectListItem() { Value = "2", Text = "師資授課情形統計表" },
                    new SelectListItem() { Value = "3", Text = "滿意度調查表" },
                    new SelectListItem() { Value = "4", Text = "年度評核結果總表" }
                };
                list.Insert(0, new SelectListItem() { Text = "請選擇", Value = "" });
                return list;
            }
        }

        /// <summary>
        /// 匯出檔案格式
        /// </summary>
        [Display(Name = "匯出檔案格式")]
        [Control(Mode = Control.Hidden)]
        public string ExportFormat { get; set; }

        public string GetContentType(string path1)
        {
            var types = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".jpg", "image/jpeg" },
                { ".png", "image/png" },
                // 添加更多檔案類型
            };

            var ext = System.IO.Path.GetExtension(path1).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream"; // 預設為二進位檔案
        }

    }
}
