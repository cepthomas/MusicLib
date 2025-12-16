using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Ephemera.NBagOfTricks;


// namespace Ephemera.NBagOfTricks  TODO2 remove MusicDefinitions.cs from NBOT

namespace Ephemera.MusicLib
{
    /// <summary>Definitions for use inside scripts. For doc see MusicDefinitions.md.</summary>
    public class MusicDefs
    {
        #region Fields
        /// <summary>The chord/scale note definitions. Key is chord/scale name, value is list of constituent notes.</summary>
        static readonly Dictionary<string, List<string>> _chordsScales = [];

        /// <summary>TODO support other scales?</summary>
        const int NOTES_PER_OCTAVE = 12;
        #endregion




        static readonly string[] _chordDefs = [];

        static readonly string[] _scaleDefs = [];

        static readonly List<string> _noteNames = [];

        static readonly List<string> _intervals = [];

        /// <summary>Helpers.</summary>
        static readonly List<int> _naturals = [ 0, 2, 4, 5, 7, 9, 11 ]; //TODO1 ?


        /// <summary>
        /// Load chord and scale definitions.
        /// </summary>
        static MusicDefs()
        {
            _chordsScales.Clear();

            foreach(string sl in _chordDefs.Concat(_scaleDefs))
            {
                var parts = sl.SplitByToken("|");
                var name = parts[0];
                _chordsScales[name] = parts[1].SplitByToken(" ");
            }
        }

        #region Note definitions
        // /// <summary>All possible note names and aliases.</summary>
        // static readonly List<string> _noteNames =
        // [
        //     "C",  "Db", "D", "Eb", "E",  "F",  "Gb", "G", "Ab", "A",  "Bb", "B",
        //     "B#", "C#", "",  "D#", "Fb", "E#", "F#", "",  "G#", "",   "A#", "Cb",
        //     "1",  "2",  "3", "4",  "5",  "6",  "7",  "8", "9",  "10", "11", "12"
        // ];

        // /// <summary>Helpers.</summary>
        // static readonly List<int> _naturals =
        // [
        //     0, 2, 4, 5, 7, 9, 11
        // ];

        // /// <summary>Helpers.</summary>
        // static readonly List<string> _intervals =
        // [
        //     "1", "b2", "2", "b3", "3", "4",  "b5",  "5", "#5", "6",  "b7", "7",
        //     "",  "",   "9", "#9", "",  "11", "#11", "",  "",   "13", "",   ""
        // ];

        // /// <summary>All the builtin chord defs.</summary>
        // static readonly string[] _chordDefs =
        // [
        //     "M       | 1 3 5             | Named after the major 3rd interval between root and 3.",
        //     "sus2    | 1 2 5             | Sometimes considered as an inverted sus4 (GCD).",
        //     "5       | 1 5               | Power chord."
        // ];

        // /// <summary>All the builtin scale defs.</summary>
        // static readonly string[] _scaleDefs =
        // [
        //     "Acoustic                      | 1 2 3 #4 5 6 b7              | Acoustic scale                           | whole tone        | minor",
        //     "Aeolian                       | 1 2 b3 4 5 b6 b7             | Aeolian mode or natural minor scale      | minor             | Phrygian",
        //     "WholeTone                     | 1 2 3 #4 #5 #6               | Whole tone scale                         |                   |",
        //     "Yo                            | 1 b3 4 5 b7                  | Yo scale                                 |                   |"
        // ];
        #endregion

        #region Public note manipulation functions
        /// <summary>
        /// Convert note number into name.
        /// </summary>
        /// <param name="inote">Note number</param>
        /// <param name="octave">Include octave</param>
        /// <returns></returns>
        public static string NoteNumberToName(int inote, bool octave = true)
        {
            var split = SplitNoteNumber(inote);
            string s = octave ? $"{_noteNames[split.root]}{split.octave}" : $"{_noteNames[split.root]}";
            return s;
        }

        /// <summary>
        /// Convert note name into number.
        /// </summary>
        /// <param name="snote">The root of the note without octave.</param>
        /// <returns>The number or -1 if invalid.</returns>
        public static int NoteNameToNumber(string snote)
        {
            int inote = _noteNames.IndexOf(snote) % NOTES_PER_OCTAVE;
            return inote;
        }

        /// <summary>
        /// Parse note or notes from input value.
        /// </summary>
        /// <param name="noteString">Standard string to parse.</param>
        /// <returns>List of note numbers - empty if invalid.</returns>
        public static List<int> GetNotesFromString(string noteString)
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
                int? noteNum = NoteNameToNumber(snote);
                if (noteNum is not null)
                {
                    // Transpose octave.
                    noteNum += (octave + 1) * NOTES_PER_OCTAVE;

                    if (parts.Count > 1)
                    {
                        // It's a chord. M, M7, m, m7, etc. Determine the constituents.
                        var chordNotes = _chordsScales[parts[1]];
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

                            int? iint = GetInterval(interval);
                            if (iint is not null)
                            {
                                iint = down ? iint - NOTES_PER_OCTAVE : iint;
                                notes.Add(noteNum.Value + iint.Value);
                            }
                        }
                    }
                    else
                    {
                        // Just the root.
                        notes.Add(noteNum.Value);
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
        public static bool IsNatural(int notenum)
        {
            return _naturals.Contains(SplitNoteNumber(notenum).root % NOTES_PER_OCTAVE);
        }

        /// <summary>
        /// Split a midi note number into root note and octave.
        /// </summary>
        /// <param name="notenum">Absolute note number</param>
        /// <returns>tuple of root and octave</returns>
        public static (int root, int octave) SplitNoteNumber(int notenum)
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
        public static int GetInterval(string sinterval)
        {
            int flats = sinterval.Count(c => c == 'b');
            int sharps = sinterval.Count(c => c == '#');
            sinterval = sinterval.Replace(" ", "").Replace("b", "").Replace("#", "");

            int iinterval = _intervals.IndexOf(sinterval);
            return iinterval == -1 ? -1 : iinterval + sharps - flats;
        }

        /// <summary>
        /// Get interval name from note number offset.
        /// </summary>
        /// <param name="iint">The name or empty if invalid.</param>
        /// <returns></returns>
        public static string GetInterval(int iint)
        {
            return iint >= _intervals.Count ? "" : _intervals[iint % _intervals.Count];
        }

        /// <summary>
        /// Try to make a note and/or chord string from the param. If it can't find a chord return the individual notes.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static List<string> FormatNotes(List<int> notes)
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
        public static void AddChordScale(string name, string notes)
        {
            _chordsScales[name] = notes.SplitByToken(" ");
        }

        /// <summary>
        /// Get a defined chord or scale definition.
        /// </summary>
        /// <param name="name">which</param>
        /// <returns>The list of notes or empty if invalid.</returns>
        public static List<string> GetChordScale(string name)
        {
            List<string> ret = [];
            if(_chordsScales.TryGetValue(name, out List<string>? value))
            {
                ret = value;
            }
            //throw new ArgumentException($"Invalid chord or scale: {name}");
            return ret;
        }
        #endregion



        ///////////////////////////////////// TODO1 /////////////////////////////////////////




        // static readonly string[] _chordDefs = [];

        // static readonly string[] _scaleDefs = [];

        // static readonly List<string> _noteNames = [];

        // static readonly List<string> _intervals = [];

        // /// <summary>Helpers.</summary>
        // static readonly List<int> _naturals = [ 0, 2, 4, 5, 7, 9, 11 ]; //TODO1 ?



// [chords]
// ;Chord   Notes            | Description
// M     = 1 3 5             | Named after the major 3rd interval between root and 3.
// ...
// sus2  = 1 2 5             | Sometimes considered as an inverted sus4 (GCD).
// 5     = 1 5               | Power chord.

// [scales]
// ; Scale                   Notes                       | Description                              | Lower tetrachord  | Upper tetrachord
// Acoustic               = 1 2 3 #4 5 6 b7              | Acoustic scale                           | whole tone        | minor
// ...
// WholeTone              = 1 2 3 #4 #5 #6               | Whole tone scale                         |                   |
// Yo                     = 1 b3 4 5 b7                  | Yo scale                                 |                   

// [notes]
// C  = 0
// Db = 1
// ...
// 10 = 9
// 11 = 10
// 12 = 11
// 

// [intervals]
// 1  = 0
// #1 = 1
// ...
// #11 = 18
// 13 = 21




        /// <summary>
        /// Make content from the definitions.
        /// </summary>
        /// <returns>Content.</returns>
        public static string GenMarkdown(string fn) // TODO2 could be lua script?
        {
            // key is section name, value is line
            Dictionary<string, List<string>> res = [];
            var ir = new IniReader(fn);

            List<string> ls = []; // TODO1 order these - alpha or int key

    //; Scale = Notes                       ,Description                          ,Lower tetrachord, Upper tetrachord
    //Acoustic = 1 2 3 #4 5 6 b7             ,Acoustic                             ,whole tone      ,minor           



            ls.Add("# Builtin Scales");
            ls.Add("Scale   | Notes             | Description       | Lower tetrachord  | Upper tetrachord");
            ls.Add("------- | ----------------- | ----------------- | ----------------  | ----------------");
            ir.Contents["scales"].Values.ForEach(kv =>
            {
                var parts = kv.Value.SplitByToken(",");
                var rhs = string.Join("|", parts);
                ls.Add( $"{kv.Key}|{rhs}");
            });

            ls.Add("# Builtin Chords");
            ls.Add("Chord   | Notes             | Description");
            ls.Add("------- | ----------------- | -----------");
            ir.Contents["chords"].Values.ForEach(kv =>
            {
                var parts = kv.Value.SplitByToken(",");
                var rhs = string.Join("|", parts);
                ls.Add($"{kv.Key}|{rhs}");
            });


            ls.Add("# Supported Notes");
            ls.Add("Note");
            ls.Add("---- ");
            ir.Contents["notes"].Values.ForEach(kv => { ls.Add($"{kv.Key}"); });

            ls.Add("# Supported Intervals");
            ls.Add("Interval");
            ls.Add("------- ");
            ir.Contents["intervals"].Values.ForEach(kv => { ls.Add($"{kv.Key}"); });

            return string.Join(Environment.NewLine, ls);
        }

        /// <summary>
        /// Make content from the definitions.
        /// </summary>
        /// <returns>Content.</returns>
        public static string GenLua(string fn) //TODO2 could be lua script?
        {
            // key is section name, value is line
            Dictionary<string, List<string>> res = [];
            var ir = new IniReader(fn);

            List<string> ls = [];

            ls.Add("local M = {}");
            ls.Add("M.NOTES_PER_OCTAVE = 12");
            ls.Add("M.MIDDLE_C4 = 60");
            ls.Add("M.DEFAULT_OCTAVE = 4 -- middle ");

            ls.Add("--- All the builtin chord defs.");
            ls.Add("local chords =");
            ls.Add("{");
            ls.Add("--  Chord   | Notes      | Description");
            ir.Contents["chords"].Values.ForEach(kv => ls.Add($"{kv.Key} = {kv.Value},"));
            ls.Add("}");

            ls.Add("--- All the builtin scale defs.");
            ls.Add("local scales =");
            ls.Add("{");
            ls.Add("--  Scale            | Notes             | Description        | Lower tetrachord  | Upper tetrachord");
            ir.Contents["scales"].Values.ForEach(kv => ls.Add($"{kv.Key} = {kv.Value},"));
            ls.Add("}");

            ls.Add("--- All possible note names and aliases as offset from middle C.");
            ls.Add("local notes =");
            ls.Add("{");
            ir.Contents["notes"].Values.ForEach(kv => ls.Add($"['{kv.Key}'] = {kv.Value},"));
            ls.Add("}");

            ls.Add("--- Intervals as used in chord and scale defs.");
            ls.Add("local intervals =");
            ls.Add("{");
            ir.Contents["intervals"].Values.ForEach(kv => ls.Add($"['{kv.Key}'] = {kv.Value},"));
            ls.Add("}");

            ls.Add("return M");

            return string.Join(Environment.NewLine, ls);
        }
    }
}
