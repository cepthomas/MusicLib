using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.IO;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace Ephemera.MusicLib.Test
{
    public partial class MainForm : Form
    {
        #region Fields - app
        const string ERROR = "ERR";
        const string WARN = "WRN";
        const string INFO = "INF";
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // The text output.
            txtViewer.Font = new Font("Cascadia Code", 9);
            txtViewer.WordWrap = true;
            txtViewer.MatchText.Add(ERROR, Color.LightPink);
            txtViewer.MatchText.Add(WARN, Color.Plum);
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
        void One_Click(object sender, EventArgs e)
        {
            Tell(INFO, $">>>>> One.");

            TestDefFile();
        }

        void Two_Click(object sender, EventArgs e)
        {
            Tell(INFO, $">>>>> Two.");
        }
        #endregion

        //-------------------------------------------------------------------------------//
        /// <summary>Test def file loading etc.</summary>
        void TestDefFile()
        {
            Tell(INFO, $">>>>> Low level loading.");
            string fn = Path.Combine(AppContext.BaseDirectory, "music_defs.ini");

            // key is section name, value is line
            Dictionary<string, List<string>> res = [];
            var ir = new IniReader(fn);

            ir.Contents.ForEach(ic =>
            {
                Tell(INFO, $"section:{ic.Key} => {ic.Value.Values.Count}");
            });

            Tell(INFO, $">>>>> Gen Markdown.");
            var smd = MusicDefs.GenMarkdown(fn);
            Tell(INFO, $"Markdown:{smd.Left(400)}");

            Tell(INFO, $">>>>> Gen Lua.");
            var sld = MusicDefs.GenLua(fn);
            Tell(INFO, $"Lua:{sld.Left(400)}");
        }

        #region Misc internals
        /// <summary>Tell me something good.</summary>
        /// <param name="s">What</param>
        void Tell(string cat, string s, [CallerFilePath] string file = "", [CallerLineNumber] int line = -1)
        {
            var fn = Path.GetFileName(file);
            txtViewer.AppendLine($"{cat} {fn}({line}) {s}");
        }
        #endregion
    }
}
