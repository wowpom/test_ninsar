using System;

namespace Game.Domain
{
    public sealed class SymbolGridBuilder
    {
        private const int InitialCellCapacity = 64;

        private Symbol[] _cells = Array.Empty<Symbol>();
        private int _count;
        private int _width;
        private int _height;
        private int _rowLength;
        private bool _hasWidth;

        public string AddRow(ReadOnlySpan<byte> line)
        {
            var error = AppendPartial(line, true);
            if (error != null)
            {
                return error;
            }

            return EndRow();
        }

        public string AddRow(ReadOnlySpan<char> line)
        {
            var start = 0;
            var end = line.Length;

            while (start < end && IsTrimmed(ToByte(line[start])))
            {
                start++;
            }

            while (end > start && IsTrimmed(ToByte(line[end - 1])))
            {
                end--;
            }

            for (var index = start; index < end; index++)
            {
                var error = AppendSymbol(ToByte(line[index]));
                if (error != null)
                {
                    return error;
                }
            }

            return EndRow();
        }

        public string AppendPartial(ReadOnlySpan<byte> fragment)
        {
            return AppendPartial(fragment, false);
        }

        public OperationResult<ISymbolGrid> Complete()
        {
            if (_rowLength > 0)
            {
                var error = EndRow();
                if (error != null)
                {
                    return OperationResult<ISymbolGrid>.Fail(error);
                }
            }

            if (!_hasWidth || _height == 0)
            {
                return OperationResult<ISymbolGrid>.Fail("В файле нет ни одной строки с цифрами.");
            }

            if (_cells.Length != _count)
            {
                Array.Resize(ref _cells, _count);
            }

            return OperationResult<ISymbolGrid>.Ok(new InMemorySymbolGrid(_width, _height, _cells));
        }

        private string AppendPartial(ReadOnlySpan<byte> fragment, bool trimTrailing)
        {
            var start = 0;
            var end = fragment.Length;

            if (_rowLength == 0)
            {
                while (start < end && IsTrimmed(fragment[start]))
                {
                    start++;
                }
            }

            if (trimTrailing)
            {
                while (end > start && IsTrimmed(fragment[end - 1]))
                {
                    end--;
                }
            }

            for (var index = start; index < end; index++)
            {
                var error = AppendSymbol(fragment[index]);
                if (error != null)
                {
                    return error;
                }
            }

            return null;
        }

        private string EndRow()
        {
            if (_rowLength == 0)
            {
                return null;
            }

            if (!_hasWidth)
            {
                _width = _rowLength;
                _hasWidth = true;
            }
            else if (_rowLength != _width)
            {
                return $"Строка {_height + 1} длиной {_rowLength}, а первая — {_width}. Таблица должна быть прямоугольной.";
            }

            _height++;
            _rowLength = 0;
            return null;
        }

        private string AppendSymbol(byte character)
        {
            if ((long)_count + 1 > int.MaxValue)
            {
                return "Таблица слишком большая: ячеек больше, чем влезает в int.";
            }

            if (!SymbolGridParser.TryParseSymbol(character, out var symbol))
            {
                return $"В строке {_height + 1}, позиция {_rowLength + 1}: символ '{(char)character}'. Нужны только {SymbolGridParser.DescribeKnownSymbols()}.";
            }

            EnsureCapacity(_count + 1);
            _cells[_count] = symbol;
            _count++;
            _rowLength++;
            return null;
        }

        private void EnsureCapacity(int needed)
        {
            if (_cells.Length >= needed)
            {
                return;
            }

            var capacity = _cells.Length == 0 ? InitialCellCapacity : _cells.Length;
            while (capacity < needed)
            {
                if (capacity > int.MaxValue / 2)
                {
                    capacity = int.MaxValue;
                    break;
                }

                capacity *= 2;
            }

            Array.Resize(ref _cells, capacity);
        }

        private static byte ToByte(char character)
        {
            return character <= byte.MaxValue ? (byte)character : byte.MaxValue;
        }

        private static bool IsTrimmed(byte value)
        {
            return value == (byte)' ' || value == (byte)'\t';
        }
    }
}
