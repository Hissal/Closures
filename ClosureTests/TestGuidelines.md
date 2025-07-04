﻿# Guidelines
- Each test should be independent and not rely on the state of other tests.
- Tests should be clear and concise, focusing on a single aspect of the closure functionality.
- Use descriptive names for test methods to indicate what they are testing.
- Ensure that all edge cases are covered, including empty contexts, null values, and invalid inputs.
- Document any assumptions made in the tests.
- If a test fails, provide a clear error message that indicates what was expected and what was actually received.
- Consider using setup and teardown methods to prepare the test environment and clean up after tests, if necessary.
- Ensure that tests are run in a consistent environment to avoid flaky tests.
- Keep tests organized and grouped by type of closure being tested.

# Closure Tests

## Default tests
**Each one of these must be tested on all types of closures**
- ReceivesContext
- ReceivesTupleContext_AndModifiesTestContextValue
- AddAndRemove_Works
- Add_MultipleTimes_Works
- Remove_MultipleTimes_Works

## Argument closure tests
**Each one of these must be tested on all types of closures that take in a normal argument**
- ReceivesArg

## Ref argument closure tests
**Each one of these must be tested on all types of closures that take in a ref argument**
- ReceivesRefArg
- ModifiesRefArgValue
- ModifiesRefArgValue_MultipleDelegates
- ModifiesRefArgValue_MultipleInvocations

## Mutating closure tests
**Each one of these must be tested on all types mutating closures that modify the stored context**
- Retain_RetainsModifiedContext
- Reset_ResetsModifiedContext

## Ref closure tests
**Each one of these must be tested on all types of ref closures where the context is passed as ref**
- ModifiesOriginalContext
- ModifiesOriginalContext_MultipleDelegates
- ModifiesOriginalContext_MultipleInvocations
- ModifyingContext_ModifiesOriginalContext
- ModifyingOriginalContext_ModifiesContextAndRefContext
- AssigningRefContextToAnotherVariable_AndModifyingIt_ModifiesContext

## Func closure tests
**Each one of these must be tested on all types of closure funcs**
- ReturnsExpectedValue

# Edge case tests
**Each type of closure should also have edge cases tested in a separate file categorized by the closure type**

## Default edge case tests
**Each one of these must be tested on all types of "Normal" closures**
- NullContext_Invoke_DoesNotThrow
- NullDelegate_Invoke_DoesNotThrow (For ref arg closures call this with non ref arg)
- NullDelegate_Add_DoesNotThrow
- Add_NullDelegate_DoesNotThrow
- NullDelegate_Remove_DoesNotThrow
- Remove_NullDelegate_DoesNotThrow
- ExceptionDuringInvocation_Throws
- ExceptionDuringInvocation_TryCatch_CatchesThrownException
- ConcurrentAddRemoveInvoke_IsThreadSafe (Exclude on ref closures)

## Argument closure edge cases
**Each one of these must be tested on all types of closures that take in a normal argument**
- NullArgument_Invoke_DoesNotThrow

## Ref argument closure edge cases
**Each one of these must be tested on all types of closures that take in a ref argument**
- NullDelegate_Invoke_RefArg_DoesNotThrow
- NullRefArgument_Invoke_DoesNotThrow
- NullRefArg_SettingValue_ModifiesOriginalRef

## Mutating closure edge cases
**Each one of these must be tested on all types mutating closures that modify the stored context**
- Reset_NullDelegate_DoesNotThrow
- ConcurrentInvoke_SharesContextAcrossThreads

## Ref closure edge cases
**Each one of these must be tested on all types of ref closures where the context is passed as ref**
- NullContext_Invoke_SetsValueToOriginalContext

# Separated Tests

## Creation Tests
Every closure type should have a test using the Closure.(Type)
factory method to ensure it works correctly.
These should be in a separate file from functional tests.
Ensure that the created type is correct, the context is set up properly, and that the Delegate is correct.
- `Closure.Action` for action closures
- `Closure.Func` for func closures

