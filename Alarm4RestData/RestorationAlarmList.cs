//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Alarm4Rest.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class RestorationAlarmList
    {
        public int PkAlarmListID { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<byte> PointType { get; set; }
        public Nullable<int> FkIndexID { get; set; }
        public string StationName { get; set; }
        public string PointName { get; set; }
        public Nullable<int> AlarmType { get; set; }
        public Nullable<byte> Flashing { get; set; }
        public Nullable<double> ActualValue { get; set; }
        public string Message { get; set; }
        public string SourceName { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<byte> SourceType { get; set; }
        public Nullable<byte> AlarmFlag { get; set; }
        public string GroupPointName { get; set; }
        public string GroupDescription { get; set; }
        public string Priority { get; set; }
    }
}
