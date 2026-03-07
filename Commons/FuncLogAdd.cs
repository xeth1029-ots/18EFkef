using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using Turbo.DataLayer;

namespace WKEFSERVICE.Commons
{
    public enum ModifyEvent { NONE, BEFORE, AFTER }
    public enum ModifyType { NONE, QUERY, CREATE, UPDATE, DELETE, UPLOAD, DOWNLOAD, PRINT }
    public enum ModifyState { NONE, SUCCESS, FAIL }

    public class FuncLogAdd
    {
        /// <summary>
        /// 寫入 操作LOG                                                  <br />
        /// events => 異動事件 NONE, BEFORE, AFTER                        <br />
        /// type   => 異動類別 NONE, QUERY, CREATE, UPDATE, DELETE, PRINT <br />
        /// state  => 異動狀態 NONE, SUCCESS, FAIL                        <br />
        /// func   => 呼叫來源 Program name                               <br />
        /// update => 更新用的 Entities                                   <br />
        /// where  => 更新條件的 Entities                                 <br />
        /// Remark => 附註用，可傳入資料表名稱或 SqlMap 名                  <br />
        /// </summary>
        public static void Do(ModifyEvent events, ModifyType type, ModifyState state, string func, object update, object where, string Remark)
        {
            string arg = "";
            switch (type)
            {
                case ModifyType.QUERY:
                    {
                        var where_arg = Trans_Object(where, true, true, "=");
                        arg =
                            "資料表/函數名：" + Remark + " | " +
                            "條件：" + string.Join(",", where_arg.ToArray());
                        break;
                    }
                case ModifyType.CREATE:
                    {
                        var update_arg = Trans_Object(update, false, true, "=");
                        arg =
                            "資料表/函數名：" + Remark + " | " +
                            "新增：" + string.Join(",", update_arg.ToArray());
                        break;
                    }
                case ModifyType.UPDATE:
                    {
                        var update_arg = Trans_Object(update, false, true, "=");
                        var where_arg = Trans_Object(where, true, true, "=");
                        arg =
                            "資料表/函數名：" + Remark + " | " +
                            "修改：" + string.Join(",", update_arg.ToArray()) + " | " +
                            "條件：" + string.Join(",", where_arg.ToArray());
                        break;
                    }
                case ModifyType.DELETE:
                    {
                        var update_arg = Trans_Object(update, false, true, "=");
                        var where_arg = Trans_Object(where, true, true, "=");
                        arg =
                            "資料表/函數名：" + Remark + " | " +
                            "刪除：" + string.Join(",", update_arg.ToArray()) + " | " +
                            "條件：" + string.Join(",", where_arg.ToArray());
                        break;
                    }
                case ModifyType.PRINT:
                    {
                        var where_arg = Trans_Object(where, true, true, "=");
                        arg =
                            "資料表/函數名：" + Remark + " | " +
                            "條件：" + string.Join(",", where_arg.ToArray());
                        break;
                    }
                default: return;
            }
            string tmpStr = "";
            if (events != ModifyEvent.NONE) tmpStr = events.ToString() + " ";
            // Log 準備寫入
            BaseDAO dao = new BaseDAO();
            int rtn = dao.Insert<funclog>(new funclog()
            {
                username = SessionModel.Get().UserInfo.UserNo,
                logtype = tmpStr + type.ToString(),
                logfunc = func,
                createtime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                arg = arg,
                result = state.ToString()
            });
        }

        /// <summary>
        /// Update 或 Where 的文字產生                 <br />
        /// src             => 來源資料集              <br />
        /// isNull_Continue => 是否遇到空白跳過         <br />
        /// isNull_Text     => 是否遇到空白產生NULL字串 <br />
        /// </summary>
        private static List<string> Trans_Object(object src, bool isNull_Continue, bool isNull_Text, string CutString)
        {
            var Result = new List<string>();
            string[] SkipFields = { "PWD" };
            string[] SkipSysFields = {
                "System.String[]",
                "System.Collections.Generic.List",
                "Turbo.DataLayer.PaginationInfo",
            };
            if (src == null) return Result;
            if (src is Hashtable)
            {
                Hashtable tmpSrc = (Hashtable)src;
                foreach (DictionaryEntry item in tmpSrc)
                {
                    if (SkipFields.Where(x => item.Key.TONotNullString().Contains(x)).Any()) continue;
                    if (SkipSysFields.Where(x => item.Value.TONotNullString().Contains(x)).Any()) continue;
                    Result.Add(item.Key + CutString + item.Value);
                }
            }
            else if (src is List<Hashtable>)
            {
                List<Hashtable> tmpSrc = (List<Hashtable>)src;
                int i = 0;
                foreach (var loop in tmpSrc)
                {
                    i++;
                    Result.Add("[List-row]" + CutString + i.ToString());
                    foreach (DictionaryEntry item in loop)
                    {
                        if (SkipFields.Where(x => item.Key.TONotNullString().Contains(x)).Any()) continue;
                        if (SkipSysFields.Where(x => item.Value.TONotNullString().Contains(x)).Any()) continue;
                        Result.Add(item.Key + CutString + item.Value);
                    }
                }
            }
            else
            {
                foreach (var item in src.GetType().GetProperties())
                {
                    string name = item.Name;
                    string value = item.GetValue(src).TONotNullString();
                    if (SkipFields.Where(x => name.Contains(x)).Any()) continue;
                    if (SkipSysFields.Where(x => value.Contains(x)).Any()) continue;
                    if (isNull_Continue && value == "") continue;
                    if (isNull_Text && value == "") value = "NULL";
                    Result.Add(name + CutString + value);
                }
            }
            return Result;
        }
    }
}