using System;
using System.Collections.Generic;
using System.Text;
using CGA.MainMenu;


namespace CGA
{
    /// <summary>
    /// All possible states of <see cref="IAppCore"/>.
    /// </summary>
    public enum AppCoreState
    {
        /// <summary> The initial state. </summary>
        JUST_CREATED,
        LAUNCHED,
        FINISHED,
        DISPOSED
    }


    /// <summary>
    /// Represents the main application component.
    /// The whole application exists to wrap this component and guide user to it (via various menus).
    /// <para>
    /// Example:
    /// It could be the main game screen with gameplay.
    /// </para>
    /// </summary>
    public interface IAppCore : IDisposable
    {
        /// <summary>
        /// The first arg - the previous state.
        /// The second arg - the new state.
        /// </summary>
        event Action<AppCoreState, AppCoreState> OnStateChanged;

        AppCoreState State { get; }

        void Launch();
    }
}
