namespace Penguor.Compiler.Debugging
{
    record OnLoggedEventArgs
    {
        public string? SourceFile { get; }
        public LogLevel LogLevel { get; }

        public OnLoggedEventArgs(string? sourceFile, LogLevel logLevel)
        {
            SourceFile = sourceFile;
            LogLevel = logLevel;
        }
    }
}