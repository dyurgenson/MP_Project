using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using YA_Metro.Enums;
using YA_Metro.Models;
using YA_Metro.Utilities;

namespace YA_Metro.AI
{
    /// <summary>
    /// Класс ModelingSettings представляет собой модель настроек для моделирования движения поездов в системе YA_Metro.
    /// Он содержит информацию о дне недели, времени начала и окончания моделирования, шаге моделирования, ветке и поездах.
    /// Также предоставляет методы для сохранения и загрузки настроек.
    /// </summary>
    [Serializable]
    public class ModelingSettings
    {
        private Branch _branch;
        private readonly string _settingsDirectory = Application.StartupPath + "/Settings";

        /// <summary>
        /// День недели, в который проводится моделирование.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Время начала моделирования.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания моделирования.
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// Шаг моделирования (интервал времени между обновлениями).
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Ветка метро, для которой проводится моделирование.
        /// </summary>
        public Branch Branch
        {
            get => this._branch;
            set
            {
                this._branch = value;
                this.LoadSettings();
            }
        }

        /// <summary>
        /// Коллекция поездов, участвующих в моделировании.
        /// </summary>
        public ObservableCollection<Train> Trains { get; set; }

        /// <summary>
        /// Конструктор по умолчанию, устанавливает свойства по умолчанию.
        /// </summary>
        public ModelingSettings() => this.SetDefaultProperties();

        /// <summary>
        /// Конструктор с параметрами для инициализации настроек.
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="startTime">Время начала моделирования.</param>
        /// <param name="finishTime">Время окончания моделирования.</param>
        /// <param name="step">Шаг моделирования.</param>
        /// <param name="branch">Ветка метро.</param>
        /// <param name="trains">Коллекция поездов.</param>
        public ModelingSettings(
          DayOfWeek dayOfWeek,
          DateTime startTime,
          DateTime finishTime,
          int step,
          Branch branch,
          ObservableCollection<Train> trains)
        {
            this.DayOfWeek = dayOfWeek;
            this.StartTime = startTime;
            this.FinishTime = finishTime;
            this.Step = step;
            this.Branch = branch;
            this.Trains = trains;
        }

        /// <summary>
        /// Сохраняет настройки в файл.
        /// </summary>
        public void SaveSettings()
        {
            string path = string.Format("{0}/{1}.sets", (object)this._settingsDirectory, (object)this.GetBranchUniqueName(this._branch));
            if (!Directory.Exists(this._settingsDirectory))
                Directory.CreateDirectory(this._settingsDirectory);
            try
            {
                using (Stream serializationStream = (Stream)File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                    new BinaryFormatter().Serialize(serializationStream, (object)this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Загружает настройки из файла.
        /// </summary>
        private void LoadSettings()
        {
            string path = string.Format("{0}/{1}.sets", (object)this._settingsDirectory, (object)this.GetBranchUniqueName(this._branch));
            if (!Directory.Exists(this._settingsDirectory) || !File.Exists(path))
            {
                this.SetDefaultProperties();
            }
            else
            {
                try
                {
                    using (Stream serializationStream = (Stream)File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        if (!(new BinaryFormatter().Deserialize(serializationStream) is ModelingSettings modelingSettings))
                            return;
                        this.StartTime = modelingSettings.StartTime;
                        this.FinishTime = modelingSettings.FinishTime;
                        this.Step = modelingSettings.Step;
                        this.Trains = modelingSettings.Trains;
                        this.Branch = modelingSettings.Branch;
                        this.DayOfWeek = modelingSettings.DayOfWeek;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Устанавливает свойства по умолчанию.
        /// </summary>
        private void SetDefaultProperties()
        {
            this.DayOfWeek = DayOfWeek.Sunday;
            this.Step = 30;
            this.StartTime = new DateTime().AddSeconds(21600.0);
            this.FinishTime = new DateTime().AddSeconds(86400.0);
            this.Trains = new ObservableCollection<Train>();
        }

        /// <summary>
        /// Возвращает уникальное имя для ветки метро.
        /// </summary>
        /// <param name="branch">Ветка метро.</param>
        /// <returns>Уникальное имя ветки.</returns>
        private string GetBranchUniqueName(Branch branch)
        {
            int index = branch.Stations.FindIndex((Predicate<Station>)(x => x.State == AvailableState.Working));
            int num1 = index != -1 ? branch.Stations[index].Id : -1;
            int lastIndex = branch.Stations.FindLastIndex((Predicate<Station>)(x => x.State == AvailableState.Working));
            int num2 = lastIndex != -1 ? branch.Stations[lastIndex].Id : -1;
            return Security.GetMd5Hash(string.Format("{0};;{1};;{2};;{3}", (object)branch.Id, (object)branch.Stations.Count<Station>((Func<Station, bool>)(x => x.State == AvailableState.Working)), (object)num1, (object)num2));
        }
    }
}