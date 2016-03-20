using AttributeManager.AttribSettings;
using AttributeManager.ThemeCatalog;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rnd.Common;

namespace AttributeManager
{
    public class MainWindowViewModel : BindableBase
    {
        private ThemeCatalogViewModel _themeCatalogViewModel = new ThemeCatalogViewModel();
        private AttribSettingsViewModel _attribSettingsViewModel = new AttribSettingsViewModel();
        
        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public MainWindowViewModel() {}

        public void LoadCurrentView()
        {
            CurrentViewModel = _attribSettingsViewModel;
        }

    }
}
