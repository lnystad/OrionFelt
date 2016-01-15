using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;
    using OrionLag.Common.Utils;
    using OrionLag.Input.ViewModel;
    using OrionLag.Input.Views;
    using OrionLag.WpfBase;

    public class UserInputControlViewModel : TargetWindowBase

    {
        private IInputPatitionerService m_inputService;
        private ILagGenerator m_lagGeneratorService;
        public UserInputControlViewModel(IInputPatitionerService inputService, ILagGenerator lagGeneratorService)
        {
            m_inputService = inputService;
            m_lagGeneratorService = lagGeneratorService;
            int count=0;
            m_inputRows = new ObservableCollection<InputData>();
            while (count < 10)
            {
                InputData data = new InputData();
                data.SkytterNr = count;
                m_inputRows.Add(data);
                count++;
            }

            InitData();
        }

        private void InitData()
        {
            var spec = m_lagGeneratorService.GetSpecFromConfiguration();

            m_startLagNr = spec.StartLagNr;
            m_antallSkiver = spec.antallSkiver;
            m_antallskyttereilaget = spec.antallskyttereilaget;
            m_antallHold = spec.antallHold;
            m_startTime = spec.StartTime;
            this.m_minutesEachTeam = spec.MinutesEachTeam;
            this.m_GenererAvbrekk = spec.avbrekk;

        }

        private ObservableCollection<InputData> m_inputRows;

        
        private int m_startLagNr;
        public int StartLagNr
        {
            get { return m_startLagNr; }
            set
            {
                SetProperty(ref m_startLagNr, value, () => StartLagNr);
            }
        }

        private int m_antallSkiver;
        public int AntallSkiver
        {
            get { return m_antallSkiver; }
            set
            {
                SetProperty(ref m_antallSkiver, value, () => AntallSkiver);
            }
        }

        private int m_antallskyttereilaget;
        public int Antallskyttereilaget
        {
            get { return m_antallskyttereilaget; }
            set
            {
                SetProperty(ref m_antallskyttereilaget, value, () => Antallskyttereilaget);
            }
        }

        private int m_antallHold;
        public int AntallHold
        {
            get { return m_antallHold; }
            set
            {
                SetProperty(ref m_antallHold, value, () => AntallHold);
            }
        }

        private bool m_GenererAvbrekk;
        public bool GenererAvbrekk
        {
            get { return m_GenererAvbrekk; }
            set
            {
                SetProperty(ref m_GenererAvbrekk, value, () => GenererAvbrekk);
            }
        }

        private int m_minutesEachTeam;
        public int MinutesEachTeam
        {
            get { return m_minutesEachTeam; }
            set
            {
                SetProperty(ref m_minutesEachTeam, value, () => MinutesEachTeam);
            }
        }

        private DateTime? m_startTime;
        public DateTime? StartTime
        {
            get { return m_startTime; }
            set
            {
                SetProperty(ref m_startTime, value, () => StartTime);
            }
        }

        public ObservableCollection<InputData> InputRows
        {
            get { return m_inputRows; }
            set
            {
                SetProperty(ref m_inputRows, value, () => InputRows);
            }
        }

        public void OnReadInputbutton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var input = m_inputService.GetAllComptitiors();

            InputRows = new ObservableCollection<InputData>(input);
            return;
        }

        public void OnWriteInputbutton_OnClickOn(object sender, RoutedEventArgs routedEventArgs)
        {
            if (InputRows.Count == 0)
            {
                return;
            }
            var spec = new LagGeneratorSpec()
                           {
                               antallHold = this.m_antallHold,
                               antallSkiver = this.m_antallSkiver,
                               antallskyttereilaget = this.m_antallskyttereilaget,
                               avbrekk = this.m_GenererAvbrekk,
                               MinutesEachTeam = this.m_minutesEachTeam,
                               StartLagNr = this.m_startLagNr,
                               StartTime = this.m_startTime
                           };
            var list = m_lagGeneratorService.GenererLag(InputRows.ToList(), spec);
            DateTime start = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second);
            if (this.m_startTime.HasValue)
            {
                start = this.m_startTime.Value;
            }

            LagOppsettViewModel viewmodel  = new LagOppsettViewModel(list, m_minutesEachTeam, start);
            var view = new LagOppsettView(viewmodel);

            OpenWindow(view, "Data input");

        }

        private void OpenWindow(object windowContent, string title = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = string.Concat("[No Name (", windowContent.GetType().Name, ")]");
            }

            var window = new Window { Content = windowContent, Height = 600, Width = 900, Title = title };
            window.Show();
        }
    }
}
