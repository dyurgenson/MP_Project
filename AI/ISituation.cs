using System;

namespace YA_Metro.AI
{
    /// <summary>
    /// Интерфейс ISituation определяет контракт для классов, которые могут возвращать информацию о непредвиденных задержках.
    /// </summary>
    public interface ISituation
    {
        /// <summary>
        /// Метод GetUnexpectedDelay возвращает объект UnexpectedDelay, содержащий информацию о непредвиденной задержке на основе текущего времени.
        /// </summary>
        /// <param name="CurrentTime">Текущее время, которое используется для определения задержки.</param>
        /// <returns>Объект UnexpectedDelay, содержащий сообщение о задержке и её продолжительность.</returns>
        UnexpectedDelay GetUnexpectedDelay(DateTime CurrentTime);
    }
}