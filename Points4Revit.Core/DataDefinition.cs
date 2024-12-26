using Autodesk.Revit.DB;
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

    public class ElementData
    {
        public string Name { get; set; }
        public ElementId Id { get; set; }
    }

    public class LevelData : ElementData
    {
        public double Elevation { get; set; }
        public bool IsActive { get; set; }
    }
}
