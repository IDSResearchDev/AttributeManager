using System;
using AttributeManager.BaseClass;
using System.Windows.Input;
using System.Windows;
using Forms = System.Windows.Forms;
using AttributeManager.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities = Rnd.Common.Utilities;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AttributeManager
{
    public class MainWindowViewModel : BindableBase, IDataErrorInfo
    {
        public static string LocalAppFolder = Path.Combine(new Utilities().LocalAppData, "AttributeManager");
        public static string LocalUpdaterFile = Path.Combine(LocalAppFolder, "updater.ini");
        public static string AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);


        private Utilities _utilities;
        List<AttributeModel> componentDictionary;

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public MainWindowViewModel()
        {
            CanValidate = false;
            CheckLatestUpdate();
        }

        public void LoadCurrentView()
        {
            _utilities = new Utilities();
            //CurrentViewModel = _mainvViewModel;
        }

        private Visibility _progressVisible = Visibility.Collapsed;
        public Visibility ProgressVisible
        {
            get { return _progressVisible; }
            set
            {
                SetProperty(ref _progressVisible, value);

            }
        }

        private string _outputdirectory = string.Empty;
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
            Tuple<Forms.DialogResult, string> dialog;
            Forms.DialogResult dialogresult = new Forms.DialogResult();
            string selectedpath = string.Empty;

            if (cmdparam.Equals("default"))
            {
                dialog = _utilities.FileDialog("Select the excel template.", "Excel files", "xls");
                dialogresult = dialog.Item1;
                selectedpath = dialog.Item2;

            }
            else
            {
                dialog = _utilities.FolderDialog();
                dialogresult = dialog.Item1;
                selectedpath = dialog.Item2;
            }

            return (dialogresult != Forms.DialogResult.OK) ? path : selectedpath;
        }


        #region Commands

        public ICommand Create
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    CanValidate = true;
                    CreateJfiles();

                    //if (String.IsNullOrEmpty(DefaultAttributeDirectory) || String.IsNullOrEmpty(OutputDirectory)) return;

                    //if (!_utilities.CheckIfFileExists(DefaultAttributeDirectory) && !_utilities.CheckIfDirectoryExists(OutputDirectory)) return;


                    //Task t = Task.Run(() =>
                    //{
                    //    ProgressVisible = Visibility.Visible;
                    //    IExcelReader reader = new ExcelReader(DefaultAttributeDirectory);
                    //    var components = reader.GetComponents();

                    //    componentDictionary = reader.GetComponentDictionary();
                    //    reader.ForceDispose();

                    //    var appDomain = AppDomain.CurrentDomain.BaseDirectory;
                    //    var standardFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\standard.txt");
                    //    int counter = 1;
                    //    foreach (var component in components)
                    //    {
                    //        var jfile = File.ReadAllText(standardFile);
                    //        var filePath = Path.Combine(OutputDirectory, $"{component.Size}.j{component.ComponentNumber}");
                    //        _utilities.CreateFileWithText(filePath, jfile);


                    //        Dictionary<string, string> attributes = new Dictionary<string, string>();
                    //        attributes.Add($"joint_attributes.saveas_file", GetAttributeFormatType(component.Size, "string"));
                    //        attributes.Add($"joint_attributes.get_menu", GetAttributeFormatType(component.Size, "string"));
                    //        foreach (var attribute in component.Attributes)
                    //        {
                    //            var id = GetAttribute(attribute.Key, 1);
                    //            var paramType = GetAttribute(attribute.Key, 2);

                    //            if (id != null && paramType != null)
                    //            {
                    //                // check paramtype
                    //                // set/change attribute.value base on paramtype  
                    //                attributes.Add($"joint_attributes.{id}", GetAttributeFormatType(attribute.Value, paramType.ToLower()));
                    //            }
                    //        }
                    //        _utilities.UpdateTextFileValues(filePath: filePath, delimiter: ' ', newValues: attributes);
                    //        counter++;
                    //    }
                    //});
                    //t.Wait();

                    //ProgressVisible = Visibility.Collapsed;
                    //MessageBox.Show("Attribute files created.");
                });
            }
        }

        public async void CreateJfiles()
        {
            if (String.IsNullOrEmpty(DefaultAttributeDirectory) || String.IsNullOrEmpty(OutputDirectory)) return;
            if (!_utilities.CheckIfFileExists(DefaultAttributeDirectory) && !_utilities.CheckIfDirectoryExists(OutputDirectory)) return;

            await Task.Run(() =>
            {
                ProgressVisible = Visibility.Visible;
                IExcelReader reader = new ExcelReader(DefaultAttributeDirectory);
                var components = reader.GetComponents();

                componentDictionary = reader.GetComponentDictionary();
                reader.ForceDispose();

                var appDomain = AppDomain.CurrentDomain.BaseDirectory;
                var standardFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\standard.txt");
                int counter = 1;
                foreach (var component in components)
                {
                    var jfile = File.ReadAllText(standardFile);
                    var filePath = Path.Combine(OutputDirectory, $"{component.Size}.j{component.ComponentNumber}");
                    _utilities.CreateFileWithText(filePath, jfile);


                    Dictionary<string, string> attributes = new Dictionary<string, string>();
                    attributes.Add($"joint_attributes.saveas_file", GetAttributeFormatType(component.Size, "string"));
                    attributes.Add($"joint_attributes.get_menu", GetAttributeFormatType(component.Size, "string"));
                    foreach (var attribute in component.Attributes)
                    {
                        var id = GetAttribute(attribute.Key, 1);
                        var paramType = GetAttribute(attribute.Key, 2);

                        if (id != null && paramType != null)
                        {
                            // check paramtype
                            // set/change attribute.value base on paramtype  
                            attributes.Add($"joint_attributes.{id}", GetAttributeFormatType(attribute.Value, paramType.ToLower()));
                        }
                    }
                    _utilities.UpdateTextFileValues(filePath: filePath, delimiter: ' ', newValues: attributes);
                    counter++;
                }

                ProgressVisible = Visibility.Collapsed;
                MessageBox.Show("Attribute files created.");
            });
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

        #region Validation Error
        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (CanValidate)
                {
                    if (columnName == "DefaultAttributeDirectory")
                    {
                        if (string.IsNullOrEmpty(DefaultAttributeDirectory))
                        {
                            return "Please select excel file.";
                        }

                        if (!_utilities.CheckIfFileExists(DefaultAttributeDirectory))
                        {
                            return "Please provide existing excel file template.";
                        }

                    }

                    if (columnName == "OutputDirectory")
                    {
                        if (string.IsNullOrEmpty(OutputDirectory))
                        {
                            return "Please provide output directory.";
                        }

                        if (!_utilities.CheckIfDirectoryExists(OutputDirectory))
                        {
                            return "Output directory is not exist.";
                        }

                    }
                }

                return string.Empty;
            }

        }

        private bool _canValidate;
        private bool CanValidate
        {
            get { return _canValidate; }
            set
            {
                _canValidate = value;
                OnPropertyChanged("CanValidate");
                OnPropertyChanged("DefaultAttributeDirectory");
                OnPropertyChanged("OutputDirectory");
            }
        }

        public ICommand CheckUpdate
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    string updater = Path.Combine(LocalAppFolder, @"updater.exe");
                    if (!File.Exists(updater))
                    {
                        MessageBox.Show(this.GetCurrentWindow(), "Updater not found.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (File.Exists(LocalUpdaterFile))
                    {
                        if (CheckLatestUpdate())
                        {
                            UpdateSettingView update = new UpdateSettingView();
                            update.Owner = this.GetCurrentWindow();
                            update.ShowDialog();
                        }
                        else
                        {
                            Process.Start(updater);
                        }
                    }
                });
            }
        }
        #endregion

        #region Misc

        public string GetAttribute(string attributeName, int property)
        {
            var attribute = componentDictionary.Where(p => p.AttributeName == attributeName).Select(p => p.AttridbuteId).ToList();
            switch (property)
            {
                case 2:
                    attribute = componentDictionary.Where(p => p.AttributeName == attributeName).Select(p => p.ParameterType).ToList();
                    break;
                case 3:
                    attribute = componentDictionary.Where(p => p.AttributeName == attributeName).Select(p => p.Tab).ToList();
                    break;
                case 1:
                default:
                    break;
            }
            return attribute.Count <= 0 ? null : attribute[0];
        }

        public string GetAttributeFormatType(string value, string paramType)
        {
            string newValue = value;
            switch (paramType)
            {
                case "string":
                    newValue = $"\"{value}\"";
                    break;
                case "double":
                    if (String.IsNullOrEmpty(value))
                    {
                        newValue = "-2147483648";
                    }
                    else
                    {
                        double wnum = 0.0;
                        double dnum = 0.0;
                        if (value.Trim().Contains(" "))
                        {
                            var v = value.Trim().Split(' ');
                            Double.TryParse(v[0], out wnum);

                            if (v[1].Contains("/"))
                            {
                                dnum = GetDecimalValue(v[1]);
                            }
                        }
                        else
                        {
                            if (value.Trim().Contains("/"))
                            {
                                dnum = GetDecimalValue(value);
                            }
                            else
                            {
                                Double.TryParse(value.Trim(), out wnum);
                            }
                        }
                        newValue = ((wnum + dnum) * 25.4).ToString();
                    }
                    break;
                case "integer":
                    if (String.IsNullOrEmpty(value))
                    {
                        newValue = "-2147483648";
                    }
                    break;
                default:
                    break;
            }
            return newValue;
        }

        private double GetDecimalValue(string fraction)
        {
            var d = fraction.Trim().Split('/');
            double numerator = 0;
            double denominator = 0;
            Double.TryParse(d[0], out numerator);
            Double.TryParse(d[1], out denominator);

            return (numerator / denominator);
        }

        #endregion

        #region Update

        private string _getUpdate;
        public string GetUpdate
        {
            get { return _getUpdate; }
            set
            {
                _getUpdate = value;
                SetProperty(ref _getUpdate, value);
            }
        }

        private string _checkForUpdate;
        public string CheckForUpdate
        {
            get { return _checkForUpdate; }
            set
            {
                _checkForUpdate = value;
                SetProperty(ref _checkForUpdate, value);
            }
        }
        private bool CheckLatestUpdate()
        {
            bool value = false;
            if (File.Exists(LocalUpdaterFile))
            {
                var aiuFile = "attribute_manager_update.aiu";
                var util = new Rnd.Common.Utilities();
                var updatePath = Path.Combine(util.GetTextFileValue(LocalUpdaterFile, '=', "DownloadsFolder"), aiuFile);
                if (File.Exists(updatePath))
                {
                    var updateVersion = new Version(util.GetTextFileValue(updatePath, '=', "Version")).ToString(3);

                    if (VersionComparer.IsUptoDate(updateVersion, AppVersion))
                    {
                        value = true;
                        GetUpdate = string.Empty;
                        CheckForUpdate = "Check for Update";
                    }
                    else
                    {
                        GetUpdate = "Get latest version ";
                        CheckForUpdate = updateVersion;
                    }
                }
                else
                {
                    MessageBox.Show($"Attribute Manager update file ({aiuFile}) doesn't exist.", "Update not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    value = true;
                }
            }
            return value;
        }
        #endregion
    }

}
