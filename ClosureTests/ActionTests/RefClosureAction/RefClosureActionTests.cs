using Closures;

namespace ClosureTests.ActionTests.RefClosureAction;

[TestFixture]
public class RefClosureActionTests {
    class TestClass {
        public int Value { get; set; }
    }

    // Default tests

    [Test]
    public void RefClosureAction_ReceivesContext() {
        int context = 5;
        int expected = 5;

        var closure = RefClosure.Action(ref context, (ref int ctx) => Assert.That(ctx, Is.EqualTo(expected)));
        closure.Invoke();
    }

    [Test]
    public void RefClosureAction_ReceivesTupleContext_AndModifiesTestContextValue() {
        var testContext = new TestClass();
        int expected = 5;

        var tupleContext = (testContext, expected);
        var closure = RefClosure.Action(ref tupleContext,
            (ref (TestClass testContext, int expected) ctx) => ctx.testContext.Value = ctx.expected);
        closure.Invoke();

        Assert.That(testContext.Value, Is.EqualTo(expected));
    }

    // Ref closure tests

    [Test]
    public void RefClosureAction_ModifiesOriginalContext() {
        int context = 5;
        int addition = 3;
        int expected = context + addition;

        var closure = RefClosure.Action(ref context, (ref int ctx) => ctx += addition);
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected)); // Context modified after invoke
    }

    [Test]
    public void RefClosureAction_ModifiesOriginalContext_MultipleInvocations() {
        int context = 5;
        int addition = 3;
        int expected = context + addition * 2;

        var closure = RefClosure.Action(ref context, (ref int ctx) => ctx += addition);

        closure.Invoke();
        closure.Invoke();
        Assert.That(context, Is.EqualTo(expected));
    }

    [Test]
    public void RefClosureAction_ModifyingOriginalContext_ModifiesContextAndRefContext() {
        int context = 5;
        int expected = 7;

        var closure = RefClosure.Action(ref context, (ref int ctx) => { /* no op */
        });
        context = expected;

        bool isContextModified = closure.Context == expected;
        bool isRefContextModified = closure.RefContext == expected;
        
        Assert.Multiple(() => {
            Assert.That(isContextModified, Is.True, "Context should be modified");
            Assert.That(isRefContextModified, Is.True, "RefContext should be modified");
        });
    }

    // [Test]
    // public void RefClosureAction_AssigningRefContextToAnotherVariable_AndModifyingIt_ModifiesContext() {
    //     int context = 5;
    //     int expected = 7;
    //
    //     var closure = Closure.Action(ref context, (ref int ctx) => ctx = expected);
    //     ref var refContext = ref closure.RefContext; // Assigning RefContext to another variable
    //     refContext = expected; // Modifying the ref variable
    //
    //     Assert.That(context, Is.EqualTo(expected));
    // }
}