using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Ephemera.NBagOfTricks;


// was namespace Ephemera.NBagOfTricks MusicDefinitions.cs

namespace Ephemera.MusicLib
{
    /// <summary>Definitions for use inside scripts. For doc see MusicDefinitions.md.</summary>
    public class MusicDefs
    {
        #region Singleton
        public static MusicDefs Instance { get { _instance ??= new MusicDefs(); return _instance; } }
        static MusicDefs? _instance;
        #endregion

        #region Fields
        /// <summary>All the builtin scale defs.</summary>
        readonly Dictionary<string, string> _scales = [];

        /// <summary>All the builtin chord defs.</summary>
        readonly Dictionary<string, string> _chords = [];

        /// <summary>All possible note names and aliases.</summary>
        readonly Dictionary<string, int> _notes = [];

        /// <summary>Helpers.</summary>
        readonly Dictionary<string, int> _intervals = [];

        /// <summary>Black and white. TODO put in ini file.</summary>
        readonly List<int> _naturals = [ 0, 2, 4, 5, 7, 9, 11 ];

        /// <summary>The combined chord/scale note definitions. Scripts can add customs.
        /// Key is chord/scale name, value is list of constituent notes.</summary>
        readonly Dictionary<string, List<string>> _compounds = [];

        /// <summary>TODO support other scales?</summary>
        const int NOTES_PER_OCTAVE = 12;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Load all the music definitions.
        /// </summary>
        MusicDefs()
        {
            var ir = new IniReader();
            ir.ParseString(Properties.Resources.music_defs);

            ir.GetValues("scales").ForEach(kv =>
            {
                var parts = kv.Value.SplitByTokens("(),");
                _scales[kv.Key] = parts[0];
                _compounds[kv.Key] = parts[0].SplitByToken(" ");
            });

            ir.GetValues("chords").ForEach(kv =>
            {
                var parts = kv.Value.SplitByTokens("(),");
                _chords[kv.Key] = parts[0];
                _compounds[kv.Key] = parts[0].SplitByToken(" ");
            });

            ir.GetValues("notes").ForEach(kv => 
            {
                //Db = 1
                // ls.Add($"    ['{kv.Key}'] = {kv.Value},");
                _notes[kv.Key] = int.Parse(kv.Value);
            });

            ir.GetValues("intervals").ForEach(kv =>
            {
                //#1 = 1
                // ls.Add($"    ['{kv.Key}'] = {kv.Value},"));
                _intervals[kv.Key] = int.Parse(kv.Value);
            });
        }
        #endregion

        #region Public note manipulation functions
        /// <summary>
        /// Convert note number into name.
        /// </summary>
        /// <param name="inote">Note number</param>
        /// <param name="octave">Include octave</param>
        /// <returns>The note name or empty if invalid.</returns>
        public string NoteNumberToName(int inote, bool octave = true)
        {
            var nname = "";
            var split = SplitNoteNumber(inote);
            var root = _notes.Where(kv => kv.Value == split.root);
            if (root.Any())
            {
                nname = octave ? $"{root.First().Key}{split.octave}" : $"{root.First().Key}";
            }
            return nname;
        }

        /// <summary>
        /// Parse note or notes from input value.
        /// </summary>
        /// <param name="noteString">Standard string to parse.</param>
        /// <returns>List of note numbers - empty if invalid.</returns>
        public List<int> GetNotesFromString(string noteString)
        {
            List<int> notes = [];

            // Parse the input value.
            // Note: Need exception handling here to protect from user script errors.
            try
            {
                // Could be:
                // F4 - named note
                // F4.dim7 - named key/chord
                // F4.FOO - user defined key/chord or scale
                // F4.major - named key/scale

                // Break it up.
                var parts = noteString.SplitByToken(".");
                string snote = parts[0];

                // Start with octave.
                int octave = 4; // default is middle C
                string soct = parts[0].Last().ToString();

                if (soct.IsInteger())
                {
                    octave = int.Parse(soct);
                    snote = snote[..^1];
                }

                // Figure out the root note.
                int noteNum = _notes[snote] % NOTES_PER_OCTAVE;

                if (noteNum >= 0)
                {
                    // Transpose octave.
                    noteNum += (octave + 1) * NOTES_PER_OCTAVE;

                    if (parts.Count > 1)
                    {
                        // It's a chord. M, M7, m, m7, etc. Determine the constituents.
                        var chordNotes = _compounds[parts[1]];
                        //var chordNotes = chordParts[0].SplitByToken(" ");

                        for (int p = 0; p < chordNotes.Count; p++)
                        {
                            string interval = chordNotes[p];
                            bool down = false;

                            if (interval.StartsWith('-'))
                            {
                                down = true;
                                interval = interval.Replace("-", "");
                            }

                            int iint = GetInterval(interval);
                            if (iint >= 0)
                            {
                                iint = down ? iint - NOTES_PER_OCTAVE : iint;
                                notes.Add(noteNum + iint);
                            }
                        }
                    }
                    else
                    {
                        // Just the root.
                        notes.Add(noteNum);
                    }
                }
                else
                {
                    notes.Clear();
                }
            }
            catch (Exception)
            {
                notes.Clear();
                //throw new InvalidOperationException("Invalid note or chord: " + noteString);
            }

            return notes;
        }

        /// <summary>
        /// Is it a white key?
        /// </summary>
        /// <param name="notenum">Which note</param>
        /// <returns>True/false</returns>
        public bool IsNatural(int notenum)
        {
            return _naturals.Contains(SplitNoteNumber(notenum).root % NOTES_PER_OCTAVE);
        }

        /// <summary>
        /// Split a midi note number into root note and octave.
        /// </summary>
        /// <param name="notenum">Absolute note number</param>
        /// <returns>tuple of root and octave</returns>
        (int root, int octave) SplitNoteNumber(int notenum)
        {
            int root = notenum % NOTES_PER_OCTAVE;
            int octave = (notenum / NOTES_PER_OCTAVE) - 1;
            return (root, octave);
        }

        /// <summary>
        /// Get interval offset from name.
        /// </summary>
        /// <param name="sinterval"></param>
        /// <returns>Offset or -1 if invalid.</returns>
        public int GetInterval(string sinterval)
        {
            int flats = sinterval.Count(c => c == 'b');
            int sharps = sinterval.Count(c => c == '#');
            sinterval = sinterval.Replace(" ", "").Replace("b", "").Replace("#", "");

            if (_intervals.ContainsKey(sinterval))
            {
                int iinterval = _intervals[sinterval];
                return iinterval + sharps - flats;
            }

            return -1;
        }

        /// <summary>
        /// Get interval name from note number offset.
        /// </summary>
        /// <param name="iint">The name or empty if invalid.</param>
        /// <returns></returns>
        public string GetIntervalName(int iint)
        {
            var sint = _intervals.Where(kv => kv.Value == iint);
            return sint.Any() ? sint.First().Key : "";
        }

        /// <summary>
        /// Try to make a note and/or chord string from the param. If it can't find a chord return the individual notes.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public List<string> FormatNotes(List<int> notes)
        {
            List<string> snotes = [];

            // Dissect root note.
            foreach (int n in notes)
            {
                int octave = SplitNoteNumber(n).octave;
                int root = SplitNoteNumber(n).root;
                snotes.Add($"\"{NoteNumberToName(root)}{octave}\"");
            }

            return snotes;
        }

        /// <summary>
        /// Add a new chord or scale definition.
        /// </summary>
        /// <param name="name">which</param>
        /// <param name="notes">what</param>
        public void AddCompound(string name, string notes)
        {
            _compounds[name] = notes.SplitByToken(" ");
        }

        /// <summary>
        /// Get a defined chord or scale definition.
        /// </summary>
        /// <param name="name">which</param>
        /// <returns>The list of notes or empty if invalid.</returns>
        public List<string> GetCompound(string name)
        {
            List<string> ret = [];
            if(_compounds.TryGetValue(name, out List<string>? value))
            {
                ret = value;
            }
            //throw new ArgumentException($"Invalid chord or scale: {name}");
            return ret;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Make content from the definitions.
        /// </summary>
        /// <returns>Content.</returns>
        public List<string> GenMarkdown()
        {
            var ir = new IniReader();
            ir.ParseString(Properties.Resources.music_defs);

            List<string> ls = [];

            ls.Add("# Builtin Scales");
            ls.Add("|Scale   | Notes             | Description       | Lower tetrachord  | Upper tetrachord|");
            ls.Add("|------- | ----------------- | ----------------- | ----------------  | ----------------|");
            ir.GetValues("scales").ForEach(kv =>
            {
                // k: Acoustic  v: 1 2 3 #4 5 6 b7 (Acoustic,whole tone,minor)
                var parts = kv.Value.SplitByTokens("(),");
                // [1 2 3 #4 5 6 b7][Acoustic][whole tone][minor]
                while (parts.Count < 4) { parts.Add(""); };
                var rhs = string.Join("|", parts);
                ls.Add($"|{kv.Key}|{rhs}|");
            });
            ls.Add("");

            ls.Add("# Builtin Chords");
            ls.Add("|Chord   | Notes             | Description|");
            ls.Add("|------- | ----------------- | -----------|");
            ir.GetValues("chords").ForEach(kv =>
            {
                var parts = kv.Value.SplitByTokens("(),");
                var rhs = string.Join("|", parts);
                ls.Add($"|{kv.Key}|{rhs}|");
            });
            ls.Add("");

            ls.Add("# Supported Notes");
            ls.Add("|Name |Offset |");
            ls.Add("|---  |-----  |");
            ir.GetValues("notes").ForEach(kv => { ls.Add($"|{kv.Key}|{kv.Value}|"); });
            ls.Add("");

            ls.Add("# Supported Intervals");
            ls.Add("|Name |Offset |");
            ls.Add("|---  |-----  |");
            ir.GetValues("intervals").ForEach(kv => { ls.Add($"|{kv.Key}|{kv.Value}|"); });
            ls.Add("");

            return ls;
        }

        /// <summary>
        /// Make content from the definitions.
        /// </summary>
        /// <returns>Content.</returns>
        public List<string> GenLua(string fn)
        {
            var ir = new IniReader();
            ir.ParseString(Properties.Resources.music_defs);

            List<string> ls = [];

            ls.Add("local M = {}");
            ls.Add("");
            ls.Add("M.NOTES_PER_OCTAVE = 12");
            ls.Add("M.MIDDLE_C = 60");
            ls.Add("M.DEFAULT_OCTAVE = 4");

            ls.Add("");
            ls.Add("-- All the builtin scale defs.");
            ls.Add("local scales =");
            ls.Add("{");
            ir.GetValues("scales").ForEach(kv =>
            {
                // k: Acoustic  v: 1 2 3 #4 5 6 b7 (Acoustic,whole tone,minor)
                var parts = kv.Value.SplitByTokens("(),");
                // [1 2 3 #4 5 6 b7][Acoustic][whole tone][minor]
                ls.Add($"    {kv.Key} = '{parts[0]}',");
            });
            ls.Add("}");

            ls.Add("");
            ls.Add("-- All the builtin chord defs.");
            ls.Add("local chords =");
            ls.Add("{");
            ir.GetValues("chords").ForEach(kv =>
            {
                var parts = kv.Value.SplitByTokens("(),");
                ls.Add($"    ['{kv.Key}'] = '{parts[0]}',");
            });
            ls.Add("}");

            ls.Add("");
            ls.Add("-- All possible note names and aliases as offset from middle C.");
            ls.Add("local notes =");
            ls.Add("{");
            ir.GetValues("notes").ForEach(kv => ls.Add($"    ['{kv.Key}'] = {kv.Value},"));
            ls.Add("}");

            ls.Add("");
            ls.Add("-- Intervals as used in chord and scale defs.");
            ls.Add("local intervals =");
            ls.Add("{");
            ir.GetValues("intervals").ForEach(kv => ls.Add($"    ['{kv.Key}'] = {kv.Value},"));
            ls.Add("}");

            ls.Add("");
            ls.Add("return M");

            return ls;
        }
        #endregion
    }
}
