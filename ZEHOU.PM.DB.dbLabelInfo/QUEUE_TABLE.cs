//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZEHOU.PM.DB.dbLabelInfo
{
    using System;
    using System.Collections.Generic;
    
    public partial class QUEUE_TABLE
    {
        public string PatientID { get; set; }
        public string ID_Card_No { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string PatientName { get; set; }
        public int BloodWindow { get; set; }
        public int OrderID { get; set; }
        public System.DateTime QueuingTime { get; set; }
        public int Priority { get; set; }
        public int PastCounts { get; set; }
        public int State { get; set; }
        public int VoiceBroadcastState { get; set; }
        public string RESERVE1 { get; set; }
        public string RESERVE2 { get; set; }
        public string RESERVE3 { get; set; }
        public decimal RESERVE4 { get; set; }
        public decimal RESERVE5 { get; set; }
        public decimal RESERVE6 { get; set; }
        public string EquipmentID { get; set; }
    
        public virtual DR DR { get; set; }
        public virtual PRIORITY_RULE_TABEL PRIORITY_RULE_TABEL { get; set; }
    }
}
