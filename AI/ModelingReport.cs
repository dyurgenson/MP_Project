﻿using System;
using System.Collections.Generic;
using YA_Metro.Models;
using YA_Metro.Utilities.Logger;

namespace YA_Metro.AI
{
    /// <summary>
    /// Класс ModelingReport представляет собой модель отчета о моделировании движения поездов в системе YA_Metro.
    /// Он содержит информацию о дне недели, времени начала и окончания моделирования, логах, шаге моделирования, ветке и поездах.
    /// </summary>
    public class ModelingReport
    {
        /// <summary>
        /// День недели, в который проводилось моделирование.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Время начала моделирования.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания моделирования.
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// Коллекция записей лога, сгенерированных во время моделирования.
        /// </summary>
        public IEnumerable<LogEntry> LogEntries { get; set; }

        /// <summary>
        /// Шаг моделирования (интервал времени между обновлениями).
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Ветка метро, для которой проводилось моделирование.
        /// </summary>
        public Branch Branch { get; set; }

        /// <summary>
        /// Коллекция поездов, участвующих в моделировании.
        /// </summary>
        public IEnumerable<Train> Trains { get; set; }

        /// <summary>
        /// Сохраняет отчет в формате PDF.
        /// </summary>
        /// <param name="fileName">Имя файла для сохранения.</param>
        public void SaveAsPdf(string fileName)
        {
            // Реализация сохранения отчета в формате PDF.
        }

        /// <summary>
        /// Сохраняет отчет в формате XLS (Excel).
        /// </summary>
        /// <param name="fileName">Имя файла для сохранения.</param>
        public void SaveAsXls(string fileName)
        {
            // Реализация сохранения отчета в формате XLS.
        }

        /// <summary>
        /// Сохраняет отчет в формате TXT.
        /// </summary>
        /// <param name="fileName">Имя файла для сохранения.</param>
        public void SaveAsTxt(string fileName)
        {
            // Реализация сохранения отчета в формате TXT.
        }
    }
}