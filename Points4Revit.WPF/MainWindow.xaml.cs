using Newtonsoft.Json;
using Points4Revit.Core;
using Points4Revit.Core.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Points4Revit.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowDataContext();
        }

        private void DoubleOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^-?\d*(\.\d*)?$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as MainWindowDataContext;
            var objectData = new ObjectData()
            {
                ObjectType = ObjectType.Point,
                PointData = new List<PointData>()
                    {
                        new PointData() { X = dc.X, Y = dc.Y, Z = dc.Z }
                    }
            };

            File.WriteAllText(Common.pathToTmpFile, JsonConvert.SerializeObject(objectData));
        }
    }
}
