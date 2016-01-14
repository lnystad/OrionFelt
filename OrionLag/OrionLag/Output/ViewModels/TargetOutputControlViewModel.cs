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

    using OrionLag.Input.ViewModel;
    using OrionLag.Utils;
    using OrionLag.WpfBase;

    public class TargetOutputControlViewModel : TargetWindowBase
    {
        private string m_inputPath;
        XmlSerializer m_lagser = new XmlSerializer(typeof(Lag));
        public TargetOutputControlViewModel()
        {
            m_inputPath = @"C:\Orion2\Data\Felt\test";
            ReadAllLag();
        }

        private void ReadAllLag()
        {
            var listSkiver = new List<SkiverViewModel>();
            var filesInDir = Directory.GetFiles(m_inputPath, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in filesInDir)
            {
                using (var fileReader = new System.IO.StreamReader(file, new UTF8Encoding()))
                {
                    var funnetLag = m_lagser.Deserialize(fileReader) as Lag;

                    if (funnetLag != null)
                    {
                        foreach (var skive in funnetLag.SkiverILaget)
                        {
                            var newSKive = new SkiverViewModel(funnetLag.LagNummer, skive);
                            listSkiver.Add(newSKive);
                        }
                    }
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
