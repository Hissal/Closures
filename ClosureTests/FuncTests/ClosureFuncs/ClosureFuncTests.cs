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