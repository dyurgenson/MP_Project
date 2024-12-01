using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace YA_Metro
{
    /// <summary>
    /// Основной класс приложения, наследующий от Application.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Инициализирует компоненты приложения.
        /// </summary>
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            // Устанавливаем стартовый URI для главного окна приложения
            this.StartupUri = new Uri("Windows/MainWindow.xaml", UriKind.Relative);
        }

        /// <summary>
        /// Точка входа в приложение.
        /// </summary>
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [STAThread]
        public static void Main()
        {
            // Создаем экземпляр приложения
            App app = new App();

            // Инициализируем компоненты приложения
            app.InitializeComponent();

            // Запускаем приложение
            app.Run();
        }
    }
}