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

            //TestDefFile();

            //TestMusicDefs();
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
            var smd = MusicDefs.Instance.GenMarkdown(fn);
            File.WriteAllText(Path.Join(_outPath, "musicdefs.md"), smd);

            Tell(INFO, $">>>>> Gen Lua.");
            var sld = MusicDefs.Instance.GenLua(fn);
            File.WriteAllText(Path.Join(_outPath, "musicdefs.lua"), sld);
        }


        // //-------------------------------------------------------------------------------//
        // /// <summary>Test note functions.</summary>
        // void TestMusicDefs()
        // {
        // }

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

    //-------------------------------------------------------------------------------//
    public class MUSICLIB_FILE : TestSuite
    {
        public override void RunSuite()
        {
            // Tell(INFO, $">>>>> Low level loading.");
            string fn = Path.Combine(AppContext.BaseDirectory, "music_defs.ini");

            // key is section name, value is line
            Dictionary<string, List<string>> res = [];
            var ir = new IniReader(fn);

            // ir.Contents.ForEach(ic =>
            // {
            //     Tell(INFO, $"section:{ic.Key} => {ic.Value.Values.Count}");
            // });

            // Tell(INFO, $">>>>> Gen Markdown.");
            var smd = MusicDefs.Instance.GenMarkdown(fn);
            //File.WriteAllText(Path.Join(_outPath, "musicdefs.md"), smd);

            // Tell(INFO, $">>>>> Gen Lua.");
            var sld = MusicDefs.Instance.GenLua(fn);
            //File.WriteAllText(Path.Join(_outPath, "musicdefs.lua"), sld);
        }
    }

    //-------------------------------------------------------------------------------//
    public class MUSICLIB_API : TestSuite
    {
        public override void RunSuite()
        {
            var md = MusicDefs.Instance;

            //List<int> notes = [1, 2, 3];
            UT_FALSE(md.IsNatural(3));
            UT_TRUE(md.IsNatural(4));
            UT_TRUE(md.IsNatural(5));
            UT_FALSE(md.IsNatural(-1));
            UT_TRUE(md.IsNatural(333));
            UT_EQUAL(md.GetInterval("2"), 2);
            UT_EQUAL(md.GetInterval("b5"), 6);
            UT_EQUAL(md.GetInterval("#11"), 18);
            UT_EQUAL(md.GetInterval("xxx"), -1);
            UT_EQUAL(md.GetNotesFromString("Db.7#9").Count, 5);
            UT_EQUAL(md.GetNotesFromString("booga").Count, 0);
            UT_EQUAL(md.FormatNotes([1, 2, 3]).Count, 3);

            UT_EQUAL(md.GetIntervalName(12), "8");
            UT_EQUAL(md.GetIntervalName(13), "");
            UT_EQUAL(md.GetIntervalName(25), "");
            UT_EQUAL(md.NoteNumberToName(60), "C4");
            UT_EQUAL(md.NoteNumberToName(75), "Eb5");
            UT_EQUAL(md.NoteNumberToName(-1), "");
            UT_EQUAL(md.NoteNumberToName(145), "Db11");
            UT_EQUAL(md.GetCompound("MelodicMinorAscending").Count, 7);
            UT_EQUAL(md.GetCompound("7#9").Count, 5);
            UT_EQUAL(md.GetCompound("my_scale").Count, 0);
            md.AddCompound("my_scale", "#2 b4 5 #9 13");
            UT_EQUAL(md.GetCompound("my_scale").Count, 5);
        }
    }
}
