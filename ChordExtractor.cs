using System.Collections.Generic;

namespace TelegramBot
{
    internal class ChordExtractor
    {
        private readonly IExistChordParser _parser;
        private readonly string _text;

        public ChordExtractor(IExistChordParser parser, string text)
        {
            _parser = parser;
            _text = text;
        }

        public List<Chord> GetChords()
        {
            var list = new List<Chord>();
            var index = 0;
            while (index < _text.Length)
            {
                int chordSymbolsCount;
                var chord = ExtractChords(index, out chordSymbolsCount);
                if (chord != null)
                {
                    list.Add(chord);
                    index += chordSymbolsCount;
                }
                else
                {
                    index++;
                }
            }
            return list;
        }

        private Chord ExtractChords(int index, out int strLength)
        {
            var maxChordNameLength = _text.Length - index;
            for (strLength = maxChordNameLength; strLength >= 1; strLength--)
            {
                var substr = _text.Substring(index, strLength);
                var foundSequence = _parser.FindExistChord(substr);
                if (foundSequence != null)
                {
                    return new Chord(substr, foundSequence);
                }
            }
            return null;
        }
    }

    internal class Chord
    {
        public string Name { get; set; }
        public string FileName { get; set; }

        public Chord(string name, string fileName)
        {
            Name = name;
            FileName = fileName;
        }
    }
}