using Khsw.Instrument.Demo.Infrastructures;
using Khsw.Instrument.Demo.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.ViewModels
{
    /// <summary>
    /// 主界面ViewModel
    /// </summary>
    public class ShellViewModel : BindableBase
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        #endregion

        #region Properties
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        #endregion

        public ShellViewModel(IModuleManager moduleManager, IRegionManager regionManager, IDialogService dialogService)
        {
            _moduleManager = moduleManager;
            _regionManager = regionManager;
            _dialogService = dialogService;

            _title = "调试Demo";

            InitRegion();
        }

        private void InitRegion()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ControlDemoContentRegion, typeof(ControlDemoView));
            _regionManager.RegisterViewWithRegion(RegionNames.OtherDemoContentRegion, typeof(OtherDemoView));


        }
    }
}
