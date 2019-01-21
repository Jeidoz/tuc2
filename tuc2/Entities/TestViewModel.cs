using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tuc2.Entities
{
    public class TestViewModel : INotifyPropertyChanged
    {
        private bool isSelected;
        private string inputData;
        private string outputData;

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
