using System;

namespace Game.Domain
{
    public static class SymbolGridParser
    {
        private const char ByteOrderMark = '\uFEFF';

        public static OperationResult<ISymbolGrid> Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return OperationResult<ISymbolGrid>.Fail("Файл пустой.");
            }

            var builder = new SymbolGridBuilder();
            var span = text.AsSpan();
            if (span.Length > 0 && span[0] == ByteOrderMark)
            {
                span = span.Slice(1);
            }

            var start = 0;
            for (var index = 0; index <= span.Length; index++)
            {
                var isEnd = index == span.Length;
                var isBreak = !isEnd && (span[index] == '\n' || span[index] == '\r');
                if (!isEnd && !isBreak)
                {
                    continue;
                }

                var error = builder.AddRow(span.Slice(start, index - start));
                if (error != null)
                {
                    return OperationResult<ISymbolGrid>.Fail(error);
                }

                if (isEnd)
                {
                    break;
                }

                if (span[index] == '\r' && index + 1 < span.Length && span[index + 1] == '\n')
                {
                    index++;
                }

                start = index + 1;
            }

            return builder.Complete();
        }

        public static bool TryParseSymbol(byte character, out Symbol symbol)
        {
            symbol = default;

            if (character < (byte)'0' || character > (byte)'9')
            {
                return false;
            }

            var digit = (byte)(character - (byte)'0');
            if (!Enum.IsDefined(typeof(Symbol), digit))
            {
                return false;
            }

            symbol = (Symbol)digit;
            return true;
        }

        public static string DescribeKnownSymbols()
        {
            var values = (Symbol[])Enum.GetValues(typeof(Symbol));
            var digits = new string[values.Length];

            for (var index = 0; index < values.Length; index++)
            {
                digits[index] = ((byte)values[index]).ToString();
            }

            return string.Join(", ", digits);
        }
    }
}
