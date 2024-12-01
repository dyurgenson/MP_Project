using System;
using YA_Metro.Enums;

namespace YA_Metro.Models
{
    /// <summary>
    /// Класс Section представляет собой модель участка метрополитена в системе YA_Metro.
    /// Он содержит информацию о идентификаторе, времени прохождения, состоянии, начальной и конечной станциях участка.
    /// </summary>
    [Serializable]
    public class Section
    {
        /// <summary>
        /// Идентификатор участка.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Время прохождения участка (в секундах).
        /// </summary>
        public int Time { get; private set; }

        /// <summary>
        /// Состояние участка (работает, на обслуживании, в разработке).
        /// </summary>
        public AvailableState State { get; private set; }

        /// <summary>
        /// Начальная станция участка.
        /// </summary>
        public Station From { get; private set; }

        /// <summary>
        /// Конечная станция участка.
        /// </summary>
        public Station To { get; private set; }

        /// <summary>
        /// Конструктор класса Section.
        /// </summary>
        /// <param name="id">Идентификатор участка.</param>
        /// <param name="time">Время прохождения участка (в секундах).</param>
        /// <param name="from">Начальная станция участка.</param>
        /// <param name="to">Конечная станция участка.</param>
        /// <param name="state">Состояние участка (по умолчанию - Working).</param>
        public Section(int id, int time, Station from, Station to, AvailableState state = AvailableState.Working)
        {
            this.Id = id;
            this.Time = time;
            this.From = from;
            this.To = to;
            this.State = state;
        }
    }
}