using Arithmetic.BigInt.Interfaces;

namespace Arithmetic.BigInt.MultiplyStrategy;

internal class KaratsubaMultiplier : IMultiplier
{
    internal delegate uint[] SpanMultiplyDelegate(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b);
    private readonly SpanMultiplyDelegate _baseStrategy;
    private readonly int _threshold;

    public KaratsubaMultiplier() : this(SimpleMultiplier.MultiplyMagnitudes, BetterBigInteger.KARATSUBA_THRESHOLD) { }

    public KaratsubaMultiplier(SpanMultiplyDelegate baseStrategy, int threshold = BetterBigInteger.KARATSUBA_THRESHOLD)
    {
        _baseStrategy = baseStrategy ?? throw new ArgumentNullException(nameof(baseStrategy));
        _threshold = threshold;
    }

    public BetterBigInteger Multiply(BetterBigInteger a, BetterBigInteger b)
    {
        try
        {
            uint[] resultDigits = MultiplyRecursive(a.GetDigits(), b.GetDigits());
            bool resultSign = a.IsNegative ^ b.IsNegative;
            return new BetterBigInteger(resultDigits, resultSign);
        }
        catch (OutOfMemoryException)
        {
            throw;
        }
    }

    private uint[] MultiplyRecursive(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
    {
        uint[] z0;
        uint[] z1;
        uint[] z2;

        if (Math.Max(a.Length, b.Length) < _threshold)
        {
            return _baseStrategy(a, b);
        }

        int m = Math.Max(a.Length, b.Length) / 2;

        ReadOnlySpan<uint> a0 = a.Slice(0, Math.Min(a.Length, m));
        ReadOnlySpan<uint> b0 = b.Slice(0, Math.Min(b.Length, m));

        ReadOnlySpan<uint> a1 = a.Length > m ? a.Slice(m) : ReadOnlySpan<uint>.Empty;
        ReadOnlySpan<uint> b1 = b.Length > m ? b.Slice(m) : ReadOnlySpan<uint>.Empty;

        z0 = MultiplyRecursive(a0, b0);
        z2 = MultiplyRecursive(a1, b1);

        uint[] sumA = BetterBigInteger.AddMagnitudes(a0, a1);
        uint[] sumB = BetterBigInteger.AddMagnitudes(b0, b1);

        z1 = MultiplyRecursive(sumA, sumB);

        uint[] zMid = BetterBigInteger.SubtractMagnitudes(z1, z2);
        zMid = BetterBigInteger.SubtractMagnitudes(zMid, z0);

        uint[] shiftedZMid = new uint[zMid.Length + m];
        if (zMid.Length > 0)
        {
            Array.Copy(zMid, 0, shiftedZMid, m, zMid.Length);
        }

        uint[] shiftedZ2 = new uint[z2.Length + 2 * m];
        if (z2.Length > 0)
        {
            Array.Copy(z2, 0, shiftedZ2, 2 * m, z2.Length);
        }

        uint[] result = BetterBigInteger.AddMagnitudes(z0, shiftedZMid);
        result = BetterBigInteger.AddMagnitudes(result, shiftedZ2);

        return TrimZeros(result);
    }

    private static uint[] TrimZeros(uint[] arr)
    {
        int realLength = arr.Length;
        while (realLength > 1 && arr[realLength - 1] == 0)
        {
            realLength--;
        }

        if (realLength == arr.Length) { return arr; }

        uint[] trimmed = new uint[realLength];
        Array.Copy(arr, trimmed, realLength);

        return trimmed;
    }
}
