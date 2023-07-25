using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileSizeDetector.Converters
{
    internal class ByteToGigaByteStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long longValue)
            {
                var mbvalue = (double)longValue / 1048576;
                if (mbvalue > 400)
                    return $"{((double)longValue / 1073741824).ToString("0.00")} GB";
                else
                    return $"{mbvalue.ToString("0.00")} MB";
            }
            else
                return "unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}