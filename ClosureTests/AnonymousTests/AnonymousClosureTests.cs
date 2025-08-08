using Closures;

namespace ClosureTests.AnonymousTests;

[TestFixture]
public class AnonymousClosureTests {
    class TestClass {
        public int Value { get; set; }
    }
    // Default tests
        
    [Test]
    public void ClosureAction_ReceivesContext() {
        int context = 5;
        int expected = 5;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }
        
    [Test]
    public void ClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;

        var testContext = new TestClass();

        var closure = AnonymousClosure.Create(AnonymousValue.From((testContext, expected)), ((TestClass testContext, int expected) ctx) => { 
            ctx.testContext.Value = ctx.expected;
        });
        closure.Invoke();

        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    [Test]
    public void ClosureAction_AddAndRemove_Works() {
        int context = 1;
        int callSum = 0;
        void Handler(int ctx) => callSum += ctx;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), Handler);
        closure.Add(Handler);
        closure.Invoke();
        closure.Remove(Handler);
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(3 * context));
    }
    
    [Test]
    public void ClosureAction_Add_MultipleTimes_Works() {
        int context = 2;
        int actionCount = 5;
            
        int callSum = 0;
        void Handler(int ctx) => callSum += ctx;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), Handler);
        
        // Add multiple actions (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(actionCount * context));
    }
        
    [Test]
    public void ClosureAction_RemoveMultipleTimes_Works() {
        int context = 3;
        int actionCount = 5;
        int amountToCall = 2;
        int expectedPerCall = context;
        
        int callSum = 0;
        void Handler(int ctx) => callSum += ctx;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), Handler);
        
        // Add multiple actions (actionCount - 1, because one is already there)
        for (int i = 0; i < actionCount - 1; i++) {
            closure.Add(Handler);
        }
            
        // Remove all but the specified number of actions
        for (int i = 0; i < actionCount - amountToCall; i++) {
            closure.Remove(Handler);
        }
        closure.Invoke();

        Assert.That(callSum, Is.EqualTo(amountToCall * expectedPerCall));
    }
    
    // Argument tests
    
    [Test]
    public void MutatingClosureRefAction_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }
    
    // Ref argument tests
    
    [Test]
    public void MutatingClosureRefAction_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(ref arg);
    }
    
    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue_MultipleDelegates() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 3;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int val) => val += ctx);
        closure.Add((int ctx, ref int val) => val += ctx);
        closure.Add((int ctx, ref int val) => val += ctx);
        
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void MutatingClosureRefAction_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Mutating tests
    
    [Test]
    public void MutatingClosureRefAction_Retain_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (ref int ctx, int arg) => ctx += arg, MutatingClosureBehaviour.Retain);
        closure.Invoke(addition);
            
        Assert.That(closure.Context.As<int>(), Is.EqualTo(expected));
    }
    
    [Test]
    public void MutatingClosureRefAction_Reset_ResetsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (ref int ctx, int arg) => ctx += arg, MutatingClosureBehaviour.Reset);
        closure.Invoke(addition);
            
        Assert.That(closure.Context.As<int>(), Is.EqualTo(expected));
    }
    
    // Func Tests

    [Test]
    public void AnonymousClosure_ReturnsCorrectValue() {
        var context = 5;
        
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx) => ctx);
        var result = closure.Invoke<int>();
        
        Assert.That(result, Is.EqualTo(context), "Anonymous closure did not return the expected value.");
    }
    
    [Test]
    public void AnonymousClosure_WrongValueTypeThrows() {
        var context = 5;
        
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx) => ctx);

        Assert.Throws<InvalidCastException>(() => {
            closure.Invoke<string>();
        }, "Anonymous closure did not throw an InvalidCastException when invoking with the wrong value type.");
    }
    
    // Func arg Tests

    [Test]
    public void AnonymousClosure_Arg_ReturnsCorrectValue() {
        var context = 5;
        var argument = 5;
        var expected = context + argument;
        
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, int arg) => ctx + arg);
        var result = closure.Invoke<int, int>(argument);
        
        Assert.That(result, Is.EqualTo(expected), "Anonymous closure did not return the expected value.");
    }
    
    [Test]
    public void AnonymousClosure_Arg_WrongValueTypeThrows() {
        var context = 5;
        
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, int arg) => ctx + arg);
        Assert.Throws<InvalidCastException>(() => {
            closure.Invoke<string, int>("42");
        }, "Anonymous closure did not throw an InvalidCastException when invoking with the wrong value type.");
    }
}