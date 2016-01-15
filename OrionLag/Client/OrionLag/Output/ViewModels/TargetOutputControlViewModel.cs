using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Output.ViewModels
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;
    using OrionLag.Input.ViewModel;
    using OrionLag.WpfBase;

    public class TargetOutputControlViewModel : TargetWindowBase
    {
        private string m_inputPath;
      

        private IOppropDataService m_InputDataService;
        public TargetOutputControlViewModel(IOppropDataService indataService)
        {
            m_InputDataService = indataService;
            ReadAllLag();
        }

        private void ReadAllLag()
        {
            var listSkiver = new List<SkiverViewModel>();
            var alllLag = m_InputDataService.FetchAllLag();
            foreach (var etLag in alllLag)
            {
                foreach (var skive in etLag.SkiverILaget)
                {
                    var newSKive = new SkiverViewModel(etLag.LagNummer, skive);
                    listSkiver.Add(newSKive);
                }
            }
          
            var sorted = listSkiver.OrderBy(x => x.LagNummer).ThenBy(y => y.SkiveNummer).ToList();

            AlleLagAlleSkiver = null;
            AlleLagAlleSkiver = new ObservableCollection<SkiverViewModel>(sorted);
        }

        private int m_holdId;
        public int HoldId
        {
            get { return m_holdId; }
            set { SetProperty(ref m_holdId, value, () => HoldId); }
        }

        private SkiverViewModel m_selectedAlleLagAlleSkiver;
        public SkiverViewModel SelectedAlleLagAlleSkiver
        {
            get { return m_selectedAlleLagAlleSkiver; }

            set { SetProperty(ref m_selectedAlleLagAlleSkiver, value, () => SelectedAlleLagAlleSkiver); }
        }

        private ObservableCollection<SkiverViewModel> m_alleLagAlleSkiver;
        public ObservableCollection<SkiverViewModel> AlleLagAlleSkiver
        {
            get { return m_alleLagAlleSkiver; }
           
            set { SetProperty(ref m_alleLagAlleSkiver, value, () => AlleLagAlleSkiver); }
        }
    }
}
