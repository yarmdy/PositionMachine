//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZEHOU.PM.DB.dbLabelMid
{
    using System;
    using System.Collections.Generic;
    
    public partial class Refund_Label
    {
        public string BarCode { get; set; }
        public string PatientID { get; set; }
        public string DeviceID { get; set; }
        public string Department { get; set; }
        public string Bed { get; set; }
        public string SampleName { get; set; }
        public Nullable<int> LableType { get; set; }
        public Nullable<int> PrintCount { get; set; }
        public string TubeColor { get; set; }
        public string TestGroup { get; set; }
        public string TestOrder { get; set; }
        public string EmergenteInfo { get; set; }
        public string BS01 { get; set; }
        public string BS02 { get; set; }
        public string BS03 { get; set; }
        public string BS04 { get; set; }
        public string BS05 { get; set; }
        public string BS06 { get; set; }
        public string BS07 { get; set; }
        public string BS08 { get; set; }
        public string BS09 { get; set; }
        public string BS10 { get; set; }
        public string LS01 { get; set; }
        public string LS02 { get; set; }
        public string LS03 { get; set; }
        public string LS04 { get; set; }
        public string LS05 { get; set; }
        public string LS06 { get; set; }
        public string LS07 { get; set; }
        public string LS08 { get; set; }
        public string LS09 { get; set; }
        public string LS10 { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<int> Refund_LabelStatus { get; set; }
        public string Refund_LabelUserID { get; set; }
        public Nullable<System.DateTime> Refund_LabelTime { get; set; }
        public Nullable<int> ResponseStatus { get; set; }
        public Nullable<System.DateTime> ResponseTime { get; set; }
    }
}