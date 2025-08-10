using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs;

[TestFixture]
public class ClosureFuncWithArgTests {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default tests

    [Test]
    public void ClosureFuncWithArg_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var closure = Closure.Func(context, (int ctx, int a) => {
            Assert.That(ctx, Is.EqualTo(expected));
            return ctx;
        });
        closure.Invoke(arg);
    }

    [Test]
    public void ClosureFuncWithArg_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var closure = Closure.Func((testContext, expected), ((TestClass testContext, int expected) ctx, int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    // Argument tests
    
    [Test]
    public void ClosureFuncWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Func(context, (int ctx, int a) => { 
            Assert.That(ctx + a, Is.EqualTo(expected));
            return ctx;
        });
        closure.Invoke(arg);
    }
    
    // Func tests
    
    [Test]
    public void ClosureFuncWithArg_ReturnsExpectedValue() {
        int context = 10;
        int addition = 5;
        int expected = context + addition;
        
        var closure = Closure.Func(context, (int ctx, int arg) => ctx + arg);
        int result = closure.Invoke(addition);
        Assert.That(result, Is.EqualTo(expected));
    }
}