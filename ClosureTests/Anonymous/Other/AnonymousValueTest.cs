using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.Other;

[TestFixture]
[TestOf(typeof(AnonymousValue))]
public class AnonymousValueTest {
    [TestCase('a')]
    [TestCase('Z')]
    public void Char_Roundtrip(char value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<char>(), Is.True);
            Assert.That(anon.As<char>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(char)));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Bool_Roundtrip(bool value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<bool>(), Is.True);
            Assert.That(anon.As<bool>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(bool)));
        });
    }

    [TestCase((byte)1)]
    [TestCase(byte.MaxValue)]
    public void Byte_Roundtrip(byte value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<byte>(), Is.True);
            Assert.That(anon.As<byte>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(byte)));
        });
    }

    [TestCase((sbyte)-1)]
    [TestCase(sbyte.MaxValue)]
    public void SByte_Roundtrip(sbyte value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<sbyte>(), Is.True);
            Assert.That(anon.As<sbyte>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(sbyte)));
        });
    }

    [TestCase((short)-123)]
    [TestCase(short.MaxValue)]
    public void Short_Roundtrip(short value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<short>(), Is.True);
            Assert.That(anon.As<short>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(short)));
        });
    }

    [TestCase((ushort)123)]
    [TestCase(ushort.MaxValue)]
    public void UShort_Roundtrip(ushort value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<ushort>(), Is.True);
            Assert.That(anon.As<ushort>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(ushort)));
        });
    }

    [TestCase(-123)]
    [TestCase(int.MaxValue)]
    public void Int_Roundtrip(int value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<int>(), Is.True);
            Assert.That(anon.As<int>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(int)));
        });
    }

    [TestCase(uint.MaxValue)]
    [TestCase(123u)]
    public void UInt_Roundtrip(uint value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<uint>(), Is.True);
            Assert.That(anon.As<uint>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(uint)));
        });
    }

    [TestCase(-123L)]
    [TestCase(long.MaxValue)]
    public void Long_Roundtrip(long value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<long>(), Is.True);
            Assert.That(anon.As<long>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(long)));
        });
    }

    [TestCase(ulong.MaxValue)]
    [TestCase(123UL)]
    public void ULong_Roundtrip(ulong value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<ulong>(), Is.True);
            Assert.That(anon.As<ulong>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(ulong)));
        });
    }

    [TestCase(1.23f)]
    [TestCase(float.MaxValue)]
    public void Float_Roundtrip(float value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<float>(), Is.True);
            Assert.That(anon.As<float>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(float)));
        });
    }

    [TestCase(1.23)]
    [TestCase(double.MaxValue)]
    public void Double_Roundtrip(double value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<double>(), Is.True);
            Assert.That(anon.As<double>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(double)));
        });
    }

    [TestCase(1.23)]
    [TestCase(123456789.123456789)]
    public void Decimal_Roundtrip(decimal value) {
        var anon = AnonymousValue.From(value);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<decimal>(), Is.True);
            Assert.That(anon.As<decimal>(), Is.EqualTo(value));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(decimal)));
        });
    }

    [Test]
    public void Reference_Roundtrip() {
        var obj = new object();
        var anon = AnonymousValue.From(obj);
        
        Assert.Multiple(() => {
            Assert.That(anon.Is<object>(), Is.True);
            Assert.That(anon.As<object>(), Is.SameAs(obj));
            Assert.That(anon.GetUnderlyingType(), Is.EqualTo(typeof(object)));
        });
    }

    [Test]
    public void Set_UpdatesValueAndType() {
        var anon = AnonymousValue.From(1);
        anon.SetValue(2);
        Assert.That(anon.As<int>(), Is.EqualTo(2));
        anon.SetValue("hello");
        Assert.That(anon.As<string>(), Is.EqualTo("hello"));
    }

    [Test]
    public void Equals_WorksForSameValue() {
        var a = AnonymousValue.From(123);
        var b = AnonymousValue.From(123);
        
        Assert.Multiple(() => {
            Assert.That(a, Is.EqualTo(b));
#pragma warning disable NUnit2010
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
#pragma warning restore NUnit2010
        });
    }

    [Test]
    public void Equals_WorksForDifferentValue() {
        var a = AnonymousValue.From(123);
        var b = AnonymousValue.From(456);
        Assert.Multiple(() => {
            Assert.That(a, Is.Not.EqualTo(b));
#pragma warning disable NUnit2010
            Assert.That(a == b, Is.False);
            Assert.That(a != b, Is.True);
#pragma warning restore NUnit2010
        });
    }

    [Test]
    public void As_InvalidCast_Throws() {
        var anon = AnonymousValue.From(123);
        Assert.Throws<InvalidCastException>(() => anon.As<string>());
    }

    [Test]
    public void Set_NullReference_Throws() {
        var anon = AnonymousValue.From(123);
        Assert.Throws<ArgumentNullException>(() => anon.SetValue<object>(null!));
    }
}