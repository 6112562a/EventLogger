using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFComponent.Extension
{
    public class EnumToItemsSource : System.Windows.Markup.MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_type)
                .Cast<object>()
                .Select(e => new { Value = e, DisplayName = e.ToString() });
        }
    }
}
