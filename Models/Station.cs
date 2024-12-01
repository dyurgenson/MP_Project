using System;
using YA_Metro.Drawing;
using YA_Metro.Enums;

namespace YA_Metro.Models
{
    /// <summary>
    /// Класс Station представляет собой модель станции метрополитена в системе YA_Metro.
    /// Он содержит информацию о идентификаторе, названии, координатах, состоянии и стиле текста станции.
    /// </summary>
    [Serializable]
    public class Station
    {
        /// <summary>
        /// Идентификатор станции.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Название станции.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Координаты станции.
        /// </summary>
        public PointD Coord { get; private set; }

        /// <summary>
        /// Состояние станции (работает, на обслуживании, в разработке).
        /// </summary>
        public AvailableState State { get; private set; }

        /// <summary>
        /// Стиль текста для названия станции.
        /// </summary>
        public TextStyle TextStyle { get; private set; }

        /// <summary>
        /// Координаты названия станции.
        /// </summary>
        public int CoordsOfName { get; private set; }

        /// <summary>
        /// Конструктор класса Station.
        /// </summary>
        /// <param name="id">Идентификатор станции.</param>
        /// <param name="name">Название станции.</param>
        /// <param name="coord">Координаты станции.</param>
        /// <param name="coords_of_name">Координаты названия станции (по умолчанию 0).</param>
        /// <param name="state">Состояние станции (по умолчанию Working).</param>
        /// <param name="textStyle">Стиль текста для названия станции (по умолчанию используется TextStyle.Default).</param>
        public Station(int id, string name, PointD coord, int coords_of_name = 0, AvailableState state = AvailableState.Working, TextStyle textStyle = null)
        {
            this.Id = id;
            this.Name = name;
            this.Coord = coord;
            this.CoordsOfName = coords_of_name;
            this.State = state;
            this.TextStyle = textStyle ?? TextStyle.Default;
        }
    }
}