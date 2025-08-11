using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.Other;

// TODO: Add tests for missing methods

[TestFixture]
[TestOf(typeof(AnonymousClosureUtil))]
public class AnonymousClosureUtilTest {
    class EmptyClosure : IClosure { }

    class DummyClosure : IClosure<int, Action<int>> {
        public int Context { get; init; }
        public Action<int> Delegate { get; init; }

        public DummyClosure(int ctx, Action<int> action) {
            Context = ctx;
            Delegate = action;
        }
    }

    class DummyAnonymousClosure : IAnonymousClosure {
        public AnonymousValue Context { get; init; }
        public Delegate Delegate { get; init; }
        public MutatingBehaviour MutatingBehaviour { get; init; }

        public bool Is<TClosureType>() where TClosureType : IClosure {
            return AnonymousClosureUtil.CanConvert<DummyAnonymousClosure, TClosureType>(this);
        }

        public bool InvokableAs<TDelegate>() where TDelegate : Delegate {
            return AnonymousClosureUtil.InvokableAs<TDelegate>(Delegate);
        }
    }

    [Test]
    public void GetInterfaceType_ReturnsCorrectInterface() {
        var type = AnonymousClosureUtil.GetInterfaceType<DummyClosure>();
        Assert.That(type, Is.EqualTo(typeof(IClosure<int, Action<int>>)));
    }

    [Test]
    public void GetInterfaceType_ThrowsIfNoInterface() {
        Assert.Throws<InvalidOperationException>(() => AnonymousClosureUtil.GetInterfaceType<EmptyClosure>());
    }

    [Test]
    public void GetInterfaceGenericArguments_ReturnsCorrectArguments() {
        var iface = typeof(IClosure<int, Action<int>>);
        var args = AnonymousClosureUtil.GetInterfaceGenericArguments(iface);
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(Action<int>) }));
    }

    [Test]
    public void CanConvert_ReturnsTrueForMatchingTypes() {
        var anon = new DummyAnonymousClosure {
            Context = AnonymousValue.From(42),
            Delegate = new Action<int>(_ => { })
        };
        Assert.That(AnonymousClosureUtil.CanConvert<DummyAnonymousClosure, DummyClosure>(anon), Is.True);
    }

    [Test]
    public void CanConvert_ReturnsFalseForMismatchedTypes() {
        var anon = new DummyAnonymousClosure {
            Context = AnonymousValue.From('c'),
            Delegate = new Action<string>(_ => { })
        };
        Assert.That(AnonymousClosureUtil.CanConvert<DummyAnonymousClosure, DummyClosure>(anon), Is.False);
    }

    [Test]
    public void InvokableAs_ReturnsFalseForMismatchedTypes() {
        var action = new Action<int, int>((int ctx, int arg) => { });
        var func = new Func<int, int>((int ctx) => ctx * 2);
        var refActionWithArg = new RefAction<int, int>((ref int ctx, ref int arg) => { });

        Assert.Multiple(() => {
            Assert.That(AnonymousClosureUtil.InvokableAs<Func<int>>(action), Is.False);
            Assert.That(AnonymousClosureUtil.InvokableAs<Func<int, int>>(action), Is.False);
            Assert.That(AnonymousClosureUtil.InvokableAs<Action<int, int>>(action), Is.False);
            Assert.That(AnonymousClosureUtil.InvokableAs<RefAction<int>>(action), Is.False);
            Assert.That(AnonymousClosureUtil.InvokableAs<Action>(action), Is.False);
            
            Assert.That(AnonymousClosureUtil.InvokableAs<Func<int, int>>(func), Is.False);
            Assert.That(AnonymousClosureUtil.InvokableAs<Action<int, int>>(func), Is.False);
            
            Assert.That(AnonymousClosureUtil.InvokableAs<Action<int>>(refActionWithArg), Is.False);
            Assert.That(AnonymousClosureUtil.InvokableAs<RefFunc<int, int>>(refActionWithArg), Is.False);
        });
    }

    [Test]
    public void InvokableAs_ReturnsTrueForMatchingTypes() {
        var action = new Action<int, int>((int ctx, int arg) => { });
        var refAction = new RefAction<int>((ref int ctx) => { });
        var refActionWithArg = new RefAction<int, int>((ref int ctx, ref int arg) => { });
        
        var func = new Func<int, int>((int ctx) => ctx * 2);
        var funcWithArg = new Func<int, int, int>((int ctx, int arg) => ctx + arg);
        var funcWithRefArg = new RefFuncWithNormalContext<int, int, int>((int ctx, ref int arg) => ctx * arg);
        
        Assert.Multiple(() => {
            Assert.That(AnonymousClosureUtil.InvokableAs<Action<int>>(action), Is.True);
            Assert.That(AnonymousClosureUtil.InvokableAs<Action>(refAction), Is.True);
            Assert.That(AnonymousClosureUtil.InvokableAs<RefAction<int>>(refActionWithArg), Is.True);
            
            Assert.That(AnonymousClosureUtil.InvokableAs<Func<int>>(func), Is.True);
            Assert.That(AnonymousClosureUtil.InvokableAs<Func<int, int>>(funcWithArg), Is.True);
            Assert.That(AnonymousClosureUtil.InvokableAs<RefFunc<int, int>>(funcWithRefArg), Is.True);
        });
    }

    [TestCase(typeof(InvalidOperationException), ExpectedResult = false)]
    [TestCase(typeof(InvalidCastException), ExpectedResult = false)]
    [TestCase(typeof(ArgumentException), ExpectedResult = false)]
    [TestCase(typeof(MissingMethodException), ExpectedResult = true)]
    public bool ShouldThrow_HandleExpected_Works(Type exceptionType) {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "testex")!;
        return AnonymousClosureUtil.ShouldThrow(ex, ExceptionHandlingPolicy.HandleExpected);
    }

    [TestCase(typeof(InvalidOperationException), ExpectedResult = false)]
    [TestCase(typeof(InvalidCastException), ExpectedResult = false)]
    [TestCase(typeof(ArgumentException), ExpectedResult = false)]
    [TestCase(typeof(MissingMethodException), ExpectedResult = false)]
    public bool ShouldThrow_HandleNone_Works(Type exceptionType) {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "testex")!;
        return AnonymousClosureUtil.ShouldThrow(ex, ExceptionHandlingPolicy.HandleNone);
    }

    [TestCase(typeof(InvalidOperationException), ExpectedResult = true)]
    [TestCase(typeof(InvalidCastException), ExpectedResult = true)]
    [TestCase(typeof(ArgumentException), ExpectedResult = true)]
    [TestCase(typeof(MissingMethodException), ExpectedResult = true)]
    public bool ShouldThrow_HandleAll_Works(Type exceptionType) {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "testex")!;
        return AnonymousClosureUtil.ShouldThrow(ex, ExceptionHandlingPolicy.HandleAll);
    }

    [Test]
    public void IsAction_TrueForAction() {
        Action a = () => { };
        Assert.That(AnonymousClosureUtil.IsAction(a), Is.True);
    }

    [Test]
    public void IsAction_FalseForFunc() {
        Func<int> f = () => 1;
        Assert.That(AnonymousClosureUtil.IsAction(f), Is.False);
    }

    [Test]
    public void IsAction_Type_TrueForAction() {
        Action a = () => { };
        Assert.That(AnonymousClosureUtil.IsAction(a.GetType()), Is.True);
    }

    [Test]
    public void IsAction_Type_FalseForFunc() {
        Func<int> f = () => 1;
        Assert.That(AnonymousClosureUtil.IsAction(f.GetType()), Is.False);
    }

    [Test]
    public void IsAction_GenericType_TrueForAction() {
        Assert.That(AnonymousClosureUtil.IsAction<Action<int>>(), Is.True);
    }

    [Test]
    public void IsAction_GenericType_FalseForFunc() {
        Assert.That(AnonymousClosureUtil.IsAction<Func<int>>(), Is.False);
    }

    [Test]
    public void IsFunc_TrueForFunc() {
        Func<int> f = () => 1;
        Assert.That(AnonymousClosureUtil.IsFunc(f), Is.True);
    }

    [Test]
    public void IsFunc_FalseForAction() {
        Action a = () => { };
        Assert.That(AnonymousClosureUtil.IsFunc(a), Is.False);
    }

    [Test]
    public void IsFunc_Type_TrueForFunc() {
        Func<int> f = () => 1;
        Assert.That(AnonymousClosureUtil.IsFunc(f.GetType()), Is.True);
    }

    [Test]
    public void IsFunc_Type_FalseForAction() {
        Action a = () => { };
        Assert.That(AnonymousClosureUtil.IsFunc(a.GetType()), Is.False);
    }

    [Test]
    public void IsFunc_GenericType_TrueForFunc() {
        Assert.That(AnonymousClosureUtil.IsFunc<Func<int>>(), Is.True);
    }

    [Test]
    public void IsFunc_GenericType_FalseForAction() {
        Assert.That(AnonymousClosureUtil.IsFunc<Action<int>>(), Is.False);
    }

    [Test]
    public void GetGenericArguments_Delegate_ReturnsCorrectTypes() {
        Action<int, string> del = (i, s) => { };
        var args = AnonymousClosureUtil.GetGenericArguments(del);
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(string) }));
    }

    [Test]
    public void GetGenericArguments_GenericType_ReturnsCorrectTypes() {
        var args = AnonymousClosureUtil.GetGenericArguments<Action<int, string>>();
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(string) }));
    }

    [Test]
    public void GetGenericArguments_Type_ReturnsCorrectTypes() {
        var args = AnonymousClosureUtil.GetGenericArguments(typeof(Action<int, string>));
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(string) }));
    }

    [Test]
    public void HasArg_ReturnsTrueIfHasArg() {
        Action<int, string> del = (i, s) => { };
        Assert.That(AnonymousClosureUtil.HasArg(del), Is.True);
    }

    [Test]
    public void HasArg_ReturnsFalseIfNoArg() {
        Action<int> del = (i) => { };
        Assert.That(AnonymousClosureUtil.HasArg(del), Is.False);
    }

    [Test]
    public void HasArgOfType_ReturnsTrueIfArgMatches() {
        Action<int, string> del = (i, s) => { };
        Assert.That(AnonymousClosureUtil.HasArgOfType<string>(del), Is.True);
    }

    [Test]
    public void HasArgOfType_ReturnsFalseIfArgDoesNotMatch() {
        Action<int, string> del = (i, s) => { };
        Assert.That(AnonymousClosureUtil.HasArgOfType<double>(del), Is.False);
    }
}