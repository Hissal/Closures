using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.AnonymousClosures;

[TestFixture]
public class AnonymousClosureTests {
    class TestClass {
        public int Value { get; set; }
    }
    // Default tests
        
    [Test]
    public void Action_ReceivesContext() {
        int context = 5;
        int expected = 5;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }
        
    [Test]
    public void Action_ReceivesTupleContext_AndModifiesTestContextValue() {
        int expected = 5;

        var testContext = new TestClass();

        var closure = AnonymousClosure.Create(AnonymousValue.From((testContext, expected)), ((TestClass testContext, int expected) ctx) => { 
            ctx.testContext.Value = ctx.expected;
        });
        closure.Invoke();

        Assert.That(testContext.Value, Is.EqualTo(expected));
    }
    
    // Argument tests
    
    [Test]
    public void ActionWithArg_ReceivesArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(arg);
    }
    
    // Ref argument tests
    
    [Test]
    public void ActionWithRefArg_ReceivesRefArg() {
        int context = 5;
        int arg = 2;
        int expected = context + arg;
            
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int a) => Assert.That(ctx + a, Is.EqualTo(expected)));
        closure.Invoke(ref arg);
    }
    
    [Test]
    public void ActionWithRefArg_ModifiesRefArgValue() {
        int context = 5;
        int arg = 3;
        int expected = arg + context;
            
        var closure = AnonymousClosure.Create<RefActionWithNormalContext<int, int>>(AnonymousValue.From(context), (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    [Test]
    public void ActionWithRefArg_ModifiesRefArgValue_MultipleInvocations() {
        int context = 5;
        int arg = 3;
        int expected = arg + context * 2;
            
        var closure = AnonymousClosure.Create<RefActionWithNormalContext<int, int>>(AnonymousValue.From(context), (int ctx, ref int val) => val += ctx);
        closure.Invoke(ref arg);
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(expected));
    }
    
    // Mutating tests
    
    [Test]
    public void ActionWithArg_MutatingBehaviourMutate_RetainsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;
            
        var closure = AnonymousClosure.Create<ActionWithRefContext<int, int>>(AnonymousValue.From(context), (ref int ctx, int arg) => ctx += arg, MutatingBehaviour.Mutate);
        closure.Invoke(addition);
            
        Assert.That(closure.Context.As<int>(), Is.EqualTo(expected));
    }
    
    [Test]
    public void ActionWithArg_MutatingBehaviourReset_ResetsModifiedContext() {
        int context = 5;
        int addition = 3;
        int expected = context;
            
        var closure = AnonymousClosure.Create<ActionWithRefContext<int, int>>(AnonymousValue.From(context), (ref int ctx, int arg) => ctx += arg, MutatingBehaviour.Reset);
        closure.Invoke(addition);
            
        Assert.That(closure.Context.As<int>(), Is.EqualTo(expected));
    }
    
    // Func Tests

    [Test]
    public void Func_ReturnsCorrectValue() {
        var context = 5;
        
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx) => ctx);
        var result = closure.Invoke<int>();
        
        Assert.That(result, Is.EqualTo(context), "Anonymous closure did not return the expected value.");
    }
    
    // Func arg Tests

    [Test]
    public void FuncWithArg_ReturnsCorrectValue() {
        var context = 5;
        var argument = 5;
        var expected = context + argument;
        
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, int arg) => ctx + arg);
        var result = closure.Invoke<int, int>(argument);
        
        Assert.That(result, Is.EqualTo(expected), "Anonymous closure did not return the expected value.");
    }
    
    // Func ref arg Tests

    [Test]
    public void FuncWithRefArg_ReturnsCorrectValue() {
        var context = 5;
        var argument = 5;
        var expected = context + argument;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int arg) => ctx + arg);
        var result = closure.Invoke<int, int>(ref argument);

        Assert.That(result, Is.EqualTo(expected), "Anonymous closure did not return the expected value.");
    }
    
    // Anonymous Action Tests

    // Action TryInvoke no arg
    [Test]
    public void AnonymousClosure_Action_TryInvoke_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int calledWith = 0;
        Action<int> action = ctx => calledWith = ctx * 2;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), action);

        var result = closure.TryInvoke();

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(calledWith, Is.EqualTo(10));
        });
    }

    [Test]
    public void AnonymousClosure_Action_TryInvoke_ReturnsFailure_OnException() {
        int context = 1;
        Action<int> action = ctx => throw new InvalidCastException("fail");

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), action);

        var result = closure.TryInvoke();

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }
    
    // Action TryInvoke with arg
    [Test]
    public void AnonymousClosure_Action_TryInvoke_WithArg_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int calledWith = 0;
        Action<int, int> action = (ctx, arg) => calledWith = ctx + arg;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), action);

        var result = closure.TryInvoke(3);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(calledWith, Is.EqualTo(8));
        });
    }

    [Test]
    public void AnonymousClosure_Action_TryInvoke_WithArg_ReturnsFailure_OnException() {
        int context = 1;
        Action<int, int> action = (ctx, arg) => throw new InvalidCastException("fail");

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), action);

        var result = closure.TryInvoke(2);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }
    
    // Action TryInvoke with ref arg
    [Test]
    public void AnonymousClosure_Action_TryInvoke_WithRefArg_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int calledWith = 0;
        Action<int, int> action = (ctx, arg) => calledWith = ctx + arg;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), action);

        int arg = 4;
        var result = closure.TryInvoke(ref arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(calledWith, Is.EqualTo(9));
        });
    }

    [Test]
    public void AnonymousClosure_Action_TryInvoke_WithRefArg_ReturnsFailure_OnException() {
        int context = 1;
        Action<int, int> action = (ctx, arg) => throw new InvalidCastException("fail");

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), action);

        int arg = 2;
        var result = closure.TryInvoke(ref arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }

    // Anonymous Func Tests

    // Func TryInvoke no arg
    [Test]
    public void AnonymousClosure_Func_TryInvoke_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int expected = context * 3;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx) => ctx * 3);

        var result = closure.TryInvoke<int>();

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosure_Func_TryInvoke_ReturnsFailure_OnException() {
        int context = 1;
        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (Func<int, int>)((int ctx) => throw new InvalidCastException("fail")));

        var result = closure.TryInvoke<int>();

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }

    // Func TryInvoke with arg
    [Test]
    public void AnonymousClosure_Func_TryInvoke_WithArg_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int arg = 3;
        int expected = context + arg;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, int a) => ctx + a);

        var result = closure.TryInvoke<int, int>(arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosure_Func_TryInvoke_WithArg_ReturnsFailure_OnException() {
        int context = 1;
        int arg = 2;
        var closure = AnonymousClosure.Create(AnonymousValue.From(context),
            (Func<int, int, int>)((int ctx, int a) => throw new InvalidCastException("fail")));

        var result = closure.TryInvoke<int, int>(arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }

    // Func TryInvoke with ref arg
    [Test]
    public void AnonymousClosure_Func_TryInvoke_WithRefArg_ReturnsSuccess_OnValidCall() {
        int context = 5;
        int arg = 3;
        int expected = context + arg;

        var closure = AnonymousClosure.Create(AnonymousValue.From(context), (int ctx, ref int a) => ctx + a);

        var result = closure.TryInvoke<int, int>(ref arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Test]
    public void AnonymousClosure_Func_TryInvoke_WithRefArg_ReturnsFailure_OnException() {
        int context = 1;
        int arg = 2;
        var closure = AnonymousClosure.Create(AnonymousValue.From(context),
            (Func<int, int, int>)((int ctx, int a) => throw new InvalidCastException("fail")));

        var result = closure.TryInvoke<int, int>(ref arg);

        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.TypeOf<InvalidCastException>());
        });
    }
    
    // Anonymous Tests

    [Test]
    public void AnonymousClosure_Is_Works() {
        var closure = AnonymousClosure.Create(AnonymousValue.From(42), (int ctx) => { });

        Assert.Multiple(() => {
            Assert.That(closure.Is<ClosureAction<int>>(), Is.True);
            Assert.That(closure.Is<MutatingClosureFunc<int, int>>(), Is.False);
        });
    }
    
    [Test]
    public void AnonymousClosure_InvokableAs_Works() {
        var closure = AnonymousClosure.Create(AnonymousValue.From(42), (int ctx) => { });

        Assert.Multiple(() => {
            Assert.That(closure.InvokableAs<Action>(), Is.True);
            Assert.That(closure.InvokableAs<Action<int>>(), Is.False);
        });
    }

    [Test]
    public void AnonymousClosure_Equals_And_HashCode_Work() {
        int context = 7;
        Action<int> action = ctx => { };

        var a = AnonymousClosure.Create(AnonymousValue.From(context), action);
        a.Invoke();

        var b = AnonymousClosure.Create(AnonymousValue.From(context), action);

        Assert.That(a, Is.EqualTo(b));
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }
}