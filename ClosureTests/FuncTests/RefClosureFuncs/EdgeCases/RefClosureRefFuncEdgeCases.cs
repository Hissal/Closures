using Lh.Closures;

namespace ClosureTests.FuncTests.RefClosureFuncs.EdgeCases;

[TestFixture]
public class RefClosureRefFuncEdgeCases {
    class TestClass { public int Value { get; set; } }

    [Test]
    public void RefClosureRefFunc_NullContext_Invoke_DoesNotThrow() {
        string context = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref string ctx, ref int arg) => ctx);
            int arg = 0;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void RefClosureRefFunc_NullDelegate_Invoke_DoesNotThrow() {
        string context = "abc";
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<string, int, string>)null);
            int arg = 0;
            closure.Invoke(arg);
        });
    }

    [Test]
    public void RefClosureRefFunc_NullDelegate_Add_DoesNotThrow() {
        string context = "abc";
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<string, int, string>)null);
            closure.Add((ref string ctx, ref int arg) => ctx);
        });
    }

    [Test]
    public void RefClosureRefFunc_Add_NullDelegate_DoesNotThrow() {
        string context = "abc";
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref string ctx, ref int arg) => ctx);
            closure.Add(null);
        });
    }

    [Test]
    public void RefClosureRefFunc_NullDelegate_Remove_DoesNotThrow() {
        string context = "abc";
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<string, int, string>)null);
            closure.Remove((ref string ctx, ref int arg) => ctx);
        });
    }

    [Test]
    public void RefClosureRefFunc_Remove_NullDelegate_DoesNotThrow() {
        string context = "abc";
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref string ctx, ref int arg) => ctx);
            closure.Remove(null);
        });
    }

    [Test]
    public void RefClosureRefFunc_ExceptionDuringInvocation_Throws() {
        string context = "abc";
        int arg = 0;
        Assert.Throws<InvalidOperationException>(() => {
            var closure = Closure.Func(ref context, (ref string ctx, ref int arg) => {
                throw new InvalidOperationException("fail");
                return 2;
            });
            closure.Invoke(arg);
        });
    }

    [Test]
    public void RefClosureRefFunc_ExceptionDuringInvocation_TryCatch_CatchesThrownException() {
        string context = "abc";
        int arg = 0;
        var closure = Closure.Func(ref context, (ref string ctx, ref int arg) => {
            throw new InvalidOperationException("fail");
            return 2;
        });
        try {
            closure.Invoke(arg);
            Assert.Fail("Exception was not thrown");
        } catch (InvalidOperationException ex) {
            Assert.That(ex.Message, Is.EqualTo("fail"));
        }
    }
    
    // Argument closure edge cases
    
    [Test]
    public void RefClosureRefFunc_NullArgument_Invoke_DoesNotThrow() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref int ctx, ref int? arg) => {
                Assert.That(arg, Is.Null, "Argument should be null");
                return arg ?? ctx;
            });
            closure.Invoke(null);
        }, "Null argument should not throw");
    }
    
    // Ref argument edge cases

    [Test]
    public void RefClosureRefFunc_NullDelegate_Invoke_RefArg_DoesNotThrow() {
        string context = "abc";
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<string, int, string>)null);
            int arg = 0;
            closure.Invoke(ref arg);
        });
    }

    [Test]
    public void RefClosureRefFunc_NullRefArgument_Invoke_DoesNotThrow() {
        string context = "abc";
        int? arg = null;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (ref string ctx, ref int? arg) => ctx);
            closure.Invoke(ref arg);
        });
    }

    [Test]
    public void RefClosureRefFunc_NullRefArg_SettingValue_ModifiesOriginalRef() {
        string context = "abc";
        var closure = Closure.Func(ref context, (ref string ctx, ref string arg) => arg = "changed");
        string arg = null;
        closure.Invoke(ref arg);
        Assert.That(arg, Is.EqualTo("changed"));
    }
    
    // Ref closure edge cases
    
    [Test]
    public void RefClosureRefFunc_NullContext_Invoke_SetsValueToOriginalContext() {
        string context = null;
        var closure = Closure.Func(ref context, (ref string ctx, ref int arg) => ctx = "set");
        int arg = 0;
        closure.Invoke(arg);
        Assert.That(context, Is.EqualTo("set"));
    }
    
    // Func closure edge cases

    [Test]
    public void RefClosureRefFunc_NullDelegate_ReturnsDefault() {
        int context = 5;
        Assert.DoesNotThrow(() => {
            var closure = Closure.Func(ref context, (RefFunc<int,int,int>)null);
            int result = closure.Invoke(0);
            Assert.That(result, Is.EqualTo(0), "Default return value should be 0 when no delegate is set");
        }, "ClosureFunc with null delegate should not throw");
    }
}