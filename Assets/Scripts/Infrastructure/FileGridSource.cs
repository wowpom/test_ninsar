using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Domain;

namespace Game.Infrastructure
{
    public sealed class FileGridSource : IGridSource
    {
        private const int BufferSize = 64;

        private static readonly byte[] Utf8Preamble = Encoding.UTF8.GetPreamble();

        private readonly byte[] _buffer = new byte[BufferSize];

        public async UniTask<OperationResult<ISymbolGrid>> LoadAsync(CancellationToken cancellationToken)
        {
            var path = GridFilePath.Full;

            if (!File.Exists(path))
            {
                return OperationResult<ISymbolGrid>.Fail(
                    $"Не нашёл {GridFilePath.FileName}. Положите его сюда:\n{path}");
            }

            try
            {
                var builder = new SymbolGridBuilder();

                using (var stream = new FileStream(
                           path,
                           FileMode.Open,
                           FileAccess.Read,
                           FileShare.Read,
                           1,
                           FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    var filled = 0;
                    var skipBom = true;

                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var bytesRead = await stream.ReadAsync(
                            _buffer.AsMemory(filled, _buffer.Length - filled),
                            cancellationToken);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        filled += bytesRead;

                        if (skipBom)
                        {
                            if (filled < Utf8Preamble.Length)
                            {
                                continue;
                            }

                            filled = StripPreamble(_buffer, filled);
                            skipBom = false;
                            if (filled == 0)
                            {
                                continue;
                            }
                        }

                        var error = ConsumeCompleteLines(builder, _buffer, ref filled);
                        if (error != null)
                        {
                            return OperationResult<ISymbolGrid>.Fail(error);
                        }
                    }

                    if (skipBom)
                    {
                        filled = StripPreamble(_buffer, filled);
                    }

                    if (filled > 0)
                    {
                        var error = AddRemaining(builder, _buffer, filled);
                        if (error != null)
                        {
                            return OperationResult<ISymbolGrid>.Fail(error);
                        }
                    }
                }

                await UniTask.SwitchToMainThread(cancellationToken);
                return builder.Complete();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (IOException exception)
            {
                return OperationResult<ISymbolGrid>.Fail($"Не смог прочитать {path}: {exception.Message}");
            }
            catch (UnauthorizedAccessException exception)
            {
                return OperationResult<ISymbolGrid>.Fail($"Нет доступа к {path}: {exception.Message}");
            }
        }

        private static int StripPreamble(byte[] buffer, int filled)
        {
            var preamble = Utf8Preamble.AsSpan();
            var chunk = buffer.AsSpan(0, filled);
            if (chunk.Length < preamble.Length || !chunk.Slice(0, preamble.Length).SequenceEqual(preamble))
            {
                return filled;
            }

            var remaining = filled - preamble.Length;
            if (remaining > 0)
            {
                chunk.Slice(preamble.Length, remaining).CopyTo(buffer.AsSpan());
            }

            return remaining;
        }

        private static string ConsumeCompleteLines(SymbolGridBuilder builder, byte[] buffer, ref int filled)
        {
            var chunk = buffer.AsSpan(0, filled);
            var lineStart = 0;

            while (lineStart < chunk.Length)
            {
                var lineBreakIndex = chunk.Slice(lineStart).IndexOfAny((byte)'\n', (byte)'\r');
                if (lineBreakIndex < 0)
                {
                    break;
                }

                var error = builder.AddRow(chunk.Slice(lineStart, lineBreakIndex));
                if (error != null)
                {
                    return error;
                }

                var lineEnd = lineStart + lineBreakIndex;
                lineStart = lineEnd + LineBreakLength(chunk.Slice(lineEnd));
            }

            var rest = chunk.Slice(lineStart);
            if (rest.Length == 0)
            {
                filled = 0;
                return null;
            }

            if (rest.Length == chunk.Length)
            {
                filled = 0;
                return builder.AppendPartial(rest);
            }

            rest.CopyTo(chunk);
            filled = rest.Length;
            return null;
        }

        private static string AddRemaining(SymbolGridBuilder builder, byte[] buffer, int filled)
        {
            return builder.AddRow(buffer.AsSpan(0, filled));
        }

        private static int LineBreakLength(ReadOnlySpan<byte> span)
        {
            if (span.Length >= 2 && span[0] == (byte)'\r' && span[1] == (byte)'\n')
            {
                return 2;
            }

            return 1;
        }
    }
}
