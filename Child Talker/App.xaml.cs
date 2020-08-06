using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Child_Talker.Utilities;
using Child_Talker.Utilities.HardwareIntegrations;

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string StartupPath { private set; get; }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main()
        {
            Child_Talker.App app = new Child_Talker.App();

            StartupPath = Environment.CurrentDirectory.Split(new string[] {"bin"}, StringSplitOptions.None )[0];
            
            Console.WriteLine(StartupPath);
            ListEmbeddedResourceNames();
            ThreeButtonArduinoIntegration ai = new ThreeButtonArduinoIntegration();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;


            app.InitializeComponent();
            _ = app.Run();

        }

        
        static void ListEmbeddedResourceNames()
        {
            Trace.WriteLine("Listing Embedded Resource Names");

            foreach (var resource in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                Trace.WriteLine("Resource: " + resource);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Child_Talker.EmbeddedReferences.Newtonsoft.Json.dll"))
            {
                var assemblyData = new Byte[stream.Length];
                _ = stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }


    }
}
