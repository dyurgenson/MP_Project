namespace YA_Metro.Enums
{
    /// <summary>
    /// Перечисление ModelingState представляет собой состояния, в которых может находиться процесс моделирования в системе YA_Metro.
    /// </summary>
    public enum ModelingState
    {
        /// <summary>
        /// Состояние, когда моделирование не запущено.
        /// </summary>
        NotRunning,

        /// <summary>
        /// Состояние, когда моделирование запущено и выполняется.
        /// </summary>
        Running,

        /// <summary>
        /// Состояние, когда моделирование приостановлено.
        /// </summary>
        Pause,
    }
}