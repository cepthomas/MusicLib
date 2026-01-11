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
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


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
            _outPath = Path.Join(MiscUtils.GetSourcePath(), "out");
            DirectoryInfo di = new(_outPath);
            di.Create();

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

    //-------------------------------------------------------------------------------//
    public class MUSICLIB_API : TestSuite
    {
        public override void RunSuite()
        {
            //List<int> notes = [1, 2, 3];
            UT_FALSE(MusicDefs.IsNatural(3));
            UT_TRUE(MusicDefs.IsNatural(4));
            UT_TRUE(MusicDefs.IsNatural(5));
            UT_FALSE(MusicDefs.IsNatural(-1));
            UT_TRUE(MusicDefs.IsNatural(333));
            UT_EQUAL(MusicDefs.GetInterval("2"), 2);
            UT_EQUAL(MusicDefs.GetInterval("b5"), 6);
            UT_EQUAL(MusicDefs.GetInterval("#11"), 18);
            UT_EQUAL(MusicDefs.GetInterval("xxx"), -1);
            UT_EQUAL(MusicDefs.GetNotesFromString("Db.7#9").Count, 5);
            UT_EQUAL(MusicDefs.GetNotesFromString("booga").Count, 0);
            UT_EQUAL(MusicDefs.FormatNotes([1, 2, 3]).Count, 3);

            UT_EQUAL(MusicDefs.GetIntervalName(12), "8");
            UT_EQUAL(MusicDefs.GetIntervalName(13), "");
            UT_EQUAL(MusicDefs.GetIntervalName(25), "");
            UT_EQUAL(MusicDefs.NoteNumberToName(60), "C4");
            UT_EQUAL(MusicDefs.NoteNumberToName(75), "Eb5");
            UT_EQUAL(MusicDefs.NoteNumberToName(-1), "");
            UT_EQUAL(MusicDefs.NoteNumberToName(145), "Db11");
            UT_EQUAL(MusicDefs.GetCompound("MelodicMinorAscending").Count, 7);
            UT_EQUAL(MusicDefs.GetCompound("7#9").Count, 5);
            UT_EQUAL(MusicDefs.GetCompound("my_scale").Count, 0);
            MusicDefs.AddCompound("my_scale", "#2 b4 5 #9 13");
            UT_EQUAL(MusicDefs.GetCompound("my_scale").Count, 5);
        }
    }
}
