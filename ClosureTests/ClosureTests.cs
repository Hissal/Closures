using Closures;

namespace ClosureTests {
    [TestFixture]
    public class ClosureTests {
        [Test]
        public void ActionClosure_Invoke_CallsActionWithContext() {
            int context = 42;
            int result = 0;
            var closure = Closure.CreateAction(context, ctx => result = ctx + 1);

            closure.Invoke();

            Assert.That(result, Is.EqualTo(43));
        }
        
        [Test]
        public void ActionClosure_WithRefContext_Invoke_ModifiesTheContext() {
            int context = 5;
            int value = 10;
            var closure = Closure.CreateAction(context, (ref int ctx) => ctx += value);

            closure.Invoke();

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(5)); // Original context not modified by closure
                Assert.That(value, Is.EqualTo(10)); // Original value remains unchanged
                Assert.That(closure.Context, Is.EqualTo(15)); // Closure's context modified
            });
        }

        [Test]
        public void ActionClosure_WithArg_Invoke_CallsActionWithContextAndArg() {
            string context = "prefix";
            string result = null;
            var closure = Closure.CreateAction<string, string>(context, (ctx, arg) => result = ctx + arg);

            closure.Invoke("Test");

            Assert.That(result, Is.EqualTo("prefixTest"));
        }
        
        [Test]
        public void ActionClosure_WithArgAndRefContext_Invoke_ModifiesTheContext() {
            int context = 5;
            int value = 10;
            var closure = Closure.CreateAction(context, (ref int ctx, int val) => {
                ctx += val;
                value = ctx; // This will modify the value in the closure
            });

            closure.Invoke(3);

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(5)); // Original context not modified by closure
                Assert.That(value, Is.EqualTo(8)); // Value modified by closure
                Assert.That(closure.Context, Is.EqualTo(8)); // Closure's context modified
            });
        }

        [Test]
        public void RefActionClosure_WithNormalContext_Invoke_ModifiesArgValue() {
            int context = 5;
            int value = 10;
            var closure = Closure.CreateRefAction(context, (int ctx, ref int val) => val += ctx);

            closure.Invoke(ref value);

            Assert.That(value, Is.EqualTo(15));
        }

        [Test]
        public void RefActionClosure_WithRefContext_Invoke_ModifiesContextValue() {
            int context = 3;
            int value = 4;
            var closure = Closure.CreateRefAction(context, (ref int ctx, ref int val) => {
                val += ctx;
                ctx *= 2; // This won't affect the original context but will affect the closure's context
            });

            closure.Invoke(ref value);

            Assert.Multiple(() => {
                Assert.That(value, Is.EqualTo(7)); // Value modified by closure
                Assert.That(context, Is.EqualTo(3)); // Original context remains unchanged
                Assert.That(closure.Context, Is.EqualTo(6)); // Closure's context modified
            });
        }

        [Test]
        public void FuncClosure_Invoke_ReturnsExpectedResult() {
            int context = 7;
            var closure = Closure.CreateFunc(context, ctx => ctx * 2);

            int result = closure.Invoke();

            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void FuncClosure_WithArg_Invoke_ReturnsExpectedResult() {
            string context = "foo";
            var closure = Closure.CreateFunc<string, int, string>(context, (ctx, arg) => ctx + arg);

            string result = closure.Invoke(123);

            Assert.That(result, Is.EqualTo("foo123"));
        }

        [Test]
        public void RefFuncClosure_Invoke_ModifiesValueAndReturnsResult() {
            int context = 3;
            int value = 4;
            var closure = Closure.CreateRefFunc(context, (int ctx, ref int val) => {
                val += ctx;
                return val * 2;
            });

            int result = closure.Invoke(ref value);

            Assert.That(value, Is.EqualTo(7));
            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void ActionClosure_AddAndRemoveAction_Works() {
            int context = 1;
            int callCount = 0;
            void Handler(int ctx) => callCount += ctx;

            var closure = Closure.CreateAction(context, Handler);
            closure.AddAction(Handler);
            closure.Invoke();
            closure.RemoveAction(Handler);
            closure.Invoke();

            Assert.That(callCount, Is.EqualTo(3 * context));
        }

        [Test]
        public void FuncClosure_AddAndRemoveFunc_Works() {
            int context = 2;
            int callCount = 0;

            int Handler(int ctx) {
                callCount++;
                return ctx * 2;
            }

            var closure = Closure.CreateFunc(context, Handler);
            closure.AddFunc(Handler);
            int result = closure.Invoke();
            closure.RemoveFunc(Handler);
            int result2 = closure.Invoke();

            Assert.Multiple(() => {
                Assert.That(callCount, Is.EqualTo(3));
                Assert.That(result, Is.EqualTo(4)); // Both handlers called, last one should give the result
                Assert.That(result2, Is.EqualTo(4)); // Only one handler called
            });
        }

        [Test]
        public void RefFuncClosure_AddAndRemoveFunc_WorksAndPassesRefToEachDelegate() {
            int context = 2;
            int callCount = 0;
            int arg = 2;

            int Handler(int ctx, ref int arg) {
                callCount++;
                arg += ctx;
                return arg * 2;
            }

            var closure = Closure.CreateRefFunc<int, int, int>(context, Handler);
            closure.AddFunc(Handler);
            int result = closure.Invoke(ref arg);
            closure.RemoveFunc(Handler);
            int result2 = closure.Invoke(ref arg);

            Assert.Multiple(() => {
                Assert.That(callCount, Is.EqualTo(3)); // Both handlers called at first, then one
                Assert.That(result, Is.EqualTo(12)); // Both handlers called, last one should give the result
                Assert.That(result2, Is.EqualTo(16)); // Only one handler called
                Assert.That(arg, Is.EqualTo(8)); // Final value of arg after all calls
            });
        }
    }
}