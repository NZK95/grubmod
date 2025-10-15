using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace grubmod
{
    internal class ComboOrTextTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComboTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate CheckBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Option option)
            {
                switch (option.Fields.OptionType)
                {
                    case var s when s == Labels.NORMAL_OPTION_DEFINITION: return ComboTemplate;
                    case var s when s == Labels.NUMERIC_OPTION_DEFINITION: return TextTemplate;
                    case var s when s == Labels.CHECKBOX_OPTION_DEFINITION: return CheckBoxTemplate;
                    default: return base.SelectTemplate(item, container);
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
