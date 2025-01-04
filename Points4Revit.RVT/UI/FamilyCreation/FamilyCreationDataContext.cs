using Autodesk.Revit.DB;
using Points4Revit.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace Points4Revit.RVT.UI.FamilyCreation
{
    internal class FamilyCreationDataContext : INotifyPropertyChanged
    {
        private readonly Document document;
        public FamilyCreationDataContext(Document document)
        {
            this.document = document;

            var categories = new List<CategoryData>();
            foreach (Category category in document.Settings.Categories)
                if (category.IsTagCategory == false && category.CategoryType == CategoryType.Model)
                    categories.Add(new CategoryData() { Id = category.Id, Name = category.Name, Category = category });

            Categories = categories.OrderBy(c => c.Name).ToList();
            Category = Categories.First();
            
        }

        private List<CategoryData> categories;
        public List<CategoryData> Categories
        {
            get => categories;
            set
            {
                categories = value;
            }
        }

        private CategoryData category;
        public CategoryData Category
        {
            get => category;
            set
            {
                category = value;
                var familiesInCategory = new List<FamilyData>();
                foreach (FamilySymbol family in new FilteredElementCollector(document).OfClass(typeof(FamilySymbol)).WhereElementIsElementType())
                {
                    var c = family.Category;
                    var n = family.Name;
                    if (family.Category != null && family.Category.Id == Category.Id)
                        familiesInCategory.Add(new FamilyData() { Id = family.Id, Name = family.Name, FamilySymbol = family, FamilyType = family.FamilyName });
                }
                Families = familiesInCategory;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Families"));

                if (familiesInCategory.Count > 0)
                {
                    Family = familiesInCategory.First();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Family"));
                }
            }
        }

        private List<FamilyData> families;
        public List<FamilyData> Families
        {
            get => families;
            set
            {
                families = value;
            }
        }

        private FamilyData family;
       

        public FamilyData Family
        {
            get => family;
            set
            {
                family = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Family"));
            }
        }

        public Document Document => document;

        public event PropertyChangedEventHandler PropertyChanged;
       
    }
}
