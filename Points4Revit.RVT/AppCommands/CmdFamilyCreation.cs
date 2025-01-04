using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Points4Revit.RVT.UI.FamilyCreation;

namespace Points4Revit.RVT.AppCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdFamilyCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var familyCreation = new FamilyCreationWindow(commandData.Application.ActiveUIDocument.Document);
            new System.Windows.Interop.WindowInteropHelper(familyCreation)
            {
                Owner = Autodesk.Windows.ComponentManager.ApplicationWindow
            };
            familyCreation.ShowDialog();
            return Result.Succeeded;
        }
    }
}
