using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using OrionLag.Annotations;
    using OrionLag.Utils;

    public class LagViewModel : INotifyPropertyChanged
    {
        private Lag m_lag ;

        public LagViewModel(Lag lag)
        {
            m_lag = lag;
        }





        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
