using Closures;

namespace ClosureTests.FuncTests.ClosureFuncs;

[TestFixture]
public class ClosureFuncTests {
    class TestClass {
        public int Value { get; set; }
    }
    
    // Default tests
        
    [Test]
    public void ClosureFunc_ReceivesContext() {
        int context = 5;
        int expected = 5;
            
        var closure = Closure.Func(context, ctx => { 
            Assert.That(ctx, Is.EqualTo(expected));
            return ctx;
        });
        
        closure.Invoke();
    }
        
    [Test]
    public void ClosureFunc_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;

        var testContext = new TestClass();

        var closure = Closure.Func((testContext, expected), ctx => ctx.testContext.Value = ctx.expected);
        closure.Invoke();
        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void ClosureFunc_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        int Handler(int ctx) => callSum += ctx;

        var closure = Closure.Func(context, Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(3 * context));
    }
    
    [Test]
    public void ClosureFunc_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
            
        int callSum = 0;
        int Handler(int ctx) => callSum += ctx;

        var closure = Closure.Func(context, Handler);
        
        // Add multiple funcs (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }
        
    [Test]
    public void ClosureFunc_RemoveMultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        
        int callSum = 0;
        int Handler(int ctx) => callSum += ctx;

        var closure = Closure.Func(context, Handler);
        
        // Add multiple funcs (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
            
        // Remove all but the specified number of funcs
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }
    
    // Func tests
    
    [Test]
    public void ClosureFunc_ReturnsExpectedValue() {
        int context = 10;
        int expected = context * 2;
        
        var closure = Closure.Func(context, ctx => ctx * 2);
        int result = closure.Invoke();
        Assert.That(result, Is.EqualTo(expected));
    }
}