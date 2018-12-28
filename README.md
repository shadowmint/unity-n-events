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
    echo Assets/pkg-all >> .gitignore
    echo Assets/pkg-all.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/pkg-all folder.

## Development

    cd test
    npm install

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are located in the tests folder.
