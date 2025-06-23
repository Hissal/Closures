# Guidelines
- Each test should be independent and not rely on the state of other tests.
- Tests should be clear and concise, focusing on a single aspect of the action closure functionality.
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
**Each one of these must be tested on all types of closures that take in an argument**
- ReceivesArg

## Ref argument closure tests
**Each one of these must be tested on all types of closures that take in a ref argument**
- ModifiesRefArgValue
- ModifiesRefArgValue_MultipleDelegates
- ModifiesRefArgValue_MultipleInvocations

## Mutating closure tests
**Each one of these must be tested on all types mutating closures that modify the stored context**
- Retain_RetainsModifiedContext
- Reset_ResetsModifiedContext

## Ref closure tests
**Each one of these must be tested on all types of ref closures that modify the original context**
- ModifiesOriginalContext
- ModifiesOriginalContext_MultipleDelegates
- ModifiesOriginalContext_MultipleInvocations
- ModifyingOriginalContext_ModifiesContextAndRefContext

## Func closure tests
**Each one of these must be tested on all types of closure funcs**
- ReturnsExpectedValue

# Seperated Tests

## Creation Tests
Every closure type should have a test using the Closure.(Type)
factory method to ensure it works correctly.
These should be in a separate file from functional tests.
Ensure that the created type is correct, the context is set up properly,
and that the Delegate is correct.
- `Closure.Action` for action closures
- `Closure.Func` for func closures

