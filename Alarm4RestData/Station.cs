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
    
    public partial class Station
    {
        public int PkStationID { get; set; }
        public string StationName { get; set; }
        public int StationNumber { get; set; }
        public string Detail { get; set; }
        public string DCSName { get; set; }
        public Nullable<byte> DCSNumber { get; set; }
        public string RegionName { get; set; }
        public Nullable<byte> RegionNumber { get; set; }
    }
}