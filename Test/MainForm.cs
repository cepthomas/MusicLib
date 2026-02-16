using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.IO;
using System.Diagnostics;
using System.Drawing.Design;
using Ephemera.NBagOfTricks;


namespace Ephemera.MusicLib.Test
{
    public partial class MainForm : Form
    {
        #region Fields - app
        const string ERROR = "ERR";
        const string WARN = "WRN";
        const string INFO = "INF";
        #endregion

        /// <summary>Where to put things.</summary>
        readonly string _outPath = "???";

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Make sure out path exists.
            _outPath = MiscUtils.GetSourcePath();

            // The text output.
            txtViewer.Font = new Font("Cascadia Code", 9);
            txtViewer.WordWrap = true;
        }

        /// <summary>
        /// Window is set up now.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                // MidiLibException ex
                // AppException ex

                Tell(ERROR, ex.Message);
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Start here
        void Go_Click(object sender, EventArgs e)
        {
            Tell(INFO, $">>>>> Go start.");

            //TestDefFile();

            //TestMusicDefs();

            Tell(INFO, $">>>>> Go end.");
        }
        #endregion

        //-------------------------------------------------------------------------------//
        /// <summary>Test def file loading etc.</summary>
        void TestDefFile()
        {
            Tell(INFO, $">>>>> Low level loading.");

            var myPath = MiscUtils.GetSourcePath();
            string fn = Path.Combine(myPath, "..", "music_defs.ini");
            var ir = new IniReader();
            ir.ParseFile(fn);

            ir.GetSectionNames().ForEach(name =>
            {
                Tell(INFO, $"section:{name} => {ir.GetValues(name).Count}");
            });

            Tell(INFO, $">>>>> Gen Markdown.");
            var sMusicDefs = MusicDefs.GenMarkdown();
            File.WriteAllText(Path.Join(_outPath, "music_defs.MusicDefs"), string.Join(Environment.NewLine, sMusicDefs));

            Tell(INFO, $">>>>> Gen Lua.");
            var sld = MusicDefs.GenLua();
            File.WriteAllText(Path.Join(_outPath, "music_defs.lua"), string.Join(Environment.NewLine, sld));
        }


        #region Internals
        /// <summary>Tell me something good.</summary>
        /// <param name="s">What</param>
        void Tell(string cat, string s, [CallerFilePath] string file = "", [CallerLineNumber] int line = -1)
        {
            var fn = Path.GetFileName(file);
            txtViewer.AppendText($"{cat} {fn}({line}) {s}{Environment.NewLine}");
        }
        #endregion
    }
}
