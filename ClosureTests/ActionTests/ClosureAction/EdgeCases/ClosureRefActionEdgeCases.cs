using Closures;

namespace ClosureTests.ActionTests.ClosureAction.EdgeCases;

[TestFixture]
public class ClosureRefActionEdgeCases {
    class TestClass {
        public int Value { get; set; }
    }

    [Test]
    public void ClosureRefAction_NullContext_Invoke_DoesNotThrow() {
        TestClass? testContext = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.RefAction(testContext, (TestClass ctx, ref int arg) => Assert.That(ctx, Is.Null, "Context should be null"));
            int arg = 42;
            closure.Invoke(ref arg);
        }, "Closure with null context should not throw an exception");
    }

    [Test]
    public void ClosureRefAction_ExceptionDuringInvocation_Throws() {
        int context = 5;
        var closure = Closure.RefAction(context, (int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        Assert.Throws<InvalidOperationException>(() => {
            closure.Invoke(ref arg);
        }, "Closure should throw an exception during invocation");
    }

    [Test]
    public void ClosureRefAction_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        int context = 5;
        var closure = Closure.RefAction(context, (int ctx, ref int arg) => throw new InvalidOperationException("Test exception"));
        int arg = 42;
        try {
            closure.Invoke(ref arg);
            Assert.Fail("Closure should throw an exception during invocation");
        }
        catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("Test exception"), "Caught exception message should match");
        }
    }
    
    // Argument edge cases
    
    [Test]
    public void ClosureRefAction_NullArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.RefAction(context, (int ctx, ref int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            closure.Invoke(null);
        }, "Closure with null argument should not throw an exception");
    }
    
    // Ref argument edge cases
    
    [Test]
    public void ClosureRefAction_NullRefArg_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.RefAction(context, (int ctx, ref int? arg) => Assert.That(arg, Is.Null, "Argument should be null"));
            int? arg = null;
            closure.Invoke(ref arg);
        }, "Closure with null ref argument should not throw an exception");
    }
     
    [Test]
    public void ClosureRefAction_NullRefArg_SettingValue_ModifiesOriginalRef() {
        int context = 5;
        var closure = Closure.RefAction(context, (int ctx, ref int? arg) => {
            Assert.That(arg, Is.Null, "Argument should be null");
            arg = 42; // Set a value to the ref argument
        });
        
        int? arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo(42), "Ref argument should be set to the value provided in the closure");
    }
}