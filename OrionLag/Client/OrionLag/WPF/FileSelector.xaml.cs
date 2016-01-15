using System;
using System.Windows;
using System.Windows.Controls;

namespace OrionLag.WPF
{
    using System.IO;
    using System.Windows.Forms;

    using FileDialog = Microsoft.Win32.FileDialog;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
    using UserControl = System.Windows.Controls.UserControl;

    /// <summary>
    /// Interaction logic for FileSelector.xaml
    /// </summary>
    public partial class FileSelector : UserControl
    {
        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register(
            "FileFilter", typeof(string), typeof(FileSelector));

        public static readonly DependencyProperty MaxDisplayLengthProperty =
            DependencyProperty.Register(
                "MaxDisplayLength",
                typeof(int),
                typeof(FileSelector),
                new FrameworkPropertyMetadata(0, OnMaxDisplayLengthPropertyChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(FileSelectorMode), typeof(FileSelector));

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName",
            typeof(string),
            typeof(FileSelector),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFileNamePropertyChanged));

        public static readonly DependencyProperty RootFolderProperty = DependencyProperty.Register(
            "RootFolder",
            typeof(Environment.SpecialFolder?),
            typeof(FileSelector));

        public FileSelector()
        {
            InitializeComponent();
        }

        private static void OnFileNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileSelector;
            if (control != null)
            {
                control.txtFileName.Text = (string)e.NewValue;
            }
        }

        private static void OnMaxDisplayLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileSelector;
            if (control != null)
            {
                control.txtFileName.MaxLength = (int)e.NewValue;
            }
        }

        public FileSelectorMode Mode
        {
            get { return (FileSelectorMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public Environment.SpecialFolder? RootFolder
        {
            get { return (Environment.SpecialFolder?)GetValue(RootFolderProperty); }
            set { SetValue(RootFolderProperty, value); }
        }

        public int MaxDisplayLength
        {
            get { return (int)GetValue(MaxDisplayLengthProperty); }
            set { SetValue(MaxDisplayLengthProperty, value); }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == FileSelectorMode.Open || Mode == FileSelectorMode.Save)
            {
                // File dialog
                var dlg = Mode == FileSelectorMode.Open ? (Microsoft.Win32.FileDialog)new Microsoft.Win32.OpenFileDialog() : new Microsoft.Win32.SaveFileDialog();

                dlg.Filter = FileFilter;
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    FileName = dlg.FileName;
                }
            }
            else if (Mode == FileSelectorMode.OpenOrCreate)
            {
                FileDialog dlg = null;
                if (string.IsNullOrEmpty(FileName))
                {
                    dlg = new Microsoft.Win32.SaveFileDialog();

                }
                else
                {
                    if (File.Exists(FileName))
                    {
                        dlg = new OpenFileDialog();
                    }
                    else
                    {
                        dlg = new SaveFileDialog();
                    }
                }

                dlg.Filter = FileFilter;
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    FileName = dlg.FileName;
                }

            }
            else
            {
                // Folder dialog
                var dlg = new FolderBrowserDialog
                {
                    RootFolder = RootFolder ?? Environment.SpecialFolder.Desktop
                };

                var startDir = GetStartDirectory(FileName);
                if (!string.IsNullOrWhiteSpace(startDir))
                {
                    dlg.SelectedPath = startDir;
                }

                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    FileName = dlg.SelectedPath;
                }
            }
        }

        private string GetStartDirectory(string dir)
        {
            while (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                try
                {
                    dir = Path.GetDirectoryName(dir);
                }
                catch (ArgumentException)
                {
                    // Invalid characters in path
                    return null;
                }
            }

            return dir;
        }

        private void TxtFileName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FileName = txtFileName.Text;
        }
    }

    public enum FileSelectorMode
    {
        Open = 0,
        Save = 1,
        Folder = 2,
        OpenOrCreate = 3,
    }
}
