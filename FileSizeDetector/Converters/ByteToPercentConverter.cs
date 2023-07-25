using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileSizeDetector.Converters
{
    internal class ByteToPercentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            long sizeOfDrive = (long)values[0];
            long sizeOfFolderOrFile = (long)values[1];

            return $"{((double)sizeOfFolderOrFile / (double)sizeOfDrive*100).ToString("0.00")} %";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}