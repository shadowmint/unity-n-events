# n-events

This is an unexciting but robust event handler and action system for Unity3d.

## Usage

Basic usage of this module revolves around the three core classes:

- `EventHandler` is a generic event handler.
- `Timer` is a suspendable replacement for Time for tests and gameplay.
- `Actions` is a high level interface for running and dispatching actions.

The basic actions usage is to implement `IAction` and then dispatch an instance
to an `Actions` object:

    private class SimpleAction : IAction
    {
        public Actions Actions { get; set; }
        public Timer Timer { get; set; }

        public void Execute()
        {
            // Do something, then invoke `Complete` to finished.
            Actions.Complete(this);
        }
    }

    ...

    myActions.Execute<SimpleAction>();

You can also handle event completion via callbacks:

    var task = new MyAction();
    instance.Execute(task, (ep) =>
    {
        if (ep.Is(task))
        {
            // Do something when a specific task is completed.
            // ...
        }
    });

There is some complexity about how event handlers work, see the tests in the
`Editor/` folder for each class for usage examples.

One thing to note about `Timer` is that it does not update each frame and must
be manually updated using `Step()` (or `Force()` in tests).

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
