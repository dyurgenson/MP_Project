using System.Collections.Generic;
using YA_Metro.Models;

namespace YA_Metro.Data
{
    /// <summary>
    /// Интерфейс IRepository определяет контракт для классов, которые предоставляют доступ к данным метрополитена.
    /// Он включает методы для получения информации о метрополитене, ветках, станциях, участках и расписании.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Возвращает объект Metro, содержащий информацию о метрополитене.
        /// </summary>
        /// <returns>Объект Metro.</returns>
        Metro GetMetro();

        /// <summary>
        /// Возвращает коллекцию объектов Branch, содержащих информацию о ветках метрополитена.
        /// </summary>
        /// <returns>Коллекция объектов Branch.</returns>
        IEnumerable<Branch> GetBranches();

        /// <summary>
        /// Возвращает коллекцию объектов Station, содержащих информацию о станциях метрополитена.
        /// </summary>
        /// <returns>Коллекция объектов Station.</returns>
        IEnumerable<Station> GetStations();

        /// <summary>
        /// Возвращает коллекцию объектов Section, содержащих информацию о участках метрополитена.
        /// </summary>
        /// <returns>Коллекция объектов Section.</returns>
        IEnumerable<Section> GetSections();

        /// <summary>
        /// Возвращает объект Schedule, содержащий расписание для указанной ветки метрополитена.
        /// </summary>
        /// <param name="branchId">Идентификатор ветки метрополитена.</param>
        /// <returns>Объект Schedule.</returns>
        Schedule GetSchedule(int branchId);
    }
}