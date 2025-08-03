using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs;

[TestFixture]
public class ClosureRefFuncTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void ClosureRefFunc_ReceivesContext() {
        int context = 5;
        int arg = 7;
        int expected = 5;

        var closure = Closure.Func(context, (int ctx, ref int a) => { 
            Assert.That(ctx, Is.EqualTo(expected));
            return ctx;
        });
        closure.Invoke(arg);
    }

    [Test]
    public void ClosureRefFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;
        int arg = 1;

        var closure = Closure.Func((testContext, expected), ((TestClass testContext, int expected) ctx, ref int a) => ctx.testContext.Value = ctx.expected);
        closure.Invoke(arg);
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    [Test]
    public void ClosureRefFunc_AddAndRemove_Works() {
        int context = 1;
        int arg = 2;
        int callSum = 0;
        int Handler(int ctx, ref int a) => callSum += ctx + a;

        var closure = Closure.Func<int, int, int>(context, Handler);
        closure.Add(Handler);
        closure.Invoke(arg);
        closure.Remove(Handler);
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(3 * (context + arg)));
    }

    [Test]
    public void ClosureRefFunc_Add_MultipleTimes_Works() {
        int context = 2;
        int arg = 3;
        int actionCount = 5;
        int callSum = 0;
        int Handler(int ctx, ref int a) => callSum += ctx + a;

        var closure = Closure.Func<int, int, int>(context, Handler);
        
        // Add multiple funcs (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke(arg);

        Assert.That(callSum, Is.EqualTo(actionCount * (context + arg)));
    }

    [Test]
    public void ClosureRefFunc_Remove_MultipleTimes_Works() {
        int context = 3;
        int arg = 4;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context + arg;
        
        int callSum = 0;
        int Handler(int ctx, ref int a) => callSum += ctx + a;

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

        // Assert that the call sum is equal to the remaining funcs times the context plus the argument
        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }
    
    // Argument tests
    
    [Test]
    public void ClosureRefFunc_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Func(context, (int ctx, ref int a) => {
            Assert.That(ctx + a, Is.EqualTo(expected));
            return ctx;
        });
        closure.Invoke(arg);
    }
    
    // Ref argument tests
    
    [Test]
    public void ClosureRefFunc_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = Closure.Func(context, (int ctx, ref int a) => {
            Assert.That(ctx + a, Is.EqualTo(expected));
            return ctx;
        });
        closure.Invoke(ref arg);
    }
    
    [Test]
    public void ClosureRefFunc_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;
            
        var closure = Closure.Func(context, (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void ClosureRefFunc_ModifiesRefArgValue_MultipleDelegates() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 3;
            
        var closure = Closure.Func(context, (int ctx, ref int val) => val += ctx);
        closure.Add((int ctx, ref int val) => val += ctx);
        closure.Add((int ctx, ref int val) => val += ctx);
        
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void ClosureRefFunc_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;
            
        var closure = Closure.Func(context, (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Func tests
    
    [Test]
    public void ClosureRefFunc_ReturnsExpectedValue() {
        int context = 10;
        int addition = 5;
        int expected = context + addition;
        
        var closure = Closure.Func(context, (int ctx, ref int arg) => ctx + arg);
        int result = closure.Invoke(addition);
        Assert.That(result, Is.EqualTo(expected));
    }
}