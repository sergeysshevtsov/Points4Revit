using Autodesk.Revit.DB;
using Points4Revit.Core;
using Points4Revit.RVT.UI.WallsCreation;
using System.Collections.Generic;
using System.Linq;

namespace Points4Revit.RVT.RevitService
{
    public class CreateWallByPoints
    {
        public static List<ElementId> Commit(Document document, WallsCreationDataContext dc, ObjectData objectData)
        {
            App.EnqueueTask((uiapp) =>
            {
                using (Transaction tr = new Transaction(document, "Create wall by points"))
                {
                    tr.Start();
                    var points = objectData.PointData;

                    

                    var transactionStatus = tr.Commit();
                   
                }
            });
            return null;
        }
    }
}
