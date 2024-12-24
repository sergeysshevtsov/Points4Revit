using Autodesk.Revit.DB;
using Points4Revit.RVT.UI.WallsCreation;

namespace Points4Revit.RVT.Utils
{
    internal class WindowsHandler
    {
        private static WallsCreationWindow wallsCreationWindow;

        public static WallsCreationWindow CreateWCW(Document document)
        {
            if (null == wallsCreationWindow)
                return wallsCreationWindow = new WallsCreationWindow(document);
            else
            {
                wallsCreationWindow.Close();
                return DisposeWCW();
            }
        }

        public static WallsCreationWindow DisposeWCW() => wallsCreationWindow = null;

        public static Autodesk.Revit.UI.Result ShowWindow(System.Windows.Window window)
        {
            if (window != null)
            {
                new System.Windows.Interop.WindowInteropHelper(window)
                {
                    Owner = Autodesk.Windows.ComponentManager.ApplicationWindow
                };
                window.Show();
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
}
