using System;

namespace YA_Metro.Utilities.Logger
{
    /// <summary>
    /// Класс, представляющий запись лога.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Дата и время записи лога.
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Серьезность записи лога.
        /// </summary>
        public Severity Severity { get; private set; }

        /// <summary>
        /// Сообщение записи лога.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Конструктор для создания записи лога с указанными параметрами.
        /// </summary>
        /// <param name="message">Сообщение записи лога.</param>
        /// <param name="dateTime">Дата и время записи лога.</param>
        /// <param name="severity">Серьезность записи лога (по умолчанию Severity.Info).</param>
        public LogEntry(string message, DateTime dateTime, Severity severity = Severity.Info)
        {
            this.Message = message;
            this.DateTime = dateTime;
            this.Severity = severity;
        }

        /// <summary>
        /// Конструктор для создания записи лога с текущим временем.
        /// </summary>
        /// <param name="message">Сообщение записи лога.</param>
        /// <param name="severity">Серьезность записи лога (по умолчанию Severity.Info).</param>
        public LogEntry(string message, Severity severity = Severity.Info)
          : this(message, DateTime.Now, severity)
        {
        }

        /// <summary>
        /// Возвращает строковое представление записи лога в формате [UTC DateTime] Message.
        /// </summary>
        /// <returns>Строковое представление записи лога.</returns>
        public override string ToString() => string.Format("[{0}] {1}", this.DateTime.ToUniversalTime(), this.Message);

        /// <summary>
        /// Возвращает строковое представление записи лога с использованием указанного формата даты.
        /// </summary>
        /// <param name="dateMask">Формат даты.</param>
        /// <returns>Строковое представление записи лога.</returns>
        public string ToString(string dateMask) => string.Format("[{0}] {1}", this.DateTime.ToString(dateMask), this.Message);
    }
}