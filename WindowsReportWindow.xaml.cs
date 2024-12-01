using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using YA_Metro.AI;
using YA_Metro.Models;
using YA_Metro.Utilities.Logger;

namespace YA_Metro.Windows
{
    /// <summary>
    /// Окно отчета о моделировании, наследующее от Window.
    /// </summary>
    public partial class ReportWindow : Window, IComponentConnector
    {
        private readonly ModelingReport _modelingReport;

        /// <summary>
        /// Конструктор класса ReportWindow.
        /// </summary>
        /// <param name="report">Отчет о моделировании.</param>
        public ReportWindow(ModelingReport report)
        {
            this._modelingReport = report;
            this.InitializeComponent();
            this.LoadReport();
        }

        /// <summary>
        /// Загружает данные отчета в интерфейс окна.
        /// </summary>
        private void LoadReport()
        {
            this.BranchNameTxt.Text = this._modelingReport.Branch.Name;
            this.DayOfWeekTxt.Text = this.GetNameDayOfWeek(this._modelingReport.DayOfWeek);
            this.StartTimeTxt.Text = this._modelingReport.StartTime.ToString("HH:mm");
            this.FinishTimeTxt.Text = this._modelingReport.FinishTime.ToString("HH:mm");
            this.StepTxt.Text = this._modelingReport.Step.ToString();
            this.CountTrainsTxt.Text = this._modelingReport.Trains.Count<Train>().ToString();
            this.TrainsList.ItemsSource = (IEnumerable)this._modelingReport.Trains;
            foreach (LogEntry logEntry in this._modelingReport.LogEntries)
                this.LogsRtb.AppendText(logEntry.ToString("HH:mm:ss") + "\r");
        }

        /// <summary>
        /// Возвращает название дня недели на русском языке.
        /// </summary>
        /// <param name="dayOfWeek">День недели.</param>
        /// <returns>Название дня недели на русском языке.</returns>
        private string GetNameDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Воскресенье";
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Wednesday:
                    return "Среда";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Friday:
                    return "Пятница";
                case DayOfWeek.Saturday:
                    return "Суббота";
                default:
                    return "";
            }
        }
    }
}