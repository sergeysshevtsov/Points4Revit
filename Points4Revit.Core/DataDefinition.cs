using Points4Revit.Core.Enums;
using System.Collections.Generic;

namespace Points4Revit.Core
{
    public class ObjectData
    {
        public ObjectType ObjectType { get; set; }
        public object ObjectSettings { get; set; } = null;
        public List<PointData> PointData { get; set; }
    }

    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
