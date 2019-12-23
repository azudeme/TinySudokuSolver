using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku {
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
            private bool[] number = new bool[9] { true, true, true, true, true, true, true, true, true };
            public IReadOnlyList<bool> Number { get => number; }
            public void Unset(int n) => number[n - 1] = false;
            public void SetFixed(int n) => number = Enumerable.Range(0, 9).Select((x, i) => i == n-1).ToArray();
            public bool IsFixed => Number.Count(t => t == true) == 1;
            public int AsNumber => IsFixed ? Number.TakeWhile(i => !i).Count()+1 : 0;
            public Field(int n) {
                if (n > 0) SetFixed(n);
            }
        }

        static void Main(string[] args) {
            var field = new Field[9, 9];

            var z = new Field(0).AsNumber;

            var lines = game.Split('\n');

            for (int i = 0; i < lines.Length; i++) {
                for (int j = 0; j < 9; j++) {
                    field[j, i] = new Field(lines[i].Trim()[j] - '0');
                }
            }

            bool Unsure(Field f) => !f.IsFixed;
            bool Sure(Field f) => f.IsFixed;
            IEnumerable<Field> LineV(int x) => Enumerable.Range(0, 9).Select(i => field[x, i]);
            IEnumerable<Field> LineH(int y) => Enumerable.Range(0, 9).Select(i => field[i, y]);

            IEnumerable<(int x, int y, Field f)> AllFields() {
                for (int y = 0; y < 9; y++) {
                    for (int x = 0; x < 9; x++) {
                        yield return (x, y, field[x, y]);
                    }
                }
            }

            IEnumerable<Field> QuadrantFromPos(int x, int y) {
                x -= x % 3;
                y -= y % 3;
                for (int i = x; i < x + 3; i++) {
                    for (int j = y; j < y + 3; j++) {
                        yield return field[i, j];
                    }
                }
            }

            IEnumerable<Field> AvailableFields(int x, int y) =>
                LineH(y).Concat(LineV(x)).Concat(QuadrantFromPos(x, y));

            bool changed;
            do {
                changed = false;
                foreach (var (x, y, f) in AllFields().Where(k => Unsure(k.f))) {
                    foreach (var sureField in AvailableFields(x, y).Where(Sure)) {
                        f.Unset(sureField.AsNumber);
                    }
                    if (f.IsFixed) changed = true;
                }
            } while (changed);

            foreach (var f in AllFields()) {
                Console.Write(f.f.AsNumber);
                Console.Write("\t");
                if (f.x == 8) Console.WriteLine();
            }
        }
    }
}
