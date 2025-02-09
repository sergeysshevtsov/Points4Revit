using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Points4Revit.Core;
using Points4Revit.RVT.UI.WallThickness;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Points4Revit.RVT.RevitService
{
    public class CreateWallThicknessByPoints
    {
        private static List<PointData> wallThicknessCreationPoints = new List<PointData>();
        public static List<ElementId> Commit(UIApplication app, WallThicknessCreationDataContext dc, ObjectData objectData)
        {
            var document = app.ActiveUIDocument.Document;
            var elementId = ElementId.InvalidElementId;

            using (Transaction tr = new Transaction(document, "Create wall by points"))
            {
                tr.Start();

                var objectType = objectData.ObjectType;
                var points = objectData.PointData;
                var elementIDList = new List<ElementId>();

                switch (objectType)
                {
                    case Core.Enums.ObjectType.Point:
                        wallThicknessCreationPoints.Add(points[0]);
                        break;
                }

                WallType newWallType = null;
                if (wallThicknessCreationPoints.Count == 3)
                    try
                    {
                        var selectedWallType = document.GetElement(dc.WallType.Id) as WallType;

                        var sp = new XYZ(wallThicknessCreationPoints[0].X, wallThicknessCreationPoints[0].Y, wallThicknessCreationPoints[0].Z);
                        var ep = new XYZ(wallThicknessCreationPoints[1].X, wallThicknessCreationPoints[1].Y, wallThicknessCreationPoints[1].Z);
                        var p = new XYZ(wallThicknessCreationPoints[2].X, wallThicknessCreationPoints[2].Y, wallThicknessCreationPoints[2].Z);
                        var line = Line.CreateBound(sp, ep);
                        XYZ lineDirection = line.Direction;
                        XYZ vectorToPoint = p - line.GetEndPoint(0);
                        double t = vectorToPoint.DotProduct(lineDirection) / lineDirection.DotProduct(lineDirection);
                        XYZ closestPointOnLine = line.GetEndPoint(0) + t * lineDirection;
                        double distance = p.DistanceTo(closestPointOnLine);

                        string wtpName = dc.WallType.Name;
                        double value = Math.Round(distance, 2);

                        var newWallTypeName = wtpName + "_" + value;
                        if (dc.WallTypes.FirstOrDefault(wt => wt.Name.Equals(newWallTypeName)) is ElementData elementData)
                            elementId = elementData.Id;

                        newWallType = selectedWallType.Duplicate(newWallTypeName) as WallType;
                        CompoundStructure cs = newWallType.GetCompoundStructure();
                        double layerWidth = value;

                        int layerIndex = cs.GetFirstCoreLayerIndex();
                        IList<CompoundStructureLayer> cslayers = cs.GetLayers();
                        if (cslayers.Count == 1)
                            cs.SetLayerWidth(layerIndex, layerWidth);
                        else
                        {
                            int j = 0;
                            double additionalWidth = 0;
                            foreach (CompoundStructureLayer csl in cslayers)
                            {
                                if (j != layerIndex)
                                    additionalWidth += csl.Width;
                                j++;
                            }
                            cs.SetLayerWidth(layerIndex, layerWidth - additionalWidth);
                        }

                        newWallType.SetCompoundStructure(cs);
                        wallThicknessCreationPoints = new List<PointData>();
                        elementId = newWallType.Id;
                    }
                    catch { }

                if (!elementId.Equals(ElementId.InvalidElementId))
                {
                    var wallTypes = (from wallType in new FilteredElementCollector(document).OfClass(typeof(WallType)).ToElements()
                                     select new ElementData()
                                     {
                                         Id = wallType.Id,
                                         Name = wallType.Name,
                                     }).ToList();

                    dc.WallTypes = wallTypes.OrderBy(e => e.Name).ToList();
                    dc.WallType = wallTypes.First(w => w.Id.Equals(elementId));
                }

                if (dc.ApplyNewWallType && dc.NumberOfSelectedWalls != 0)
                    foreach (var wall in dc.Walls)
                        wall.WallType = newWallType;

                var transactionStatus = tr.Commit();
            }

            return null;
        }
    }
}
