using Khsw.Instrument.Demo.DataModels;
using Khsw.Instrument.Demo.ViewModels;
using Khsw.Instrument.Demo.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Khsw.Instrument.Demo.Views
{
    /// <summary>
    /// ControlDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class ControlDemoView : UserControl
    {
        public ControlDemoView(IRegionManager regionManager)
        {
            InitializeComponent();

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
        {
            ViewModel?.SaveData();
        }

        public ControlDemoViewModel ViewModel
        {
            get
            {
                return DataContext as ControlDemoViewModel;
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var dataItem = e.Row.Item as CommandDataModel;
            // 如果列是 DataGridTemplateColumn 并且只读，取消编辑模式
            if (dataItem != null && dataItem.IsReadOnly)
            {
                e.Cancel = true;
            }
        }
    }
}
