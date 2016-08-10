using System;
using System.Collections.Generic;

namespace TelegramBot
{
    internal class ChordParser
    {
        private readonly string _text;

        public ChordParser(string text)
        {
            _text = text;
        }

        public IEnumerable<string> GetChordNames()
        {
            var list = new List<string>();
            var index = 0;
            while (index < _text.Length)
            {
                var chord = ExtractChord(index);
                if (chord != null)
                {
                    var textValue = chord.ToString();
                    list.Add(textValue);
                    index += textValue.Length;
                }
                else
                {
                    index++;
                }
            }
            return list;
        }

        private Chords? ExtractChord(int index)
        {
            Chords chord;
            var maxChordNameLength = _text.Length - index;
            for (var j = 1; j <= maxChordNameLength; j++)
            {
                var substr = _text.Substring(index, j);
                if (Enum.TryParse(substr, true, out chord))
                {
                    return chord;
                }
            }
            return null;
        }
    }

    internal enum Chords
    {
        A, Am, B, Bm, C, Cm, D, Dm, E, Em, F, Fm, G, Gm
    }
}