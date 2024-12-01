using System;
using YA_Metro.Drawing;
using YA_Metro.Enums;

namespace YA_Metro.Models
{
    /// <summary>
    /// Класс Train представляет собой модель поезда в системе YA_Metro.
    /// Он содержит информацию о идентификаторе, скорости, ветке, начальной станции, направлении, состоянии, предыдущем поезде,
    /// текущей станции, текущем участке, координатах, угле наклона, текущем расстоянии и оставшемся времени остановки.
    /// </summary>
    [Serializable]
    public class Train
    {
        /// <summary>
        /// Идентификатор поезда.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Скорость поезда.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Ветка, на которой находится поезд.
        /// </summary>
        public Branch Branch { get; private set; }

        /// <summary>
        /// Начальная станция поезда.
        /// </summary>
        public Station StartStation { get; set; }

        /// <summary>
        /// Направление движения поезда.
        /// </summary>
        public TrainDirection Direction { get; set; }

        /// <summary>
        /// Состояние поезда (движется, остановлен, на парковке).
        /// </summary>
        public TrainState State { get; set; }

        /// <summary>
        /// Предыдущий поезд на линии.
        /// </summary>
        public Train PreviuosTrain { get; set; }

        /// <summary>
        /// Текущая станция поезда.
        /// </summary>
        public Station CurrentStation { get; set; }

        /// <summary>
        /// Текущий участок, по которому движется поезд.
        /// </summary>
        public Section CurrentSection { get; set; }

        /// <summary>
        /// Координаты поезда.
        /// </summary>
        public PointD Coord { get; set; }

        /// <summary>
        /// Угол наклона поезда.
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Текущее расстояние, пройденное поездом.
        /// </summary>
        public double CurrentDistanse { get; set; }

        /// <summary>
        /// Оставшееся время остановки поезда.
        /// </summary>
        public double RemainingTimeStop { get; set; }

        /// <summary>
        /// Конструктор класса Train.
        /// </summary>
        /// <param name="id">Идентификатор поезда.</param>
        /// <param name="speed">Скорость поезда.</param>
        /// <param name="branch">Ветка, на которой находится поезд.</param>
        /// <param name="startStation">Начальная станция поезда.</param>
        /// <param name="direction">Направление движения поезда (по умолчанию Direct).</param>
        /// <param name="state">Состояние поезда (по умолчанию Parking).</param>
        /// <param name="coord">Координаты поезда (по умолчанию (0, 0)).</param>
        /// <param name="angle">Угол наклона поезда (по умолчанию 0).</param>
        public Train(
          int id,
          double speed,
          Branch branch,
          Station startStation,
          TrainDirection direction = TrainDirection.Direct,
          TrainState state = TrainState.Parking,
          PointD coord = default(PointD),
          double angle = 0.0)
        {
            this.Id = id;
            this.Speed = speed;
            this.Branch = branch;
            this.StartStation = startStation;
            this.Direction = direction;
            this.State = state;
            this.Coord = coord;
            this.Angle = angle;
        }

        /// <summary>
        /// Устанавливает значения по умолчанию для свойств поезда.
        /// </summary>
        public void SetDefaultValues()
        {
            this.Coord = new PointD(0.0, 0.0);
            this.Angle = 0.0;
            this.Speed = 0.0;
            this.State = TrainState.Parking;
            this.CurrentStation = this.StartStation;
            this.CurrentSection = (Section)null;
        }
    }
}