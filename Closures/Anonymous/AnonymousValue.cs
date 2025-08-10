using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Closures;

internal enum ValueType {
    Reference,

    Char,
    Bool,

    Byte,
    SByte,
    Short,
    UShort,
    Int,
    UInt,
    Long,
    ULong,

    Float,
    Double,
    Decimal,
}

internal static class ValueTypeExtensions {
    public static bool IsOrReference<T>(this ValueType valueType) {
        return valueType switch {
            ValueType.Char => typeof(T) == typeof(char),
            ValueType.Bool => typeof(T) == typeof(bool),

            ValueType.Byte => typeof(T) == typeof(byte),
            ValueType.SByte => typeof(T) == typeof(sbyte),
            ValueType.Short => typeof(T) == typeof(short),
            ValueType.UShort => typeof(T) == typeof(ushort),
            ValueType.Int => typeof(T) == typeof(int),
            ValueType.UInt => typeof(T) == typeof(uint),
            ValueType.Long => typeof(T) == typeof(long),
            ValueType.ULong => typeof(T) == typeof(ulong),

            ValueType.Float => typeof(T) == typeof(float),
            ValueType.Double => typeof(T) == typeof(double),
            ValueType.Decimal => typeof(T) == typeof(decimal),

            ValueType.Reference => true,
            _ => false
        };
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct AnonymousValue : IEquatable<AnonymousValue> {
    [FieldOffset(0)] ValueType ValueType;

    [FieldOffset(8)] object? unknownValue;

    [FieldOffset(16)] char charValue;
    [FieldOffset(16)] bool boolValue;

    [FieldOffset(16)] byte byteValue;
    [FieldOffset(16)] sbyte sbyteValue;
    [FieldOffset(16)] short shortValue;
    [FieldOffset(16)] ushort ushortValue;
    [FieldOffset(16)] int intValue;
    [FieldOffset(16)] uint uintValue;
    [FieldOffset(16)] long longValue;
    [FieldOffset(16)] ulong ulongValue;

    [FieldOffset(16)] float floatValue;
    [FieldOffset(16)] double doubleValue;
    [FieldOffset(16)] decimal decimalValue;

    public static AnonymousValue From<T>(T value) where T : notnull {
        return typeof(T) switch {
            { } t when t == typeof(char) => Char(Unsafe.As<T, char>(ref value)),
            { } t when t == typeof(bool) => Bool(Unsafe.As<T, bool>(ref value)),

            { } t when t == typeof(byte) => Byte(Unsafe.As<T, byte>(ref value)),
            { } t when t == typeof(sbyte) => SByte(Unsafe.As<T, sbyte>(ref value)),
            { } t when t == typeof(short) => Short(Unsafe.As<T, short>(ref value)),
            { } t when t == typeof(ushort) => UShort(Unsafe.As<T, ushort>(ref value)),
            { } t when t == typeof(int) => Int(Unsafe.As<T, int>(ref value)),
            { } t when t == typeof(uint) => UInt(Unsafe.As<T, uint>(ref value)),
            { } t when t == typeof(long) => Long(Unsafe.As<T, long>(ref value)),
            { } t when t == typeof(ulong) => ULong(Unsafe.As<T, ulong>(ref value)),

            { } t when t == typeof(float) => Float(Unsafe.As<T, float>(ref value)),
            { } t when t == typeof(double) => Double(Unsafe.As<T, double>(ref value)),
            { } t when t == typeof(decimal) => Decimal(Unsafe.As<T, decimal>(ref value)),

            _ => Unknown(value)
        };
    }


    public bool Is<T>() => typeof(T) == GetUnderlyingType();

    public T As<T>() {
        if (ValueType.IsOrReference<T>()) {
            return ValueType switch {
                ValueType.Char => Unsafe.As<char, T>(ref charValue),
                ValueType.Bool => Unsafe.As<bool, T>(ref boolValue),

                ValueType.Byte => Unsafe.As<byte, T>(ref byteValue),
                ValueType.SByte => Unsafe.As<sbyte, T>(ref sbyteValue),
                ValueType.Short => Unsafe.As<short, T>(ref shortValue),
                ValueType.UShort => Unsafe.As<ushort, T>(ref ushortValue),
                ValueType.Int => Unsafe.As<int, T>(ref intValue),
                ValueType.UInt => Unsafe.As<uint, T>(ref uintValue),
                ValueType.Long => Unsafe.As<long, T>(ref longValue),

                ValueType.ULong => Unsafe.As<ulong, T>(ref ulongValue),

                ValueType.Float => Unsafe.As<float, T>(ref floatValue),
                ValueType.Double => Unsafe.As<double, T>(ref doubleValue),
                ValueType.Decimal => Unsafe.As<decimal, T>(ref decimalValue),

                ValueType.Reference => unknownValue switch {
                    T t => t,
                    null => throw new ArgumentNullException(nameof(unknownValue)),
                    _ => InvalidCast()
                },
                _ => InvalidCast()
            };
        }

        return InvalidCast();

        T InvalidCast() => throw new InvalidCastException($"Cannot cast AnonymousValue to {typeof(T).Name}.");
    }

    public void SetValue<T>(T value) {
        switch (value) {
            case char c:
                charValue = c;
                ValueType = ValueType.Char;
                return;
            case bool b:
                boolValue = b;
                ValueType = ValueType.Bool;
                return;

            case byte b:
                byteValue = b;
                ValueType = ValueType.Byte;
                return;
            case sbyte sb:
                sbyteValue = sb;
                ValueType = ValueType.SByte;
                return;
            case short s:
                shortValue = s;
                ValueType = ValueType.Short;
                return;
            case ushort us:
                ushortValue = us;
                ValueType = ValueType.UShort;
                return;
            case int i:
                intValue = i;
                ValueType = ValueType.Int;
                return;
            case uint ui:
                uintValue = ui;
                ValueType = ValueType.UInt;
                return;
            case long l:
                longValue = l;
                ValueType = ValueType.Long;
                return;
            case ulong ul:
                ulongValue = ul;
                ValueType = ValueType.ULong;
                return;

            case float f:
                floatValue = f;
                ValueType = ValueType.Float;
                return;
            case double d:
                doubleValue = d;
                ValueType = ValueType.Double;
                return;
            case decimal dec:
                decimalValue = dec;
                ValueType = ValueType.Decimal;
                return;

            default:
                unknownValue = value ??
                               throw new ArgumentNullException(nameof(value),
                                   "Cannot set null value to AnonymousValue.");
                ValueType = ValueType.Reference;
                return;
        }
    }

    public Type? GetUnderlyingType() {
        return ValueType switch {
            ValueType.Char => typeof(char),
            ValueType.Bool => typeof(bool),

            ValueType.Byte => typeof(byte),
            ValueType.SByte => typeof(sbyte),
            ValueType.Short => typeof(short),
            ValueType.UShort => typeof(ushort),
            ValueType.Int => typeof(int),
            ValueType.UInt => typeof(uint),
            ValueType.Long => typeof(long),
            ValueType.ULong => typeof(ulong),

            ValueType.Float => typeof(float),
            ValueType.Double => typeof(double),
            ValueType.Decimal => typeof(decimal),

            _ => unknownValue?.GetType()
        };
    }

    static AnonymousValue Unknown(object value) => new() { unknownValue = value, ValueType = ValueType.Reference };

    static AnonymousValue Char(char value) => new() { charValue = value, ValueType = ValueType.Char };
    static AnonymousValue Bool(bool value) => new() { boolValue = value, ValueType = ValueType.Bool };

    static AnonymousValue Byte(byte value) => new() { byteValue = value, ValueType = ValueType.Byte };
    static AnonymousValue SByte(sbyte value) => new() { sbyteValue = value, ValueType = ValueType.SByte };
    static AnonymousValue Short(short value) => new() { shortValue = value, ValueType = ValueType.Short };
    static AnonymousValue UShort(ushort value) => new() { ushortValue = value, ValueType = ValueType.UShort };
    static AnonymousValue Int(int value) => new() { intValue = value, ValueType = ValueType.Int };
    static AnonymousValue UInt(uint value) => new() { uintValue = value, ValueType = ValueType.UInt };
    static AnonymousValue Long(long value) => new() { longValue = value, ValueType = ValueType.Long };
    static AnonymousValue ULong(ulong value) => new() { ulongValue = value, ValueType = ValueType.ULong };

    static AnonymousValue Float(float value) => new() { floatValue = value, ValueType = ValueType.Float };
    static AnonymousValue Double(double value) => new() { doubleValue = value, ValueType = ValueType.Double };
    static AnonymousValue Decimal(decimal value) => new() { decimalValue = value, ValueType = ValueType.Decimal };

    public bool Equals(AnonymousValue other) {
        if (ValueType != other.ValueType)
            return false;

        return ValueType switch {
            ValueType.Char => charValue == other.charValue,
            ValueType.Bool => boolValue == other.boolValue,

            ValueType.Byte => byteValue == other.byteValue,
            ValueType.SByte => sbyteValue == other.sbyteValue,
            ValueType.Short => shortValue == other.shortValue,
            ValueType.UShort => ushortValue == other.ushortValue,
            ValueType.Int => intValue == other.intValue,
            ValueType.UInt => uintValue == other.uintValue,
            ValueType.Long => longValue == other.longValue,
            ValueType.ULong => ulongValue == other.ulongValue,

            ValueType.Float => floatValue.Equals(other.floatValue),
            ValueType.Double => doubleValue.Equals(other.doubleValue),
            ValueType.Decimal => decimalValue.Equals(other.decimalValue),

            _ => Equals(unknownValue, other.unknownValue)
        };
    }

    public override bool Equals(object? obj) {
        return obj is AnonymousValue value && Equals(value);
    }

    public override int GetHashCode() {
        return ValueType switch {
            ValueType.Char => HashCode.Combine(ValueType, charValue),
            ValueType.Bool => HashCode.Combine(ValueType, boolValue),
            ValueType.Byte => HashCode.Combine(ValueType, byteValue),
            ValueType.SByte => HashCode.Combine(ValueType, sbyteValue),
            ValueType.Short => HashCode.Combine(ValueType, shortValue),
            ValueType.UShort => HashCode.Combine(ValueType, ushortValue),
            ValueType.Int => HashCode.Combine(ValueType, intValue),
            ValueType.UInt => HashCode.Combine(ValueType, uintValue),
            ValueType.Long => HashCode.Combine(ValueType, longValue),
            ValueType.ULong => HashCode.Combine(ValueType, ulongValue),
            ValueType.Float => HashCode.Combine(ValueType, floatValue),
            ValueType.Double => HashCode.Combine(ValueType, doubleValue),
            ValueType.Decimal => HashCode.Combine(ValueType, decimalValue),
            ValueType.Reference => HashCode.Combine(ValueType, unknownValue?.GetHashCode() ?? 0),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static bool operator ==(AnonymousValue left, AnonymousValue right) {
        return left.Equals(right);
    }

    public static bool operator !=(AnonymousValue left, AnonymousValue right) {
        return !(left == right);
    }
}