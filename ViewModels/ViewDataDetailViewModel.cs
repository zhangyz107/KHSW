using AutoMapper;
using Khsw.Instrument.Demo.Commons.Helper;
using Khsw.Instrument.Demo.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khsw.Instrument.Demo.ViewModels
{
    public class ViewDataDetailViewModel : BindableBase, IDialogAware
    {
        #region Fields
        private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _container;
        private readonly IDialogService _dialogService;
        private string _commandContent;
        private string _title = "查看数据详情";
        private int _maxByte = 32;
        #endregion

        #region Properties

        public ObservableCollection<MessageDataModel> MessageList { get; } = new ObservableCollection<MessageDataModel>();

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion

        public ViewDataDetailViewModel(
            IContainerExtension container,
            IModuleManager moduleManager,
            IRegionManager regionManager,
            IDialogService dialogService)
        {
            _container = container;
            _moduleManager = moduleManager;
            _regionManager = regionManager;
            _dialogService = dialogService;
        }

        public DialogCloseListener RequestClose { get; }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _commandContent = parameters.GetValue<string>("commandContent");
            if (!string.IsNullOrEmpty(_commandContent))
            {
                try
                {
                    var content = _commandContent.ToByteArray().AsSpan();
                    if (content.Length > 0)
                    {
                        var rowNumbers = content.Length % _maxByte != 0 ? content.Length / _maxByte + 1 : content.Length / _maxByte;
                        for (int i = 0; i < rowNumbers; i++)
                        {
                            var rowData = i == rowNumbers - 1 ? content.Slice(i * _maxByte) : content.Slice(i * _maxByte, _maxByte);
                            var messageContent = rowData.ToArray().ToAppendString();
                            var message = new MessageDataModel()
                            {
                                Title = $"{i * _maxByte}-{i * _maxByte + rowData.Length - 1}",
                                Content = messageContent,
                            };
                            MessageList.Add(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    _dialogService.ShowDialog("AlertDialog", new DialogParameters($"message={e.Message}"));
                }
            }
        }
    }
}
