using System;
using System.Collections.Generic;

namespace YA_Metro.Utilities.Logger
{
    /// <summary>
    /// Класс, реализующий интерфейс ILogger для записи и управления записями лога.
    /// </summary>
    public class Logger : ILogger
    {
        private readonly List<LogEntry> _logEntries;

        /// <summary>
        /// Событие, возникающее при добавлении новой записи лога.
        /// </summary>
        public event NewLogEntryHandler OnNewLogEntry;

        /// <summary>
        /// Конструктор класса Logger.
        /// </summary>
        public Logger() => this._logEntries = new List<LogEntry>();

        /// <summary>
        /// Записывает новую запись лога.
        /// </summary>
        /// <param name="logEntry">Запись лога для записи.</param>
        public void Write(LogEntry logEntry)
        {
            this._logEntries.Add(logEntry);
            if (this.OnNewLogEntry == null)
                return;
            this.OnNewLogEntry(logEntry);
        }

        /// <summary>
        /// Записывает сообщение в лог с указанными параметрами.
        /// </summary>
        /// <param name="message">Сообщение для записи.</param>
        /// <param name="dateTime">Дата и время записи.</param>
        /// <param name="severity">Серьезность записи (по умолчанию Severity.Info).</param>
        public void Write(string message, DateTime dateTime, Severity severity = Severity.Info) => this.Write(new LogEntry(message, dateTime, severity));

        /// <summary>
        /// Возвращает все записи лога.
        /// </summary>
        /// <returns>Перечисление записей лога.</returns>
        public IEnumerable<LogEntry> GetLogEntries() => (IEnumerable<LogEntry>)this._logEntries;

        /// <summary>
        /// Очищает все записи лога.
        /// </summary>
        public void Clear() => this._logEntries.Clear();

        /// <summary>
        /// Делегат для обработки события добавления новой записи лога.
        /// </summary>
        /// <param name="logEntry">Запись лога, которая была добавлена.</param>
        public delegate void NewLogEntryHandler(LogEntry logEntry);
    }
}