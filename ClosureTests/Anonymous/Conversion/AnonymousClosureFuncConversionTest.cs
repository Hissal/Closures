using Closures;

namespace ClosureTests.Anonymous.Conversion;

[TestFixture]
[TestOf(typeof(AnonymousClosureConversionExtensions))]
public class AnonymousClosureFuncConversionTest {
    [Test]
    public void ClosureFunc() {
        var closureFunc = Closure.Func(10, (int ctx) => ctx + 1);
        var anonClosure = closureFunc.AsAnonymousFunc();
        var convertedBack = anonClosure.AsClosureFunc<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }

    [Test]
    public void ClosureFuncWithArg() {
        var closureFunc = Closure.Func(10, (int ctx, int arg) => ctx + arg);
        var anonClosure = closureFunc.AsAnonymousFunc();
        var convertedBack = anonClosure.AsClosureFunc<int, int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }

    [Test]
    public void ClosureRefFuncWithRefArg() {
        var closureFunc = Closure.RefFunc(10, (int ctx, ref int arg) => ctx + arg);
        var anonClosure = closureFunc.AsAnonymousFunc();
        var convertedBack = anonClosure.AsClosureRefFunc<int, int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }

    [Test]
    public void MutatingClosureFunc() {
        var closureFunc = MutatingClosure.Func(10, (ref int ctx) => ++ctx);
        var anonClosure = closureFunc.AsAnonymousFunc();
        var convertedBack = anonClosure.AsMutatingClosureFunc<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }

    [Test]
    public void MutatingClosureFuncWithArg() {
        var closureFunc = MutatingClosure.Func(10, (ref int ctx, int arg) => ctx + arg);
        var anonClosure = closureFunc.AsAnonymousFunc();
        var convertedBack = anonClosure.AsMutatingClosureFunc<int, int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }

    [Test]
    public void MutatingClosureRefFuncWithRefArg() {
        var closureFunc = MutatingClosure.RefFunc(10, (ref int ctx, ref int arg) => ctx + arg);
        var anonClosure = closureFunc.AsAnonymousFunc();
        var convertedBack = anonClosure.AsMutatingClosureRefFunc<int, int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }
}