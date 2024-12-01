using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YA_Metro.Drawing;
using YA_Metro.Enums;
using YA_Metro.Models;

namespace YA_Metro.Data
{
    /// <summary>
    /// Класс JsonRepository реализует интерфейс IRepository и предоставляет методы для чтения данных метрополитена из JSON-файлов.
    /// Он использует библиотеку Newtonsoft.Json для работы с JSON и предоставляет доступ к данным о метрополитене, ветках, станциях, участках и расписании.
    /// </summary>
    public class JsonRepository : IRepository
    {
        private IEnumerable<Branch> _branches;
        private IEnumerable<Station> _stations;
        private IEnumerable<Section> _sections;

        /// <summary>
        /// Путь к директории с JSON-файлами.
        /// </summary>
        public string RepositoryPath { get; protected set; }

        /// <summary>
        /// Конструктор класса JsonRepository.
        /// </summary>
        /// <param name="repositoryPath">Путь к директории с JSON-файлами.</param>
        public JsonRepository(string repositoryPath) => this.RepositoryPath = repositoryPath;

        /// <summary>
        /// Возвращает объект Metro, содержащий информацию о метрополитене.
        /// </summary>
        /// <returns>Объект Metro.</returns>
        public Metro GetMetro()
        {
            JObject jobject = this.GetjObject(this.RepositoryPath + "/metro.json");
            string name = jobject != null ? (string)jobject["name"] : throw new Exception();
            string description = (string)jobject["description"];
            List<Branch> list = this.GetBranches().ToList<Branch>();
            DateTime dateTime = new DateTime();
            DateTime timeFrom = dateTime.AddSeconds((double)(int)jobject["time"][(object)"from"]);
            dateTime = new DateTime();
            DateTime timeTo = dateTime.AddSeconds((double)(int)jobject["time"][(object)"to"]);
            return new Metro(name, description, list, timeFrom, timeTo);
        }

        /// <summary>
        /// Возвращает коллекцию объектов Branch, содержащих информацию о ветках метрополитена.
        /// </summary>
        /// <returns>Коллекция объектов Branch.</returns>
        public IEnumerable<Branch> GetBranches()
        {
            if (this._branches != null)
                return this._branches;
            JArray jarray = this.GetJArray(this.RepositoryPath + "/branches.json");
            if (jarray == null)
                throw new Exception();
            List<Branch> branches = new List<Branch>();
            foreach (JToken jtoken in jarray)
            {
                JToken item = jtoken;
                List<Station> stations = this.GetStations().Where<Station>((Func<Station, bool>)(x => item[(object)"stations"].Select<JToken, int>((Func<JToken, int>)(i => (int)i)).Contains<int>(x.Id))).ToList<Station>();
                branches.Add(new Branch((int)item[(object)"id"], (string)item[(object)"name"], stations, this.GetSections().Where<Section>((Func<Section, bool>)(y => stations.Select<Station, int>((Func<Station, int>)(x => x.Id)).Contains<int>(y.From.Id) || stations.Select<Station, int>((Func<Station, int>)(x => x.Id)).Contains<int>(y.To.Id))).ToList<Section>(), ColorTranslator.FromHtml((string)item[(object)"color"]), this.GetSchedule((int)item[(object)"id"])));
            }
            this._branches = (IEnumerable<Branch>)branches;
            return (IEnumerable<Branch>)branches;
        }

        /// <summary>
        /// Возвращает коллекцию объектов Station, содержащих информацию о станциях метрополитена.
        /// </summary>
        /// <returns>Коллекция объектов Station.</returns>
        public IEnumerable<Station> GetStations()
        {
            if (this._stations != null)
                return this._stations;
            List<Station> list = (this.GetJArray(this.RepositoryPath + "/stations.json") ?? throw new Exception()).Select<JToken, Station>((Func<JToken, Station>)(item => new Station((int)item[(object)"id"], (string)item[(object)"name"], new PointD((double)item[(object)"coord"][(object)0], (double)item[(object)"coord"][(object)1]), (int)item[(object)"coords_of_name"], (AvailableState)(int)item[(object)"state"], this.ParseTextStyle(item)))).ToList<Station>();
            this._stations = (IEnumerable<Station>)list;
            return (IEnumerable<Station>)list;
        }

        /// <summary>
        /// Возвращает коллекцию объектов Section, содержащих информацию о участках метрополитена.
        /// </summary>
        /// <returns>Коллекция объектов Section.</returns>
        public IEnumerable<Section> GetSections()
        {
            if (this._sections != null)
                return this._sections;
            List<Section> list = (this.GetJArray(this.RepositoryPath + "/sections.json") ?? throw new Exception()).Select<JToken, Section>((Func<JToken, Section>)(item => new Section((int)item[(object)"id"], (int)item[(object)"time"], this.GetStations().Where<Station>((Func<Station, bool>)(x => x.Id == (int)item[(object)"from"])).ToList<Station>()[0], this.GetStations().Where<Station>((Func<Station, bool>)(x => x.Id == (int)item[(object)"to"])).ToList<Station>()[0], (AvailableState)(int)item[(object)"state"]))).ToList<Section>();
            this._sections = (IEnumerable<Section>)list;
            return (IEnumerable<Section>)list;
        }

        /// <summary>
        /// Возвращает объект Schedule, содержащий расписание для указанной ветки метрополитена.
        /// </summary>
        /// <param name="branchId">Идентификатор ветки метрополитена.</param>
        /// <returns>Объект Schedule.</returns>
        public Schedule GetSchedule(int branchId)
        {
            JArray jarray = this.GetJArray(this.RepositoryPath + "/schedule.json");
            if (jarray == null)
                throw new Exception();
            List<Schedule.Item> items = new List<Schedule.Item>();
            foreach (JToken jtoken in jarray)
            {
                if ((int)jtoken[(object)nameof(branchId)] == branchId)
                {
                    foreach (DayOfWeek dayOfWeek in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
                    {
                        DayOfWeek value = dayOfWeek;
                        items.AddRange(jtoken[(object)"schedule"][(object)((int)value).ToString()].Select<JToken, Schedule.Item>((Func<JToken, Schedule.Item>)(t => new Schedule.Item((int)value, (int)t[(object)"from"], (int)t[(object)"to"], (int)t[(object)"interval"], (int)t[(object)"stay"]))));
                    }
                }
            }
            return new Schedule(items);
        }

        /// <summary>
        /// Читает содержимое файла.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Содержимое файла в виде строки.</returns>
        private string ReadFile(string fileName)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(fileName))
                    return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                int num = (int)MessageBox.Show("Ошибка при получении данных из файла:\n" + fileName, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return string.Empty;
            }
        }

        /// <summary>
        /// Возвращает объект JArray, содержащий данные из JSON-файла.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Объект JArray.</returns>
        private JArray GetJArray(string fileName)
        {
            try
            {
                return JArray.Parse(this.ReadFile(fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                int num = (int)MessageBox.Show("Ошибка при получении данных из файла:\n" + fileName, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return (JArray)null;
            }
        }

        /// <summary>
        /// Возвращает объект JObject, содержащий данные из JSON-файла.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Объект JObject.</returns>
        private JObject GetjObject(string fileName)
        {
            try
            {
                return JObject.Parse(this.ReadFile(fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                int num = (int)MessageBox.Show("Ошибка при получении данных из файла:\n" + fileName, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return (JObject)null;
            }
        }

        /// <summary>
        /// Парсит стиль текста из JSON-объекта.
        /// </summary>
        /// <param name="item">JSON-объект.</param>
        /// <returns>Объект TextStyle.</returns>
        private TextStyle ParseTextStyle(JToken item)
        {
            TextStyle textStyle = TextStyle.Default;
            if (item[(object)"style"] == null || item[(object)"style"][(object)"margin"] == null)
                return textStyle;
            textStyle = new TextStyle(new PointD((double)item[(object)"style"][(object)"margin"][(object)0], (double)item[(object)"style"][(object)"margin"][(object)1]));
            return textStyle;
        }
    }
}