using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.MusicLib.Test
{
    //-------------------------------------------------------------------------------//
    public class MUSICLIB_API : TestSuite
    {
        public override void RunSuite()
        {
            //List<int> notes = [1, 2, 3];
            Assert(!MusicDefs.IsNatural(3));
            Assert(MusicDefs.IsNatural(4));
            Assert(MusicDefs.IsNatural(5));
            Assert(!MusicDefs.IsNatural(-1));
            Assert(MusicDefs.IsNatural(333));
            Assert(MusicDefs.GetInterval("2") == 2);
            Assert(MusicDefs.GetInterval("b5") == 6);
            Assert(MusicDefs.GetInterval("#11") == 18);
            Assert(MusicDefs.GetInterval("xxx") == -1);
            Assert(MusicDefs.GetNotesFromString("Db.7#9").Count == 5);
            Assert(MusicDefs.GetNotesFromString("booga").Count == 0);
            Assert(MusicDefs.FormatNotes([1, 2, 3]).Count == 3);

            Assert(MusicDefs.GetIntervalName(12) == "8");
            Assert(MusicDefs.GetIntervalName(13) == "");
            Assert(MusicDefs.GetIntervalName(25) == "");
            Assert(MusicDefs.NoteNumberToName(60) == "C4");
            Assert(MusicDefs.NoteNumberToName(75) == "Eb5");
            Assert(MusicDefs.NoteNumberToName(-1) == "");
            Assert(MusicDefs.NoteNumberToName(145) == "Db11");
            Assert(MusicDefs.GetCompound("MelodicMinorAscending").Count == 7);
            Assert(MusicDefs.GetCompound("7#9").Count == 5);
            Assert(MusicDefs.GetCompound("my_scale").Count == 0);
            MusicDefs.AddCompound("my_scale", "#2 b4 5 #9 13");
            Assert(MusicDefs.GetCompound("my_scale").Count == 5);
        }
    }
}
