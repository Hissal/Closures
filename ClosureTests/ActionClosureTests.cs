using Closures;

namespace ClosureTests {
    [TestFixture]
    public class ActionClosureTests {
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
            var closure = Closure.CreateAction(context, (int ctx, ref int val) => val += ctx);

            closure.Invoke(ref value);

            Assert.That(value, Is.EqualTo(15));
        }

        [Test]
        public void RefActionClosure_WithRefContext_Invoke_ModifiesContextValue() {
            int context = 3;
            int value = 4;
            var closure = Closure.CreateAction(context, (ref int ctx, ref int val) => {
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
        public void PassedRefActionClosure_ContextIsUpdatedAfterInvoke() {
            int context = 5;
            var closure = Closure.CreateAction(ref context, (ref int ctx) => ctx *= 2);

            closure.Invoke();

            Assert.That(context, Is.EqualTo(10));
        }

        [Test]
        public void PassedRefActionClosure_WithArg_ContextIsUpdatedAfterInvoke() {
            int context = 3;
            var closure = Closure.CreateAction(ref context, (ref int ctx, int arg) => ctx += arg);

            closure.Invoke(7);

            Assert.That(context, Is.EqualTo(10));
        }

        [Test]
        public void PassedRefRefActionClosure_ContextIsUpdatedAfterInvoke() {
            int context = 4;
            int arg = 6;
            var closure = Closure.CreateAction(ref context, (ref int ctx, ref int a) => {
                ctx += a;
                a *= 2;
            });

            closure.Invoke(ref arg);

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(10)); // context updated
                Assert.That(arg, Is.EqualTo(12));     // arg updated
            });
        }
    }
}
