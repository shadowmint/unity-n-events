# n-events

This is an unexciting but robust event handler and action system for Unity3d.

## Usage

    class DummyCompletionEvent 
    {
      public bool Complete;
    }

    class DummyAction
    {
      private EventHandler _events = new EventHandler();

      public void Execute()
      {
        _events.Trigger(new DummyCompletionEvent { Complete = true });
        _events.ClearEventHandlers<DummyCompletionEvent>();
      }

      public DummyAction OnComplete(Action<DummyCompletionEvent> handler)
      {
        _events.AddEventHandler(handler);
        return this;
      }
    }

    ...
     
    new DummyAction().OnComplete((ep) => { ... }).Execute();

The `EventHandler` type is a light weight interface that can be attached to objects
with relatively low cost.

The extension methods `TriggerDeferred` and `TriggerAsync` support deferred event 
dispatch for interactive events, etc.

## Install

From your unity project folder:

    npm init
    npm install shadowmint/unity-n-events --save
    echo Assets/packages >> .gitignore
    echo Assets/packages.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/packages folder.

## Development

Setup and run tests:

    npm install
    npm install ..
    cd test
    npm install
    gulp

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

This module replaces `N.Events` which is removed in 0.0.3 of n-core.

### Tests

All tests are wrapped in `#if ...` blocks to prevent test spam.

You can enable tests in: `Player settings > Other Settings > Scripting Define Symbols`

The test key for this package is: N_EVENTS_TESTS
