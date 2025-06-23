using Lh.Closures;

namespace ClosureTests {
    [TestFixture]
    public class FuncClosureTests {
        [Test]
        public void FuncClosure_Invoke_ReturnsExpectedResult() {
            int context = 7;
            var closure = Closure.Func(context, ctx => ctx * 2);

            int result = closure.Invoke();

            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void FuncClosure_WithRefContext_Invoke_ReturnsExpectedResult_And_ModifiesContext() {
            int context = 5;
            int value = 10;
            var closure = Closure.Func(context, (ref int ctx) => {
                ctx += value;
                return ctx * 2;
            });

            int result = closure.Invoke();

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(5)); // Original context not modified by closure
                Assert.That(result, Is.EqualTo(30)); // Result based on modified context
                Assert.That(closure.Context, Is.EqualTo(15)); // Closure's context modified
            });
        }
        
        // [Test]
        // public void FuncClosure_InvokeSeparate_ReturnsExpectedResults() {
        //     int context = 3;
        //     var closure = Closure.Func(context, ctx => ctx + 2);
        //     closure.AddFunc(ctx => ctx + 3); // Adding another function to the closure
        //     closure.AddFunc(ctx => ctx + 4); // Adding a third function to the closure
        //
        //     var results = closure.InvokeSeparate().ToArray();
        //     
        //     Assert.Multiple(() => {
        //         Assert.That(results, Has.Length.EqualTo(3)); // Three functions should be invoked
        //         Assert.That(results[0], Is.EqualTo(5)); // First function result
        //         Assert.That(results[1], Is.EqualTo(6)); // Second function result
        //         Assert.That(results[2], Is.EqualTo(7)); // Third function result
        //     });
        // }

        [Test]
        public void FuncClosure_WithArg_Invoke_ReturnsExpectedResult() {
            string context = "foo";
            var closure = Closure.Func<string, int, string>(context, (ctx, arg) => ctx + arg);

            string result = closure.Invoke(123);

            Assert.That(result, Is.EqualTo("foo123"));
        }

        [Test]
        public void FuncClosure_WithArgAndRefContext_Invoke_ReturnsExpectedResult_And_ModifiesStoredContext() {
            int context = 5;
            int value = 10;
            var closure = Closure.Func(context, (ref int ctx, int arg) => {
                ctx += arg;
                return ctx * 2;
            });

            int result = closure.Invoke(3);

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(5)); // Original context not modified by closure
                Assert.That(result, Is.EqualTo(16)); // Result based on modified context
                Assert.That(closure.Context, Is.EqualTo(8)); // Closure's context modified
            });
        }

        [Test]
        public void RefFuncClosure_Invoke_ModifiesValueAndReturnsResult() {
            int context = 3;
            int value = 4;
            var closure = Closure.Func(context, (int ctx, ref int val) => {
                val += ctx;
                return val * 2;
            });

            int result = closure.Invoke(ref value);

            Assert.Multiple(() => {
                Assert.That(value, Is.EqualTo(7));
                Assert.That(result, Is.EqualTo(14));
            });
        }

        [Test]
        public void RefFuncClosure_WithRefContext_Invoke_ModifiesStoredContext_And_ReturnsResult() {
            int context = 2;
            int value = 5;
            var closure = Closure.Func(context, (ref int ctx, ref int val) => {
                val += ctx;
                ctx *= 2; // This won't affect the original context but will affect the closure's context
                return val * 2;
            });

            int result = closure.Invoke(ref value);

            Assert.Multiple(() => {
                Assert.That(value, Is.EqualTo(7)); // Value modified by closure
                Assert.That(result, Is.EqualTo(14)); // Result based on modified value
                Assert.That(closure.Context, Is.EqualTo(4)); // Closure's context modified
            });
        }

        [Test]
        public void FuncClosure_AddAndRemoveFunc_Works() {
            int context = 2;
            int callCount = 0;

            int Handler(int ctx) {
                callCount++;
                return ctx * 2;
            }

            var closure = Closure.Func(context, Handler);
            closure.Add(Handler);
            int result = closure.Invoke();
            closure.Remove(Handler);
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

            var closure = Closure.Func<int, int, int>(context, Handler);
            closure.Add(Handler);
            int result = closure.Invoke(ref arg);
            closure.Remove(Handler);
            int result2 = closure.Invoke(ref arg);

            Assert.Multiple(() => {
                Assert.That(callCount, Is.EqualTo(3)); // Both handlers called at first, then one
                Assert.That(result, Is.EqualTo(12)); // Both handlers called, last one should give the result
                Assert.That(result2, Is.EqualTo(16)); // Only one handler called
                Assert.That(arg, Is.EqualTo(8)); // Final value of arg after all calls
            });
        }

        [Test]
        public void PassedRefFuncClosure_ContextIsUpdatedAfterInvoke() {
            int context = 5;
            var closure = Closure.Func(ref context, (ref int ctx) => {
                ctx *= 3;
                return ctx + 1;
            });

            int result = closure.Invoke();

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(15)); // context updated
                Assert.That(result, Is.EqualTo(16));  // result reflects updated context
            });
        }

        [Test]
        public void PassedRefFuncClosure_WithArg_ContextIsUpdatedAfterInvoke() {
            int context = 7;
            var closure = Closure.Func(ref context, (ref int ctx, int arg) => {
                ctx += arg;
                return ctx * 2;
            });

            int result = closure.Invoke(8);

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(15)); // context updated
                Assert.That(result, Is.EqualTo(30));  // result reflects updated context
            });
        }

        [Test]
        public void PassedRefRefFuncClosure_ContextAndArgAreUpdatedAfterInvoke() {
            int context = 2;
            int arg = 10;
            var closure = Closure.Func(ref context, (ref int ctx, ref int a) => {
                ctx += a;
                a *= 2;
                return ctx + a;
            });

            int result = closure.Invoke(ref arg);

            Assert.Multiple(() => {
                Assert.That(context, Is.EqualTo(12)); // context updated
                Assert.That(arg, Is.EqualTo(20));     // arg updated
                Assert.That(result, Is.EqualTo(32));  // result reflects updated values
            });
        }
    }
}
