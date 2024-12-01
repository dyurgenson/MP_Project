using System;
using System.Collections.Generic;
using System.Linq;

namespace YA_Metro.Models
{
    /// <summary>
    /// Класс Schedule представляет собой модель расписания для ветки метрополитена в системе YA_Metro.
    /// Он содержит список элементов расписания, каждый из которых описывает интервалы и остановки для определенного дня недели и времени.
    /// </summary>
    [Serializable]
    public class Schedule
    {
        private readonly List<Schedule.Item> _items;

        /// <summary>
        /// Конструктор класса Schedule.
        /// </summary>
        /// <param name="items">Список элементов расписания.</param>
        public Schedule(List<Schedule.Item> items) => this._items = items ?? new List<Schedule.Item>();

        /// <summary>
        /// Возвращает интервал для указанного дня недели и времени.
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="time">Время.</param>
        /// <returns>Интервал.</returns>
        public int GetInterval(DayOfWeek dayOfWeek, DateTime time) => this.GetItem(dayOfWeek, time).Interval;

        /// <summary>
        /// Возвращает интервал для указанного дня недели и времени (в секундах).
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="time">Время (в секундах).</param>
        /// <returns>Интервал.</returns>
        public int GetInterval(DayOfWeek dayOfWeek, int time) => this.GetItem(dayOfWeek, new DateTime().AddSeconds((double)time)).Interval;

        /// <summary>
        /// Возвращает время остановки для указанного дня недели и времени.
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="time">Время.</param>
        /// <returns>Время остановки.</returns>
        public int GetStay(DayOfWeek dayOfWeek, DateTime time) => this.GetItem(dayOfWeek, time).Stay;

        /// <summary>
        /// Возвращает время остановки для указанного дня недели и времени (в секундах).
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="time">Время (в секундах).</param>
        /// <returns>Время остановки.</returns>
        public int GetStay(DayOfWeek dayOfWeek, int time) => this.GetItem(dayOfWeek, new DateTime().AddSeconds((double)time)).Stay;

        /// <summary>
        /// Возвращает элемент расписания для указанного дня недели и времени.
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="time">Время.</param>
        /// <returns>Элемент расписания.</returns>
        private Schedule.Item GetItem(DayOfWeek dayOfWeek, DateTime time) => this._items.FirstOrDefault<Schedule.Item>((Func<Schedule.Item, bool>)(item => dayOfWeek == item.DayOfWeek && item.From <= time && time <= item.To));

        /// <summary>
        /// Класс Item представляет собой элемент расписания, содержащий информацию о дне недели, времени начала и окончания, интервале и времени остановки.
        /// </summary>
        [Serializable]
        public class Item
        {
            /// <summary>
            /// День недели.
            /// </summary>
            public DayOfWeek DayOfWeek { get; private set; }

            /// <summary>
            /// Время начала интервала.
            /// </summary>
            public DateTime From { get; private set; }

            /// <summary>
            /// Время окончания интервала.
            /// </summary>
            public DateTime To { get; private set; }

            /// <summary>
            /// Интервал между поездами.
            /// </summary>
            public int Interval { get; private set; }

            /// <summary>
            /// Время остановки поезда на станции.
            /// </summary>
            public int Stay { get; private set; }

            /// <summary>
            /// Конструктор класса Item.
            /// </summary>
            /// <param name="dayOfWeek">День недели.</param>
            /// <param name="from">Время начала интервала.</param>
            /// <param name="to">Время окончания интервала.</param>
            /// <param name="interval">Интервал между поездами.</param>
            /// <param name="stay">Время остановки поезда на станции.</param>
            public Item(DayOfWeek dayOfWeek, DateTime from, DateTime to, int interval, int stay)
            {
                this.DayOfWeek = dayOfWeek;
                this.From = from;
                this.To = to;
                this.Interval = interval;
                this.Stay = stay;
            }

            /// <summary>
            /// Конструктор класса Item с использованием времени в секундах.
            /// </summary>
            /// <param name="dayOfWeek">День недели.</param>
            /// <param name="from">Время начала интервала (в секундах).</param>
            /// <param name="to">Время окончания интервала (в секундах).</param>
            /// <param name="interval">Интервал между поездами.</param>
            /// <param name="stay">Время остановки поезда на станции.</param>
            public Item(int dayOfWeek, int from, int to, int interval, int stay)
            {
                this.DayOfWeek = (DayOfWeek)dayOfWeek;
                this.From = new DateTime().AddSeconds((double)from);
                this.To = new DateTime().AddSeconds((double)to);
                this.Interval = interval;
                this.Stay = stay;
            }
        }
    }
}