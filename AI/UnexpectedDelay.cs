namespace YA_Metro.AI
{
    /// <summary>
    /// Класс UnexpectedDelay представляет собой модель для хранения информации о непредвиденной задержке.
    /// Он используется для логирования, оповещения пользователей или других целей, связанных с обработкой и отображением информации о задержках.
    /// </summary>
    public class UnexpectedDelay
    {
        /// <summary>
        /// Сообщение, описывающее причину задержки.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Значение задержки, скорее всего, в секундах или другой единице времени.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Конструктор класса UnexpectedDelay.
        /// </summary>
        /// <param name="message">Сообщение, описывающее причину задержки.</param>
        /// <param name="delay">Значение задержки.</param>
        public UnexpectedDelay(string message, int delay)
        {
            this.Message = message;
            this.Delay = delay;
        }
    }
}