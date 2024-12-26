using Autodesk.Revit.DB;

namespace Points4Revit.RVT.RevitService
{
    public class CreateModelLine
    {
        public static void Commit(Document document, Line line, ElementId elementId)
        {
            DetailCurve detailCurve = document.Create.NewDetailCurve(document.ActiveView, line);
            if (document.GetElement(elementId) is GraphicsStyle egs)
                detailCurve.LineStyle = egs;
        }
    }
}
