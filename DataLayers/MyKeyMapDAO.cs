using WKEFSERVICE.Services;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Turbo.Commons;
using Turbo.DataLayer;

namespace WKEFSERVICE.DataLayers
{
    public class MyKeyMapDAO : KeyMapDAO
    {
        private new static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region RemoveListItem 從 KeyMapModel 型別項目集合內移除指定項目

        /// <summary>從 KeyMapModel 型別項目集合內移除指定項目。</summary>
        /// <param name="list">項目清單</param>
        /// <param name="excludeCode">要排除的項目代碼。</param>
        public void RemoveListItem(IList<KeyMapModel> list, string excludeCode)
        {
            if (list != null && list.Count > 0)
            {
                var del = new List<KeyMapModel>();
                foreach (var n in list)
                {
                    if (n.CODE == excludeCode) del.Add(n);
                }
                foreach (var n in del)
                {
                    list.Remove(n);
                }
            }
        }

        /// <summary>從 KeyMapModel 型別項目集合內移除指定項目。</summary>
        /// <param name="list">項目清單</param>
        /// <param name="excludeCode">要排除的項目代碼。參數輸入範例： "A" 或是 "A", "B", ...。</param>
        public void RemoveListItem(IList<KeyMapModel> list, params string[] excludeCode)
        {
            if (list != null && list.Count > 0)
            {
                var del = new List<KeyMapModel>();
                foreach (var code in excludeCode)
                {
                    foreach (var n in list)
                    {
                        if (n.CODE == code) del.Add(n);
                    }
                }
                foreach (var n in del)
                {
                    list.Remove(n);
                }
            }
        }

        /// <summary>從 KeyMapModel 型別項目集合內移除指定項目。</summary>
        /// <param name="list">項目清單</param>
        /// <param name="excludeCodes">要排除的項目代碼陣列（或是項目代碼集合）。參數輸入範例：new [] { "A", "E" }。</param>
        public void RemoveListItem(IList<KeyMapModel> list, IEnumerable<string> excludeCodes)
        {
            if (list != null && list.Count > 0)
            {
                var del = new List<KeyMapModel>();
                foreach (var code in excludeCodes)
                {
                    foreach (var n in list)
                    {
                        if (n.CODE == code) del.Add(n);
                    }
                }
                foreach (var n in del)
                {
                    list.Remove(n);
                }
            }
        }

        #endregion RemoveListItem 從 KeyMapModel 型別項目集合內移除指定項目

        /// <summary>
        /// 傳入指定的代碼類別(CodeMapType), 以 IList KeyMapModel 的格式回傳系統代碼清單,
        /// 會自動檢查 Session 中是否已有 Cache 資料, 有則直接回傳,
        /// 若沒有cache資料, 則轉呼叫 KeyMapDao.GetCodeMapList() 取得後 cache 到 Session 中
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public IList<KeyMapModel> GetCachedCodeMapList(CodeMapType codeType)
        {
            IList<KeyMapModel> list;
            object obj = HttpContext.Current.Session[$"CodeMap_{codeType}"];
            if (obj != null && obj is IList<KeyMapModel>)
            {
                list = (IList<KeyMapModel>)obj;
            }
            else
            {
                list = new MyKeyMapDAO().GetCodeMapList(codeType);
                HttpContext.Current.Session[$"CodeMap_{codeType}"] = list;
            }

            IList<KeyMapModel> listNew = new List<KeyMapModel>();
            foreach (var item in list)
            {
                KeyMapModel keyMap = new KeyMapModel()
                {
                    CODE = item.CODE,
                    TEXT = item.TEXT,
                    Selected = item.Selected
                };
                listNew.Add(keyMap);
            }

            return listNew;
        }

        #region GetCodeMapList2 取得「代碼-名稱」項目清單

        /// <summary>
        /// 取得「代碼-名稱」項目清單
        /// </summary>
        /// <param name="type">代碼類別（CodeMapType）</param>
        /// <param name="parms">（非必要參數）在 SqlMap SQL 內的查詢條件參數。</param>
        /// <param name="excludeCodes">（非必要參數）要排除的項目代碼陣列（或是項目代碼集合）</param>
        /// <returns></returns>
        public IList<KeyMapModel> GetCodeMapList2(CodeMapType type, object parms = null, IEnumerable<string> excludeCodes = null)
        {
            if (string.IsNullOrEmpty(type.SQL))
            {
                throw new ArgumentNullException("GetCodeMapList2 error: The CodeMapType.SQL value can not be empty.");
            }
            else
            {
                var list = base.QueryForListAll<KeyMapModel>(type.SQL, parms);
                if (excludeCodes != null) this.RemoveListItem(list, excludeCodes);
                return list;
            }
        }

        #endregion GetCodeMapList2 取得「代碼-名稱」項目清單


    }
}