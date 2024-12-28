using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Points4Revit.RVT.UI.WallThickness;

namespace Points4Revit.RVT.AppCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class CmdCreateWallThicknessCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var wallThicknessCreation = new WallThicknessCreationWindow(commandData.Application);
            new System.Windows.Interop.WindowInteropHelper(wallThicknessCreation)
            {
                Owner = Autodesk.Windows.ComponentManager.ApplicationWindow
            };
            wallThicknessCreation.ShowDialog();
            return Result.Succeeded;
        }
    }
}
