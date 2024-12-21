using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Points4Revit.RVT.AppCommands
{
    public class CmdWallsCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }
    }
}
