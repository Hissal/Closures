using Closures;
using Closures.Anonymous;

namespace ClosureTests.Anonymous.Other;

[TestFixture]
[TestOf(typeof(AnonymousHelper))]
public class AnonymousHelperTest {
    class EmptyClosure : IClosure {
    }
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
            return AnonymousHelper.CanConvert<DummyAnonymousClosure, TClosureType>(this);
        }
    }

    [Test]
    public void GetInterfaceType_ReturnsCorrectInterface() {
        var type = AnonymousHelper.GetInterfaceType<DummyClosure>();
        Assert.That(type, Is.EqualTo(typeof(IClosure<int, Action<int>>)));
    }

    [Test]
    public void GetInterfaceType_ThrowsIfNoInterface() {
        Assert.Throws<InvalidOperationException>(() => AnonymousHelper.GetInterfaceType<EmptyClosure>());
    }

    [Test]
    public void GetInterfaceGenericArguments_ReturnsCorrectArguments() {
        var iface = typeof(IClosure<int, Action<int>>);
        var args = AnonymousHelper.GetInterfaceGenericArguments(iface);
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(Action<int>) }));
    }

    [Test]
    public void CanConvert_ReturnsTrueForMatchingTypes() {
        var anon = new DummyAnonymousClosure {
            Context = AnonymousValue.From(42),
            Delegate = new Action<int>(_ => { })
        };
        Assert.That(AnonymousHelper.CanConvert<DummyAnonymousClosure, DummyClosure>(anon), Is.True);
    }

    [Test]
    public void CanConvert_ReturnsFalseForMismatchedTypes() {
        var anon = new DummyAnonymousClosure {
            Context = AnonymousValue.From('c'),
            Delegate = new Action<string>(_ => { })
        };
        Assert.That(AnonymousHelper.CanConvert<DummyAnonymousClosure, DummyClosure>(anon), Is.False);
    }

    [TestCase(typeof(InvalidOperationException), ExpectedResult = false)]
    [TestCase(typeof(InvalidCastException), ExpectedResult = false)]
    [TestCase(typeof(ArgumentException), ExpectedResult = false)]
    [TestCase(typeof(MissingMethodException), ExpectedResult = true)]
    public bool ShouldThrow_HandleExpected_Works(Type exceptionType) {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "testex")!;
        return AnonymousHelper.ShouldThrow(ex, ExceptionHandlingPolicy.HandleExpected);
    }

    [TestCase(typeof(InvalidOperationException), ExpectedResult = false)]
    [TestCase(typeof(InvalidCastException), ExpectedResult = false)]
    [TestCase(typeof(ArgumentException), ExpectedResult = false)]
    [TestCase(typeof(MissingMethodException), ExpectedResult = false)]
    public bool ShouldThrow_HandleNone_Works(Type exceptionType) {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "testex")!;
        return AnonymousHelper.ShouldThrow(ex, ExceptionHandlingPolicy.HandleNone);
    }
    
    [TestCase(typeof(InvalidOperationException), ExpectedResult = true)]
    [TestCase(typeof(InvalidCastException), ExpectedResult = true)]
    [TestCase(typeof(ArgumentException), ExpectedResult = true)]
    [TestCase(typeof(MissingMethodException), ExpectedResult = true)]
    public bool ShouldThrow_HandleAll_Works(Type exceptionType) {
        var ex = (Exception)Activator.CreateInstance(exceptionType, "testex")!;
        return AnonymousHelper.ShouldThrow(ex, ExceptionHandlingPolicy.HandleAll);
    }

    [Test]
    public void IsAction_TrueForAction() {
        Action a = () => { };
        Assert.That(AnonymousHelper.IsAction(a), Is.True);
    }

    [Test]
    public void IsAction_FalseForFunc() {
        Func<int> f = () => 1;
        Assert.That(AnonymousHelper.IsAction(f), Is.False);
    }

    [Test]
    public void IsFunc_TrueForFunc() {
        Func<int> f = () => 1;
        Assert.That(AnonymousHelper.IsFunc(f), Is.True);
    }

    [Test]
    public void IsFunc_FalseForAction() {
        Action a = () => { };
        Assert.That(AnonymousHelper.IsFunc(a), Is.False);
    }

    [Test]
    public void GetGenericArguments_Delegate_ReturnsCorrectTypes() {
        Action<int, string> del = (i, s) => { };
        var args = AnonymousHelper.GetGenericArguments(del);
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(string) }));
    }

    [Test]
    public void GetGenericArguments_Type_ReturnsCorrectTypes() {
        var args = AnonymousHelper.GetGenericArguments(typeof(Action<int, string>));
        Assert.That(args, Is.EqualTo(new[] { typeof(int), typeof(string) }));
    }

    [Test]
    public void HasArgOfType_ReturnsTrueIfArgMatches() {
        Action<int, string> del = (i, s) => { };
        Assert.That(AnonymousHelper.HasArgOfType<string>(del), Is.True);
    }

    [Test]
    public void HasArgOfType_ReturnsFalseIfArgDoesNotMatch() {
        Action<int, string> del = (i, s) => { };
        Assert.That(AnonymousHelper.HasArgOfType<double>(del), Is.False);
    }
}