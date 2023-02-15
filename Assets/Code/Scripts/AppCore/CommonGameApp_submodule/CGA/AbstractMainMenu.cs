using System;


namespace CGA.MainMenu
{
    /// <summary>
    /// All possible states of <see cref="AbstractMainMenu"/>.
    /// </summary>
    public enum MainMenuState
    {
        /// <summary> The initial state. </summary>
        JUST_CREATED,
        LAUNCHED,
        /// <summary> User asks to start a new game. </summary>
        NEW_GAME_REQUESTED,
        /// <summary> User asks to quit. </summary>
        EXIT_REQUESTED,
        /// <summary> Paused and invisible. </summary>
        HIDDEN
    }



    /// <summary>
    /// Is responsible for interacting with user and figuring out what he wants to do with the app.
    /// (Allow user to start game, for instance, or exit the app.)
    /// </summary>
    public abstract class AbstractMainMenu
    {
        public abstract MainMenuState CurrentState { get; }


        /// <summary>
        /// A previous state (first arg) has transited to a new state (second arg).
        /// </summary>
        public abstract event Action<MainMenuState, MainMenuState> OnStateChanged;

        public abstract void Launch();

        /// <summary>
        /// Go to <see cref="MainMenuState.HIDDEN"/> state.
        /// </summary>
        public abstract void Hide();

        public abstract void UnHide();
    }
}
