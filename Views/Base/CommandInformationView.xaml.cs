using Khsw.Instrument.Demo.Models;
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

namespace Khsw.Instrument.Demo.Views.Base
{
    /// <summary>
    /// CommandInformationView.xaml 的交互逻辑
    /// </summary>
    public partial class CommandInformationView : UserControl
    {
        public CommandInformationView()
        {
            InitializeComponent();
        }

        public void AppendWriteLine(RecordMessageDataModel message)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ViewModel?.AppendWriteLine(message);
            }));

        }

        public CommandInformationViewModel ViewModel
        {
            get
            {
                return DataContext as CommandInformationViewModel;
            }
        }
    }
}
