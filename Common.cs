using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using Ephemera.NBagOfTricks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Text.Json.Serialization;


namespace Ephemera.MusicLib
{
    /// <summary>Library error.</summary>
    public class MusicLibException(string message) : Exception(message) { }

    /// <summary>User selection options.</summary>
    public enum SnapType { Bar, Beat, Sub }

    //public class Defs
    //{
    //    /// <summary>Default value.</summary>
    //    public const double DEFAULT_VOLUME = 0.8;

    //    /// <summary>Allow UI controls some more headroom.</summary>
    //    public const double MAX_VOLUME = 2.0;
    //}

    public class MidiSettings
    {
        /// <summary>How to snap.</summary>
        [DisplayName("Snap Type")]
        [Description("How to snap to grid.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SnapType Snap { get; set; } = SnapType.Beat;

        [DisplayName("Internal Time Resolution")]
        [Description("aka DeltaTicksPerQuarterNote or subdivisions per beat.")]
        [Browsable(false)] // TODO Implement user selectable later maybe.
        [JsonIgnore()]
        public int InternalPPQ { get; set; } = 32;

        /// <summary>Only 4/4 time supported.</summary>
        [Browsable(false)]
        [JsonIgnore()]
        public int BeatsPerBar { get { return 4; } }

        /// <summary>Convenience.</summary>
        [Browsable(false)]
        [JsonIgnore()]
        public int SubsPerBeat { get { return InternalPPQ; } }

        /// <summary>Convenience.</summary>
        [Browsable(false)]
        [JsonIgnore()]
        public int SubsPerBar { get { return InternalPPQ * BeatsPerBar; } }
    }
}
