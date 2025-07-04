﻿using Lh.Closures;

namespace ClosureTests.ActionTests;

[TestFixture]
public class ClosureActionCreationTests {
    public void DummyAction(int context) { }
    public void DummyActionWithArg(int context, int arg) { }
    public void DummyActionWithRefArg(int context, ref int arg) { }
    public void DummyActionWithRefContext(ref int context) { }
    public void DummyActionWithRefContextAndNormalArg(ref int context, int arg) { }
    public void DummyActionWithRefContextAndRefArg(ref int context, ref int arg) { }
    
    [Test]
    public void ClosureAction_CreatesCorrectly() {
        int context = 10;
        var action = new Action<int>(DummyAction);
        var closure = Closure.Action(context, action);

        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IClosureAction<int, Action<int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(action));
        });
    }

    [Test]
    public void ClosureAction_WithArg_CreatesCorrectly() {
        int context = 10;
        var action = new Action<int, int>(DummyActionWithArg);
        var closure = Closure.Action(context, action);

        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IClosureAction<int, int, Action<int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(action));
        });
    }

    [Test]
    public void ClosureRefAction_WithRefArg_CreatesCorrectly() {
        int context = 10;
        var action = new RefActionWithNormalContext<int, int>(DummyActionWithRefArg);
        var closure = Closure.Action(context, action);

        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IClosureAction<int, int, RefActionWithNormalContext<int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(action));
        });
    }

    [Test]
    public void MutatingClosureAction_CreatesCorrectly() {
        int context = 10;
        var action = new RefAction<int>(DummyActionWithRefContext);
        var closure = Closure.Action(context, action);

        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IMutatingClosure>());
            Assert.That(closure, Is.InstanceOf<IClosureAction<int, RefAction<int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(action));
        });
    }

    [Test]
    public void MutatingClosureAction_WithArg_CreatesCorrectly() {
        int context = 10;
        var action = new ActionWithRefContext<int, int>(DummyActionWithRefContextAndNormalArg);
        var closure = Closure.Action(context, action);

        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IMutatingClosure>());
            Assert.That(closure, Is.InstanceOf<IClosureAction<int, int, ActionWithRefContext<int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(action));
        });
    }

    [Test]
    public void MutatingClosureRefAction_WithRefArg_CreatesCorrectly() {
        int context = 10;
        var action = new RefAction<int, int>(DummyActionWithRefContextAndRefArg);
        var closure = Closure.Action(context, action);

        Assert.Multiple(() => {
            Assert.That(closure, Is.InstanceOf<IMutatingClosure>());
            Assert.That(closure, Is.InstanceOf<IClosureAction<int, int, RefAction<int, int>>>());
            Assert.That(closure.Context, Is.EqualTo(context));
            Assert.That(closure.Delegate, Is.EqualTo(action));
        });
    }

    [Test]
    public void RefClosureAction_CreatesCorrectly() {
        int context = 10;
        var action = new RefAction<int>(DummyActionWithRefContext);
        var closure = Closure.Action(ref context, action);
        
        Assert.That(closure.Context, Is.EqualTo(context));
        Assert.That(closure.RefContext, Is.EqualTo(context));
        Assert.That(closure.Delegate, Is.EqualTo(action));
    }
    
    [Test]
    public void RefClosureAction_WithArg_CreatesCorrectly() {
        int context = 10;
        var action = new ActionWithRefContext<int, int>(DummyActionWithRefContextAndNormalArg);
        var closure = Closure.Action(ref context, action);
        
        Assert.That(closure.Context, Is.EqualTo(context));
        Assert.That(closure.RefContext, Is.EqualTo(context));
        Assert.That(closure.Delegate, Is.EqualTo(action));
    }
    
    [Test]
    public void RefClosureAction_WithRefArg_CreatesCorrectly() {
        int context = 10;
        var action = new RefAction<int, int>(DummyActionWithRefContextAndRefArg);
        var closure = Closure.Action(ref context, action);

        Assert.That(closure.Context, Is.EqualTo(context));
        Assert.That(closure.RefContext, Is.EqualTo(context));
        Assert.That(closure.Delegate, Is.EqualTo(action));
    }
}