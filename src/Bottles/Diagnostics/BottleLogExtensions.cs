using System;
using FubuCore;

namespace Bottles.Diagnostics
{
    public static class BottleLogExtensions
    {
        public static void Indent(this IBottleLog log)
        {
            _indent++;
        }

        public static void Unindent(this IBottleLog log)
        {
            _indent--;
        }

        public static void Indent(this IBottleLog log, Action action)
        {
            log.Indent();
            try
            {
                action();
            }
            finally
            {
                log.Unindent();
            }
        }

        public static T Indent<T>(this IBottleLog log, Func<T> action)
        {
            log.Indent();
            try
            {
                return action();
            }
            finally
            {
                log.Unindent();
            }
        }

        private static int _indent = 0;

        public static void Header1(this IBottleLog log, string format, params object[] parameters)
        {
            var text = format.ToFormat(parameters);

            var line = "".PadRight(text.Length, '-');
            log.Write(ConsoleColor.White, line);
            log.Header2(format, parameters);
            log.Write(ConsoleColor.White, line);
        }

        public static void Header2(this IBottleLog log, string format, params object[] parameters)
        {
            log.Write(ConsoleColor.White, format, parameters);
        }

        public static void Highlight(this IBottleLog log, string format, params object[] parameters)
        {
            log.Write(ConsoleColor.Cyan, format, parameters);
        }

        public static void Success(this IBottleLog log, string format, params object[] parameters)
        {
            log.Write(ConsoleColor.Green, format, parameters);
        }

        public static void Fail(this IBottleLog log, string format, params object[] parameters)
        {
            format = indentFormat(format);
            LogWriter.Current.MarkFailure(format.ToFormat(parameters));
        }

        public static void Trace(this IBottleLog log, string format, params object[] parameters)
        {
            log.Write(ConsoleColor.Gray, format, parameters);
        }

        public static void PrintHorizontalLine(this IBottleLog log)
        {
            log.Write(ConsoleColor.White, "".PadRight(80, '-'));
        }

        public static void Write(this IBottleLog log, ConsoleColor color, string format, params object[] parameters)
        {
            format = indentFormat(format);

            LogWriter.Current.Trace(color, format, parameters);
        }

        private static string indentFormat( string format)
        {
            var spaces = _indent > 0 ? string.Empty.PadRight(_indent * 2, ' ') : string.Empty;
            format = spaces + format;
            return format;
        }
    }
}