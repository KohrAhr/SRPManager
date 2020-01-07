using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SRPManagerV2.Functions
{
    public static class ImageFunctions
    {
        public static ImageSource GetIconFromResource(string icon)
        {
            return (ImageSource)new ImageSourceConverter().ConvertFromString($"pack://application:,,,/Resources/Bitmaps/{icon}.ico");
        }
    }
}
