using System.Security.Cryptography;
using System.Text;

namespace YA_Metro.Utilities
{
    /// <summary>
    /// Класс, предоставляющий методы для работы с безопасностью, такие как хеширование.
    /// </summary>
    public class Security
    {
        /// <summary>
        /// Вычисляет MD5-хеш для заданной строки.
        /// </summary>
        /// <param name="input">Входная строка, для которой нужно вычислить хеш.</param>
        /// <returns>Строка, представляющая MD5-хеш входной строки.</returns>
        public static string GetMd5Hash(string input)
        {
            // Создаем экземпляр MD5 для вычисления хеша
            using (MD5 md5Hash = MD5.Create())
            {
                // Преобразуем входную строку в массив байтов и вычисляем хеш
                byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Создаем StringBuilder для удобства формирования строки хеша
                StringBuilder stringBuilder = new StringBuilder();

                // Преобразуем каждый байт хеша в шестнадцатеричную строку
                foreach (byte num in hash)
                {
                    stringBuilder.Append(num.ToString("x2"));
                }

                // Возвращаем строку, представляющую MD5-хеш
                return stringBuilder.ToString();
            }
        }
    }
}