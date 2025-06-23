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