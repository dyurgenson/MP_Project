namespace YA_Metro.Enums
{
    /// <summary>
    /// Перечисление TrainState представляет собой состояния, в которых может находиться поезд в системе YA_Metro.
    /// </summary>
    public enum TrainState
    {
        /// <summary>
        /// Состояние, когда поезд находится в движении.
        /// </summary>
        Moving,

        /// <summary>
        /// Состояние, когда поезд останавливается на станции.
        /// </summary>
        Stopping,

        /// <summary>
        /// Состояние, когда поезд находится на парковке (например, в депо).
        /// </summary>
        Parking,
    }
}