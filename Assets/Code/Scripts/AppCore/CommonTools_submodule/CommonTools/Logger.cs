using System;
using System.Diagnostics;
using System.IO;


namespace CommonTools
{
    public static class Logger
    {
        private static string PATH_TO_LOG_FILE;

        private static StreamWriter _writer;
        private static int level = 0;

        public static bool IsPaused { get; private set; } = true;

        static Logger()
        {
            Init();
        }


        [Conditional("LOGGING")]
        static void Init()
        {
            PATH_TO_LOG_FILE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "qos_log.txt");
            _writer = File.AppendText(PATH_TO_LOG_FILE);
        }


        [Conditional("LOGGING")]
        public static void Verbose(string msg)
        {
            if (IsPaused) return;
            _writer.Write(new string('\t', level));
            _writer.WriteLine(msg);
            _writer.Flush();
        }


        [Conditional("LOGGING")]
        public static void Error(string msg)
        {
            if (IsPaused) return;
            _writer.WriteLine("ERROR!");
            _writer.Write(new string('\t', level));
            _writer.WriteLine(msg);
            _writer.Flush();
        }


        [Conditional("LOGGING")]
        public static void Debug(string msg)
        {
            if (IsPaused) return;
            _writer.WriteLine("DEBUG!");
            _writer.Write(new string('\t', level));
            _writer.WriteLine(msg);
            _writer.Flush();
        }


        [Conditional("LOGGING")]
        public static void AddLevel()
        {
            level++;
        }


        [Conditional("LOGGING")]
        public static void RemoveLevel()
        {
            level--;
            if (level < 0)
            {
                throw new Exception();
            }
        }


        [Conditional("LOGGING")]
        public static void Pause()
        {
            if (IsPaused) return;
            IsPaused = true;
            _writer.WriteLine("ËÎÃÃÅÐ îòêëþ÷åí!");
        }


        [Conditional("LOGGING")]
        public static void UnPause()
        {
            if (!IsPaused) return;
            _writer.WriteLine("ËÎÃÃÅÐ ñíîâà âêëþ÷åí!");
            IsPaused = false;
        }


        [Conditional("LOGGING")]
        public static void Close()
        {
            _writer.Close();
            _writer.Dispose();
            _writer = null;
        }

    }
}