using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using Points4Revit.Core;
using Points4Revit.Core.Enums;
using System.Collections.Generic;
using System.IO;

namespace Points4Revit.ACD
{
    public class App
    {
        private readonly string pathToTmpFile = Path.Combine(Path.GetTempPath(), "p4r");

        [CommandMethod("point4revit", CommandFlags.UsePickSet | CommandFlags.Redraw | CommandFlags.Modal)]
        public void Points2Revit()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;

            PromptPointResult promptPointResult;
            PromptPointOptions promptPointOptions = new PromptPointOptions("");

            do
            {
                promptPointOptions.Message = "\nPick point (Press ESC for Cancel): ";
                promptPointResult = editor.GetPoint(promptPointOptions);
                Point3d point = promptPointResult.Value;

                if (promptPointResult.Status != PromptStatus.Cancel)
                {
                    var objectData = new ObjectData()
                    {
                        ObjectType = ObjectType.Point,
                        PointData = new List<PointData>()
                        {
                            new PointData() { X = point.X, Y = point.Y, Z = point.Z }
                        }
                    };

                    editor.WriteMessage($"Point data transmitted!");
                    File.WriteAllText(Path.Combine(Path.GetTempPath(), "p4r"), JsonConvert.SerializeObject(objectData));
                }
            }
            while (promptPointResult.Status != PromptStatus.Cancel);
        }

        [CommandMethod("line4revit", CommandFlags.UsePickSet | CommandFlags.Redraw | CommandFlags.Modal)]
        public void PickFirstTest()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;

            PromptSelectionResult promptSelectionResult;
            PromptSelectionOptions selectionOpts = new PromptSelectionOptions
            {
                MessageForAdding = "\nSelect line or polyline: ",
                SingleOnly = true
            };

            do
            {
                promptSelectionResult = editor.SelectImplied();
                if (promptSelectionResult.Status == PromptStatus.Error)
                    promptSelectionResult = editor.GetSelection(selectionOpts);
                else
                    editor.SetImpliedSelection(new ObjectId[0]);

                if (promptSelectionResult.Status == PromptStatus.OK)
                {
                    Transaction tr = document.TransactionManager.StartTransaction();
                    try
                    {
                        ObjectId[] objectIds = promptSelectionResult.Value.GetObjectIds();
                        foreach (ObjectId objectId in objectIds)
                        {
                            Entity ent = (Entity)tr.GetObject(objectId, OpenMode.ForRead);
                            var type = ent.GetType();

                            if (type == typeof(Line))
                            {
                                var line = ent as Line;
                                var sp = line.StartPoint;
                                var ep = line.EndPoint;

                                var objectData = new ObjectData()
                                {
                                    ObjectType = ObjectType.Line,
                                    PointData = new List<PointData>()
                                    {
                                        new PointData() { X = sp.X, Y = sp.Y, Z = sp.Z },
                                        new PointData() { X = ep.X, Y = ep.Y, Z = ep.Z }
                                    }
                                };

                                editor.WriteMessage($"Line data ({line.Id}) transmitted!");
                                File.WriteAllText(pathToTmpFile, JsonConvert.SerializeObject(objectData));
                            }

                            if (type == typeof(Polyline))
                            {
                                var pline = ent as Polyline;
                                var verts = pline.NumberOfVertices;
                                var points = new List<PointData>();
                                for (int i = 0; i < verts; i++)
                                {
                                    var pt = pline.GetPoint3dAt(i);
                                    points.Add(new PointData() { X = pt.X, Y = pt.Y, Z = pt.Z });
                                }

                                var objectData = new ObjectData()
                                {
                                    ObjectType = ObjectType.Polyline,
                                    PointData = points
                                };

                                editor.WriteMessage($"Polyline data ({pline.Id}) transmitted!");
                                File.WriteAllText(pathToTmpFile, JsonConvert.SerializeObject(objectData));
                            }
                            ent.Dispose();
                        }
                        tr.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        editor.WriteMessage(ex.Message);
                        tr.Abort();
                    }
                }
            }
            while (promptSelectionResult.Status != PromptStatus.Cancel);
        }
    }
}
