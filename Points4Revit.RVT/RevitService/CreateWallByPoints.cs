using Autodesk.Revit.DB;
using Points4Revit.Core;
using Points4Revit.RVT.UI.WallsCreation;
using System.Collections.Generic;

namespace Points4Revit.RVT.RevitService
{
    public class CreateWallByPoints
    {
        public static List<PointData> WallPointsCreationMode { get; set; } = new List<PointData>();
        public static List<ElementId> Commit(Document document, WallsCreationDataContext dc, ObjectData objectData)
        {
            App.EnqueueTask((uiapp) =>
            {
                using (Transaction tr = new Transaction(document, "Create wall by points"))
                {
                    tr.Start();

                    var objectType = objectData.ObjectType;
                    var points = objectData.PointData;
                    var elementIDList = new List<ElementId>();

                    switch (objectType)
                    {
                        case Core.Enums.ObjectType.Point:
                            WallPointsCreationMode.Add(points[0]);
                            if (WallPointsCreationMode.Count == 2)
                            {
                                var ep = WallPointsCreationMode[1];
                                CreateWallByPointsData(document, WallPointsCreationMode[0], WallPointsCreationMode[1], dc, ref elementIDList);
                                WallPointsCreationMode.Clear();
                                if (dc.ChainWallMode)
                                    WallPointsCreationMode.Add(ep);
                            }
                            break;
                        case Core.Enums.ObjectType.Line:
                            WallPointsCreationMode.Clear();
                            CreateWallByPointsData(document, points[0], points[1], dc, ref elementIDList);
                            break;
                        case Core.Enums.ObjectType.Polyline:
                            WallPointsCreationMode.Clear();
                            for (var i = 0; i < points.Count - 1; i++)
                                CreateWallByPointsData(document, points[i], points[i + 1], dc, ref elementIDList);

                            if ((bool)objectData.ObjectSettings)
                                CreateWallByPointsData(document, points[points.Count - 1], points[0], dc, ref elementIDList);
                            break;
                    }

                    if (dc.ZoomToCreatedWalls)
                    {
                        uiapp.ActiveUIDocument.ShowElements(elementIDList);
                        uiapp.ActiveUIDocument.Selection.SetElementIds(elementIDList);
                    }

                    var transactionStatus = tr.Commit();
                }
            });
            return null;
        }

        public static Wall CreateWall(Document document, Line line, WallsCreationDataContext dc)
        {
            var wall = Wall.Create(document, line, dc.BottomLevel.Id, false);

            Parameter locationLine = wall.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM);
            locationLine.Set(0);

            int wallLocationIndex = dc.WallLocationLineIndex;
            int wallDirection = 0;

            if (0 == wallLocationIndex)
            {
                wallDirection = 1;
                locationLine.Set(3);
            }

            if (2 == wallLocationIndex)
            {
                wallDirection = -1;
                locationLine.Set(2);
            }

            wall.WallType = document.GetElement(dc.WallType.Id) as WallType;
            if (0 != wallDirection)
            {
                XYZ normal = XYZ.BasisZ.CrossProduct(line.Direction).Normalize();
                XYZ transform = normal.Multiply((wallDirection) * (wall.WallType.Width / 2));
                (wall.Location as LocationCurve).Move(transform);
            }

            var bottomLevelId = dc.BottomLevel.Id;
            var topLevelId = dc.TopLevel.Id;

            if (bottomLevelId != topLevelId)
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(topLevelId);
            else
            {
                Parameter height = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                height.Set(dc.WallHeight);
            }

            return wall;
        }
        private static Wall CreateWallByPointsData(Document document, PointData startPoint, PointData endPoint, WallsCreationDataContext dc, ref List<ElementId> elementIDList)
        {
            var sp = new XYZ(startPoint.X, startPoint.Y, startPoint.Z);
            var ep = new XYZ(endPoint.X, endPoint.Y, endPoint.Z);
            var line = Line.CreateBound(sp, ep);
            var wall = CreateWall(document, line, dc);
            if (dc.DrawOriginalModelLine)
                CreateModelLine.Commit(document, line, dc.LineType.Id);
            elementIDList.Add(wall.Id);
            return wall;
        }
    }
}
