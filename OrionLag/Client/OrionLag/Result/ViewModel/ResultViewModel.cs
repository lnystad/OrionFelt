using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Result.ViewModel
{
    using System.Collections.ObjectModel;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Services;
    using OrionLag.WpfBase;

    public class ResultViewModel : TargetWindowBase
    {
        private IResultDataService m_databaseService;

        public ResultViewModel(IResultDataService dataService)
        {
            m_databaseService = dataService;
            InitViewModel();
        }

        private void InitViewModel()
        {
            var lag = m_databaseService.GetLag();
            m_lagKilde = new ObservableCollection<Lag>(lag);
        }

        private Lag m_selectedLag;
        public Lag SelectedLag
        {
            get { return m_selectedLag; }
            set
            {
                m_selectedLag = value;
                this.OnPropertyChanged("SelectedLag");
                LoadResults(m_selectedLag);
            }
        }

        private void LoadResults(Lag mSelectedLag)
        {
            if (mSelectedLag == null)
            {
                ChosenLag = null;
                this.OnPropertyChanged("ChosenLag");
                return;
            }

            m_chosenLag = new ObservableCollection<ResultRowViewModel>();

            foreach (var skive in mSelectedLag.SkiverILaget)
            {
                if (skive.SkytterGuid != null)
                {
                    var results = m_databaseService.GetResultsForSkytter(skive.SkytterGuid.Value);

                    m_chosenLag.Add(new ResultRowViewModel(mSelectedLag.LagNummer, skive.SkiveNummer, skive.Skytter, results));
                }
                else
                {
                    m_chosenLag.Add(new ResultRowViewModel(mSelectedLag.LagNummer, skive.SkiveNummer));
                }
                
            }
            this.OnPropertyChanged("ChosenLag");
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

        private ObservableCollection<ResultRowViewModel> m_chosenLag;
        public ObservableCollection<ResultRowViewModel> ChosenLag
        {
            get { return m_chosenLag; }
            set
            {
                SetProperty(ref m_chosenLag, value, () => ChosenLag);
            }
        }

        private ResultRowViewModel m_selectedChosenLag;
        public ResultRowViewModel SelectedChosenLag
        {
            get { return m_selectedChosenLag; }
            set
            {
                m_selectedChosenLag = value;
                this.OnPropertyChanged("SelectedChosenLag");
            }
        }

    }
}
