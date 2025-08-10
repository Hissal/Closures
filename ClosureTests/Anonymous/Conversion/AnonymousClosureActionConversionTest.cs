using Closures;

namespace ClosureTests.Anonymous.Conversion;

[TestFixture]
[TestOf(typeof(AnonymousClosureConversionExtensions))]
public class AnonymousClosureActionConversionTest {
    [Test]
    public void ClosureAction() {
        var closureAction = Closure.Action(5, (int ctx) => { });
        var anonClosure = closureAction.AsAnonymousAction();
        var convertedBack = anonClosure.AsClosureAction<int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }

    [Test]
    public void ClosureActionWithArg() {
        var closureAction = Closure.Action(5, (int ctx, int arg) => { });
        var anonClosure = closureAction.AsAnonymousAction();
        var convertedBack = anonClosure.AsClosureAction<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }

    [Test]
    public void ClosureActionWithRefArg() {
        var closureAction = Closure.RefAction(5, (int ctx, ref int arg) => { });
        var anonClosure = closureAction.AsAnonymousAction();
        var convertedBack = anonClosure.AsClosureRefAction<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }

    [Test]
    public void MutatingClosureAction() {
        var closureAction = MutatingClosure.Action(5, (ref int ctx) => { });
        var anonClosure = closureAction.AsAnonymousAction();
        var convertedBack = anonClosure.AsMutatingClosureAction<int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }

    [Test]
    public void MutatingClosureActionWithArg() {
        var closureAction = MutatingClosure.Action(5, (ref int ctx, int arg) => { });
        var anonClosure = closureAction.AsAnonymousAction();
        var convertedBack = anonClosure.AsMutatingClosureAction<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }

    [Test]
    public void MutatingClosureRefActionWithRefArg() {
        var closureAction = MutatingClosure.RefAction(5, (ref int ctx, ref int arg) => { });
        var anonClosure = closureAction.AsAnonymousAction();
        var convertedBack = anonClosure.AsMutatingClosureRefAction<int, int>();

        Assert.Multiple(() => {
            Assert.That(anonClosure.Context.As<int>(), Is.EqualTo(closureAction.Context));
            Assert.That(anonClosure.Delegate, Is.EqualTo(closureAction.Delegate));
            Assert.That(convertedBack, Is.EqualTo(closureAction));
        });
    }
}