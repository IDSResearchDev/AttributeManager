using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rnd.Common;
using System.Windows.Input;
using System.Windows.Forms;
using AttributeManager.BaseClass;
using System.Windows;

namespace AttributeManager.AttribSettings
{
    public class AttribSettingsViewModel : BindableBase
    {
        private Utilities _utilities;

        private Visibility _progressVisible = Visibility.Collapsed;
        public Visibility ProgressVisible 
        {
            get { return _progressVisible; }
            set { /*SetProperty(ref _progressVisible, value)*/_progressVisible = value; OnPropertyChanged("ProgressVisible"); }
        }

        private string _outputdirectory;
        public string OutputDirectory
        {
            get { return _outputdirectory; }
            set { SetProperty(ref _outputdirectory, value); }
        }


        private string _defaultAttributeDirectory;
        public string DefaultAttributeDirectory
        {
            get { return _defaultAttributeDirectory; }
            set { SetProperty(ref _defaultAttributeDirectory, value); }
        }


        public AttribSettingsViewModel()
        {
            _utilities = new Utilities();
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
                // selectedpath = _utilities.FileDialog("Select the excel template.");
            }
            else
            {
                //if (cmdparam.Equals("outputdir"))
                //{
                dialog = _utilities.FolderDialog();
                dialogresult = dialog.Item1;
                selectedpath = dialog.Item2;
                //}
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
                    DefaultAttributeDirectory = ShowDialog(DefaultAttributeDirectory, @params as string);

                    //OpenFileDialog openFileDialog1 = new OpenFileDialog();

                    //openFileDialog1.InitialDirectory = @"C:\";
                    //openFileDialog1.Title = "Browse Text Files";

                    //openFileDialog1.CheckFileExists = true;
                    //openFileDialog1.CheckPathExists = true;

                    //openFileDialog1.DefaultExt = "txt";
                    //openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    //openFileDialog1.FilterIndex = 2;
                    //openFileDialog1.RestoreDirectory = true;

                    //openFileDialog1.ReadOnlyChecked = true;
                    //openFileDialog1.ShowReadOnly = true;

                    //if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    //{
                    //    DefaultAttributeDirectory = openFileDialog1.FileName;
                    //}

                });
            }

        }

        public ICommand SetOutputDirectory
        {
            get
            {
                return new DelegateCommand((@params) =>
                {
                    OutputDirectory = ShowDialog(OutputDirectory, @params as string);
                });
            }
        }

        //public ICommand SetExcelTemplate
        //{
        //    get
        //    {
        //        return new DelegateCommand((@params) =>
        //        {
        //            DefaultAttributeDirectory = ShowDialog(DefaultAttributeDirectory, @params as string);
        //        });
        //    }
        //}

        public ICommand Exit
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    //this.Close();
                    
                });
            }
        }


        #endregion

    }

}
