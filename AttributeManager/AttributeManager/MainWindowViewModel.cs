using System;
using Rnd.Common;
using AttributeManager.BaseClass;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;

namespace AttributeManager
{
    public class MainWindowViewModel : BindableBase
    {
        private Utilities _utilities;

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public MainWindowViewModel() { }

        public void LoadCurrentView()
        {
            _utilities = new Utilities();
            //CurrentViewModel = _mainvViewModel;
        }

        private Visibility _progressVisible = Visibility.Collapsed;
        public Visibility ProgressVisible
        {
            get { return _progressVisible; }
            set { SetProperty(ref _progressVisible, value); }
        }

        private string _outputdirectory=string.Empty;
        public string OutputDirectory
        {
            get { return _outputdirectory; }
            set { SetProperty(ref _outputdirectory, value); }
        }


        private string _defaultAttributeDirectory = string.Empty;
        public string DefaultAttributeDirectory
        {
            get { return _defaultAttributeDirectory; }
            set { SetProperty(ref _defaultAttributeDirectory, value); }
        }

        
        private string ShowDialog(string path, string cmdparam)
        {
            Tuple<DialogResult, string> dialog;
            DialogResult dialogresult = new DialogResult();
            string selectedpath = string.Empty;

            if (cmdparam.Equals("default"))
            {
                dialog = _utilities.FileDialog("Select the excel template.");
                dialogresult = dialog.Item1;
                selectedpath = dialog.Item2;
                
            }
            else
            {
                dialog = _utilities.FolderDialog();
                dialogresult = dialog.Item1;
                selectedpath = dialog.Item2;
            }

            return (dialogresult != DialogResult.OK) ? path : selectedpath;
        }


        #region Commands

        public ICommand Create
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ProgressVisible = Visibility.Visible;
                });
            }
        }

        public ICommand SetExcelTemplate
        {

            get
            {
                return new DelegateCommand((@params) =>
                {
                    var attribDir = ShowDialog(DefaultAttributeDirectory, @params as string);
                    DefaultAttributeDirectory = attribDir;
                });
            }
        }

        public ICommand SetOutputDirectory
        {
            get
            {
                return new DelegateCommand((@params) =>
                {
                    var outputDir = ShowDialog(OutputDirectory, @params as string);
                    OutputDirectory = outputDir;
                });
            }
        }
        
        public ICommand Exit
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Close();
                });
            }
        }
        #endregion
    }

}
