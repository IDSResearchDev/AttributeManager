using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rnd.Common;
using System.Windows.Input;
using System.Windows.Forms;

namespace AttributeManager.AttribSettings
{
    public class AttribSettingsViewModel : BindableBase
    {
        private Utilities _utilities;

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
            SetOutputDirectory = new RelayCommand<string>(OnSetOutputDirectory);
            SetDefaultAttributeDirectory = new RelayCommand<string>(OnSetDefaultDirectory);
        }

        
        
        private string ShowDialog(string path, string cmdparam)
        {
            Tuple<DialogResult, string> dialog;
            DialogResult dialogresult = new DialogResult();
            string selectedpath = string.Empty;

            if (cmdparam.Equals("default"))
            {
                dialog = _utilities.FileDialog();
                //dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dialogresult = dialog.Item1;
                selectedpath = dialog.Item2;
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
            
            return (dialogresult != System.Windows.Forms.DialogResult.OK) ? path : selectedpath;
        }


        #region Commands
        public RelayCommand<string> SetOutputDirectory { get; private set; }
        public Action<string> SetOutputDirectoryRequested = delegate { };

        private void OnSetOutputDirectory(string obj)
        {
            OutputDirectory = ShowDialog(OutputDirectory, obj); ;
            SetOutputDirectoryRequested(obj);

            //note!: open file dialog openning twice after selecting file. <<- FIX this.. WTF. REFACTOR.
        } 


        public RelayCommand<string> SetDefaultAttributeDirectory { get; private set; }
        public Action<string> SetDefaultAttributeDirectoryRequested = delegate { };
        private void OnSetDefaultDirectory(string obj)
        {
            DefaultAttributeDirectory = ShowDialog(DefaultAttributeDirectory, obj);
            SetDefaultAttributeDirectoryRequested(obj);
        }
        #endregion

    }
}
