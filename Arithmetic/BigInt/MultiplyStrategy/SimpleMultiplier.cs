using Arithmetic.BigInt.Interfaces;

namespace Arithmetic.BigInt.MultiplyStrategy;

internal class SimpleMultiplier : IMultiplier
{
    public BetterBigInteger Multiply(BetterBigInteger a, BetterBigInteger b)
    {
        var resultNegative = a.IsNegative ^ b.IsNegative;
        var resultDigits = MultiplyMagnitudes(a.GetDigits(), b.GetDigits());

        return new BetterBigInteger(resultDigits, resultNegative);
    }

    internal static uint[] MultiplyMagnitudes(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
    {
        uint[] result = new uint[a.Length + b.Length];

        for (int i = 0; i < a.Length; ++i)
        {
            ulong carry = 0;

            for (int j = 0; j < b.Length; ++j)
            {
                ulong temp = (ulong)a[i] * b[j] + result[i + j] + carry;
                result[i + j] = (uint)temp;
                carry = temp >> 32;
            }

            result[i + b.Length] += (uint)carry;
        }

        return result;
    }
}


