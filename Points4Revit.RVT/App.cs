using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Points4Revit.RVT
{
    public class App : IExternalApplication
    {
        private static Queue<Action<UIApplication>> _tasks;

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
        public Result OnStartup(UIControlledApplication application)
        {
            if (!AddMenu(application, string.Empty))
                return Result.Failed;

            _tasks = new Queue<Action<UIApplication>>();
            application.Idling += OnIdling;
            return Result.Succeeded;
        }

        //Idling process requires for running wall creation window modeless
        //this process prevents to run command if other process is running
        //allows to access to the document from outside app as modeless window
        private void OnIdling(object sender, IdlingEventArgs e)
        {
            var app = (UIApplication)sender;
            lock (_tasks)
            {
                if (_tasks.Count == 0)
                    return;

                if (_tasks.Count > 0)
                {
                    Action<UIApplication> task = _tasks.Dequeue();
                    task(app);
                }
            }
        }

        public static void EnqueueTask(Action<UIApplication> task)
        {
            lock (_tasks)
                _tasks.Enqueue(task);
        }

        private bool AddMenu(UIControlledApplication application, string tabname)
        {
            try
            {
                var assemblyPath = Assembly.GetExecutingAssembly().Location;
                var resourceString = "pack://application:,,,/Points4Revit.RVT;component/Resources/Images/Ribbon/";
                //Create Points4Revit ribbon
                //if tabname is empty use default Add-In tab
                var rb = GetRibbonPanel(application, "Points4Revit", tabname);
                //Add ribbon button wall creation
                rb.AddItem(
                    new PushButtonData("cmdWallsCreation", "Walls\nCreation", assemblyPath, "Points4Revit.RVT.AppCommands.CmdWallsCreation")
                    {
                        ToolTip = "Create walls using external points",
                        LargeImage = new BitmapImage(new Uri(string.Concat(resourceString, "32x32/WallsCreation.png")))
                    });
                //Add ribbon button wall thickness creation
                rb.AddItem(
                    new PushButtonData("cmdWallThickness", "Wall\nThickness", assemblyPath, "Points4Revit.RVT.AppCommands.CmdCreateWallThicknessCreation")
                    {
                        ToolTip = "Change selected wall thickness",
                        LargeImage = new BitmapImage(new Uri(string.Concat(resourceString, "32x32/WallThickness.png")))
                    });
                //Add ribbon button family creation
                rb.AddItem(
                    new PushButtonData("cmdFamilyCreation", "Family\nCreation", assemblyPath, "Points4Revit.RVT.AppCommands.CmdFamilyCreation")
                    {
                        ToolTip = "Create family instance using external point",
                        LargeImage = new BitmapImage(new Uri(string.Concat(resourceString, "32x32/FamilyCreation.png")))
                    });
            }
            catch
            {
                return false;
            }

            return true;
        }

        //get ribbon panel by ribbon and tab name
        private RibbonPanel GetRibbonPanel(UIControlledApplication application, string ribbonName, string tabName = null)
        {
            IList<RibbonPanel> ribbonPanels;
            if (!string.IsNullOrEmpty(tabName))
            {
                try
                {
                    application.CreateRibbonTab(tabName);
                }
                catch { }
                ribbonPanels = application.GetRibbonPanels(tabName);
            }
            else
                ribbonPanels = application.GetRibbonPanels();

            RibbonPanel ribbonPanel = null;
            foreach (RibbonPanel rp in ribbonPanels)
            {
                if (rp.Name.Equals(ribbonName))
                {
                    ribbonPanel = rp;
                    break;
                }
            }

            if (ribbonPanel == null)
                ribbonPanel = (string.IsNullOrEmpty(tabName)) ?
                    application.CreateRibbonPanel(ribbonName) :
                    application.CreateRibbonPanel(tabName, ribbonName);

            return ribbonPanel;
        }
    }
}
