using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WKEFSERVICE.Models.Entities 
{

    /// <summary>年度定版數據下載</summary>
    [Table("FINALREPORT")]
    public class FinalReport : IDBRow
    {
        /// <summary>序號</summary>
        [IdentityDBField]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)][Column("SEQ")]
        public Int64? Seq { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        //[Required][Column("YEARS", TypeName = "VARCHAR")][MaxLength(4)]
        public string Years { get; set; }

        /// <summary>
        /// 所屬轄區(0,發展署,1,北基宜花金馬分署,2,桃竹苗分署,3,中彰投分署,4,雲嘉南分署,5,高屏澎東分署)
        /// </summary>
        //[Required][Column("UNITCODE", TypeName = "VARCHAR")][MaxLength(3)]
        public string UnitCode { get; set; }

        /// <summary>
        /// 定版報表(1,師資個人授課總表、2,師資授課情形統計表、3,滿意度調查表、4,年度評核結果總表)
        /// </summary>
        //[Required][Column("FINALREPORTCODE", TypeName = "VARCHAR")][MaxLength(3)]
        public string FinalReportCode { get; set; }

        /// <summary>
        /// 報表檔案名
        /// </summary>
        //[Required][Column("FILENAME1", TypeName = "NVARCHAR")][MaxLength(333)]
        public string FileName1 { get; set; }

        /// <summary>
        /// 報表路徑
        /// </summary>
        //[Column("FILEPATH1", TypeName = "NVARCHAR")][MaxLength(333)]
        public string FilePath1 { get; set; }

        //[Column("UPDATEDACCOUNT", TypeName = "NVARCHAR")][MaxLength(15)]
        public string UpdatedAccount { get; set; }

        //[Column("UPDATEDDATETIME", TypeName = "DATETIME")]
        public DateTime? UpdatedDateTime { get; set; }

        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.FinalReport;
        }
    }
}
