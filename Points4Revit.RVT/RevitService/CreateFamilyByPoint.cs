using Autodesk.Revit.DB;
using Points4Revit.Core;
using Points4Revit.RVT.UI.FamilyCreation;
using System.Linq;

namespace Points4Revit.RVT.RevitService
{
    internal class CreateFamilyByPoint
    {
        public static void Commit(Document document, FamilyCreationDataContext dc, ObjectData objectData)
        {
            var objectType = objectData.ObjectType;
            var points = objectData.PointData;

            if (objectData.ObjectType == Core.Enums.ObjectType.Point)
            {
                var p = points.First();
                var locationPoint = new XYZ(p.X, p.Y, p.Z);

                if (dc.Family.FamilySymbol == null)
                    return;
                if (document.ActiveView.GenLevel == null)
                    return;
                try
                {
                    using (Transaction tr = new Transaction(document, "Create selected family symbol"))
                    {
                        tr.Start();
                        var symbol = dc.Family.FamilySymbol;
                        symbol.Activate();
                        FamilyInstance familyInstance = document.Create.NewFamilyInstance(locationPoint, dc.Family.FamilySymbol, null, document.ActiveView.GenLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        tr.Commit();
                    }
                }
                catch { }
            }
        }
    }
}
