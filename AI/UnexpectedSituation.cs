using System;

namespace YA_Metro.AI
{
    /// <summary>
    /// Класс UnexpectedSituation реализует интерфейс ISituation и представляет собой модель для генерации непредвиденных задержек в системе YA_Metro.
    /// Он использует случайные числа для определения продолжительности задержек в зависимости от дня недели и времени суток.
    /// </summary>
    internal class UnexpectedSituation : ISituation
    {
        private static Random rnd = new Random();
        private DayOfWeek DayOfWeek;

        /// <summary>
        /// Конструктор класса UnexpectedSituation.
        /// </summary>
        /// <param name="DayOfWeek">День недели, для которого генерируются задержки.</param>
        public UnexpectedSituation(DayOfWeek DayOfWeek) => this.DayOfWeek = DayOfWeek;

        /// <summary>
        /// Метод GetUnexpectedDelay возвращает объект UnexpectedDelay, содержащий информацию о непредвиденной задержке на основе текущего времени.
        /// </summary>
        /// <param name="CurrentTime">Текущее время, которое используется для определения задержки.</param>
        /// <returns>Объект UnexpectedDelay, содержащий сообщение о задержке и её продолжительность.</returns>
        public UnexpectedDelay GetUnexpectedDelay(DateTime CurrentTime)
        {
            int num = UnexpectedSituation.rnd.Next(1, 11);
            if (this.DayOfWeek == DayOfWeek.Saturday || this.DayOfWeek == DayOfWeek.Sunday || CurrentTime.Hour >= 6 && CurrentTime.Hour <= 18)
            {
                if (num <= 2)
                    return new UnexpectedDelay("", UnexpectedSituation.rnd.Next(50, 61));
                if (num > 2 && num <= 6)
                    return new UnexpectedDelay("", UnexpectedSituation.rnd.Next(20, 31));
                return num > 6 && num <= 8 ? new UnexpectedDelay("", UnexpectedSituation.rnd.Next(30, 51)) : new UnexpectedDelay("", 0);
            }
            if (num <= 5)
                return new UnexpectedDelay("", UnexpectedSituation.rnd.Next(50, 61));
            if (num > 5 && num <= 6)
                return new UnexpectedDelay("", UnexpectedSituation.rnd.Next(20, 31));
            return num > 6 && num <= 9 ? new UnexpectedDelay("", UnexpectedSituation.rnd.Next(30, 51)) : new UnexpectedDelay("", 0);
        }
    }
}