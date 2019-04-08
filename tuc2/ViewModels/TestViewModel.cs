using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace tuc2.ViewModels
{
    public class TestViewModel : INotifyPropertyChanged
    {
        private bool isSelected;
        private string inputData;
        private string outputData;

        public int Id { get; set; }
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value) return;
                isSelected = value;
                OnPropertyChanged();
            }
        }
        public string InputData
        {
            get { return inputData; }
            set
            {
                if (inputData == value) return;
                inputData = value;
                OnPropertyChanged();
            }
        }
        public string OutputData
        {
            get { return outputData; }
            set
            {
                if (outputData == value) return;
                outputData = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
