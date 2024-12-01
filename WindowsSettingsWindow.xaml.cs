using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using YA_Metro.AI;
using YA_Metro.Enums;
using YA_Metro.Models;

namespace YA_Metro.Windows
{
    /// <summary>
    /// Окно настроек моделирования, наследующее от Window.
    /// </summary>
    public partial class SettingsWindow : Window, IComponentConnector
    {
        private readonly ModelingSettings _settings;

        /// <summary>
        /// Конструктор класса SettingsWindow.
        /// </summary>
        /// <param name="settings">Настройки моделирования.</param>
        public SettingsWindow(ModelingSettings settings)
        {
            this._settings = settings;
            this.InitializeComponent();
            this.Title = string.Format("Настройки моделирования на ветке {0}", (object)settings.Branch.Name);
            this.LoadParams();
        }

        /// <summary>
        /// Загружает параметры моделирования в интерфейс окна.
        /// </summary>
        private void LoadParams()
        {
            this.PeriodRangeSlider.Minimum = new DateTime().AddSeconds(21600.0);
            this.PeriodRangeSlider.Maximum = new DateTime().AddSeconds(86400.0);
            this.PeriodRangeSlider.LowerValue = this._settings.StartTime > DateTime.MinValue ? this._settings.StartTime : this.PeriodRangeSlider.Minimum;
            this.PeriodRangeSlider.UpperValue = this._settings.FinishTime > DateTime.MinValue ? this._settings.FinishTime : this.PeriodRangeSlider.Maximum;
            this.PeriodRangeSlider.LowerValueChanged += new RoutedEventHandler(this.PeriodRangeSlider_LowerValueChanged);
            this.PeriodRangeSlider.UpperValueChanged += new RoutedEventHandler(this.PeriodRangeSlider_UpperValueChanged);
            this.StepSlider.Value = (double)this._settings.Step < this.StepSlider.Minimum || (double)this._settings.Step > this.StepSlider.Maximum ? this.StepSlider.Minimum : (double)this._settings.Step;
            this.StepSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.StepSlider_ValueChanged);
            int index = this._settings.Branch.Stations.FindIndex((Predicate<Station>)(x => x.State == AvailableState.Working));
            if (index != -1)
            {
                Station station = this._settings.Branch.Stations[index];
                this.StartStation.Tag = (object)station.Id;
                this.StartStation.Content = (object)station.Name;
            }
            int lastIndex = this._settings.Branch.Stations.FindLastIndex((Predicate<Station>)(x => x.State == AvailableState.Working));
            if (lastIndex != -1)
            {
                Station station = this._settings.Branch.Stations[lastIndex];
                this.FinishStation.Tag = (object)station.Id;
                this.FinishStation.Content = (object)station.Name;
            }
            foreach (Button button in this.DoWPanel.Children.OfType<Button>())
            {
                if ((DayOfWeek)button.Tag == this._settings.DayOfWeek)
                {
                    button.IsEnabled = false;
                    break;
                }
            }
            this.TrainsList.ItemsSource = (IEnumerable)this._settings.Trains;
            this.StartStation.IsEnabled = false;
        }

        /// <summary>
        /// Обработчик события изменения значения слайдера шага.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void StepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(sender is Slider slider))
                return;
            this._settings.Step = (int)slider.Value;
        }

        /// <summary>
        /// Обработчик события нажатия кнопки выбора дня недели.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button1))
                return;
            this._settings.DayOfWeek = (DayOfWeek)button1.Tag;
            foreach (Button button2 in this.DoWPanel.Children.OfType<Button>())
                button2.IsEnabled = !button2.Equals((object)button1);
        }

        /// <summary>
        /// Обработчик события изменения нижнего значения слайдера периода.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void PeriodRangeSlider_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is YA_Metro.UI.DateRangeSlider.DateRangeSlider dateRangeSlider))
                return;
            this._settings.StartTime = dateRangeSlider.LowerValue;
        }

        /// <summary>
        /// Обработчик события изменения верхнего значения слайдера периода.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void PeriodRangeSlider_UpperValueChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is YA_Metro.UI.DateRangeSlider.DateRangeSlider dateRangeSlider))
                return;
            this._settings.FinishTime = dateRangeSlider.UpperValue;
        }

        /// <summary>
        /// Обработчик события нажатия кнопки добавления поезда.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void AddTrain_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            if (this.NumText.Text.Length >= 1 && int.TryParse(this.NumText.Text, out id))
            {
                if (this._settings.Trains.Count<Train>((Func<Train, bool>)(x => x.Id == id)) > 0)
                    return;
                int stationId = !this.StartStation.IsEnabled ? int.Parse(this.StartStation.Tag.ToString()) : int.Parse(this.FinishStation.Tag.ToString());
                Station startStation = this._settings.Branch.Stations.Find((Predicate<Station>)(x => x.Id == stationId));
                this._settings.Trains.Add(new Train(id, 1.0, this._settings.Branch, startStation));
                this.TrainsList.UnselectAll();
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопки удаления поезда.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void DeleteTrain_Click(object sender, RoutedEventArgs e)
        {
            if (this.TrainsList.SelectedIndex == -1)
                return;
            this._settings.Trains.RemoveAt(this.TrainsList.SelectedIndex);
        }

        /// <summary>
        /// Обработчик события нажатия кнопки выбора станции.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void StationButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button button in this.StationSelector.Children.OfType<Button>())
                button.IsEnabled = !button.Equals((object)(sender as Button));
        }

        /// <summary>
        /// Обработчик события закрытия окна.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Window_Closing(object sender, CancelEventArgs e) => this._settings.SaveSettings();

        /// <summary>
        /// Обработчик события изменения выбора в списке поездов.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void TrainsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            this.DeleteTrain.IsEnabled = listView != null && listView.SelectedIndex != -1;
        }

        /// <summary>
        /// Обработчик события ввода текста в поле идентификатора поезда.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void NumText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (int.TryParse(e.Text, out int _))
                return;
            e.Handled = true;
        }
    }
}