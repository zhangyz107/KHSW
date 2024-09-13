using Khsw.Instrument.Demo.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Khsw.Instrument.Demo.Views
{
    public class ControlDemoViewSelector : DataTemplateSelector
    {
        public DataTemplate TextBoxTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate ButtonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return base.SelectTemplate(item, container);

            if (item is CommandDataModel command)
                switch (command.InputMode)
                {
                    case Commons.Enums.InputModeEnum.Direct:
                        return TextBoxTemplate;
                    case Commons.Enums.InputModeEnum.Combobox:
                        return ComboBoxTemplate;
                    case Commons.Enums.InputModeEnum.Dialog:
                        return ButtonTemplate;
                }

            return base.SelectTemplate(item, container);
        }
    }
}
