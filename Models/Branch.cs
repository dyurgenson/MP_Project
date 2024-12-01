using System;
using System.Collections.Generic;
using System.Drawing;

namespace YA_Metro.Models
{
    /// <summary>
    /// Класс Branch представляет собой модель ветки метрополитена в системе YA_Metro.
    /// Он содержит информацию о идентификаторе, названии, станциях, участках, цвете и расписании ветки.
    /// </summary>
    [Serializable]
    public class Branch
    {
        /// <summary>
        /// Идентификатор ветки.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Название ветки.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Список станций, входящих в ветку.
        /// </summary>
        public List<Station> Stations { get; private set; }

        /// <summary>
        /// Список участков, входящих в ветку.
        /// </summary>
        public List<Section> Sections { get; private set; }

        /// <summary>
        /// Цвет ветки.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Расписание ветки.
        /// </summary>
        public Schedule Schedule { get; private set; }

        /// <summary>
        /// Конструктор класса Branch.
        /// </summary>
        /// <param name="id">Идентификатор ветки.</param>
        /// <param name="name">Название ветки.</param>
        /// <param name="stations">Список станций, входящих в ветку.</param>
        /// <param name="sections">Список участков, входящих в ветку.</param>
        /// <param name="color">Цвет ветки.</param>
        /// <param name="schedule">Расписание ветки.</param>
        public Branch(
          int id,
          string name,
          List<Station> stations,
          List<Section> sections,
          Color color,
          Schedule schedule)
        {
            this.Id = id;
            this.Name = name;
            this.Stations = stations ?? new List<Station>();
            this.Sections = sections ?? new List<Section>();
            this.Color = color;
            this.Schedule = schedule;
        }
    }
}