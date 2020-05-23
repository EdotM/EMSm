![EMSm-Logo](https://www.eforge.net/EMSmImages/0d0a3187-9e07-4475-9480-bd41b011ad73/EMSm-Logo-with-Text2.jpg)  

[![Build Status](https://dev.azure.com/ehrengrubermanfred/EMSm/_apis/build/status/EdotMdot.EMSm?branchName=master)](https://dev.azure.com/ehrengrubermanfred/EMSm/_build/latest?definitionId=3&branchName=master)
[![NuGet Badge](https://buildstats.info/nuget/EMSm)](https://www.nuget.org/packages/EMSm/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)  


# A simple, TDD-testable hierarchical state machine for .Net

## Changelog
* v1.1.0: intial version.
* v1.2.0: support of older .Net-Frameworks implemented.

## Features
* Entry-/Exit- Implementation.
* Synchronous-/Asynchronous- Mode.
* Perfect to realize the behavior of embedded systems.
* UML-statechart-applicability.
* Use of transitions-tables to define the hierarchical structure.
* Great overview of all states because every state is represented with a separate class.
* Each state is TDD-testable.
* Full flexible due to state-logic-implementation in Do()-method.
* Command-injection can be accomplished from any thread.
* Variable-injection provides clarity and great testability.
* States can be designed for reusability.
* Supports Done-Flag-Pattern.
* Current-state-restoring using state-paths.
* Simple to learn :-).

## Quick Start

![StateChart](https://www.eforge.net/EMSmImages/0d0a3187-9e07-4475-9480-bd41b011ad73/StateChart.jpg)

```csharp
class InnerState1 : State
{
    private int counter;
 
    protected override void Entry()
    {
        /*Do some stuff here, which should run once
         a transition to this state occurs*/
        this.counter = 0;
        base.Entry();
    }
    protected override Enum Do()
    {
        /*This method is excuted on every RunCycle 
         as long as this state is the active one.
         Returns a transition for switching to another state*/
        if (this.counter++ >= 5)
            return Transitions.Transition1;
        return base.Do();
    }
    protected override void Exit()
    {
        /*Here you can do some cleanup,
         which run before a transition to
         another state happens*/
        base.Exit();
    }
}
```

```csharp
class InnerState2 : State
{
    private int counter;
 
    protected override void Entry()
    {
        /*Do some stuff here, which should run once
         a transition to this state occurs*/
        this.counter = 0;
        base.Entry();
    }
    protected override Enum Do()
    {
        /*This method is excuted on every RunCycle 
         as long as this state is the active one.
         Returns a transition for switching to another state*/
        if (this.counter++ >= 99)
            return Transitions.Transition2;
        return base.Do();
    }
    protected override void Exit()
    {
        /*Here you can do some cleanup,
         which run before a transition to
         another state happens*/
        base.Exit();
    }
}
```

```csharp
class OuterState : State
{
    public override TransitionsTable TransitionsTable
    {
        get => new TransitionsTable {
        //Initialtransition which should endup to InnerState 1
        new TransitionEntry{    
            Transition=Transitions.Initial,
            StateType=typeof(InnerState1),
            StateName="InnerState 1"},
        //Transition 1, which should endup to InnerState 2
        new TransitionEntry{
            Transition=Transitions.Transition1,
            StateType=typeof(InnerState2),
            StateName="Innerstate 2"},
        //Transition 2, which should endup to InnerState 1
        new TransitionEntry{
            Transition=Transitions.Transition2,
            StateType=typeof(InnerState1),
            StateName="Innerstate 1"},
        };
    }
 
    protected override void Entry()
    {
        /*Do some stuff here, which should run once
         a transition to this state occurs*/
        base.Entry();
    }
    protected override Enum Do()
    {
        /*This method is excuted on every RunCycle 
         if this state is the active one*/
        return base.Do();
    }
    protected override void Exit()
    {
        /*Here you can do some cleanup,
         which run before a transition to
         another state happens*/
        base.Exit();
    }
}
```

### RunCycle:
```csharp
OuterState outerState = new OuterState();
outerState.RunCycle();
```  
![StateChart-sync-async](https://www.eforge.net/EMSmImages/0d0a3187-9e07-4475-9480-bd41b011ad73/StateChart-sync-async.jpg)  
  
  
Please visit [https://www.eforge.net/EMSm](https://www.eforge.net/EMSm) for further documentation and tutorials
