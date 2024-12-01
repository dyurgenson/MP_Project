using System;
using System.Collections.Generic;

namespace YA_Metro.Utilities.Logger
{
    /// <summary>
    /// Интерфейс для реализации логгера.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Записывает запись лога.
        /// </summary>
        /// <param name="logEntry">Запись лога для записи.</param>
        void Write(LogEntry logEntry);

        /// <summary>
        /// Записывает сообщение в лог с указанными параметрами.
        /// </summary>
        /// <param name="message">Сообщение для записи.</param>
        /// <param name="dateTime">Дата и время записи.</param>
        /// <param name="severity">Серьезность записи (по умолчанию Severity.Info).</param>
        void Write(string message, DateTime dateTime, Severity severity = Severity.Info);

        /// <summary>
        /// Возвращает все записи лога.
        /// </summary>
        /// <returns>Перечисление записей лога.</returns>
        IEnumerable<LogEntry> GetLogEntries();

        /// <summary>
        /// Очищает все записи лога.
        /// </summary>
        void Clear();
    }
}