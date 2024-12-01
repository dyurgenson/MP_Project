using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace YA_Metro.UI.DateRangeSlider
{
    /// <summary>
    /// Этот класс представляет пользовательский элемент управления DateRangeSlider,
    /// который позволяет пользователю выбирать диапазон дат с помощью двух слайдеров.
    /// </summary>
    public partial class DateRangeSlider : UserControl, IComponentConnector
    {
        /// <summary>
        /// Константа для форматирования времени.
        /// </summary>
        private const string StrMask = "HH:mm";

        /// <summary>
        /// Зависимости свойств для минимальной и максимальной даты.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(DateTime), typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider), (PropertyMetadata)new UIPropertyMetadata((object)DateTime.Now.AddDays(-15.0)));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(DateTime), typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider), (PropertyMetadata)new UIPropertyMetadata((object)DateTime.Now.AddDays(15.0)));

        /// <summary>
        /// Зависимости свойств для нижнего и верхнего значения диапазона.
        /// </summary>
        public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register(nameof(LowerValue), typeof(DateTime), typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider), (PropertyMetadata)new UIPropertyMetadata((object)DateTime.Now.AddDays(-7.0)));
        public static readonly DependencyProperty UpperValueProperty = DependencyProperty.Register(nameof(UpperValue), typeof(DateTime), typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider), (PropertyMetadata)new UIPropertyMetadata((object)DateTime.Now.AddDays(7.0)));

        /// <summary>
        /// Зависимости свойств для строкового представления нижнего и верхнего значения.
        /// </summary>
        public static readonly DependencyProperty StrLowerValueProperty;
        public static readonly DependencyProperty StrUpperValueProperty;

        /// <summary>
        /// Зависимость свойства для минимального диапазона.
        /// </summary>
        public static readonly DependencyProperty MinRangeProperty;

        /// <summary>
        /// События для изменения нижнего и верхнего значения.
        /// </summary>
        public event RoutedEventHandler LowerValueChanged;
        public event RoutedEventHandler UpperValueChanged;

        /// <summary>
        /// Свойства для минимальной и максимальной даты.
        /// </summary>
        public DateTime Minimum
        {
            get => (DateTime)this.GetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.MinimumProperty);
            set => this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.MinimumProperty, (object)value);
        }

        public DateTime Maximum
        {
            get => (DateTime)this.GetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.MaximumProperty);
            set => this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.MaximumProperty, (object)value);
        }

        /// <summary>
        /// Свойства для нижнего и верхнего значения диапазона.
        /// </summary>
        public DateTime LowerValue
        {
            get => (DateTime)this.GetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.LowerValueProperty);
            set
            {
                this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.LowerValueProperty, (object)value);
                this.StrLowerValue = value.ToString("HH:mm");
            }
        }

        public DateTime UpperValue
        {
            get => (DateTime)this.GetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.UpperValueProperty);
            set
            {
                this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.UpperValueProperty, (object)value);
                this.StrUpperValue = value.ToString("HH:mm");
            }
        }

        /// <summary>
        /// Свойства для строкового представления нижнего и верхнего значения.
        /// </summary>
        public string StrLowerValue
        {
            get => this.LowerValue.ToString("HH:mm");
            set => this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.StrLowerValueProperty, (object)value);
        }

        public string StrUpperValue
        {
            get => this.UpperValue.ToString("HH:mm");
            set => this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.StrUpperValueProperty, (object)value);
        }

        /// <summary>
        /// Свойство для минимального диапазона.
        /// </summary>
        public int MinRange
        {
            get => (int)this.GetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.MinRangeProperty);
            set => this.SetValue(YA_Metro.UI.DateRangeSlider.DateRangeSlider.MinRangeProperty, (object)value);
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public DateRangeSlider()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.Slider_Loaded);
        }

        /// <summary>
        /// Метод, вызываемый при загрузке элемента управления.
        /// </summary>
        protected void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            this.LowerSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.LowerSlider_ValueChanged);
            this.UpperSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.UpperSlider_ValueChanged);
        }

        /// <summary>
        /// Метод, вызываемый при изменении значения нижнего слайдера.
        /// </summary>
        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpperSlider.Value = Math.Max(this.UpperSlider.Value, this.LowerSlider.Value + (double)this.MinRange);
            DateTime dateTime1 = new DateTime().AddMinutes(Math.Round(this.UpperSlider.Value));
            DateTime dateTime2 = this.UpperSlider.Value - (double)this.MinRange < this.LowerSlider.Value ? new DateTime().AddMinutes(Math.Round(this.UpperSlider.Value - (double)this.MinRange)) : new DateTime().AddMinutes(Math.Round(this.LowerSlider.Value));
            if (this.UpperValue > dateTime2)
            {
                this.LowerValue = dateTime2;
                this.UpperValue = dateTime1;
            }
            else
            {
                this.UpperValue = dateTime1;
                this.LowerValue = dateTime2;
            }
            if (this.LowerValueChanged == null)
                return;
            this.LowerValueChanged((object)this, (RoutedEventArgs)e);
        }

        /// <summary>
        /// Метод, вызываемый при изменении значения верхнего слайдера.
        /// </summary>
        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.LowerSlider.Value = Math.Max(Math.Min(this.UpperSlider.Value - (double)this.MinRange, this.LowerSlider.Value), this.Minimum.Subtract(DateTime.MinValue).TotalMinutes);
            DateTime dateTime1;
            DateTime dateTime2;
            if (this.LowerSlider.Value + (double)this.MinRange <= this.UpperSlider.Value)
            {
                dateTime1 = new DateTime();
                dateTime2 = dateTime1.AddMinutes(Math.Round(this.UpperSlider.Value));
            }
            else
            {
                dateTime1 = new DateTime();
                dateTime2 = dateTime1.AddMinutes(Math.Round(this.LowerSlider.Value + (double)this.MinRange));
            }
            DateTime dateTime3 = dateTime2;
            dateTime1 = new DateTime();
            DateTime dateTime4 = dateTime1.AddMinutes(Math.Round(this.LowerSlider.Value));
            if (this.UpperValue > dateTime4)
            {
                this.LowerValue = dateTime4;
                this.UpperValue = dateTime3;
            }
            else
            {
                this.UpperValue = dateTime3;
                this.LowerValue = dateTime4;
            }
            if (this.UpperValueChanged == null)
                return;
            this.UpperValueChanged((object)this, (RoutedEventArgs)e);
        }

        /// <summary>
        /// Статический конструктор для инициализации зависимостей свойств.
        /// </summary>
        static DateRangeSlider()
        {
            Type propertyType1 = typeof(string);
            Type ownerType1 = typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider);
            DateTime dateTime1 = DateTime.Now;
            dateTime1 = dateTime1.AddDays(-7.0);
            UIPropertyMetadata typeMetadata1 = new UIPropertyMetadata((object)dateTime1.ToString("HH:mm"));
            YA_Metro.UI.DateRangeSlider.DateRangeSlider.StrLowerValueProperty = DependencyProperty.Register(nameof(StrLowerValue), propertyType1, ownerType1, (PropertyMetadata)typeMetadata1);
            Type propertyType2 = typeof(string);
            Type ownerType2 = typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider);
            DateTime dateTime2 = DateTime.Now;
            dateTime2 = dateTime2.AddDays(7.0);
            UIPropertyMetadata typeMetadata2 = new UIPropertyMetadata((object)dateTime2.ToString("HH:mm"));
            YA_Metro.UI.DateRangeSlider.DateRangeSlider.StrUpperValueProperty = DependencyProperty.Register(nameof(StrUpperValue), propertyType2, ownerType2, (PropertyMetadata)typeMetadata2);
            YA_Metro.UI.DateRangeSlider.DateRangeSlider.MinRangeProperty = DependencyProperty.Register(nameof(MinRange), typeof(int), typeof(YA_Metro.UI.DateRangeSlider.DateRangeSlider), (PropertyMetadata)new UIPropertyMetadata((object)30));
        }
    }
}