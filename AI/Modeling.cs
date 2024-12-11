using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using YA_Metro.Enums;
using YA_Metro.Models;
using YA_Metro.Utilities.Logger;

namespace YA_Metro.AI
{
    /// <summary>
    /// Класс Modeling отвечает за моделирование движения поездов в системе YA_Metro.
    /// Он управляет таймером, логированием, состоянием моделирования и обработкой событий.
    /// </summary>
    public class Modeling
    {
        private readonly Timer _timer;
        private ILogger _logger;
        private readonly ISituation _situation;
        private ModelingReport _report;

        /// <summary>
        /// Событие, возникающее при обновлении позиции поездов.
        /// </summary>
        public event Modeling.UpdateTrainsPositionHandler TrainPositionUpdated;

        /// <summary>
        /// Событие, возникающее при завершении моделирования.
        /// </summary>
        public event Modeling.EndedModeligHangler ModelingEnded;

        /// <summary>
        /// Настройки моделирования.
        /// </summary>
        public ModelingSettings Settings { get; private set; }

        /// <summary>
        /// Текущее состояние моделирования.
        /// </summary>
        public ModelingState State { get; private set; }

        /// <summary>
        /// Текущее время моделирования.
        /// </summary>
        public DateTime CurrentTime { get; private set; }

        /// <summary>
        /// Конструктор класса Modeling.
        /// </summary>
        /// <param name="settings">Настройки моделирования.</param>
        public Modeling(ModelingSettings settings)
        {
            this.Settings = settings;
            this.CurrentTime = new DateTime();
            this._situation = (ISituation)new UnexpectedSituation(this.Settings.DayOfWeek);
            this._timer = new Timer(1000.0);
            this._timer.Elapsed += new ElapsedEventHandler(this._timer_Elapsed);
            this.State = ModelingState.NotRunning;
        }

        /// <summary>
        /// Конструктор класса Modeling без параметров.
        /// </summary>
        public Modeling()
          : this(new ModelingSettings())
        {
        }

        /// <summary>
        /// Запускает моделирование.
        /// </summary>
        public void Start()
        {
            this._timer.Start();
            this.CurrentTime = this.Settings.StartTime;
            this._logger.Clear();
            this.State = ModelingState.Running;
            this.RunTrainsMovement();
        }

        /// <summary>
        /// Приостанавливает моделирование.
        /// </summary>
        public void Pause()
        {
            this._timer.Stop();
            this.State = ModelingState.Pause;
        }

        /// <summary>
        /// Возобновляет моделирование после паузы.
        /// </summary>
        public void Resume()
        {
            this._timer.Start();
            this.State = ModelingState.Running;
        }

        /// <summary>
        /// Останавливает моделирование и генерирует отчет.
        /// </summary>
        public void Stop()
        {
            this._timer.Stop();
            this.GenerateReport();
            foreach (Train train in (Collection<Train>)this.Settings.Trains)
                train.SetDefaultValues();
            this.CurrentTime = this.Settings.StartTime;
            this.State = ModelingState.NotRunning;
            if (this.ModelingEnded == null)
                return;
            this.ModelingEnded();
        }

        /// <summary>
        /// Генерирует отчет о моделировании.
        /// </summary>
        private void GenerateReport() => this._report = new ModelingReport()
        {
            DayOfWeek = this.Settings.DayOfWeek,
            StartTime = this.Settings.StartTime,
            FinishTime = this.CurrentTime,
            Branch = this.Settings.Branch,
            Step = this.Settings.Step,
            LogEntries = this._logger.GetLogEntries(),
            Trains = (IEnumerable<Train>)this.Settings.Trains
        };

        /// <summary>
        /// Возвращает отчет о моделировании.
        /// </summary>
        /// <returns>Отчет о моделировании.</returns>
        public ModelingReport GetReport() => this._report;

        /// <summary>
        /// Инициализирует логгер для моделирования.
        /// </summary>
        /// <param name="logger">Логгер для использования.</param>
        public void InitLogger(ILogger logger) => this._logger = logger;

        /// <summary>
        /// Обработчик события таймера.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.CurrentTime >= this.Settings.FinishTime)
            {
                this.Stop();
            }
            else
            {
                this.ChangeTrainsCoords();
                this.CurrentTime = this.CurrentTime.AddSeconds((double)this.Settings.Step);
                if (this.TrainPositionUpdated == null)
                    return;
                this.TrainPositionUpdated();
            }
        }

        /// <summary>
        /// Устанавливает предыдущие поезда для всех поездов, кроме первого.
        /// </summary>
        private void EstablishPrevioursTrainsWihoutFirstTrains()
        {
            for (int index1 = 0; index1 < this.Settings.Trains.Count; ++index1)
            {
                if (this.Settings.Trains[index1].PreviuosTrain == null)
                {
                    for (int index2 = index1 - 1; index2 >= 0; --index2)
                    {
                        if (this.Settings.Trains[index2].Direction == this.Settings.Trains[index1].Direction)
                        {
                            this.Settings.Trains[index1].PreviuosTrain = this.Settings.Trains[index2];
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Устанавливает предыдущие поезда для всех поездов.
        /// </summary>
        private void EstablishPrevioursTrains()
        {
            TrainDirection direction = this.Settings.Trains[0].Direction;
            int count = this.Settings.Trains.Count;
            foreach (Train train in (Collection<Train>)this.Settings.Trains)
                train.PreviuosTrain = (Train)null;
            for (int index = count - 1; index >= 0; --index)
            {
                if (this.Settings.Trains[index].Direction != direction)
                {
                    this.Settings.Trains[0].PreviuosTrain = this.Settings.Trains[index];
                    break;
                }
            }
            if (this.Settings.Trains[0].PreviuosTrain == null)
            {
                this.Settings.Trains[0].PreviuosTrain = this.Settings.Trains[count - 1];
                this.EstablishPrevioursTrainsWihoutFirstTrains();
            }
            else
            {
                for (int index1 = 0; index1 < this.Settings.Trains.Count; ++index1)
                {
                    if (this.Settings.Trains[index1].Direction != direction)
                    {
                        for (int index2 = count - 1; index2 >= 0; --index2)
                        {
                            if (this.Settings.Trains[index2].Direction == direction)
                            {
                                this.Settings.Trains[index1].PreviuosTrain = this.Settings.Trains[index2];
                                break;
                            }
                        }
                        break;
                    }
                }
                this.EstablishPrevioursTrainsWihoutFirstTrains();
            }
        }

        /// <summary>
        /// Возвращает индекс станции в списке станций.
        /// </summary>
        /// <param name="station">Станция, индекс которой нужно найти.</param>
        /// <returns>Индекс станции.</returns>
        private int GetIndexOfStation(Station station) => this.Settings.Branch.Stations.IndexOf(station);

        /// <summary>
        /// Возвращает индекс участка в списке участков.
        /// </summary>
        /// <param name="section">Участок, индекс которого нужно найти.</param>
        /// <returns>Индекс участка.</returns>
        private int GetIndexOfSection(Section section) => this.Settings.Branch.Sections.IndexOf(section);

        /// <summary>
        /// Возвращает угол наклона вектора движения поезда.
        /// </summary>
        /// <param name="train">Поезд, для которого нужно определить угол.</param>
        /// <returns>Угол наклона вектора движения.</returns>
        private double GetAngle(Train train) => train.CurrentSection == null ? 0.0 : YA_Metro.Utilities.Math.AngleBetweenVectorAndXAxis(YA_Metro.Utilities.Math.OrthogonalVector(train.CurrentSection.From.Coord, train.CurrentSection.To.Coord));

        /// <summary>
        /// Инициализирует поля поездов перед началом моделирования.
        /// </summary>
        private void InitializationTrainsFields()
        {
            if (this.Settings.Trains.Count == 0)
                return;
            foreach (Train train in (Collection<Train>)this.Settings.Trains)
            {
                train.Speed = 0.0;
                train.CurrentDistanse = 0.0;
                train.CurrentStation = train.StartStation;
                train.Coord = train.StartStation.Coord;
                train.Angle = 0.0;
                train.CurrentDistanse = 0.0;
                train.RemainingTimeStop = 0.0;
                train.State = TrainState.Parking;
                int indexOfStation = this.GetIndexOfStation(train.StartStation);
                int num = train.StartStation == this.Settings.Branch.Stations[0] ? 0 : (this.Settings.Branch.Stations[indexOfStation - 1].State == AvailableState.Working ? 1 : 0);
                train.Direction = num != 0 ? TrainDirection.Opposite : TrainDirection.Direct;
                for (int index = 0; index < this.Settings.Branch.Sections.Count; ++index)
                {
                    if (train.Direction == TrainDirection.Direct && train.StartStation == this.Settings.Branch.Sections[index].From || train.Direction == TrainDirection.Opposite && train.StartStation == this.Settings.Branch.Sections[index].To)
                    {
                        train.CurrentSection = this.Settings.Branch.Sections[index];
                        break;
                    }
                }
            }
            this.EstablishPrevioursTrains();
        }

        /// <summary>
        /// Инициализирует поля поезда перед началом движения.
        /// </summary>
        /// <param name="train">Поезд, который нужно инициализировать.</param>
        private void InitializationMovingTrain(Train train)
        {
            train.Speed = 2.5;
            train.State = TrainState.Moving;
            train.CurrentStation = (Station)null;
            train.Angle = this.GetAngle(train);
            this._logger.Write(string.Format("Поезд №{0} отбыл со станции {1}", (object)train.Id, (object)train.StartStation.Name), this.CurrentTime);
        }

        /// <summary>
        /// Запускает движение поездов.
        /// </summary>
        public void RunTrainsMovement()
        {
            bool flag1 = false;
            bool flag2 = false;
            this.InitializationTrainsFields();
            foreach (Train train in (Collection<Train>)this.Settings.Trains)
            {
                if (!flag1 && train.Direction == TrainDirection.Direct)
                {
                    flag1 = true;
                    this.InitializationMovingTrain(train);
                }
                else if (!flag2 && train.Direction == TrainDirection.Opposite)
                {
                    flag2 = true;
                    this.InitializationMovingTrain(train);
                }
            }
        }

        /// <summary>
        /// Возвращает скорость поезда.
        /// </summary>
        /// <param name="train">Поезд, для которого нужно определить скорость.</param>
        /// <returns>Скорость поезда.</returns>
        private double GetSpeed(Train train)
        {
            double speed = 1.0;
            if (train.CurrentSection.Time > this.Settings.Branch.Schedule.GetInterval(this.Settings.DayOfWeek, this.CurrentTime))
            {
                speed = (double)(train.CurrentSection.Time / this.Settings.Branch.Schedule.GetInterval(this.Settings.DayOfWeek, this.CurrentTime));
                if (speed > 1.5)
                    speed = 1.5;
            }
            return speed;
        }

        /// <summary>
        /// Возвращает следующий участок для поезда.
        /// </summary>
        /// <param name="train">Поезд, для которого нужно определить следующий участок.</param>
        /// <returns>Следующий участок.</returns>
        private Section GetSection(Train train)
        {
            int indexOfSection = this.GetIndexOfSection(train.CurrentSection);
            if (train.Direction == TrainDirection.Direct && indexOfSection < this.Settings.Branch.Sections.Count - 1)
                return this.Settings.Branch.Sections[indexOfSection + 1];
            return train.Direction == TrainDirection.Opposite && indexOfSection > 0 ? this.Settings.Branch.Sections[indexOfSection - 1] : (Section)null;
        }

        /// <summary>
        /// Устанавливает поля поезда после остановки.
        /// </summary>
        /// <param name="train">Поезд, который остановился.</param>
        private void EstablishTransFieldsAfterStopping(Train train)
        {
            bool flag = false;
            if (train.Direction == TrainDirection.Direct)
            {
                train.CurrentStation = train.CurrentSection.To;
                train.Coord = train.CurrentStation.Coord;
                int indexOfStation = this.GetIndexOfStation(train.CurrentStation);
                if (indexOfStation == this.Settings.Branch.Stations.Count - 1 || this.Settings.Branch.Stations[indexOfStation + 1].State != AvailableState.Working)
                {
                    train.Direction = TrainDirection.Opposite;
                    flag = true;
                }
            }
            else
            {
                train.CurrentStation = train.CurrentSection.From;
                train.Coord = train.CurrentStation.Coord;
                int indexOfStation = this.GetIndexOfStation(train.CurrentStation);
                if (indexOfStation == 0 || this.Settings.Branch.Stations[indexOfStation - 1].State != AvailableState.Working)
                {
                    train.Direction = TrainDirection.Direct;
                    flag = true;
                }
            }
            this._logger.Write(string.Format("Поезд №{0} прибыл на станцию {1}", (object)train.Id, (object)train.CurrentStation.Name), this.CurrentTime);
            int delay = this._situation.GetUnexpectedDelay(this.CurrentTime).Delay;
            if (delay > 0)
                this._logger.Write(string.Format("Поезд №{0} задерживается на {1} сек. на станции {2}", (object)train.Id, (object)delay, (object)train.CurrentStation.Name), this.CurrentTime);
            if (!flag)
                train.CurrentSection = this.GetSection(train);
            train.Speed = 0.0;
            train.State = TrainState.Stopping;
            train.Angle = 0.0;
            train.CurrentDistanse = 0.0;
            train.RemainingTimeStop = (double)(this.Settings.Branch.Schedule.GetStay(this.Settings.DayOfWeek, this.CurrentTime) + delay);
        }

        /// <summary>
        /// Устанавливает поля поезда перед началом движения.
        /// </summary>
        /// <param name="train">Поезд, который начинает движение.</param>
        private void EstablishTransFieldsBeforeStarting(Train train)
        {
            train.Speed = this.GetSpeed(train);
            train.State = TrainState.Moving;
            this._logger.Write(string.Format("Поезд №{0} отбыл со станции {1}", (object)train.Id, (object)train.CurrentStation.Name), this.CurrentTime);
            train.CurrentStation = (Station)null;
            train.Angle = this.GetAngle(train);
            train.CurrentDistanse = 0.0;
            train.RemainingTimeStop = 0.0;
        }

        /// <summary>
        /// Проверяет, является ли станция конечной.
        /// </summary>
        /// <param name="station">Станция для проверки.</param>
        /// <returns>True, если станция является конечной, иначе False.</returns>
        private bool IsStationFinal(Station station)
        {
            int count = this.Settings.Branch.Stations.Count;
            int indexOfStation = this.GetIndexOfStation(station);
            return indexOfStation == 0 || indexOfStation == count - 1 || this.Settings.Branch.Stations[indexOfStation - 1].State != AvailableState.Working || this.Settings.Branch.Stations[indexOfStation + 1].State != AvailableState.Working;
        }

        /// <summary>
        /// Обновляет координаты поездов.
        /// </summary>
        public void ChangeTrainsCoords()
        {
            foreach (Train train in (Collection<Train>)this.Settings.Trains)
            {
                if (train.State == TrainState.Moving)
                {
                    train.CurrentDistanse += train.Speed * (double)this.Settings.Step;
                    if ((double)train.CurrentSection.Time <= train.CurrentDistanse)
                        this.EstablishTransFieldsAfterStopping(train);
                    else
                        train.Coord = train.Direction != TrainDirection.Direct ? YA_Metro.Utilities.Math.PointCoordsOnVector(train.CurrentSection.To.Coord, train.CurrentSection.From.Coord, train.CurrentDistanse / (double)train.CurrentSection.Time) : YA_Metro.Utilities.Math.PointCoordsOnVector(train.CurrentSection.From.Coord, train.CurrentSection.To.Coord, train.CurrentDistanse / (double)train.CurrentSection.Time);
                }
                else if (train.State == TrainState.Stopping)
                {
                    train.RemainingTimeStop = Math.Min(train.RemainingTimeStop - train.Speed * (double)this.Settings.Step, 0);
                    if (train.RemainingTimeStop <= 0.0)
                    {
                        if (train.Direction == train.PreviuosTrain.Direction && (train.Direction == TrainDirection.Direct && train.PreviuosTrain.CurrentStation == train.CurrentSection.To || train.Direction == TrainDirection.Opposite && train.PreviuosTrain.CurrentStation == train.CurrentSection.From || this.IsStationFinal(train.CurrentStation) && train.PreviuosTrain.State != TrainState.Moving && train != train.PreviuosTrain && train.CurrentStation == train.PreviuosTrain.CurrentStation))
                            train.RemainingTimeStop = train.PreviuosTrain.RemainingTimeStop;
                        else if (train.PreviuosTrain.State == TrainState.Moving && train.CurrentSection == train.PreviuosTrain.CurrentSection)
                            train.RemainingTimeStop = train.PreviuosTrain.CurrentDistanse;
                        else
                            this.EstablishTransFieldsBeforeStarting(train);
                    }
                }
                else if (train.PreviuosTrain.State == TrainState.Moving && train.PreviuosTrain.CurrentSection != train.CurrentSection)
                    this.EstablishTransFieldsBeforeStarting(train);
            }
        }

        /// <summary>
        /// Делегат для события обновления позиции поездов.
        /// </summary>
        public delegate void UpdateTrainsPositionHandler();

        /// <summary>
        /// Делегат для события завершения моделирования.
        /// </summary>
        public delegate void EndedModeligHangler();
    }
}