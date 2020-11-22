using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Elementary.Scripts
{
    public class Rational
    {
        private long nm, dn;
        private double approx;
        private bool approx_mode = false;
        private const double epsilon = 0.000000000001;

        private static long GCD(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            long rem;
            if (a < b)
            {
                rem = b;
                b = a;
                a = rem;
            }
            while (b > 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        private void simplify()
        {
            if (approx_mode) return;
            long gcd = GCD(nm, dn);
            nm /= gcd;
            dn /= gcd;
            if (dn < 0)
            {
                nm = -nm;
                dn = -dn;
            }
            if (!approx_mode && (Math.Abs(nm) > 3000000000 || dn > 3000000000))
            {
                approx_mode = true;
                approx = (double)nm / dn;
            }
        }

        private Rational(double approx)
        {
            approx_mode = true;
            this.approx = approx;
            if (Math.Abs(approx - Math.Round(approx)) < epsilon)
            {
                approx_mode = false;
                nm = (long)Math.Round(approx);
                dn = 1;
            }
        }

        public Rational(long numerator = 0, long denominator = 1)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException("Denominator cannot be zero.");
            }
            nm = numerator;
            dn = denominator;
            simplify();
        }

        public Rational(string input)
        {
            input = input.Replace(" ", "");
            if (input.Count(p => p == '/') == 1)
            {
                int divInd = input.IndexOf('/');
                nm = Convert.ToInt32(input.Substring(0, divInd));
                dn = Convert.ToInt32(input.Substring(divInd + 1));
            }
            else if (input.Count(p => p == '.') == 1)
            {
                try
                {
                    nm = Convert.ToInt32(input.Replace(".", ""));
                    dn = (int)Math.Pow(10, input.Length - input.IndexOf('.') - 1);
                }
                catch
                {
                    approx_mode = true;
                    approx = Convert.ToDouble(input);
                    dn = 1;
                }
            }
            else
            {
                nm = Convert.ToInt32(input);
                dn = 1;
            }
            if (dn == 0)
            {
                throw new DivideByZeroException("Denominator cannot be zero.");
            }
            simplify();
        }

        public static Rational operator +(Rational a) => a;
        public static Rational operator -(Rational a)
        {
            if (a.approx_mode)
                return new Rational(-a.approx);
            return new Rational(-a.nm, a.dn);
        }
        public static bool operator == (Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return Math.Abs(a.Approx - b.Approx) < epsilon;
            return a.nm == b.nm && a.dn == b.dn;
        }
        public static bool operator !=(Rational a, Rational b) =>
            !(a == b);
        public static bool operator >(Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return a.Approx > b.Approx;
            return (a - b).nm > 0;
        }
        public static bool operator <(Rational a, Rational b)
            => b > a;
        public static bool operator >=(Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return a.Approx >= b.Approx;
            return (a - b).nm >= 0;
        }
        public static bool operator <=(Rational a, Rational b)
            => b >= a;
        public static Rational operator +(Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return new Rational(a.Approx + b.Approx);
            return new Rational(a.nm * b.dn + a.dn * b.nm, a.dn * b.dn);
        }
        public static Rational operator -(Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return new Rational(a.Approx - b.Approx);
            return new Rational(a.nm * b.dn - a.dn * b.nm, a.dn * b.dn);
        }
        public static Rational operator *(Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return new Rational(a.Approx * b.Approx);
            return new Rational(a.nm * b.nm, a.dn * b.dn);
        }
        public static Rational operator /(Rational a, Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return new Rational(a.Approx / b.Approx);
            return new Rational(a.nm * b.dn, a.dn * b.nm);
        }

        public Rational Abs()
        {
            if (approx_mode)
            {
                if (approx < 0) return -this;
                return this;
            }
            if (nm < 0) return -this;
            return this;
        }

        public long Numerator
        {
            get
            {
                if (approx_mode)
                    throw new InvalidOperationException("Rational is in approximate state!");
                return nm;
            }
            set
            {
                nm = value;
                simplify();
            }
        }

        public long Denominator
        {
            get
            {
                if (approx_mode)
                    throw new InvalidOperationException("Rational is in approximate state!");
                return dn;
            }
            set
            {
                if (value == 0)
                    throw new DivideByZeroException("Denominator cannot be zero.");
                dn = value;
                simplify();
            }
        }

        public double Approx
        {
            get
            {
                if (approx_mode)
                    return approx;
                return (double)nm / dn;
            }
        }

        public override string ToString()
        {
            if (approx_mode) return Math.Round(approx, 9).ToString();
            if (dn == 1) return nm.ToString();
            if (dn != 2 && dn != 4 && dn != 8 && dn != 16)
            {
                int p = 10;
                for (int i = 1; i <= 4; i++, p *= 10)
                {
                    if (p % dn == 0)
                        return ((double)nm / dn).ToString();
                }
            }
            return nm + "/" + dn;
        }

        public string GetLaTeX()
        {
            if (approx_mode) return Math.Round(approx, 9).ToString();
            if (dn == 1) return nm.ToString();
            if (dn < 10 && Math.Abs(nm) < 10)
                return (nm > 0 ? "" : "-") + $"\\frac{Math.Abs(nm)}{dn}";
            return (nm > 0 ? "" : "-") + "\\frac{" + Math.Abs(nm) + "}{" + dn + "}";
        }

        public static implicit operator Rational(long numerator) => new Rational(numerator);
        public static implicit operator int(Rational r) => (int)Math.Floor(r.Approx);
        //public static implicit operator Rational(double approx_value) => new Rational(approx_value);

        public static void RemoveDivisor(ref Rational a, ref Rational b)
        {
            if (a.approx_mode || b.approx_mode)
                return;
            Rational common_product = a.dn * b.dn / GCD(a.dn, b.dn);
            a *= common_product;
            b *= common_product;
        }

        public Rational Clone()
        {
            if (approx_mode)
                return new Rational(approx);
            return new Rational(nm, dn);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var rational = obj as Rational;
            if (rational.approx_mode || approx_mode)
                return Math.Abs(Approx - rational.Approx) < epsilon;
            return nm == rational.nm &&
                   dn == rational.dn;
        }

        public override int GetHashCode()
        {
            if (approx_mode)
                return approx.GetHashCode();
            var hashCode = -1537561701;
            hashCode = hashCode * -1521134295 + nm.GetHashCode();
            hashCode = hashCode * -1521134295 + dn.GetHashCode();
            return hashCode;
        }
    }
}
