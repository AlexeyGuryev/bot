using System;
using System.Collections.Generic;
using System.IO;

namespace TelegramBot
{
    public class AudioCombiner
    {
        private readonly string _path;

        public AudioCombiner(string path)
        {
            _path = path;
        }

        public void Combine(string outputFilename, IEnumerable<string> filenames)
        {
            try
            {
                using (var fs = File.OpenWrite(Path.Combine(_path, outputFilename)))
                {
                    foreach (var filename in filenames)
                    {
                        var buffer = File.ReadAllBytes(filename);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    fs.Flush();
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error combine {0} from {1}. {2}", outputFilename, filenames, exception);
            }
        }
    }
}
