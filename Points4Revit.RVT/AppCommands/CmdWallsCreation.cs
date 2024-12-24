using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Points4Revit.RVT.AppCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdWallsCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Utils.WindowsHandler.ShowWindow(Utils.WindowsHandler.CreateWCW(commandData.Application.ActiveUIDocument.Document));
            return Result.Succeeded;
        }
    }
}
