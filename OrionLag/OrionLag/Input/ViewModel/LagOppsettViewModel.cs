using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;
    using System.Xml.Serialization;

    using OrionLag.Input.Data;
    using OrionLag.Utils;

    public class LagOppsettViewModel : INotifyPropertyChanged
    {
        private List<Lag> lagOppsett;
        public LagOppsettViewModel(List<Lag> lagOppsett,int minutes,DateTime startTime)
        {
            lagOppsett = lagOppsett;
            m_inputRows = new ObservableCollection<Lag>(lagOppsett);
            m_skiver = new ObservableCollection<SkiverViewModel>();
            this.LagKilde = new ObservableCollection<Lag>();
            LagStart = startTime.ToString("HH:mm");
            LagDuration = minutes.ToString();
            var sortedlagOppsett= lagOppsett.OrderBy(o => o.LagNummer).ToList();
            foreach (var lag in sortedlagOppsett)
            {
                this.LagKilde.Add(lag);
                var sortedSkiver = lag.SkiverILaget.OrderBy(x => x.SkiveNummer);
                foreach (var skive in sortedSkiver)
                {
                    m_skiver.Add(item: new SkiverViewModel(lag.LagNummer, skive));
                }
            }
        }

        private void SortGrid()
        {
            var input = m_inputRows.OrderBy(o => o.LagNummer);
            m_inputRows = new ObservableCollection<Lag>(input.ToList());

            var sortskiver = m_skiver.OrderBy(x => x.LagNummer).ThenBy(y => y.SkiveNummer);
            m_skiver = new ObservableCollection<SkiverViewModel>(sortskiver.ToList());
            NotifyPropertyChanged("Skiver");
            NotifyPropertyChanged("InputRows");
        }


        private string m_lagDuration;
        public string LagDuration
        {
            get { return m_lagDuration; }
            set
            {
                m_lagDuration = value;
                NotifyPropertyChanged("LagDuration");
            }
        }

        private string m_lagStart;
        public string LagStart
        {
            get { return m_lagStart; }
            set
            {
                m_lagStart = value;
                NotifyPropertyChanged("LagStart");
            }
        }

        private string m_filePath;
        public string  FilePath
        {
            get { return m_filePath; }
            set
            {
                m_filePath = value;
                NotifyPropertyChanged("FilePath");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private Lag m_selectedLag2;
        public Lag SelectedLag2
        {
            get { return m_selectedLag2; }
            set
            {
                m_selectedLag2 = value;
                if (m_selectedLag2 != null)
                {
                    m_selectedSkive = Skiver.FirstOrDefault(x => x.LagNummer == m_selectedLag2.LagNummer);
                    
                    NotifyPropertyChanged("SelectedSkive");
                }
                
                NotifyPropertyChanged("SelectedLag2");
            }
        }

        private ObservableCollection<Lag> m_lagKilde;
        public ObservableCollection<Lag> LagKilde
        {
            get { return m_lagKilde; }
            set
            {
                m_lagKilde = value;


                NotifyPropertyChanged("LagKilde");
            }
        }


        private SkiverViewModel m_selectedSkive;
        public SkiverViewModel SelectedSkive
        {
            get { return m_selectedSkive; }
            set
            {
                m_selectedSkive = value;
                NotifyPropertyChanged("SelectedSkive");
            }
        }

        private ObservableCollection<SkiverViewModel> m_skiver;
        public ObservableCollection<SkiverViewModel> Skiver
        {
            get { return m_skiver; }
            set
            {
                m_skiver = value;
                NotifyPropertyChanged("Skiver");
            }
        }

        private ObservableCollection<Lag> m_inputRows;

        public ObservableCollection<Lag> InputRows
        {
            get { return m_inputRows; }
            set
            {
                m_inputRows = value;
                NotifyPropertyChanged("InputRows");
            }
        }

        public DataGrid GridManager { get; set; }

        public void SetTimesButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if(string.IsNullOrEmpty(m_lagDuration))
            {
                return;
            }
            if (string.IsNullOrEmpty(m_lagStart))
            {
                return;
            }

            int minutes = 0;
            if (!int.TryParse(m_lagDuration, out minutes))
            {
                return;
            }

            var splits = m_lagStart.Split(new char[] { ':' });
            if (splits.Length != 2)
            {
                return;
            }
            int hour = 0;
            int min = 0;
            if (!int.TryParse(splits[0], out hour))
            {
                return;
            }
            if (!int.TryParse(splits[1], out min))
            {
                return;
            }
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, 0);
            TimeSpan span = new TimeSpan(0, minutes, 0);
            foreach (var lag in LagKilde)
            {
                lag.LagTid = startTime;
                startTime = startTime.Add(span);
            }

            var test = LagKilde;
            LagKilde = null;
            NotifyPropertyChanged("LagKilde");
            LagKilde = test;
            NotifyPropertyChanged("LagKilde");
        }

        public void SortButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            SortGrid();
        }

        public void GenerateFilesButtonBase_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if(string.IsNullOrEmpty(m_filePath))
            {
                return;
            }

            if (!Directory.Exists(m_filePath))
            {
                MessageBox.Show(string.Format("Path Does not exsist{0}", m_filePath),"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            Collection<Lag> lagCollection = new Collection<Lag>();
            foreach (var lag in LagKilde)
            {
                var nyttLag = new Lag(lag);
                lagCollection.Add(nyttLag);
            }

            foreach (var skive in Skiver)
            {
                var lagFunnet = lagCollection.FirstOrDefault(x => x.LagNummer == skive.LagNummer);

                var funnetSkive = lagFunnet.SkiverILaget.FirstOrDefault(x => x.SkiveNummer == skive.SkiveNummer);
                if (funnetSkive.Skytter != null)
                {
                    funnetSkive.Skytter = new Skytter(funnetSkive.Skytter);
                }

                funnetSkive.Free = funnetSkive.Free;
            }

            XmlSerializer ser = new XmlSerializer(typeof(Lag));

            foreach (var utskriftsLag in lagCollection)
            {
                string filename = Path.Combine(m_filePath, string.Format("Hold_{0}_Lag_{1}.xml","1", utskriftsLag.LagNummer));
                
                using (XmlWriter Write = new XmlTextWriter(filename, Encoding.UTF8))
                {
                    ser.Serialize(Write, utskriftsLag);
                }
            }
            
            
        }
    }
}
