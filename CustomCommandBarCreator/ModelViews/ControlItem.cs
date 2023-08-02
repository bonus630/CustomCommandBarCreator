using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomCommandBarCreator.ModelViews
{
    [Serializable]
    public abstract class ControlItem : BaseModelView
    {
        public string Guid { get; protected set; }
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public ControlItem()
        {
            this.Guid = System.Guid.NewGuid().ToString();
        }

    }
}
