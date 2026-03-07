using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Services;

namespace Turbo.Helpers
{
    /// <summary>
    /// HTML 按鈕 產生輔助方法類別
    /// </summary>
    public static class ButtonExtension
    {
        /// <summary>
        /// HTML 按鈕 產生方法
        /// </summary>
        /// <param name="list">顯示按鈕，可參照AM/C101M方式，Search:查詢/New:新增/Save:儲存/Delete:刪除/Close:關閉 ，後續逐漸增加...</param>
        /// <param name="dis_list">將該按鈕鎖定(加入disabled狀態)，以list的key值名稱若相同則鎖定</param>
        /// <returns></returns>
        public static MvcHtmlString ButtonForTurbo<TModel>(this HtmlHelper<TModel> htmlHelper, Dictionary<string, string> list, IList<string> dis_list = null)
        {
            List<string> GrpIdList = new List<string>();
         
            BaseDAO dao = new BaseDAO();
            
            // HTML標籤
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='btnbar'>");

            // 根據list遍歷相符的KeyName，產生button HTML語法
            foreach (var item in list)
            {
                // 取出list-Key
                var _fun_NAME = item.Value;
                // 若dis_list有包含此list-Key，表示會加入狀態disabled
                var _dis_NAME = "";
                if (dis_list != null) _dis_NAME = dis_list.Contains(item.Key) ? "disabled" : "";
                // 根據list-Key產製 button HTML語法
                switch (item.Key.ToUpper())
                {
                    // 送出查詢
                    case "SEARCH":
                            sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-search space-right' aria-hidden='true'></i>送出查詢</button>");
                        break;

                    // 清除重填
                    case "RESET":
                        sb.Append("<button type='reset' class='btn btn-blue'><i class='fa fa-repeat space-right' aria-hidden='true'></i>清除重填</button>");
                        break;

                    // 清除重填(Function版本)
                    case "RESET_FUC":
                        sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-repeat space-right' aria-hidden='true'></i>清除重填</button>");
                        break;

                    // 匯入
                    case "FILEINSERT":
                            sb.Append("<button type='button' class='btn btn-info' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-floppy-o' aria-hidden='true'></i>匯入</button>");
                        break;

                    // 新增
                    case "NEW":
                            sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-pencil-square-o' aria-hidden='true'></i>新增</button>");
                        break;

                    // 儲存
                    case "SAVE":
                            sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-pencil-square-o' aria-hidden='true'></i>儲存</button>");
                        break;

                    // 關閉
                    case "CLOSE":
                        sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-times-circle' aria-hidden='true'></i>關閉</button>");
                        break;

                    // 刪除
                    case "DELETE":
                            sb.Append("<button type='button' class='btn btn-danger' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-trash' aria-hidden='true'></i>刪除</button>");
                        break;

                    // 返回
                    case "BACK":
                        sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + "><i class='fa fa-reply' aria-hidden='true'></i>返回</button>");
                        break;

                    // 送出
                    case "E_SEND":
                        sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + ">送出</button>");
                        break;

                    // 預覽
                    case "E_PREVIEW":
                        sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + ">預覽</button>");
                        break;

                    // 取消
                    case "E_CANCEL":
                        sb.Append("<button type='button' class='btn btn-blueLight' onclick='" + _fun_NAME + "' " + _dis_NAME + ">取消</button>");
                        break;

                    // 調整排序
                    case "RESETSEQ":
                        sb.Append("<button type='button' class='btn btn-blueLight' onclick='" + _fun_NAME + "' " + _dis_NAME + ">調整排序</button>");
                        break;

                    // 列印
                    case "PRINT":
                        sb.Append("<button type='button' class='btn btn-blue' onclick='" + _fun_NAME + "' " + _dis_NAME + ">匯出表單</button>");
                        break;
                }
            }

            sb.Append("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}