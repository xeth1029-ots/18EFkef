using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using System.Net.Mail;
using WKEFSERVICE.Areas.A1.Models;
using Omu.ValueInjecter;
using System.Web;
using System.IO;

namespace WKEFSERVICE.DataLayers
{
    public class A1DAO : BaseDAO
    {
        public IList<TeachersModel> GetTeacher_HomePageCarousel(int TopCount, bool Random)
        {
            // 是否要隨機取值
            if (Random)
            {
                var tmpList = base.QueryForListAll<TeachersModel>("A1.getTeacher_HomePageCarousel", null).ToList();
                var rtnList = new List<TeachersModel>();
                // 開始亂數取出資料
                Random randomObj = new Random();
                if (TopCount > tmpList.ToCount()) TopCount = tmpList.ToCount();
                while (rtnList.ToCount() < TopCount)
                {
                    int rndIdx = randomObj.Next(0, tmpList.ToCount());
                    var tmpObj = tmpList[rndIdx];
                    if (!rtnList.Where(x => x == tmpObj).Any())
                    {
                        rtnList.Add(tmpObj);
                    }
                }
                return rtnList;
            }
            else
            {
                Hashtable parms = new Hashtable();
                parms["TopCount"] = " TOP(" + TopCount.ToString() + ") ";
                return base.QueryForListAll<TeachersModel>("A1.getTeacher_HomePageCarousel", parms);
            }
        }

        public IList<C101MGridModel> QueryC101M(C101MFormModel form)
        {
            return base.QueryForList<C101MGridModel>("A1.queryC101M", form);
        }

        public IList<C101MGridModel> QueryC101M_NewVer(C101MFormModel form)
        {
            return base.QueryForListAll<C101MGridModel>("A1.queryC101M", form);
        }

        public C101MDetailModel DetailC101M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable();
            parms["Seq"] = i_Seq;
            return base.QueryForObject<C101MDetailModel>("A1.detailC101M", parms);
        }

        public IList<C102MGridModel> QueryC102M(C102MFormModel form)
        {
            return base.QueryForList<C102MGridModel>("A1.queryC102M", form);
        }

        public C102MDetailModel DetailC102M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable();
            parms["Seq"] = i_Seq;
            return base.QueryForObject<C102MDetailModel>("A1.detailC102M", parms);
        }

        public IList<C102MAttachedsModel> DetailAttachedsC102M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable();
            parms["Seq"] = i_Seq;
            return base.QueryForListAll<C102MAttachedsModel>("A1.detailAttachedsC102M", parms);
        }
    }
}