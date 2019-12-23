using System;
using System.Collections.Generic;
using System.Linq;

namespace TinySudokuSolver {
    class Program {
        const string game =
            @"003020600
            900305001
            001806400
            008102900
            700000008
            006708200
            002609500
            800203009
            005010300";

        class Field {
            private bool[] number = Enumerable.Repeat(true, 9).ToArray();

            public void Unset(int n)    => number[n - 1] = false;
            public bool IsFixed         => number.Count(t => t == true) == 1;
            public int AsNumber         => IsFixed ? number.TakeWhile(i => !i).Count()+1 : 0;

            public int X { get; set; }
            public int Y { get; set; }

            public Field(int n) {
                if (n > 0) number = Enumerable.Range(0, 9).Select((_, i) => i == n - 1).ToArray();
            }
        }

        static void ParseField(string game, Field[,] field) {
            var lines = game.Split('\n');

            for (int i = 0; i < lines.Length; i++) {
                for (int j = 0; j < 9; j++) {
                    field[j, i] = new Field(lines[i].Trim()[j] - '0') { X = j, Y = i };
                }
            }
        }

        static void Main(string[] args) {
            var gameField = new Field[9, 9];
            ParseField(game, gameField);
            
            bool Unsure(Field f) => !f.IsFixed;
            bool Sure(Field f) => f.IsFixed;
            IEnumerable<Field> LineV(Field f) => Enumerable.Range(0, 9).Select(i => gameField[f.X, i]);
            IEnumerable<Field> LineH(Field f) => Enumerable.Range(0, 9).Select(i => gameField[i, f.Y]);

            IEnumerable<Field> AllFields() {
                for (int y = 0; y < 9; y++) {
                    for (int x = 0; x < 9; x++) {
                        yield return gameField[x, y];
                    }
                }
            }

            IEnumerable<Field> BlockFromPos(Field f) {
                int x = f.X - f.X % 3;
                int y = f.Y - f.Y % 3;
                for (int i = x; i < x + 3; i++) {
                    for (int j = y; j < y + 3; j++) {
                        yield return gameField[i, j];
                    }
                }
            }

            IEnumerable<Field> AvailableFieldsFor(Field f) =>
                LineH(f).Concat(LineV(f)).Concat(BlockFromPos(f)).Except(f);

            bool changed;
            do {
                changed = false;
                foreach (var unsureField in AllFields().Where(Unsure)) {
                    foreach (var solvedField in AvailableFieldsFor(unsureField).Where(Sure)) {
                        unsureField.Unset(solvedField.AsNumber);
                    }
                    changed |= unsureField.IsFixed;
                }
            } while (changed);

            foreach (var f in AllFields()) {
                Console.Write(f.AsNumber);
                Console.Write("\t");
                if (f.X == 8) Console.WriteLine();
            }
        }
    }

    static class Extensions {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> ts, T elem) => ts.Except(new[] { elem });
    }
}
