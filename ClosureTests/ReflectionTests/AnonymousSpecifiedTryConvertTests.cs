using Closures;
using Closures.Converting.Experimental;

namespace ClosureTests.ReflectionTests;

[TestFixture]
public class AnonymousSpecifiedTryConvertTests {
    delegate void UnknownDelegate<in TContext>(TContext ctx);
    struct UnknownClosure<TContext> : IClosure<TContext, UnknownDelegate<TContext>> {
        public TContext Context { get; set; }
        public UnknownDelegate<TContext> Delegate { get; set; }
        public bool DelegateIsNull => Delegate is null;

        public void Add(UnknownDelegate<TContext> action) {
            // noop
        }
        public void Remove(UnknownDelegate<TContext> action) {
            // noop
        }
    }
    
    [Test]
    public void Anonymous_ClosureAction_TryConvert() {
        var closure = Closure.Action(10, ctx => { /* Do something with ctx */ });
        var anonymous = closure.ToAnonymous();

        var success = ReflectionClosureConverter.TryConvert<ClosureAction<int>>(anonymous, out var converted);
        
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_ClosureActionWithArg_TryConvert() {
        var closure = Closure.Action(10, (int ctx, int arg) => { /* Do something with ctx */ });
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<ClosureAction<int, int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_MutatingClosureAction_TryConvert() {
        var closure = Closure.Action(10, (ref int ctx) => { /* Do something with ctx */ });
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<MutatingClosureAction<int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_MutatingClosureActionWithArg_TryConvert() {
        var closure = Closure.Action(10, (ref int ctx, int arg) => { /* Do something with ctx */ });
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<MutatingClosureAction<int, int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_ClosureFunc_TryConvert() {
        var closure = Closure.Func(10, ctx => ctx + 1);
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<ClosureFunc<int, int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_ClosureFuncWithArg_TryConvert() {
        var closure = Closure.Func(10, (int ctx, int arg) => ctx + arg);
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<ClosureFunc<int, int, int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_MutatingClosureFunc_TryConvert() {
        var closure = Closure.Func(10, (ref int ctx) => ctx + 1);
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<MutatingClosureFunc<int, int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }

    [Test]
    public void Anonymous_MutatingClosureFuncWithArg_TryConvert() {
        var closure = Closure.Func(10, (ref int ctx, int arg) => ctx + arg);
        var anonymous = closure.ToAnonymous();
        var success = ReflectionClosureConverter.TryConvert<MutatingClosureFunc<int, int, int>>(anonymous, out var converted);
        Assert.Multiple(() => {
            Assert.That(converted, Is.EqualTo(closure), "Converted closure should match the original closure");
            Assert.That(converted.Delegate, Is.EqualTo(closure.Delegate), "Converted closure delegate should match the original delegate");
            Assert.That(converted.Context, Is.EqualTo(closure.Context), "Converted closure context should match the original context");
            Assert.That(success, Is.True, "Conversion should succeed for known closure types");
        });
    }
}