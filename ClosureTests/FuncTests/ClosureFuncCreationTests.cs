using Closures;

namespace ClosureTests.FuncTests;

[TestFixture]
public class ClosureFuncCreationTests {
    public int DummyFunc(int context) => context;
    public int DummyFuncWithArg(int context, int arg) => context + arg;
    public int DummyFuncWithRefArg(int context, ref int arg) => context + arg;
    public int DummyFuncWithRefContext(ref int context) => context;
    public int DummyFuncWithRefContextAndNormalArg(ref int context, int arg) => context + arg;
    public int DummyFuncWithRefContextAndRefArg(ref int context, ref int arg) => context + arg;

    [Test]
    public void ClosureFunc_CreatesCorrectly() {
        int context = 10;
        var func = new Func<int, int>(DummyFunc);
        var closure = Closure.Func(context, func);
        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IClosureFunc<int, int, Func<int,int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void ClosureFuncWithArg_CreatesCorrectly() {
        int context = 10;
        var func = new Func<int, int, int>(DummyFuncWithArg);
        var closure = Closure.Func(context, func);
        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IClosureFunc<int, int, int, Func<int, int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void ClosureRefFuncWithRefArg_CreatesCorrectly() {
        int context = 10;
        var func = new RefFuncWithNormalContext<int, int, int>(DummyFuncWithRefArg);
        var closure = Closure.Func(context, func);
        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IClosureRefFunc<int, int, int, RefFuncWithNormalContext<int, int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void MutatingClosureFunc_CreatesCorrectly() {
        int context = 10;
        var func = new RefFunc<int, int>(DummyFuncWithRefContext);
        var closure = Closure.Func(context, func);
        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IMutatingClosure>());
            Assert.That(closure, Is.InstanceOf<IClosureFunc<int, int, RefFunc<int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void MutatingClosureFuncWithArg_CreatesCorrectly() {
        int context = 10;
        var func = new FuncWithRefContext<int, int, int>(DummyFuncWithRefContextAndNormalArg);
        var closure = Closure.Func(context, func);
        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IMutatingClosure>());
            Assert.That(closure, Is.InstanceOf<IClosureFunc<int, int, int, FuncWithRefContext<int, int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void MutatingClosureRefFuncWithRefArg_CreatesCorrectly() {
        int context = 10;
        var func = new RefFunc<int, int, int>(DummyFuncWithRefContextAndRefArg);
        var closure = Closure.Func(context, func);
        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IMutatingClosure>());
            Assert.That(closure, Is.InstanceOf<IClosureRefFunc<int, int, int, RefFunc<int, int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(func));
        });
    }

    [Test]
    public void RefClosureFunc_CreatesCorrectly() {
        int context = 10;
        var func = new RefFunc<int, int>(DummyFuncWithRefContext);
        var closure = Closure.Func(ref context, func);
        Assert.That(closure.Context, Is.EqualTo(context));
        Assert.That(closure.RefContext, Is.EqualTo(context));
        Assert.That(closure.Delegate, Is.EqualTo(func));
    }

    [Test]
    public void RefClosureFuncWithArg_CreatesCorrectly() {
        int context = 10;
        var func = new FuncWithRefContext<int, int, int>(DummyFuncWithRefContextAndNormalArg);
        var closure = Closure.Func(ref context, func);
        Assert.That(closure.Context, Is.EqualTo(context));
        Assert.That(closure.RefContext, Is.EqualTo(context));
        Assert.That(closure.Delegate, Is.EqualTo(func));
    }

    [Test]
    public void RefClosureRefFuncWithRefArg_CreatesCorrectly() {
        int context = 10;
        var func = new RefFunc<int, int, int>(DummyFuncWithRefContextAndRefArg);
        var closure = Closure.Func(ref context, func);
        Assert.That(closure.Context, Is.EqualTo(context));
        Assert.That(closure.RefContext, Is.EqualTo(context));
        Assert.That(closure.Delegate, Is.EqualTo(func));
    }
}