using EM.EMSm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Blinky
{
    #region command & transition definitions

    /// <summary>
    /// Commands of the blinky-statemachine.
    /// </summary>
    internal enum BlinkySMCommands
    {
        None,
        Enable,
        Disable,
    }

    /// <summary>
    /// Transitions of the blinky-statemachine.
    /// </summary>
    internal enum BlinkySMTransitions
    {
        Initial,
        EnableBlink,
        DisableBlink,
    }

    /// <summary>
    /// Commands of the inner-statemachine from enabled-state.
    /// </summary>
    internal enum EnabledStateCommands
    {
        None,
        ToggleBlinkSpeed,
    }

    /// <summary>
    /// Transitions of the inner-statemachine from enabled-state.
    /// </summary>
    internal enum EnabledStateTransitions
    {
        Initial,
        ChangeToSlowBlink,
        ChangeToFastBlink,
    }

    #endregion

    #region states

    /// <summary>
    /// Rootstate of the blinky-statemachine (its the one and only outermost state, please refer to https://www.eforge.net/EMSm).
    /// </summary>
    internal class BlinkySMState : State
    {
        /// <summary>
        /// The overridden transitions table. Here the structure and hierarchy
        /// of inner state machine is defined.
        /// </summary>
        public override TransitionsTable TransitionsTable
        {
            get => new TransitionsTable {
            new TransitionEntry{
                Transition=BlinkySMTransitions.Initial,
                StateType=typeof(DisabledState),
                StateName="Disabled"},
            new TransitionEntry{
                Transition=BlinkySMTransitions.EnableBlink,
                StateType=typeof(EnabledState),
                StateName="Enabled"},
            new TransitionEntry{
                Transition=BlinkySMTransitions.DisableBlink,
                StateType=typeof(DisabledState),
                StateName="Disabled"},
            };
        }
    }

    /// <summary>
    /// Disabled state.
    /// </summary>
    internal class DisabledState : State
    {
        /// <summary>
        /// This method is excuted on every RunCycle as long as this state is the active one.
        /// </summary>
        /// <returns>A transition for switching to another state.</returns>
        protected override Enum Do()
        {
            if (this.GetCommand<BlinkySMCommands>() == BlinkySMCommands.Enable)
                return BlinkySMTransitions.EnableBlink;

            return base.Do();
        }
    }

    /// <summary>
    /// Enabled state.
    /// </summary>
    internal class EnabledState : State
    {
        /// <summary>
        /// The overridden transitions table. Here the structure and hierarchy
        /// of inner state machine is defined.
        /// </summary>
        public override TransitionsTable TransitionsTable
        {
            get => new TransitionsTable {
            new TransitionEntry{
                Transition=EnabledStateTransitions.Initial,
                StateType=typeof(BlinkSlowState),
                StateName="BlinkSlow"},
            new TransitionEntry{
                Transition=EnabledStateTransitions.ChangeToFastBlink,
                StateType=typeof(BlinkFastState),
                StateName="BlinkFast"},
            new TransitionEntry{
                Transition=EnabledStateTransitions.ChangeToSlowBlink,
                StateType=typeof(BlinkSlowState),
                StateName="BlinkSlow"},
            };
        }

        /// <summary>
        /// This method is excuted on every RunCycle as long as this state is the active one.
        /// </summary>
        /// <returns>A transition for switching to another state.</returns>
        protected override Enum Do()
        {
            if (this.GetCommand<BlinkySMCommands>() == BlinkySMCommands.Disable)
                return BlinkySMTransitions.DisableBlink;

            return base.Do();
        }

        /// <summary>
        /// Here the cleanup-code, which runs before a transition to
        /// another state happens, is implemented.
        /// </summary>
        protected override void Exit()
        {
            this.GetVar<GPIO>("gpio").Lamp = false;
            base.Exit();
        }

    }
    
    /// <summary>
    /// BlinkSlow state.
    /// </summary>
    internal class BlinkSlowState : State
    {
        private const int IntervalTime = 5000;

        /// <summary>
        /// This method runs once a transition to this
        /// state occurs.
        /// </summary>
        protected override void Entry()
        {
            this.GetVar<Stopwatch>("timer").Start();
            base.Entry();
        }

        /// <summary>
        /// This method is excuted on every RunCycle as long as this state is the active one.
        /// </summary>
        /// <returns>A transition for switching to another state.</returns>
        protected override Enum Do()
        {
            if (this.GetVar<Stopwatch>("timer").ElapsedMilliseconds >= IntervalTime)
            {
                this.GetVar<Stopwatch>("timer").Restart();
                this.GetVar<GPIO>("gpio").ToggleLamp();
            }

            if (this.GetCommand<EnabledStateCommands>() == EnabledStateCommands.ToggleBlinkSpeed)
                return EnabledStateTransitions.ChangeToFastBlink;

            return base.Do();
        }
    }

    /// <summary>
    /// BlinkFast state
    /// </summary>
    internal class BlinkFastState : State
    {
        private const int IntervalTime = 1000;

        /// <summary>
        /// This method runs once a transition to this
        /// state occurs.
        protected override void Entry()
        {
            this.GetVar<Stopwatch>("timer").Start();
            base.Entry();
        }

        /// <summary>
        /// This method is excuted on every RunCycle as long as this state is the active one.
        /// </summary>
        /// <returns>A transition for switching to another state.</returns>
        protected override Enum Do()
        {
            if (this.GetVar<Stopwatch>("timer").ElapsedMilliseconds >= IntervalTime)
            {
                this.GetVar<Stopwatch>("timer").Restart();
                this.GetVar<GPIO>("gpio").ToggleLamp();
            }

            if (this.GetCommand<EnabledStateCommands>() == EnabledStateCommands.ToggleBlinkSpeed)
                return EnabledStateTransitions.ChangeToSlowBlink;

            return base.Do();
        }
    }

    #endregion
}
