using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFComponent.Common;

namespace WPFComponent.MarkupExtension
{
    public class ImageSourceExtension : System.Windows.Markup.MarkupExtension
    {
        public ImageSourceExtension()
        {

        }

        public ImageSourceExtension(string key)
        {
            Key = key;
        }
        public string Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var keys = Key.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            return DrawingHelper.Convert2Image(keys);
        }
    }
}
