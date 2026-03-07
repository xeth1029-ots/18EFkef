using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class AMGRP : IDBRow
    {
        public string GRPID { get; set; }
        public string GRPNAME { get; set; }
        public string MODUSERID { get; set; }
        public string MODUSERNAME { get; set; }
        public string MODTIME { get; set; }
        public string MODIP { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.AMGRP;
        }
    }
}