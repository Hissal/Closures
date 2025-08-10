using Closures;

namespace ClosureTests.FuncTests.MutatingClosureFuncs;

[TestFixture]
public class MutatingClosureRefFuncTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests
    [Test]
    public void MutatingClosureRefFunc_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int a) => { Assert.That(ctx, Is.EqualTo(expected)); return ctx; });
        closure.Invoke(arg);
    }

    [Test]
    public void MutatingClosureRefFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;
        var closure = MutatingClosure.RefFunc((testContext, expected), (ref (TestClass testContext, int expected) ctx, ref int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Argument tests
    [Test]
    public void MutatingClosureRefFunc_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int a) => {
            Assert.That(ctx + a, Is.EqualTo(expected)); 
            return ctx;
        });
        closure.Invoke(arg);
    }

    // Ref argument tests
    [Test]
    public void MutatingClosureRefFunc_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int a) => {
            Assert.That(ctx + a, Is.EqualTo(expected)); 
            return ctx;
        });
        closure.Invoke(ref arg);
    }
    
    [Test]
    public void MutatingClosureRefFunc_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    [Test]
    public void MutatingClosureRefFunc_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }

    // Mutating tests
    [Test]
    public void MutatingClosureRefFunc_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int arg) => ctx += arg);
        closure.Invoke(addition);
        Assert.That(closure.Context, Is.EqualTo(expected));
    }

    // Func tests
    [Test]
    public void MutatingClosureRefFunc_ReturnsExpectedValue() {
        int context = 10;
        int addition = 5;
        int expected = context + addition;
        var closure = MutatingClosure.RefFunc(context, (ref int ctx, ref int arg) => ctx + arg);
        int result = closure.Invoke(addition);
        Assert.That(result, Is.EqualTo(expected));
    }
}