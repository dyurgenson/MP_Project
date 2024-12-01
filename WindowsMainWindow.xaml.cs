using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Windows.Media;
using YA_Metro.AI;
using YA_Metro.Data;
using YA_Metro.Drawing;
using YA_Metro.Enums;
using YA_Metro.Models;
using YA_Metro.Utilities.Logger;
using System.IO;

namespace YA_Metro.Windows
{
    /// <summary>
    /// Основное окно приложения, наследующее от Window.
    /// </summary>
    public partial class MainWindow : Window, IComponentConnector
    {
        private const int VirtualWidth = 950;
        private const int VirtualHeight = 800;
        private const double AspectRatio = 1.1875;
        private GLControl _glControl;
        private readonly Metro _metro;
        private readonly Modeling _modeling;

        /// <summary>
        /// Конструктор класса MainWindow.
        /// </summary>
        public MainWindow()
        {
            string resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Resources\\yar");
            this._metro = Metro.Create((IRepository)new JsonRepository(Path.GetFullPath(resourcePath)));
            this._modeling = new Modeling();
            this._modeling.TrainPositionUpdated += new Modeling.UpdateTrainsPositionHandler(this.Modeling_TrainPositionUpdated);
            this._modeling.ModelingEnded += new Modeling.EndedModeligHangler(this.Modeling_ModelingEnded);
            YA_Metro.Utilities.Logger.Logger logger = new YA_Metro.Utilities.Logger.Logger();
            logger.OnNewLogEntry += new YA_Metro.Utilities.Logger.Logger.NewLogEntryHandler(this.Logger_OnNewLogEntry);
            this._modeling.InitLogger((ILogger)logger);
            this.InitializeComponent();
        }

        /// <summary>
        /// Обработчик события завершения моделирования.
        /// </summary>
        private void Modeling_ModelingEnded()
        {
            this.Dispatcher.Invoke(() =>
            {
                this._glControl.Invalidate();
                if (System.Windows.Forms.MessageBox.Show("Показать отчет?", "Логичный вопрос", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;
                new ReportWindow(this._modeling.GetReport()).ShowDialog();
                this.StartButton.Visibility = Visibility.Visible;
                this.ResumeButton.Visibility = Visibility.Collapsed;
                this.SettingsButton.Visibility = Visibility.Visible;
                this.PauseButton.Visibility = Visibility.Collapsed;
                this.StopButton.Visibility = Visibility.Collapsed;
            });
        }

        /// <summary>
        /// Обработчик события добавления новой записи в лог.
        /// </summary>
        /// <param name="logEntry">Запись лога.</param>
        private void Logger_OnNewLogEntry(LogEntry logEntry) => Console.WriteLine(logEntry.ToString("HH:mm:ss"));

        /// <summary>
        /// Обработчик события обновления позиции поездов.
        /// </summary>
        private void Modeling_TrainPositionUpdated()
        {
            if (this._glControl == null)
                return;
            this._glControl.Invalidate();
        }

        /// <summary>
        /// Обработчик события инициализации WindowsFormsHost.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            this._glControl = new GLControl(new GraphicsMode((ColorFormat)32, 24), 2, 0, GraphicsContextFlags.Default);
            this._glControl.MakeCurrent();
            this._glControl.Paint += new PaintEventHandler(this.GLControl_Paint);
            this._glControl.VSync = true;
            if (sender is WindowsFormsHost windowsFormsHost)
                windowsFormsHost.Child = (System.Windows.Forms.Control)this._glControl;
            GL.Ortho(0.0, 950.0, 800.0, 0.0, -1.0, 1.0);
            this.SetViewport(this._glControl.Width, this._glControl.Height);
        }

        /// <summary>
        /// Обработчик события отрисовки GLControl.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.ColorBufferBit);
            if (this._metro == null)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                this.DrawString("Ошибка отображения метро...", new PointD(250.0, 370.0), new Font(System.Drawing.FontFamily.GenericSansSerif, 20f), System.Drawing.Brushes.Gray);
                GL.Disable(EnableCap.Blend);
                this._glControl.SwapBuffers();
            }
            else
            {
                foreach (Branch branch in this._metro.Branches)
                    this.DrawBranch(branch);
                if (this._modeling.Settings.Branch != null)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.Color4(1.0, 1.0, 1.0, 0.9);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(0, 0);
                    GL.Vertex2(0, 800);
                    GL.Vertex2(950, 800);
                    GL.Vertex2(950, 0);
                    GL.End();
                    GL.Disable(EnableCap.Blend);
                    this.DrawBranch(this._modeling.Settings.Branch, true);
                }
                if (this._modeling.State != ModelingState.NotRunning)
                {
                    this.DrawString(string.Format("Время: {0}", (object)this._modeling.CurrentTime.ToString("HH:mm:ss")), new PointD(20.0, 15.0), new Font(System.Drawing.FontFamily.GenericSansSerif, 14f));
                    this.DrawTrains((IEnumerable<Train>)this._modeling.Settings.Trains);
                }
                this._glControl.SwapBuffers();
            }
        }

        /// <summary>
        /// Отрисовывает строку на экране.
        /// </summary>
        /// <param name="text">Текст для отрисовки.</param>
        /// <param name="point">Координаты точки, где начнется отрисовка.</param>
        /// <param name="font">Шрифт для отрисовки (по умолчанию GenericSansSerif, 13).</param>
        /// <param name="brush">Кисть для отрисовки (по умолчанию черная).</param>
        private void DrawString(string text, PointD point, Font font = null, System.Drawing.Brush brush = null)
        {
            if (font == null)
                font = new Font(System.Drawing.FontFamily.GenericSansSerif, 13f);
            if (brush == null)
                brush = System.Drawing.Brushes.Black;
            using (YA_Metro.Drawing.TextRenderer textRenderer = new YA_Metro.Drawing.TextRenderer(950, 800))
            {
                textRenderer.DrawString(text, font, brush, point);
                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, textRenderer.Texture);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0);
                GL.Vertex2(0, 0);
                GL.TexCoord2(1, 0);
                GL.Vertex2(950, 0);
                GL.TexCoord2(1, 1);
                GL.Vertex2(950, 800);
                GL.TexCoord2(0, 1);
                GL.Vertex2(0, 800);
                GL.End();
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }
        }

        /// <summary>
        /// Обработчик события изменения размера WindowsFormsHost.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void WindowsFormsHost_SizeChanged(object sender, SizeChangedEventArgs e) => this.SetViewport(this._glControl.Width, this._glControl.Height);

        /// <summary>
        /// Отрисовывает ветку метро.
        /// </summary>
        /// <param name="branch">Ветка метро для отрисовки.</param>
        /// <param name="drawStationNames">Флаг, указывающий, нужно ли отрисовывать названия станций.</param>
        private void DrawBranch(Branch branch, bool drawStationNames = false)
        {
            GL.Color4(branch.Color);
            GL.LineWidth(3f);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            foreach (Section section in branch.Sections)
            {
                if (section.State != AvailableState.Working)
                {
                    GL.Enable(EnableCap.LineStipple);
                    GL.LineStipple((int)(YA_Metro.Utilities.Math.VectorLength(section.From.Coord, section.To.Coord) / 8.0), (short)500);
                }
                GL.Begin(PrimitiveType.Lines);
                PointD coord = section.From.Coord;
                double x1 = coord.X;
                coord = section.From.Coord;
                double y1 = coord.Y;
                GL.Vertex2(x1, y1);
                coord = section.To.Coord;
                double x2 = coord.X;
                coord = section.To.Coord;
                double y2 = coord.Y;
                GL.Vertex2(x2, y2);
                GL.End();
                GL.Disable(EnableCap.LineStipple);
            }
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.LineSmooth);
            foreach (Station station in branch.Stations)
            {
                PointD coord = station.Coord;
                double x3 = coord.X;
                coord = station.Coord;
                double y3 = coord.Y;
                this.DrawCircle(x3, y3, 8.0);
                if (station.State != AvailableState.Working)
                {
                    GL.Color4(Color4.White);
                    coord = station.Coord;
                    double x4 = coord.X;
                    coord = station.Coord;
                    double y4 = coord.Y;
                    this.DrawCircle(x4, y4, 4.0);
                    GL.Color4(branch.Color);
                }
                if (drawStationNames)
                {
                    if (station.CoordsOfName == 0)
                    {
                        this.DrawString(station.Name, station.Coord + station.TextStyle.Margin);
                    }
                    else if (station.CoordsOfName == 1)
                    {
                        this.DrawString(station.Name, new PointD(station.Coord.X - 70, station.Coord.Y - 23) + station.TextStyle.Margin);
                    }
                    else if (station.CoordsOfName == 2)
                    {
                        this.DrawString(station.Name, new PointD(station.Coord.X - 135, station.Coord.Y) + station.TextStyle.Margin);
                    }
                    else
                    {
                        this.DrawString(station.Name, new PointD(station.Coord.X - 70, station.Coord.Y + 15) + station.TextStyle.Margin);
                    }
                }
            }
        }

        /// <summary>
        /// Устанавливает область просмотра для OpenGL.
        /// </summary>
        /// <param name="width">Ширина области просмотра.</param>
        /// <param name="height">Высота области просмотра.</param>
        private void SetViewport(int width, int height)
        {
            double num1 = (double)width / (double)height;
            Vector2d vector2d = new Vector2d(0.0, 0.0);
            double num2;
            if (num1 > 19.0 / 16.0)
            {
                num2 = (double)height / 800.0;
                vector2d.X = ((double)width - 950.0 * num2) / 2.0;
            }
            else if (num1 < 19.0 / 16.0)
            {
                num2 = (double)width / 950.0;
                vector2d.Y = ((double)height - 800.0 * num2) / 2.0;
            }
            else
                num2 = (double)width / 950.0;
            int width1 = (int)(950.0 * num2);
            int height1 = (int)(800.0 * num2);
            GL.Viewport((int)vector2d.X, (int)vector2d.Y, width1, height1);
        }

        /// <summary>
        /// Отрисовывает круг.
        /// </summary>
        /// <param name="cx">Координата X центра круга.</param>
        /// <param name="cy">Координата Y центра круга.</param>
        /// <param name="r">Радиус круга.</param>
        private void DrawCircle(double cx, double cy, double r)
        {
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Begin(PrimitiveType.TriangleFan);
            for (int index = 0; index < 360; ++index)
            {
                double num = (double)index * System.Math.PI / 180.0;
                GL.Vertex2(r * System.Math.Cos(num) + cx, r * System.Math.Sin(num) + cy);
            }
            GL.End();
        }

        /// <summary>
        /// Отрисовывает поезда.
        /// </summary>
        /// <param name="trains">Коллекция поездов для отрисовки.</param>
        private void DrawTrains(IEnumerable<Train> trains)
        {
            foreach (Train train in trains)
            {
                GL.Color4(train.Direction == TrainDirection.Direct ? Color4.Pink : Color4.Blue);
                PointD coord = train.Coord;
                double x = coord.X;
                coord = train.Coord;
                double y = coord.Y;
                this.DrawCircle(x, y, 10.0);
            }
        }

        /// <summary>
        /// Обработчик события инициализации окна.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            if (this._metro == null)
            {
                this.SidebarBranches.Visibility = Visibility.Hidden;
                this.SidebarButtons.Visibility = Visibility.Hidden;
            }
            else
            {
                foreach (Branch branch1 in this._metro.Branches)
                {
                    Branch branch = branch1;
                    System.Windows.Controls.Button button1 = new System.Windows.Controls.Button();
                    button1.Content = (object)branch.Name;
                    button1.Style = this.FindResource((object)"BranchButtonStyle") as Style;
                    System.Windows.Controls.Button element = button1;
                    element.Resources[(object)"BranchId"] = (object)branch.Id.ToString();
                    ResourceDictionary resources = element.Resources;
                    System.Drawing.Color color = branch.Color;
                    int a = (int)color.A;
                    color = branch.Color;
                    int r = (int)color.R;
                    color = branch.Color;
                    int g = (int)color.G;
                    color = branch.Color;
                    int b = (int)color.B;
                    SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
                    resources[(object)"BranchColor"] = (object)solidColorBrush;
                    element.Click += (RoutedEventHandler)((s, ev) =>
                    {
                        if (this._modeling.State != ModelingState.NotRunning)
                            return;
                        this._modeling.Settings.Branch = branch;
                        foreach (System.Windows.Controls.Button button2 in this.BranchesPanel.Children.OfType<System.Windows.Controls.Button>())
                            button2.IsEnabled = !button2.Equals((object)(s as System.Windows.Controls.Button));
                        if (this.SidebarButtons.Visibility != Visibility.Visible)
                        {
                            this.SidebarError.Visibility = Visibility.Collapsed;
                            this.SidebarButtons.Visibility = Visibility.Visible;
                        }
                        this._glControl.Invalidate();
                    });
                    this.BranchesPanel.Children.Add((UIElement)element);
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопки настроек.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (this._modeling.Settings.Branch == null)
                return;
            if (this._modeling.Settings.Branch.Stations.Count<Station>((Func<Station, bool>)(x => x.State == AvailableState.Working)) < 2)
            {
                int num = (int)System.Windows.Forms.MessageBox.Show("На выбранной ветке должны быть хотя бы две рабочие станции", "Невозможно открыть настройки", MessageBoxButtons.OK);
            }
            else
                new SettingsWindow(this._modeling.Settings).ShowDialog();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки запуска моделирования.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (this._modeling.Settings.Branch == null || this._modeling.State == ModelingState.Running)
                return;
            if (this._modeling.Settings.Trains.Count < 1)
            {
                int num = (int)System.Windows.Forms.MessageBox.Show("На выбранной ветке должен быть хотя бы один поезд", "Невозможно запустить моделирование", MessageBoxButtons.OK);
            }
            else
            {
                this._modeling.Start();
                this.StartButton.Visibility = Visibility.Collapsed;
                this.ResumeButton.Visibility = Visibility.Collapsed;
                this.SettingsButton.Visibility = Visibility.Collapsed;
                this.PauseButton.Visibility = Visibility.Visible;
                this.StopButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопки паузы.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            this._modeling.Pause();
            this.PauseButton.Visibility = Visibility.Collapsed;
            this.ResumeButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Обработчик события нажатия кнопки остановки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            this._modeling.Stop();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки возобновления.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void BtnResume_Click(object sender, RoutedEventArgs e)
        {
            this._modeling.Resume();
            this.ResumeButton.Visibility = Visibility.Collapsed;
            this.PauseButton.Visibility = Visibility.Visible;
        }
    }
}