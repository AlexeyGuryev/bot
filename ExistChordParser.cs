using System;
using System.IO;
using System.Linq;

namespace TelegramBot
{
    internal class ExistChordParser : IExistChordParser
    {
        private readonly string _path;

        public ExistChordParser(string path)
        {
            _path = path;
        }

        public string FindExistChord(string sequence)
        {
            var fileName = string.Format("{0}{1}{2}.mp3", _path, Path.DirectorySeparatorChar, sequence.ToUpper());
            var files = Directory.EnumerateFiles(_path);
            return files.FirstOrDefault(f => string.Equals(f, fileName, StringComparison.OrdinalIgnoreCase));
        }
    }
}