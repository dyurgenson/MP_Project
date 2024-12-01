using System;

namespace YA_Metro.Drawing
{
    /// <summary>
    /// Класс TextStyle представляет собой модель стиля текста, включая отступы.
    /// Он используется для настройки отображения текста в системе YA_Metro.
    /// </summary>
    [Serializable]
    public class TextStyle
    {
        /// <summary>
        /// Отступы текста.
        /// </summary>
        public PointD Margin { get; private set; }

        /// <summary>
        /// Возвращает стиль текста по умолчанию.
        /// </summary>
        public static TextStyle Default => new TextStyle(new PointD(8.0, -10.0));

        /// <summary>
        /// Конструктор класса TextStyle.
        /// </summary>
        /// <param name="margin">Отступы текста.</param>
        public TextStyle(PointD margin) => this.Margin = margin;
    }
}