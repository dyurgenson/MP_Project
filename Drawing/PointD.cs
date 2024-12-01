using System;

namespace YA_Metro.Drawing
{
    /// <summary>
    /// Структура PointD представляет собой точку на плоскости с координатами типа double.
    /// Она предоставляет методы для сравнения точек, вычисления хеш-кода и арифметических операций над точками.
    /// </summary>
    [Serializable]
    public struct PointD
    {
        /// <summary>
        /// Координата X точки.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата Y точки.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Возвращает пустую точку (0, 0).
        /// </summary>
        public static PointD Empty => new PointD(0.0, 0.0);

        /// <summary>
        /// Конструктор структуры PointD.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        public PointD(double x, double y)
          : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Переопределение метода Equals для сравнения точек.
        /// </summary>
        /// <param name="obj">Объект для сравнения.</param>
        /// <returns>True, если точки равны, иначе False.</returns>
        public override bool Equals(object obj) => obj is PointD pointD && this == pointD;

        /// <summary>
        /// Переопределение метода GetHashCode для вычисления хеш-кода точки.
        /// </summary>
        /// <returns>Хеш-код точки.</returns>
        public override int GetHashCode()
        {
            double num = this.X;
            int hashCode1 = num.GetHashCode();
            num = this.Y;
            int hashCode2 = num.GetHashCode();
            return hashCode1 ^ hashCode2;
        }

        /// <summary>
        /// Оператор равенства для сравнения двух точек.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>True, если точки равны, иначе False.</returns>
        public static bool operator ==(PointD a, PointD b) => a.X == b.X && a.Y == b.Y;

        /// <summary>
        /// Оператор неравенства для сравнения двух точек.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>True, если точки не равны, иначе False.</returns>
        public static bool operator !=(PointD a, PointD b) => !(a == b);

        /// <summary>
        /// Оператор сложения двух точек.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Новая точка, являющаяся суммой двух точек.</returns>
        public static PointD operator +(PointD a, PointD b) => new PointD(a.X + b.X, a.Y + b.Y);

        /// <summary>
        /// Оператор вычитания двух точек.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Новая точка, являющаяся разностью двух точек.</returns>
        public static PointD operator -(PointD a, PointD b) => new PointD(a.X - b.X, a.Y - b.Y);
    }
}