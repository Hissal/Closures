using Lh.Closures;

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

    [Test]
    public void ClosureFuncWithArg_AddAndRemove_Works() {
        int context = 1;
        int arg = 2;
        
        int callSum = 0;
        int Handler(int ctx, int a) => callSum += ctx + a;

        var closure = Closure.Func<int, int, int>(context, Handler);
        closure.Add(Handler);
        closure.Invoke(arg);
        closure.Remove(Handler);
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(3 * (context + arg)));
    }

    [Test]
    public void ClosureFuncWithArg_Add_MultipleTimes_Works() {
        int context = 2;
        int arg = 3;
        int actionCount = 5;
        
        int callSum = 0;
        int Handler(int ctx, int a) => callSum += ctx + a;

        var closure = Closure.Func<int, int, int>(context, Handler);
        
        // Add multiple funcs (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(actionCount * (context + arg)));
    }

    [Test]
    public void ClosureFuncWithArg_Remove_MultipleTimes_Works() {
        int context = 3;
        int arg = 4;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context + arg;
        
        
        int callSum = 0;
        int Handler(int ctx, int a) => callSum += ctx + a;

        var closure = Closure.Func<int, int, int>(context, Handler);
        
        // Add multiple funcs (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }

        // Remove all but the specified number of funcs
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        
        closure.Invoke(arg);
        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
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