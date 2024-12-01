using System;
using System.Globalization;
using System.Windows.Data;

namespace YA_Metro.UI.Converters
{
    /// <summary>
    /// Этот класс реализует интерфейс IValueConverter для преобразования между типами DateTime и int.
    /// Значение int представляет общее количество минут, прошедших с момента DateTime.MinValue.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(int))]
    public class DateTimeIntConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует объект DateTime в int, представляющий общее количество минут с момента DateTime.MinValue.
        /// </summary>
        /// <param name="value">Значение, которое нужно преобразовать.</param>
        /// <param name="targetType">Тип, к которому нужно преобразовать значение.</param>
        /// <param name="parameter">Дополнительный параметр преобразования.</param>
        /// <param name="culture">Культура, используемая при преобразовании.</param>
        /// <returns>Общее количество минут с момента DateTime.MinValue.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)(int)DateTime.Parse(value.ToString()).Subtract(DateTime.MinValue).TotalMinutes;
        }

        /// <summary>
        /// Преобразует int обратно в DateTime, добавляя указанное количество минут к DateTime.MinValue.
        /// </summary>
        /// <param name="value">Значение, которое нужно преобразовать.</param>
        /// <param name="targetType">Тип, к которому нужно преобразовать значение.</param>
        /// <param name="parameter">Дополнительный параметр преобразования.</param>
        /// <param name="culture">Культура, используемая при преобразовании.</param>
        /// <returns>Объект DateTime, представляющий дату и время, соответствующие указанному количеству минут.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)new DateTime().AddMinutes((double)int.Parse(value.ToString()));
        }
    }
}