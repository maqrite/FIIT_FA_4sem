using Arithmetic.BigInt.Interfaces;
using System.Numerics;

namespace Arithmetic.BigInt.MultiplyStrategy;

internal class FftMultiplier : IMultiplier
{
    public BetterBigInteger Multiply(BetterBigInteger a, BetterBigInteger b)
    {
        try
        {
            var resultNegative = a.IsNegative ^ b.IsNegative;
            var resultDigits = MultiplyFft(a.GetDigits(), b.GetDigits());
            return new BetterBigInteger(resultDigits, resultNegative);
        }
        catch (OutOfMemoryException)
        {
            throw;
        }
    }

    private static uint[] MultiplyFft(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
    {
        int requiredLength = (a.Length + b.Length) * 2;
        int n = 1;

        while (n < requiredLength)
        {
            n <<= 1;
        }

        Complex[] aCmp = new Complex[n];
        Complex[] bCmp = new Complex[n];

        for (int i = 0; i < a.Length; ++i)
        {
            aCmp[2 * i] = new Complex(a[i] & 0xFFFF, 0);
            aCmp[2 * i + 1] = new Complex(a[i] >> 16, 0);
        }

        for (int i = 0; i < b.Length; ++i)
        {
            bCmp[2 * i] = new Complex(b[i] & 0xFFFF, 0);
            bCmp[2 * i + 1] = new Complex(b[i] >> 16, 0);
        }

        Fft(aCmp, false);
        Fft(bCmp, false);

        for (int i = 0; i < n; ++i)
        {
            aCmp[i] *= bCmp[i];
        }

        Fft(aCmp, true);

        uint[] result = new uint[a.Length + b.Length];
        ulong carry = 0;

        for (int i = 0; i < n; ++i)
        {
            ulong value = (ulong)Math.Round(aCmp[i].Real) + carry;

            if (i % 2 == 0)
            {
                result[i / 2] = (uint)(value & 0xFFFF);
            }
            else
            {
                result[i / 2] |= (uint)((value & 0xFFFF) << 16);
            }

            carry = value >> 16;
        }

        return BetterBigInteger.TrimZeros(result);
    }

    private static void Fft(Complex[] a, bool invert)
    {
        int n = a.Length;

        for (int i = 1, j = 0; i < n; ++i)
        {
            int bit = n >> 1;
            for (; j >= bit; bit >>= 1)
            {
                j -= bit;
            }
            j += bit;

            if (i < j)
            {
                Complex temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }

        for (int len = 2; len <= n; len <<= 1)
        {
            double angle = 2 * Math.PI / len * (invert ? -1 : 1);
            Complex wlen = new Complex(Math.Cos(angle), Math.Sin(angle));

            for (int i = 0; i < n; i += len)
            {
                Complex w = Complex.One;
                for (int j = 0; j < len / 2; ++j)
                {
                    Complex u = a[i + j];
                    Complex v = a[i + j + len / 2] * w;

                    a[i + j] = u + v;
                    a[i + j + len / 2] = u - v;

                    w *= wlen;
                }
            }
        }

        if (invert)
        {
            for (int i = 0; i < n; ++i)
            {
                a[i] /= n;
            }
        }
    }
}
