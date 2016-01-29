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

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;
    using OrionLag.Server.Services;
    using OrionLag.WpfBase;

    public class LagOppsettViewModel : TargetWindowBase
    {
        private IExportLeonFormatService m_exportService;
        private ILagOppsettDataService m_databaseService;
        private List<Lag> lagOppsett;
        public LagOppsettViewModel(ILagOppsettDataService oppsetService, List<Lag> lagOppsett,int minutes,DateTime startTime)
        {
            m_databaseService = oppsetService;
            InitModel();
            LagStart = startTime.ToString("HH:mm");
            LagDuration = minutes.ToString();
            InitGrid(lagOppsett);
        }

        private void InitGrid(List<Lag> lagInput)
        {
            lagOppsett = lagInput;
            m_inputRows = new ObservableCollection<Lag>(lagInput);
            m_skiver = new ObservableCollection<SkiverViewModel>();
            this.LagKilde = new ObservableCollection<Lag>();
            
            var sortedlagOppsett = lagOppsett.OrderBy(o => o.LagNummer).ToList();
            
            foreach (var lag in sortedlagOppsett)
            {
                this.LagKilde.Add(lag);
                var sortedSkiver = lag.SkiverILaget.OrderBy(x => x.SkiveNummer);
                foreach (var skive in sortedSkiver)
                {
                    m_skiver.Add(item: new SkiverViewModel(lag.LagNummer, skive));
                }
            }

            if (sortedlagOppsett.Count > 0)
            {
                if (sortedlagOppsett[0].LagTid.HasValue)
                {
                    LagStart = sortedlagOppsett[0].LagTid.Value.ToString("HH:mm");
                }

                if (sortedlagOppsett.Count > 1)
                {
                    if (sortedlagOppsett[1].LagTid.HasValue && sortedlagOppsett[0].LagTid.HasValue)
                    {
                        TimeSpan s = sortedlagOppsett[1].LagTid.Value - sortedlagOppsett[0].LagTid.Value;
                        LagDuration = s.TotalMinutes.ToString();
                    }
                }
            }

            SortGrid();
        }

        private void InitModel()
        {
            var config = m_databaseService.GetOppsettConfig();
            this.m_filePath = config.PathFinfelt;
        }

        private void SortGrid()
        {
            var input = m_inputRows.OrderBy(o => o.LagNummer);
            m_inputRows = new ObservableCollection<Lag>(input.ToList());

            var sortskiver = m_skiver.OrderBy(x => x.LagNummer).ThenBy(y => y.SkiveNummer);
            m_skiver = new ObservableCollection<SkiverViewModel>(sortskiver.ToList());
            OnPropertyChanged("Skiver");
            OnPropertyChanged("InputRows");
        }


        private string m_lagDuration;
        public string LagDuration
        {
            get { return m_lagDuration; }
            set
            {
                SetProperty(ref m_lagDuration, value, () => LagDuration);
            }
        }

        private string m_lagStart;
        public string LagStart
        {
            get { return m_lagStart; }
            set
            {
                SetProperty(ref m_lagStart, value, () => LagStart);
            }
        }

        private string m_filePath;
        public string  FilePath
        {
            get { return m_filePath; }
            set
            {
                SetProperty(ref m_filePath, value, () => FilePath);
            }
        }

      

        private Lag m_selectedLag2;
        public Lag SelectedLag2
        {
            get { return m_selectedLag2; }
            set
            {
                m_selectedLag2 = value;
                this.OnPropertyChanged("SelectedLag2");

                if (m_selectedLag2 != null)
                {
                    m_selectedSkive = null;
                    this.OnPropertyChanged("SelectedSkive");
                    m_selectedSkive = Skiver.FirstOrDefault(x => x.LagNummer == m_selectedLag2.LagNummer);
                    
                    this.OnPropertyChanged("SelectedSkive");
                }
                
            }
        }

        private ObservableCollection<Lag> m_lagKilde;
        public ObservableCollection<Lag> LagKilde
        {
            get { return m_lagKilde; }
            set
            {
                SetProperty(ref m_lagKilde, value, () => LagKilde);
            }
        }


        private SkiverViewModel m_selectedSkive;
        public SkiverViewModel SelectedSkive
        {
            get { return m_selectedSkive; }
            set
            {
                SetProperty(ref m_selectedSkive, value, () => SelectedSkive);
            }
        }

        private ObservableCollection<SkiverViewModel> m_skiver;
        public ObservableCollection<SkiverViewModel> Skiver
        {
            get { return m_skiver; }
            set
            {
                SetProperty(ref m_skiver, value, () => Skiver);
            }
        }

        private ObservableCollection<Lag> m_inputRows;

        public ObservableCollection<Lag> InputRows
        {
            get { return m_inputRows; }
            set
            {
                SetProperty(ref m_inputRows, value, () => InputRows);
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

            foreach (var lag in m_inputRows)
            {
                var funnetlag = LagKilde.FirstOrDefault(x => x.LagNummer == lag.LagNummer);
                if (funnetlag != null)
                {
                    lag.LagTid = funnetlag.LagTid;
                }

            }

            var test = LagKilde;
            LagKilde = null;
            this.OnPropertyChanged("LagKilde");
            LagKilde = test;
            this.OnPropertyChanged("LagKilde");
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
            m_databaseService.StoreToDatabase(lagCollection, this.m_filePath);
        }

        public void ReadDatabaseButtonBase_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (string.IsNullOrEmpty(this.m_filePath))
            {
                return;
            }

            var readlag = m_databaseService.GetAllFromDatabase(this.m_filePath);
            InitGrid(readlag.ToList());
        }

        public void LeonExportButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            m_exportService = new ExportLeonFormatService();
            var readlag = m_databaseService.GetAllFromDatabase(this.m_filePath);

            m_exportService.GenerateLeonFormat(readlag.ToList());
        }
    }
}
