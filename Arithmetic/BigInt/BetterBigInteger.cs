using Arithmetic.BigInt.Interfaces;
using Arithmetic.BigInt.MultiplyStrategy;
using System.Runtime.InteropServices;

namespace Arithmetic.BigInt;

public sealed class BetterBigInteger : IBigInteger
{
    private int _signBit;

    private uint _smallValue; // Если число маленькое, храним его прямо в этом поле, а _data == null.
    private uint[]? _data;

    public bool IsNegative => _signBit == 1;

    /// От массива цифр (little endian)
    public BetterBigInteger(uint[] digits, bool isNegative = false)
    {
        InitializeFrom(digits, isNegative);
    }

    public BetterBigInteger(IEnumerable<uint> digits, bool isNegative = false) : this(digits.ToArray(), isNegative)
    {

    }

    public BetterBigInteger(string value, int radix)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("incorrect argument");
        }

        if (radix < 2 || radix > 36)
        {
            throw new ArgumentOutOfRangeException("incorrect radix");
        }

        bool isNegative = false;
        int startIndex = 0;

        if (value[0] == '-')
        {
            startIndex = 1;
            isNegative = true;
        }
        else if (value[0] == '+')
        {
            startIndex = 1;
        }

        if (startIndex == 1 && value.Length == 1)
        {
            throw new ArgumentException("incorrect string");
        }

        List<uint> result = new List<uint>();

        for (int i = startIndex; i < value.Length; ++i)
        {
            char c = value[i];
            int digitValue;

            if (c >= '0' && c <= '9')
            {
                digitValue = c - '0';
            }
            else if (c >= 'A' && c <= 'Z')
            {
                digitValue = c - 'A' + 10;
            }
            else if (c >= 'a' && c <= 'z')
            {
                digitValue = c - 'a' + 10;
            }
            else
            {
                throw new FormatException($"incorrect symbol: '{c}' in number");
            }

            if (digitValue >= radix)
            {
                throw new FormatException($"symbol '{c}' incorrect for system with base: {radix}");
            }

            ulong carry = (ulong)digitValue;

            for (int j = 0; j < result.Count; j++)
            {
                ulong temp = (ulong)result[j] * (ulong)radix + carry;

                result[j] = (uint)temp;

                carry = temp >> 32;
            }

            if (carry > 0)
            {
                result.Add((uint)carry);
            }
        }

        InitializeFrom(result.ToArray(), isNegative);
    }


    public ReadOnlySpan<uint> GetDigits()
    {
        if (_data != null)
        {
            return _data;
        }

        return MemoryMarshal.CreateReadOnlySpan(ref _smallValue, 1);
    }

    public int CompareTo(IBigInteger? other) => throw new NotImplementedException();
    public bool Equals(IBigInteger? other) => throw new NotImplementedException();
    public override bool Equals(object? obj) => obj is IBigInteger other && Equals(other);
    public override int GetHashCode() => throw new NotImplementedException();


    public static BetterBigInteger operator +(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();
    public static BetterBigInteger operator -(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();
    public static BetterBigInteger operator -(BetterBigInteger a) => throw new NotImplementedException();
    public static BetterBigInteger operator /(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();
    public static BetterBigInteger operator %(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();


    public static BetterBigInteger operator *(BetterBigInteger a, BetterBigInteger b)
       => throw new NotImplementedException("Умножение делегируется стратегии, выбирать необходимо в зависимости от размеров чисел");

    public static BetterBigInteger operator ~(BetterBigInteger a) => throw new NotImplementedException();
    public static BetterBigInteger operator &(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();
    public static BetterBigInteger operator |(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();
    public static BetterBigInteger operator ^(BetterBigInteger a, BetterBigInteger b) => throw new NotImplementedException();
    public static BetterBigInteger operator <<(BetterBigInteger a, int shift) => throw new NotImplementedException();
    public static BetterBigInteger operator >>(BetterBigInteger a, int shift) => throw new NotImplementedException();

    public static bool operator ==(BetterBigInteger a, BetterBigInteger b) => Equals(a, b);
    public static bool operator !=(BetterBigInteger a, BetterBigInteger b) => !Equals(a, b);
    public static bool operator <(BetterBigInteger a, BetterBigInteger b) => a.CompareTo(b) < 0;
    public static bool operator >(BetterBigInteger a, BetterBigInteger b) => a.CompareTo(b) > 0;
    public static bool operator <=(BetterBigInteger a, BetterBigInteger b) => a.CompareTo(b) <= 0;
    public static bool operator >=(BetterBigInteger a, BetterBigInteger b) => a.CompareTo(b) >= 0;

    public override string ToString() => ToString(10);
    public string ToString(int radix) => throw new NotImplementedException();


    private void InitializeFrom(uint[] digits, bool isNegative)
    {

        if (digits == null)
        {
            throw new ArgumentNullException("null error");
        }

        int realLength = digits.Length;

        while (realLength > 0 && digits[realLength - 1] == 0)
        {
            --realLength;
        }

        if (realLength == 0)
        {
            _smallValue = 0;
            _data = null;
            _signBit = 0;
        }
        else if (realLength == 1)
        {
            _smallValue = digits[0];
            _data = null;
            _signBit = isNegative ? 1 : 0;
        }
        else if (realLength > 1)
        {
            _smallValue = 0;
            _data = new uint[realLength];
            Array.Copy(digits, _data, realLength);
            _signBit = isNegative ? 1 : 0;
        }

    }
}
