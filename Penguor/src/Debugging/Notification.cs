namespace Penguor.Compiler.Debugging
{
    /// <summary>
    /// notifications generated by the Penguor compiler
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// the offset where the message occurred in the source code,
        /// only used for PGR messages
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// the number of the message that should be logged
        /// </summary>
        public uint Message { get; }
        /// <summary>
        /// The type of message, currently <c>PGR</c> or <c>PGRCS</c>
        /// </summary>
        public MsgType Type { get; }

        /// <summary>
        /// the arguments which get inserted into the message text
        /// </summary>
        public string[] Args { get; }
        /// <summary>
        /// the source file where the problem occurred
        /// </summary>
        public string File { get; }

        /// <summary>
        /// creates a new notification
        /// </summary>
        /// <param name="file">the file where the notification should be raised</param>
        /// <param name="offset">the offset where the notification occurred</param>
        /// <param name="message">the number of the error message</param>
        /// <param name="type">the type of message, e.g. PGR, PGRCS</param>
        /// <param name="args">additional arguments required to format the message</param>
        public Notification(string file, int offset, uint message, MsgType type, params string[] args)
        {
            File = file;
            Offset = offset;
            Message = message;
            Type = type;
            Args = args;
        }

        /// <summary>
        /// formats the message with the given input string
        /// </summary>
        /// <param name="message">should be the message corresponding to MsgNumber</param>
        public string Format(string message) => $"[{Type}-{string.Format("{0:D4}", Message)}] {string.Format(message, Args)} {(Type != MsgType.PGRCS ? GetSourcePosition(Offset, File) : "")}";
        private static string GetSourcePosition(int offset, string file)
        {
            string source;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
            {
                source = reader.ReadToEnd();
            }
            uint line = 1;
            int column = 1;
            bool gotPos = false;
            for (int i = 0; i < source.Length; i++)
            {
                if (offset == i) gotPos = true;
                if (source[i] == '\n')
                {
                    if (gotPos) break;
                    line++;
                    column = 1;
                    continue;
                }
                if (!gotPos) column++;
            }
            return $"({file}:{line}:{column})";
        }
    }
}