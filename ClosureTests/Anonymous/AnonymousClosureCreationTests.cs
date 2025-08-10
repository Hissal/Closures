using Closures;

namespace ClosureTests.Anonymous;

[TestFixture]
public class AnonymousClosureCreationTests {
    // Create methods
    [Test]
    public void AnonymousClosure_Create_CreatesCorrectly() {
        int context = 123;
        Action<int> action = _ => { };
        var anon = AnonymousClosure.Create(AnonymousValue.From(context), action);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(action));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Reset));
        });
    }

    [Test]
    public void AnonymousClosure_Create_WithMutatingBehaviour_CreatesCorrectly() {
        int context = 456;
        RefAction<int> action = (ref int x) => x++;
        var anon = AnonymousClosure.Create(AnonymousValue.From(context), action, MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(action));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Mutate));
        });
    }

    [Test]
    public void AnonymousClosure_Create_GenericDelegate_CreatesCorrectly() {
        int context = 789;
        Func<int, string> func = x => x.ToString();
        var anon = AnonymousClosure.Create<Func<int, string>>(AnonymousValue.From(context), func);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(func));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Reset));
        });
    }

    [Test]
    public void AnonymousClosure_Create_GenericDelegate_WithMutatingBehaviour_CreatesCorrectly() {
        int context = 321;
        RefFunc<int, string> func = (ref int x) => (++x).ToString();
        var anon = AnonymousClosure.Create<RefFunc<int, string>>(AnonymousValue.From(context), func,
            MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(func));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Mutate));
        });
    }

    // Targeted types

    [Test]
    public void AnonymousClosure_Action_CreatesCorrectly() {
        int context = 42;
        Action<int> action = _ => { };
        var anon = AnonymousClosure.Action(AnonymousValue.From(context), action);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(action));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Reset));
        });
    }

    [Test]
    public void AnonymousClosure_Action_WithArg_CreatesCorrectly() {
        int context = 7;
        Action<int, string> action = (_, __) => { };
        var anon = AnonymousClosure.Action<string>(AnonymousValue.From(context), action);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(action));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Reset));
        });
    }

    [Test]
    public void AnonymousClosure_Action_Mutating_CreatesCorrectly() {
        int context = 5;
        RefAction<int> action = (ref int x) => x++;
        var anon = AnonymousClosure.Action(AnonymousValue.From(context), action, MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(action));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Mutate));
        });
    }

    [Test]
    public void AnonymousClosure_Action_WithArg_Mutating_CreatesCorrectly() {
        int context = 3;
        ActionWithRefContext<int, string> action = (ref int x, string s) => x += s.Length;
        var anon = AnonymousClosure.Action<string>(AnonymousValue.From(context), action, MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(action));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Mutate));
        });
    }

    [Test]
    public void AnonymousClosure_Func_CreatesCorrectly() {
        int context = 10;
        Func<int, string> func = x => x.ToString();
        var anon = AnonymousClosure.Func<string>(AnonymousValue.From(context), func);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(func));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Reset));
        });
    }

    [Test]
    public void AnonymousClosure_Func_WithArg_CreatesCorrectly() {
        int context = 8;
        Func<int, string, string> func = (x, s) => (x + s.Length).ToString();
        var anon = AnonymousClosure.Func<string, string>(AnonymousValue.From(context), func);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(func));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Reset));
        });
    }

    [Test]
    public void AnonymousClosure_Func_Mutating_CreatesCorrectly() {
        int context = 2;
        RefFunc<int, string> func = (ref int x) => (++x).ToString();
        var anon = AnonymousClosure.Func<string>(AnonymousValue.From(context), func, MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(func));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Mutate));
        });
    }

    [Test]
    public void AnonymousClosure_Func_WithArg_Mutating_CreatesCorrectly() {
        int context = 4;
        FuncWithRefContext<int, string, string> func = (ref int x, string s) => (x += s.Length).ToString();
        var anon = AnonymousClosure.Func<string, string>(AnonymousValue.From(context), func, MutatingBehaviour.Mutate);

        Assert.Multiple(() => {
            Assert.That(anon.Context.As<int>(), Is.EqualTo(context));
            Assert.That(anon.Delegate, Is.EqualTo(func));
            Assert.That(anon.MutatingBehaviour, Is.EqualTo(MutatingBehaviour.Mutate));
        });
    }
}