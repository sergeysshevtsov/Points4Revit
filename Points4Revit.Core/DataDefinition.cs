using System.Collections.Generic;

namespace Points4Revit.Core
{
    public class ObjectData
    {
        public LineData LineData { get; set; }
        public List<PointData> PointData { get; set; }
    }

    public class LineData
    {
        public string LineId { get; set; }
        public string LineType { get; set; }
        public bool? IsPolylineClosed { get; set; }
    }

    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
