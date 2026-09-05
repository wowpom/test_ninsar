using System;
using System.Collections.Generic;
using Game.Domain;
using UnityEngine;

namespace Game.Presentation
{
    public static class CubePaletteDefinitionParser
    {
        private const string EntriesField = "entries";
        private const string MaterialAddressField = "materialAddress";

        public static OperationResult<CubePaletteDefinition> Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return OperationResult<CubePaletteDefinition>.Fail("Файл палитры пустой.");
            }

            CubePaletteDefinition definition;

            try
            {
                definition = JsonUtility.FromJson<CubePaletteDefinition>(json);
            }
            catch (ArgumentException exception)
            {
                return OperationResult<CubePaletteDefinition>.Fail(
                    $"Это не JSON: {exception.Message}", exception);
            }

            var error = Validate(definition);
            if (error != null)
            {
                return OperationResult<CubePaletteDefinition>.Fail(error);
            }

            return OperationResult<CubePaletteDefinition>.Ok(definition);
        }

        private static string Validate(CubePaletteDefinition definition)
        {
            if (definition == null)
            {
                return $"В JSON нет объекта с полем «{EntriesField}».";
            }

            if (definition.entries == null || definition.entries.Length == 0)
            {
                return $"Список «{EntriesField}» пустой, нужны записи для символов {DescribeDeclaredSymbols()}.";
            }

            var declared = new HashSet<Symbol>();

            for (var index = 0; index < definition.entries.Length; index++)
            {
                var entry = definition.entries[index];
                var position = index + 1;

                if (entry == null)
                {
                    return $"Запись №{position} пустая.";
                }

                if (entry.symbol < byte.MinValue || entry.symbol > byte.MaxValue
                    || !Enum.IsDefined(typeof(Symbol), (byte)entry.symbol))
                {
                    return $"В записи №{position} символ {entry.symbol}, а можно только {DescribeDeclaredSymbols()}.";
                }

                if (string.IsNullOrWhiteSpace(entry.materialAddress))
                {
                    return $"В записи №{position} у символа {entry.symbol} пустой «{MaterialAddressField}».";
                }

                if (!declared.Add((Symbol)entry.symbol))
                {
                    return $"Символ {entry.symbol} повторён в записи №{position}.";
                }
            }

            var symbols = (Symbol[])Enum.GetValues(typeof(Symbol));

            foreach (var symbol in symbols)
            {
                if (!declared.Contains(symbol))
                {
                    return $"Нет записи для символа {(byte)symbol}. Нужны все: {DescribeDeclaredSymbols()}.";
                }
            }

            return null;
        }

        private static string DescribeDeclaredSymbols()
        {
            var symbols = (Symbol[])Enum.GetValues(typeof(Symbol));
            var names = new string[symbols.Length];

            for (var index = 0; index < symbols.Length; index++)
            {
                names[index] = ((byte)symbols[index]).ToString();
            }

            return string.Join(", ", names);
        }
    }
}
