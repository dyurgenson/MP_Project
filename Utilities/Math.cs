using YA_Metro.Drawing;

namespace YA_Metro.Utilities
{
    /// <summary>
    /// Статический класс, предоставляющий математические функции для работы с векторами и точками.
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// Вычисляет длину вектора между двумя точками.
        /// </summary>
        /// <param name="startPoint">Начальная точка вектора.</param>
        /// <param name="endPoint">Конечная точка вектора.</param>
        /// <returns>Длина вектора.</returns>
        public static double VectorLength(PointD startPoint, PointD endPoint)
        {
            return System.Math.Sqrt(System.Math.Pow(endPoint.X - startPoint.X, 2.0) + System.Math.Pow(endPoint.Y - startPoint.Y, 2.0));
        }

        /// <summary>
        /// Вычисляет координаты вектора между двумя точками.
        /// </summary>
        /// <param name="startPoint">Начальная точка вектора.</param>
        /// <param name="endPoint">Конечная точка вектора.</param>
        /// <returns>Координаты вектора.</returns>
        public static PointD VectorCoords(PointD startPoint, PointD endPoint)
        {
            return new PointD(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
        }

        /// <summary>
        /// Вычисляет координаты точки на векторе, находящейся на заданном расстоянии от начальной точки.
        /// </summary>
        /// <param name="startPoint">Начальная точка вектора.</param>
        /// <param name="endPoint">Конечная точка вектора.</param>
        /// <param name="ratio">Отношение расстояния от начальной точки до искомой точки к длине вектора.</param>
        /// <returns>Координаты точки на векторе.</returns>
        public static PointD PointCoordsOnVector(PointD startPoint, PointD endPoint, double ratio)
        {
            return new PointD(startPoint.X + ratio * Math.VectorCoords(startPoint, endPoint).X, startPoint.Y + ratio * Math.VectorCoords(startPoint, endPoint).Y);
        }

        /// <summary>
        /// Вычисляет ортогональный вектор к заданному вектору.
        /// </summary>
        /// <param name="startPoint">Начальная точка вектора.</param>
        /// <param name="endPoint">Конечная точка вектора.</param>
        /// <returns>Координаты ортогонального вектора.</returns>
        public static PointD OrthogonalVector(PointD startPoint, PointD endPoint)
        {
            PointD vectorCoords = Math.VectorCoords(startPoint, endPoint);
            return vectorCoords.Y != 0.0 ? new PointD(1.0, -vectorCoords.X / vectorCoords.Y) : new PointD(0.0, 1.0);
        }

        /// <summary>
        /// Вычисляет угол между вектором и осью X.
        /// </summary>
        /// <param name="vectorCoords">Координаты вектора.</param>
        /// <returns>Угол между вектором и осью X в радианах.</returns>
        public static double AngleBetweenVectorAndXAxis(PointD vectorCoords)
        {
            PointD xAxisVector = new PointD(1.0, 0.0);
            return System.Math.Acos((vectorCoords.X * xAxisVector.X + vectorCoords.Y * xAxisVector.Y) /
                                    (System.Math.Sqrt(System.Math.Pow(xAxisVector.X, 2.0) + System.Math.Pow(xAxisVector.Y, 2.0)) *
                                     System.Math.Sqrt(System.Math.Pow(vectorCoords.X, 2.0) + System.Math.Pow(vectorCoords.Y, 2.0))));
        }
    }
}