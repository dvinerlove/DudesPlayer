using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DudesPlayer.Desktop.Interfaces
{
    public interface INavControl
    {
        void SelectItem(Type pageType);
        event EventHandler<Type> ItemSelected;
    }
}
