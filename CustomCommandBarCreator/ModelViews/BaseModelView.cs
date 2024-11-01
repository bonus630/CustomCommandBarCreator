using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommandBarCreator.ModelViews
{


    public class BaseModelView : INotifyPropertyChanged
    {
        private bool dirty = false;

        public bool Dirty
        {
            get { return dirty; }
            set { 
                dirty = value; 
                OnPropertyChanged();
                DirtyChanged?.Invoke(value);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<bool> DirtyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != "Dirty")
                Dirty = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
