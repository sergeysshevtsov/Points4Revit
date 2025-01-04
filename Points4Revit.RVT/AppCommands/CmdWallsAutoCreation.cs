using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Reflection;

namespace Points4Revit.RVT.AppCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdWallsAutoCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            List<Tuple<XYZ, XYZ>> lines = new List<Tuple<XYZ, XYZ>>();
            FilteredElementCollector importInstances = new FilteredElementCollector(document).OfClass(typeof(ImportInstance));
            foreach (ImportInstance importInstance in importInstances)
            {
                Options options = new Options
                {
                    DetailLevel = ViewDetailLevel.Medium // Adjust as needed
                };
                GeometryElement geomElement = importInstance.get_Geometry(options);

                foreach (GeometryObject geometryObject in geomElement)
                {

                    if (geometryObject is GeometryInstance geomInstance)
                    {
                        GeometryElement instanceGeometry = geomInstance.GetInstanceGeometry();
                        foreach (GeometryObject geometryObjectFromInstance in instanceGeometry)
                        {
                            GraphicsStyle gStyle = document.GetElement(geometryObjectFromInstance.GraphicsStyleId) as GraphicsStyle;
                            var n = gStyle.GraphicsStyleCategory.Name;
                            if (geometryObjectFromInstance is Line line_)
                            {
                                // Check if the line is part of the target layer
                                //if (importOptions.GetLayerVisibility(targetLayer))
                                {
                                    // If the line belongs to the target layer, display its details
                                    //TaskDialog.Show("Line from Layer", "Line found in Layer: " + targetLayer +
                                    //" Start: " + line_.GetEndPoint(0) + " End: " + line_.GetEndPoint(1));
                                    lines.Add(new Tuple<XYZ, XYZ>(new XYZ(line_.GetEndPoint(0).X, line_.GetEndPoint(0).Y, 0), new XYZ(line_.GetEndPoint(1).X, line_.GetEndPoint(1).Y, 0)));
                                }
                            }
                            if (geometryObjectFromInstance is PolyLine pline_)
                            {
                                // Check if the line is part of the target layer
                                //if (importOptions.GetLayerVisibility(targetLayer))
                                {
                                    // If the line belongs to the target layer, display its details
                                    //TaskDialog.Show("Line from Layer", "Line found in Layer: " + targetLayer +
                                    //" Start: " + line_.GetEndPoint(0) + " End: " + line_.GetEndPoint(1));
                                    var coordinates = pline_.GetCoordinates();

                                    for (int i = 0; i < coordinates.Count - 1; i++)
                                    {

                                        lines.Add(new Tuple<XYZ, XYZ>(coordinates[i], coordinates[i + 1]));
                                    }

                                }
                            }


                        }
                    }
                    if (geometryObject is Line line)
                    {
                        // Check if the line is part of the target layer
                        //if (importOptions.GetLayerVisibility(targetLayer))
                        {
                            // If the line belongs to the target layer, display its details
                            lines.Add(new Tuple<XYZ, XYZ>(new XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, 0), new XYZ(line.GetEndPoint(1).X, line.GetEndPoint(1).Y, 0)));
                        }
                    }
                }
            }

            List<Tuple<XYZ, XYZ>> centerLines = new List<Tuple<XYZ, XYZ>>();
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    Line line1 = Line.CreateBound(lines[i].Item1, lines[i].Item2);
                    Line line2 = Line.CreateBound(lines[j].Item1, lines[j].Item2);

                    if (!AreLinesParallel(line1, line2))
                    {
                        continue;
                    }

                    if (line1 != null && line2 != null)
                    {
                        // Check if lines are parallel

                        // Calculate the centerline midpoint between the two lines
                        XYZ midpoint = GetCenterlineMidpoint(line1, line2);



                        centerLines.Add(new Tuple<XYZ, XYZ>(line1.GetEndPoint(0), midpoint));


                    }
                }
            }
            RevitService.CreateModelLine.CommitMultipleByPoints(document, lines);
            RevitService.CreateModelLine.CommitMultipleByPoints(document, centerLines);
            return Result.Succeeded;
        }

        private XYZ GetCenterlineMidpoint(Line line1, Line line2)
        {
            // Calculate the midpoint of the two parallel lines
            XYZ start1 = line1.GetEndPoint(0);
            XYZ end1 = line1.GetEndPoint(1);

            XYZ start2 = line2.GetEndPoint(0);
            XYZ end2 = line2.GetEndPoint(1);

            // Get the middle points of each line
            XYZ mid1 = (start1 + end1) / 2;
            XYZ mid2 = (start2 + end2) / 2;

            // The centerline midpoint is the midpoint between the two middle points
            return (mid1 + mid2) / 2;
        }

        private bool AreLinesParallel(Line line1, Line line2)
        {
            // Get the direction vectors of both lines
            XYZ direction1 = line1.Direction;
            XYZ direction2 = line2.Direction;

            double dotProduct = direction1.DotProduct(direction2);

            const double tolerance = 1e-6; // A small tolerance value for comparison
            if (Math.Abs(dotProduct - 1) < tolerance || Math.Abs(dotProduct + 1) < tolerance)
            {
                XYZ point1 = line1.GetEndPoint(0); // Any point on line1
                XYZ point2 = line2.GetEndPoint(0); // Any point on line2

                // Calculate the perpendicular distance using the cross product formula
                XYZ vectorBetweenLines = point2 - point1;
                XYZ crossProduct = vectorBetweenLines.CrossProduct(direction1);
                double distance = crossProduct.GetLength() / direction1.GetLength();

                // Convert the threshold to Revit's internal units (1 mm = 0.00328084 feet)
                const double thresholdDistanceInFeet = 50 * 0.00328084; // 50 mm in feet

                // Check if the distance is greater than 50 mm (thresholdDistanceInFeet)
                if (distance > thresholdDistanceInFeet)
                {
                    return true; // Lines are parallel and distance is more than 50mm
                }
            }

            return false;
        }
    }
}
