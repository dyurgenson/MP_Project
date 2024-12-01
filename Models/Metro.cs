using System;
using System.Collections.Generic;
using YA_Metro.Data;

namespace YA_Metro.Models
{
    /// <summary>
    /// Класс Metro представляет собой модель метрополитена в системе YA_Metro.
    /// Он содержит информацию о названии, описании, ветках, времени начала и окончания работы метрополитена.
    /// </summary>
    public class Metro
    {
        /// <summary>
        /// Название метрополитена.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание метрополитена.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Список веток метрополитена.
        /// </summary>
        public List<Branch> Branches { get; private set; }

        /// <summary>
        /// Время начала работы метрополитена.
        /// </summary>
        public DateTime TimeFrom { get; private set; }

        /// <summary>
        /// Время окончания работы метрополитена.
        /// </summary>
        public DateTime TimeTo { get; private set; }

        /// <summary>
        /// Конструктор класса Metro.
        /// </summary>
        /// <param name="name">Название метрополитена.</param>
        /// <param name="description">Описание метрополитена.</param>
        /// <param name="branches">Список веток метрополитена.</param>
        /// <param name="timeFrom">Время начала работы метрополитена.</param>
        /// <param name="timeTo">Время окончания работы метрополитена.</param>
        public Metro(
          string name,
          string description,
          List<Branch> branches,
          DateTime timeFrom,
          DateTime timeTo)
        {
            this.Name = name;
            this.Description = description;
            this.Branches = branches ?? new List<Branch>();
            this.TimeFrom = timeFrom;
            this.TimeTo = timeTo;
        }

        /// <summary>
        /// Создает объект Metro, используя данные из репозитория.
        /// </summary>
        /// <param name="repository">Репозиторий, содержащий данные о метрополитене.</param>
        /// <returns>Объект Metro или null в случае ошибки.</returns>
        public static Metro Create(IRepository repository)
        {
            try
            {
                return repository.GetMetro();
            }
            catch (Exception ex)
            {
                return (Metro)null;
            }
        }
    }
}