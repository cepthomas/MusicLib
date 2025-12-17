using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.MusicLib.Test
{

    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // // other projects:
            // Application.SetHighDpiMode(HighDpiMode.SystemAware);
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // var f = new MainForm();
            // Application.Run(f);


            // orig:
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new MainForm());


            // unit test:
            TestRunner runner = new(OutputFormat.Readable);
            var cases = new[] { "MUSICLIB_API" };
            runner.RunSuites(cases);
            File.WriteAllLines(Path.Join(MiscUtils.GetSourcePath(), "out", "test.txt"), runner.Context.OutputLines);
        }
    }
}