using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Points4Revit.RVT
{
    public class App : IExternalApplication
    {
        private static Queue<Action<UIApplication>> _tasks;

        public App()
        {
           
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            if (!AddMenu(application, string.Empty))
                return Result.Failed;

            _tasks = new Queue<Action<UIApplication>>();
            application.Idling += OnIdling;
            return Result.Succeeded;
        }

        private void OnIdling(object sender, IdlingEventArgs e)
        {
            var app = (UIApplication)sender;

            lock (_tasks)
            {
                if (_tasks.Count == 0)
                {
                    return;
                }

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

                var rb = GetRibbonPanel(application, "Points4Revit", tabname);
                rb.AddItem(
                    new PushButtonData("cmdWallsCreation", "Walls\nCreation", assemblyPath, "Points4Revit.RVT.AppCommands.CmdWallsCreation")
                    {
                        ToolTip = "Create walls using external points",
                        LargeImage = new BitmapImage(new Uri(string.Concat(resourceString, "32x32/WallsCreation.png")))
                    });

                //
                //TODO: button for wall thickness calculation
                //
                //rb.AddItem(
                //    new PushButtonData("cmdWallThickness", "Wall\nThickness", assemblyPath, "Points4Revit.RVT.AppCommands.CmdWallThickness")
                //    {
                //        ToolTip = "Change selecrted wall thickness",
                //        LargeImage = new BitmapImage(new Uri(string.Concat(resourceString, "32x32/WallThickness.png")))
                //    });
            }
            catch
            {
                return false;
            }

            return true;
        }

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
