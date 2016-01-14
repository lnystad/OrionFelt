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

    using OrionLag.Input.Data;
    using OrionLag.Input.ViewModel;
    using OrionLag.Input.Views;
    using OrionLag.Utils;

    public class UserInputControlViewModel : INotifyPropertyChanged

    {

        public UserInputControlViewModel()
        {
            int count=0;
            m_inputRows = new ObservableCollection<InputData>();
            while (count < 10)
            {
                InputData data = new InputData();
                data.SkytterNr = count;
                m_inputRows.Add(data);
                count++;
            }
        }

        private ObservableCollection<InputData> m_inputRows;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private string m_lagNummer;
        public string LagNummer
        {
            get { return m_lagNummer; }
            set
            {
                m_lagNummer = value;
                NotifyPropertyChanged("LagNummer");
            }
        }

        public ObservableCollection<InputData> InputRows
        {
            get { return m_inputRows; }
            set
            {
                m_inputRows = value;
                NotifyPropertyChanged("InputRows");
            }
        }

        public void OnReadInputbutton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var input = InputFileParser.ParseFile(@"C:\Orion2\Data\Felt", "INPUT.TXT");

            InputRows = new ObservableCollection<InputData>(input);
            return;
        }

        public void OnWriteInputbutton_OnClickOn(object sender, RoutedEventArgs routedEventArgs)
        {
            if (InputRows.Count == 0)
            {
                return;
            }
            LagGenerator Generator = new LagGenerator();

            var list = Generator.GenererLag(InputRows.ToList(),12, 2, 6, true);
            LagOppsettViewModel viewmodel  = new LagOppsettViewModel(list,6,new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
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
