using System;
using AttributeManager.BaseClass;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;
using AttributeManager.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities = Rnd.Common.Utilities;

namespace AttributeManager
{
    public class MainWindowViewModel : BindableBase
    {
        private Utilities _utilities;
        List<AttributeModel> componentDictionary;

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
                        var filePath = Path.Combine(OutputDirectory,$"{component.Size}.j{component.ComponentNumber}");
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
    }

}
