using Autodesk.Revit.DB;
using Points4Revit.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Points4Revit.RVT.UI.WallsCreation
{
    public class WallsCreationDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WallsCreationDataContext(Document document, Window window)
        {
            var levels = (from level in new FilteredElementCollector(document).OfClass(typeof(Level)).ToElements()
                          select new LevelData()
                          {
                              Id = level.Id,
                              Name = level.Name,
                              Elevation = ((Level)level).Elevation,
                              IsActive = true
                          })
                        .OrderBy(e => e.Elevation)
                        .ToList();

            if (levels.Count == 0)
                window.Close();

            TopLevels = BottomLevels = new List<LevelData>(levels);
            BottomLevel = BottomLevels[0];

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

            var lineTypes = (from lineType in new FilteredElementCollector(document).OfClass(typeof(GraphicsStyle)).ToElements()
               .Cast<GraphicsStyle>()
               .Where(gs => gs.GraphicsStyleCategory.CategoryType == CategoryType.Annotation ||
                            gs.GraphicsStyleCategory.CategoryType == CategoryType.Model)
                         select new ElementData()
                         {
                             Id = lineType.Id,
                             Name = lineType.Name,
                         })
              .ToList();

            if (lineTypes.Count == 0)
                LineDrawingIsAvailable = false;
            else
            {
                LineTypes = lineTypes.OrderBy(e => e.Name).ToList();
                LineType = LineTypes[0];
            }
        }

        private LevelData bottomLevel;
        public List<LevelData> BottomLevels { get; }
        public LevelData BottomLevel
        {
            get => bottomLevel;
            set
            {
                bottomLevel = value;
                //Remove levels below the selected bottom one from the list
                var selectedItem = TopLevels.Where(i => i.Id == bottomLevel.Id).FirstOrDefault();
                TopLevels = TopLevels.Select(a => { a.IsActive = true; return a; }).ToList();
                for (int i = 0; i < TopLevels.Count; i++)
                {
                    if (TopLevels[i] != selectedItem)
                        TopLevels[i].IsActive = false;
                    else
                    {
                        topLevel = TopLevels[i];
                        break;
                    }
                }


                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TopLevels"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TopLevel"));
            }
        }

        private LevelData topLevel;
        public List<LevelData> TopLevels { get; private set; }
        public LevelData TopLevel
        {
            get => topLevel;
            set
            {
                topLevel = value;

                BottomAndTopLevelsAreEqual = false;
                if (BottomLevel.Id.Equals(value.Id))
                    BottomAndTopLevelsAreEqual = true;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BottomAndTopLevelsAreEqual"));
            }
        }

        private double wallHeight = 13.12335958005249;
        public double WallHeight
        {
            get => wallHeight;
            set => wallHeight = value;
        }

        private ElementData wallType;
        public List<ElementData> WallTypes { get; set; }
        public ElementData WallType
        {
            get => wallType;
            set => wallType = value;
        }

        public int WallLocationLineIndex { get; set; } = 0;


        private bool drawOriginalModelLine = false;
        public bool DrawOriginalModelLine
        {
            get => drawOriginalModelLine;
            set
            {
                drawOriginalModelLine = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DrawOriginalModelLine"));
            }
        }

        private ElementData lineType;
        public List<ElementData> LineTypes { get; set; }
        public ElementData LineType
        {
            get => lineType;
            set => lineType = value;
        }

        public bool ChainWallMode { get; set; }

        private bool bottomAndTopLevelsAreEqual = true;
        public bool BottomAndTopLevelsAreEqual
        {
            get => bottomAndTopLevelsAreEqual;
            set =>
                bottomAndTopLevelsAreEqual = value;
        }

        private bool lineDrawingIsAvailable = true;
        public bool LineDrawingIsAvailable
        {
            get => lineDrawingIsAvailable;
            set => lineDrawingIsAvailable = value;
        }
    }
}
