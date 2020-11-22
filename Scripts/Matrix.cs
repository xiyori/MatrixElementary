using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Elementary.Scripts
{
    public class MyMatrix
    {
        private Rational[,] values;
        private int n, m;

        public static MyMatrix Diag(Rational r, int n)
        {
            MyMatrix diag = new MyMatrix(n);
            for (int i = 0; i < n; i++)
                diag[i, i] = r;
            return diag;
        }

        public static MyMatrix Diag(Rational[] r)
        {
            MyMatrix diag = new MyMatrix(r.Length);
            for (int i = 0; i < r.Length; i++)
                diag[i, i] = r[i];
            return diag;
        }

        public MyMatrix(int n)
        {
            this.n = n;
            m = n;
            values = new Rational[n, n];
            Reset();
        }

        public MyMatrix(int n, int m)
        {
            this.n = n;
            this.m = m;
            values = new Rational[n, m];
            Reset();
        }

        public MyMatrix(Rational[,] elements)
        {
            n = (int)Math.Sqrt(elements.Length);
            values = elements;
        }

        public MyMatrix(Rational[,] elements, int width)
        {
            n = width;
            m = elements.Length / width;
            values = elements;
        }

        public void Reset()
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    values[i, j] = 0;
        }

        public void Reset(Rational number)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    values[i, j] = number;
        }

        public void Randomize(Rational min, Rational max, Rational step)
        {
            if (max < min) throw new InvalidOperationException("Max must be greater than min!");
            Random rand = new Random();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    values[i, j] = rand.Next((max - min) / step + 1) * step + min;
        }

        public void ElementarySum(int index_to, int index_from, Rational coeff, bool row = true)
        {
            if (row)
            {
                for (int j = 0; j < M; j++)
                    values[index_to, j] += coeff * values[index_from, j];
            }
            else
            {
                for (int i = 0; i < N; i++)
                    values[i, index_to] += coeff * values[i, index_from];
            }
        }

        public void ElementaryProd(int index, Rational coeff, bool row = true)
        {
            if (row)
            {
                for (int j = 0; j < M; j++)
                    values[index, j] *= coeff;
            }
            else
            {
                for (int i = 0; i < N; i++)
                    values[i, index] *= coeff;
            }
        }

        public void ElementarySwap(int index1, int index2, bool row = true)
        {
            if (index1 == index2)
                return;
            if (row)
            {
                for (int j = 0; j < M; j++)
                {
                    Rational tmp = values[index1, j].Clone();
                    values[index1, j] = values[index2, j].Clone();
                    values[index2, j] = tmp;
                }
            }
            else
            {
                for (int i = 0; i < N; i++)
                {
                    Rational tmp = values[i, index1].Clone();
                    values[i, index1] = values[i, index2].Clone();
                    values[i, index2] = tmp;
                }
            }
        }

        public MyMatrix Apply(MyMatrix vector)
        {
            MyMatrix output = new MyMatrix(n, 1);
            if (vector.N != n || vector.M != 1)
                throw new InvalidOperationException("Dimension mismatch!");
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    output[i, 0] += values[i, j] * vector[j, 0];
            return output;
        }

        public void Sum(Rational k)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    values[i, j] += k;
        }

        public Rational Sum()
        {
            Rational sum = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    sum += values[i, j];
            return sum;
        }

        public void Inverse()
        {
            values = Inverted().values;
        }

        public MyMatrix Inverted()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Matrix is not square!");
            MyMatrix tmp = new MyMatrix(n, n * 2);
            tmp.Set(this);
            for (int i = 0; i < n; i++)
                tmp[i, i + n] = 1;
            if (tmp.Gauss(n) == 0)
                throw new DivideByZeroException("Determinant zero!");
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    tmp[i, j] = tmp[i, j + n];
            tmp.M = n;
            //if (!(tmp * this).Equals(Id))
            //    throw new ArithmeticException("Cannot find inverse matrix!");
            return tmp;
        }

        public string InverseLaTeX()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Matrix is not square!");
            if (Determinant == 0)
                throw new DivideByZeroException("Determinant zero!");
            MyMatrix tmp = new MyMatrix(n, n * 2);
            tmp.Set(this);
            for (int i = 0; i < n; i++)
                tmp[i, i + n] = 1;
            string output = tmp.GaussLaTeX(n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    tmp[i, j] = tmp[i, j + n];
            tmp.M = n;
            /*try
            {
                if (!(tmp * this).Equals(Id))
                    throw new Exception();
            }
            catch { throw new ArithmeticException("Cannot find inverse matrix!"); }*/
            values = tmp.values;
            return output;
        }

        public void Transpose()
        {
            Rational temp;
            if (m == n)
            {
                for (int i = 0; i < n - 1; i++)
                    for (int j = i + 1; j < n; j++)
                    {
                        temp = values[i, j].Clone();
                        values[i, j] = values[j, i].Clone();
                        values[j, i] = temp;
                    }
            } else
            {
                int old_m = m, old_n = n;
                M = Math.Max(m, n);
                N = M;
                Transpose();
                M = old_n;
                N = old_m;
            }
        }

        public MyMatrix Transposed()
        {
            MyMatrix transposed = Clone();
            transposed.Transpose();
            return transposed;
        }

        public string ImBasis()
        {
            MyMatrix row_echelon = Clone();
            string output = row_echelon.GaussLaTeX(revert: false) + '\n';
            LaTeXExport export = new LaTeXExport(1, ",", 1, false, "$$\\text{Im basis}:\\ ");
            for (int i = 0; i < n; i++)
            {
                int j_s = 0;
                for (; j_s < m && row_echelon[i, j_s] == 0; j_s++) ;
                if (j_s >= m) break;
                export.Add(GetColumn(j_s).GetLaTeX());
            }
            if (export.Empty)
                export.Add(GetColumn(0).GetLaTeX());
            return output + export.Result;
        }

        public string KerBasis()
        {
            MyMatrix row_echelon = Clone();
            string output = row_echelon.GaussLaTeX() + '\n';
            LaTeXExport export = new LaTeXExport(1, ",", 1, false, "$$\\ker\\ \\text{basis}:\\ ");
            List<int> pivots = new List<int>();
            for (int i = 0; i < n; i++)
            {
                int j_s = 0;
                for (; j_s < m && row_echelon[i, j_s] == 0; j_s++) ;
                if (j_s >= m) break;
                pivots.Add(j_s);
            }
            int j_free = 0;
            for (int j = 0; j < m - pivots.Count; j++, j_free++)
            {
                for (; pivots.Contains(j_free); j_free++) ;
                MyMatrix basis_el = new MyMatrix(m, 1);
                basis_el[j_free, 0] = 1;
                for (int i = 0; i < pivots.Count; i++)
                    basis_el[pivots[i], 0] = -row_echelon[i, j_free];
                export.Add(basis_el.GetLaTeX());
            }
            if (export.Empty)
                export.Add(new MyMatrix(m, 1).GetLaTeX());
            return output + export.Result;
        }

        public bool CheckSymmetry()
        {
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < m; j++)
                    if (values[i, j] != values[j, i])
                        return false;
            return true;
        }

        public string SymmetricGauss()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Matrix is not square!");
            if (!CheckSymmetry())
                throw new InvalidOperationException("Matrix is not symmetric!");
            LaTeXExport export = new LaTeXExport(m);
            export.Add(GetLaTeX());
            for (int i = 0; i < n; i++)
            {
                int i_start = i;
                for (; i_start < n && values[i_start, i_start] == 0; i_start++) ;
                if (i_start == n)
                {
                    int j = i;
                    for (; j < n - 1; j++)
                    {
                        for (i_start = j + 1; i_start < n &&
                            values[i_start, j] == 0; i_start++) ;
                        if (i_start < n) break;
                    }
                    if (j == n - 1 && i_start == n)
                        break;
                    ElementarySum(j, i_start, 1);
                    ElementarySum(j, i_start, 1, false);
                    export.Add(GetLaTeX());
                    i_start = j;
                }
                if (i_start != i)
                {
                    ElementarySwap(i, i_start);
                    ElementarySwap(i, i_start, false);
                    export.Add(GetLaTeX());
                }
                for (int i_down = i + 1; i_down < n; i_down++)
                    if (values[i_down, i] != 0)
                    {
                        ElementarySum(i_down, i, -values[i_down, i] / values[i, i]);
                        ElementarySum(i_down, i, -values[i, i_down] / values[i, i], false);
                        export.Add(GetLaTeX());
                    }
            }
            bool need_entry = false;
            for (int i = 0; i < n; i++)
                if (values[i, i] != 1 && values[i, i] != -1 && values[i, i] != 0)
                {
                    need_entry = true;
                    values[i, i] /= values[i, i].Abs();
                }
            if (need_entry) export.Add(GetLaTeX());
            return export.Result;
        }

        public string JacobiMethod()
        {
            throw new NotImplementedException();
        }

        public string GramSchmidt()
        {
            string output = "\\begin{align*}\n&" + GetRow(0).GetLaTeXLine() + "\\\\\n";
            for (int i = 1; i < n; i++)
            {
                MyMatrix row = GetRow(i);
                LaTeXExport export = new LaTeXExport(m + 1, "-", 1, begin: "&", end: (i < n - 1 ? "\\\\\n" : "\n"), new_line: "\\\\\n&");
                export.Add(row.GetLaTeXLine());
                for (int j = 0; j < i; j++)
                {
                    MyMatrix j_row = GetRow(j);
                    if (j_row.IsZero) continue;
                    Rational scalar = ScalarProduct(GetRow(i), j_row),
                        square = ScalarProduct(j_row, j_row);
                    Rational.RemoveDivisor(ref scalar, ref square);
                    export.Add($"\\frac{{{scalar}}}{{{square}}}" + j_row.GetLaTeXLine());
                    row -= (scalar / square) * j_row;
                }
                export.Add(row.GetLaTeXLine(), "=", 1);
                output += export.Result;
                SetRow(i, row);
            }
            return output + "\\end{align*}";
        }

        public MyMatrix Adjunct()
        {
            if (n != m)
                throw new InvalidOperationException("Dimension mismatch!");
            MyMatrix adjunct = new MyMatrix(n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    adjunct[i, j] = Adjunct(i, j);
            return adjunct;
        }

        public Rational Determinant
        {
            get
            {
                if (n != m)
                    return null;
                if (n == 1) return values[0, 0];
                else if (n == 2) return values[0, 0] * values[1, 1] - values[0, 1] * values[1, 0];
                return Clone().Gauss();
            }
        }

        public MyMatrix SubMatrix(int i, int j)
        {
            if (n != m)
                throw new InvalidOperationException("Dimension mismatch!");
            MyMatrix submatrix = new MyMatrix(n - 1);
            int offsetx = 0, offsety = 0;
            for (int y = 0; y < n; y++)
            {
                if (y == i)
                {
                    offsety = 1;
                    continue;
                }
                offsetx = 0;
                for (int x = 0; x < n; x++)
                {
                    if (x == j)
                    {
                        offsetx = 1;
                        continue;
                    }
                    submatrix[y - offsety, x - offsetx] = values[y, x];
                }
            }
            return submatrix;
        }

        public MyMatrix Clone()
        {
            MyMatrix newmatrix = new MyMatrix(n, m);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    newmatrix[i, j] = values[i, j].Clone();
            return newmatrix;
        }

        public void Set(MyMatrix matrix)
        {
            for (int i = 0; i < Math.Min(n, matrix.n); i++)
                for (int j = 0; j < Math.Min(m, matrix.m); j++)
                    values[i, j] = matrix[i, j];
        }

        public Rational Gauss(int max_m = -1)
        {
            if (max_m == -1) max_m = m;
            Rational det = 1;
            for (int i = 0; i < Math.Min(n, max_m); i++)
            {
                int j_start = i - 1, i_start;
                do
                {
                    j_start++;
                    for (i_start = i; i_start < n &&
                        values[i_start, j_start] == 0; i_start++) ;
                } while (j_start < max_m - 1 && i_start == n);
                if (i_start == n && j_start == max_m - 1)
                {
                    det = 0;
                    break;
                }
                if (i != i_start)
                {
                    det *= -1;
                    ElementarySwap(i, i_start);
                }
                if (i != j_start)
                {
                    det *= -1;
                    ElementarySwap(i, j_start, false);
                }
                det *= values[i, i];
                ElementaryProd(i, 1 / values[i, i]);
                for (int i_down = i + 1; i_down < n; i_down++)
                    if (values[i_down, i] != 0)
                        ElementarySum(i_down, i, -values[i_down, i] / values[i, i]);
            }
            for (int i = Math.Min(n - 1, max_m - 1); i >= 0; i--)
            {
                if (values[i, i] != 0)
                {
                    for (int i_up = i - 1; i_up >= 0; i_up--)
                        if (values[i_up, i] != 0)
                            ElementarySum(i_up, i, -values[i_up, i] / values[i, i]);
                }
            }
            return det;
        }

        public string GaussLaTeX(int max_m = -1, bool revert = true)
        {
            if (max_m == -1) max_m = m;
            LaTeXExport export = new LaTeXExport(m);
            export.Add(GetLaTeX(max_m));
            bool need_entry = false;
            for (int i = 0; i < Math.Min(n, max_m); i++)
            {
                int j_start = i - 1, i_start;
                do
                {
                    j_start++;
                    for (i_start = i; i_start < n &&
                        values[i_start, j_start] == 0; i_start++) ;
                } while (j_start < max_m - 1 && i_start == n);
                if (i_start == n && j_start == max_m - 1) break;
                ElementarySwap(i, i_start);
                if (values[i, j_start] != 1) need_entry = true;
                ElementaryProd(i, 1 / values[i, j_start]);
                if (i != i_start)
                {
                    export.Add(GetLaTeX(max_m));
                    need_entry = false;
                }
                bool done_sum = false;
                for (int i_down = i + 1; i_down < n; i_down++)
                    if (values[i_down, j_start] != 0)
                    {
                        ElementarySum(i_down, i, -values[i_down, j_start] / values[i, j_start]);
                        done_sum = true;
                    }
                if (done_sum)
                {
                    export.Add(GetLaTeX(max_m));
                    need_entry = false;
                }
            }
            if (revert)
            {
                for (int i = n - 1; i >= 0; i--)
                {
                    int j_s = 0;
                    for (; j_s < max_m && values[i, j_s] == 0; j_s++) ;
                    if (j_s >= max_m) continue;
                    bool done_sum = false;
                    for (int i_up = i - 1; i_up >= 0; i_up--)
                        if (values[i_up, j_s] != 0)
                        {
                            ElementarySum(i_up, i, -values[i_up, j_s] / values[i, j_s]);
                            done_sum = true;
                        }
                    if (done_sum)
                    {
                        export.Add(GetLaTeX(max_m));
                        need_entry = false;
                    }
                } 
            }
            if (need_entry)
                export.Add(GetLaTeX(max_m));
            return export.Result;
        }

        public static MyMatrix operator *(MyMatrix A, MyMatrix B)
        {
            if (A.m != B.n) throw new InvalidOperationException("Dimension mismatch!");
            MyMatrix AB = new MyMatrix(A.n, B.m);
            for (int i = 0; i < AB.n; i++)
                for (int j = 0; j < AB.m; j++)
                    for (int l = 0; l < A.m; l++)
                        AB[i, j] += A[i, l] * B[l, j];
            return AB;
        }

        public static MyMatrix operator *(Rational a, MyMatrix B)
        {
            MyMatrix aB = B.Clone();
            for (int i = 0; i < B.n; i++)
                for (int j = 0; j < B.m; j++)
                    aB[i, j] *= a;
            return aB;
        }
        public static MyMatrix operator *(MyMatrix A, Rational b) => b * A;
        public static MyMatrix operator /(MyMatrix A, Rational b) => A * (1 / b);

        public static MyMatrix operator +(MyMatrix A, MyMatrix B)
        {
            if (A.n != B.n || A.m != B.m) throw new InvalidOperationException("Dimension mismatch!");
            MyMatrix sum = new MyMatrix(A.n, A.m);
            for (int i = 0; i < sum.n; i++)
                for (int j = 0; j < sum.m; j++)
                    sum[i, j] = A[i, j] + B[i, j];
            return sum;
        }

        public static MyMatrix operator -(MyMatrix A, MyMatrix B)
        {
            if (A.n != B.n || A.m != B.m) throw new InvalidOperationException("Dimension mismatch!");
            MyMatrix subt = new MyMatrix(A.n, A.m);
            for (int i = 0; i < subt.n; i++)
                for (int j = 0; j < subt.m; j++)
                    subt[i, j] = A[i, j] - B[i, j];
            return subt;
        }

        public static Rational ScalarProduct(MyMatrix a, MyMatrix b)
        {
            MyMatrix a_n, b_n;
            if (a.m == 1) a_n = a.Transposed();
            else a_n = a;
            if (b.m == 1) b_n = b;
            else b_n = b.Transposed();
            return (a_n * b_n).Sum();
        }

        public bool Equals(MyMatrix other)
        {
            if (n != other.n || m != other.m)
                return false;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    if (values[i, j] != other[i, j])
                        return false;
            return true;
        }

        public MyMatrix Id
        {
            get
            {
                if (n != m)
                    throw new InvalidOperationException("Dimension mismatch!");
                MyMatrix id = new MyMatrix(n);
                for (int i = 0; i < n; i++)
                    id[i, i] = 1;
                return id;
            }
        }

        public int Rank
        {
            get
            {
                MyMatrix row_echelon = Clone();
                row_echelon.Gauss();
                for (int i = 0; i < n; i++)
                {
                    int j = i;
                    for (; j < m; j++)
                        if (row_echelon[i, j] != 0)
                            break;
                    if (j == m) return i;
                }
                return n;
            }
        }

        public int N
        {
            get => n;
            set
            {
                if (value != n)
                {
                    MyMatrix tmp = new MyMatrix(value, m);
                    tmp.Set(this);
                    values = tmp.values;
                    n = value;
                }
            }
        }

        public int M
        {
            get => m;
            set
            {
                if (value != m)
                {
                    MyMatrix tmp = new MyMatrix(n, value);
                    tmp.Set(this);
                    values = tmp.values;
                    m = value;
                }
            }
        }

        public bool IsZero
        {
            get
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < m; j++)
                        if (values[i, j] != 0) return false;
                return true;
            }
        }
        public bool IsSquare => n == m;
        public int[] Dimensions => new int[] { N, M };
        public Rational Adjunct(int i, int j) => (int)Math.Pow(-1, i + j) * Minor(i, j);
        public Rational Minor(int i, int j) => SubMatrix(i, j).Determinant;
        public Rational this[int i, int j]
        {
            get { return values[i, j]; }
            set { values[i, j] = value; }
        }
        public Rational this[int[] coords]
        {
            get { return values[coords[0], coords[1]]; }
            set { values[coords[0], coords[1]] = value; }
        }
        public MyMatrix GetColumn(int j)
        {
            MyMatrix clmn = new MyMatrix(n, 1);
            for (int i = 0; i < n; i++)
                clmn[i, 0] = values[i, j].Clone();
            return clmn;
        }
        public void SetColumn(int j, MyMatrix column)
        {
            if (column.N != n || column.M != 1)
                throw new InvalidOperationException("Dimension mismatch!");
            for (int i = 0; i < n; i++)
                values[i, j] = column[i, 0];
        }
        public MyMatrix GetRow(int i)
        {
            MyMatrix rw = new MyMatrix(1, m);
            for (int j = 0; j < m; j++)
                rw[0, j] = values[i, j].Clone();
            return rw;
        }
        public void SetRow(int i, MyMatrix row)
        {
            if (row.N != 1 || row.M != m)
                throw new InvalidOperationException("Dimension mismatch!");
            for (int j = 0; j < m; j++)
                values[i, j] = row[0, j];
        }

        public List<List<Rational>> Rows
        {
            get
            {
                List<List<Rational>> rows = new List<List<Rational>>(n);
                for (int i = 0; i < n; i++)
                {
                    List<Rational> row = new List<Rational>(m);
                    for (int j = 0; j < m; j++)
                        row.Add(values[i, j]);
                    rows.Add(row);
                }
                return rows;
            }
        }

        public string GetLaTeX(string begin = "\\begin{pmatrix}",
            string end = "\\end{pmatrix}")
        {
            string output = begin + "\n";
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (j != 0) output += " & ";
                    output += values[i, j].GetLaTeX();
                }
                output += "\\\\\n";
            }
            return output + end;
        }

        public string GetLaTeX(int max_m)
        {
            if (max_m == m) return GetLaTeX();
            string begin = "\\left(\\begin{array}{",
                end = "\\end{array}\\right)";
            for (int i = 0; i < max_m; i++) begin += "c";
            begin += "|";
            for (int i = max_m; i < m; i++) begin += "c";
            begin += "}";
            return GetLaTeX(begin, end);
        }

        public string GetLaTeXLine()
        {
            string output = "\\left(";
            for (int j = 0; j < m; j++)
            {
                if (j != 0) output += ",";
                output += values[0, j].GetLaTeX();
            }
            return output + "\\right)"; 
        }
    }
}
