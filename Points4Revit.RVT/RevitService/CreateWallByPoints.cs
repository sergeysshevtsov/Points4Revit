using Autodesk.Revit.DB;
using Points4Revit.Core;
using Points4Revit.RVT.UI.WallsCreation;
using System.Collections.Generic;

namespace Points4Revit.RVT.RevitService
{
    public class CreateWallByPoints
    {
        public static List<PointData> wallPointsCreationMode = new List<PointData>();
        public static List<ElementId> Commit(Document document, WallsCreationDataContext dc, ObjectData objectData)
        {
            App.EnqueueTask((uiapp) =>
            {
                using (Transaction tr = new Transaction(document, "Create wall by points"))
                {
                    tr.Start();

                    var objectType = objectData.ObjectType;
                    var points = objectData.PointData;

                    switch (objectType)
                    {
                        case Core.Enums.ObjectType.Point:
                            break;
                        case Core.Enums.ObjectType.Line:
                            {
                                var sp = new XYZ(points[0].X, points[0].Y, points[0].Z);
                                var ep = new XYZ(points[1].X, points[1].Y, points[1].Z);
                                CreateWallByPointsData(document, sp, ep, dc);
                                break;
                            }
                        case Core.Enums.ObjectType.Polyline:
                            {
                                for (var i = 0; i < points.Count - 1; i++)
                                {
                                    var sp = new XYZ(points[i].X, points[i].Y, points[i].Z);
                                    var ep = new XYZ(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
                                    CreateWallByPointsData(document, sp, ep, dc);

                                }

                                if ((bool)objectData.ObjectSettings)
                                {
                                    var sp = new XYZ(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);
                                    var ep = new XYZ(points[0].X, points[0].Y, points[0].Z);
                                    CreateWallByPointsData(document, sp, ep, dc);
                                }
                                break;
                            }
                        default:
                            break;
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
        private static Wall CreateWallByPointsData(Document document, XYZ startPoint, XYZ endPoint, WallsCreationDataContext dc) 
        {
            var line = Line.CreateBound(startPoint, endPoint);
            var wall = CreateWall(document, line, dc);
            if (dc.DrawOriginalModelLine)
                CreateModelLine.Commit(document, line, dc.LineType.Id);
            return wall;
        }
    }
}
