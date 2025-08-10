using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.Conversion;

[TestFixture]
[TestOf(typeof(AnonymousClosureConversionExtensions))]
public class AnonymousClosureConversionTest {
    // Actions
    [Test]
    public void ClosureAction() {
        var closureAction = Closure.Action(5, (int ctx) => { });
        var anonClosure = closureAction.AsAnonymous();
        var convertedBack = anonClosure.AsClosureAction<int>();
        
        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
    
    [Test]
    public void ClosureActionWithArg() {
        var closureAction = Closure.Action(5, (int ctx, int arg) => { });
        var anonClosure = closureAction.AsAnonymous();
        var convertedBack = anonClosure.AsClosureAction<int, int>();
        
        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
    
    [Test]
    public void ClosureActionWithRefArg() {
        var closureAction = Closure.RefAction(5, (int ctx, ref int arg) => { });
        var anonClosure = closureAction.AsAnonymous();
        var convertedBack = anonClosure.AsClosureRefAction<int, int>();
        
        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
    
    [Test]
    public void MutatingClosureAction() {
        var closureAction = MutatingClosure.Action(5, (ref int ctx) => { });
        var anonClosure = closureAction.AsAnonymous();
        var convertedBack = anonClosure.AsMutatingClosureAction<int>();
        
        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
    
    [Test]
    public void MutatingClosureActionWithArg() {
        var closureAction = MutatingClosure.Action(5, (ref int ctx, int arg) => { });
        var anonClosure = closureAction.AsAnonymous();
        var convertedBack = anonClosure.AsMutatingClosureAction<int, int>();
        
        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
    
    [Test]
    public void MutatingClosureRefActionWithRefArg() {
        var closureAction = MutatingClosure.RefAction(5, (ref int ctx, ref int arg) => { });
        var anonClosure = closureAction.AsAnonymous();
        var convertedBack = anonClosure.AsMutatingClosureRefAction<int, int>();
        
        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
    
    // Funcs
    [Test]
    public void ClosureFunc() {
        var closureFunc = Closure.Func(10, (int ctx) => ctx + 1);
        var anonClosure = closureFunc.AsAnonymous();
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
        var anonClosure = closureFunc.AsAnonymous();
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
        var anonClosure = closureFunc.AsAnonymous();
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
        var anonClosure = closureFunc.AsAnonymous();
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
        var anonClosure = closureFunc.AsAnonymous();
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
        var anonClosure = closureFunc.AsAnonymous();
        var convertedBack = anonClosure.AsMutatingClosureRefFunc<int, int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureFunc.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureFunc));
        });
    }
    
    // Anonymous

    [Test]
    public void AnonymousClosureAction() {
        var anonymousAction = AnonymousClosure.Action(5, (int ctx) => { });
        var anonClosure = anonymousAction.AsAnonymous();
        var convertedBack = anonClosure.AsAnonymousAction();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(anonymousAction.Context.As<int>()));
            Assert.That(anonClosure.Delegate, Is.EqualTo(anonymousAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(anonymousAction));
        });
    }

    [Test]
    public void AnonymousClosureActionWithArg() {
        var anonymousAction = AnonymousClosure.Action(5, (int ctx, int arg) => { });
        var anonClosure = anonymousAction.AsAnonymous();
        var convertedBack = anonClosure.AsAnonymousAction<int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(anonymousAction.Context.As<int>()));
            Assert.That(anonClosure.Delegate, Is.EqualTo(anonymousAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(anonymousAction));
        });
    }
    
    [Test]
    public void AnonymousClosureFunc() {
        var anonymousFunc = AnonymousClosure.Func(10, (int ctx) => ctx + 1);
        var anonClosure = anonymousFunc.AsAnonymous();
        var convertedBack = anonClosure.AsAnonymousFunc<int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(anonymousFunc.Context.As<int>()));
            Assert.That(anonClosure.Delegate, Is.EqualTo(anonymousFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(anonymousFunc));
        });
    }

    [Test]
    public void AnonymousClosureFuncWithArg() {
        var anonymousFunc = AnonymousClosure.Func(10, (int ctx, int arg) => ctx + arg);
        var anonClosure = anonymousFunc.AsAnonymous();
        var convertedBack = anonClosure.AsAnonymousFunc<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(anonymousFunc.Context.As<int>()));
            Assert.That(anonClosure.Delegate, Is.EqualTo(anonymousFunc.Delegate));
            Assert.That(convertedBack, Is.EqualTo(anonymousFunc));
        });
    }
}