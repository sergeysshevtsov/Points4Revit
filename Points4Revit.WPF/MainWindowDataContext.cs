using System.ComponentModel;

namespace Points4Revit.WPF
{
    internal class MainWindowDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double x = 0;
        public double X
        {
            get => x;
            set => x = value;
        }

        private double y = 0;
        public double Y
        {
            get => y;
            set => y = value;
        }

        private double z = 0;
        public double Z
        {
            get => z;
            set => z = value;
        }
    }
}
