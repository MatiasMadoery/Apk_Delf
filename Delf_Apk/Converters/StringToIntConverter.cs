using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Delf_Apk.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue.ToString(); // Convierte el int a string para mostrarlo en el Entry
            return string.Empty; // Devuelve vacío si no es un int válido
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && int.TryParse(stringValue, out int intValue))
                return intValue; // Convierte el string a int al guardar en el ViewModel
            return 0; // Devuelve 0 si no se puede convertir
        }
    }
}
