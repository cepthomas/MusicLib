# MusicLib

General purpose music library. It translates between text versions of note/scale/chord and a representation
suitable for use in the digital domain.

Requires VS2022 and .NET8.

# Nomenclature

Internally, individual notes are specified as integers using the standard midi definitions.

Default definitions are in `music_defs.ini` which is loaded at runtime.

Individual notes are described by:
- "C", Db", "B#" - Named note.
- "Eb4" - Named note with octave.

Compound entities:
- "C3.Aeolian" - Scale in the key of C3.
- "F4.o7" - Chord of middle F diminished seventh.

Custom:
- User can create their own chords and scales by `AddCompound("MY_ENTITY", "1 4 6 b13")`.
- "Db5.MY_ENTITY" - Use the custom entity with root of Db.


# API

The Test project demonstrates most of the user functions.


```c#
List<int> GetNotesFromString(string noteString)
```
Convert a string into one or more notes.

```c#
List<string> FormatNotes(List<int> notes)
```
Format note(s) to standard string.

```c#
string NoteNumberToName(int inote, bool octave = true)
```
Format one note to standard string.

```c#
void AddCompound(string name, string notes)
```
Add a chord or scale with user name.

```c#
List<string> GetCompound(string name)
```
Get a chord or scale by name.

```c#
bool IsNatural(int notenum)
```
White or black key note.

```c#
int GetInterval(string sinterval)
```
Convert a string into an interval.

```c#
string GetIntervalName(int iint)
```
Convert an interval to string.

```c#
List<string> GenMarkdown()
```
Make a markdown file from `music_defs.ini`.

```c#
List<string> GenLua()
```
Make a lua file from `music_defs.ini`.
