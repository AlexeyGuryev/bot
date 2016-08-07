using System.Collections.Generic;
using System.IO;

namespace TelegramBot
{
    public class AudioCombiner
    {
        private string _path;

        public AudioCombiner(string path)
        {
            _path = path;
        }

        public void Combine(string outputFilename, IEnumerable<string> filenames)
        {
            using (var fs = File.OpenWrite(Path.Combine(_path, outputFilename)))
            {
                foreach (var filename in filenames)
                {
                    var buffer = File.ReadAllBytes(Path.Combine(_path, filename));
                    fs.Write(buffer, 0, buffer.Length);
                }
                fs.Flush();
            }
        }
    }
}
