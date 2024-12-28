using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Points4Revit.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Points4Revit.RVT.UI.WallThickness
{
    public class WallThicknessCreationDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public WallThicknessCreationDataContext(UIApplication application, Window window)
        {
            var document = application.ActiveUIDocument.Document;
            foreach (ElementId elementId in application.ActiveUIDocument.Selection.GetElementIds())
            {
                Element element = document.GetElement(elementId);
                if (element is Wall wall)
                    Walls.Add(wall);
            }

            NumberOfSelectedWalls = Walls.Count;

            var wallTypes = (from wallType in new FilteredElementCollector(document).OfClass(typeof(WallType)).ToElements()
                             select new ElementData()
                             {
                                 Id = wallType.Id,
                                 Name = wallType.Name,
                             })
            .ToList();

            if (wallTypes.Count == 0)
                window.Close();

            WallTypes = wallTypes.OrderBy(e => e.Name).ToList();
            WallType = WallTypes[0];

            SelectionExists = true;
            if (NumberOfSelectedWalls == 0)
                SelectionExists = false;
        }

        private List<ElementData> wallTypes;
        public List<ElementData> WallTypes
        {
            get => wallTypes;
            set
            {
                wallTypes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WallTypes"));
            }
        }

        private ElementData wallType;
        public ElementData WallType
        {
            get => wallType;
            set
            {
                wallType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WallType"));
            }
        }

        public List<Wall> Walls { get; set; } = new List<Wall>();
        public int NumberOfSelectedWalls { get; set; } = 0;
        public bool SelectionExists { get; set; }

        private bool applyNewWallType = true;
        public bool ApplyNewWallType
        {
            get => applyNewWallType;
            set
            {
                applyNewWallType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApplyNewWallType"));
            }
        }
    }
}
