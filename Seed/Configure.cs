using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Seed
{
    public class Configure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _measureStatus;

        public bool MeasureStatus
        {
            get
            {
                return _measureStatus;
            }
            set
            {
                _measureStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MeasureStatus"));
            }
        }
    }
}
