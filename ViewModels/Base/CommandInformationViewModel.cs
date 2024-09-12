using Khsw.Instrument.Demo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.ViewModels.Base
{
    public class CommandInformationViewModel : BindableBase
    {
        #region Fields
        private readonly IContainerExtension _container;
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        #endregion

        #region Properties
        public ObservableCollection<RecordMessageDataModel> LogInfos { get; } = new ObservableCollection<RecordMessageDataModel>();
        #endregion

        #region Commands
        private DelegateCommand _clearLogCommand;
        public DelegateCommand ClearLogCommand =>
            _clearLogCommand ?? (_clearLogCommand = new DelegateCommand(ExecuteClearLogCommand));
        #endregion

        public CommandInformationViewModel(IContainerExtension container, IModuleManager moduleManager, IRegionManager regionManager)
        {
            _container = container;
            _moduleManager = moduleManager;
            _regionManager = regionManager;
        }

        public void AppendWriteLine(RecordMessageDataModel message)
        {
            LogInfos.Add(message);
        }

        private void ExecuteClearLogCommand()
        {
            LogInfos.Clear();
        }
    }
}
