using System.Threading.Tasks;

namespace CGA
{
    /// <summary>
    /// All possible states of <see cref="IApp"/>.
    /// </summary>
    public enum AppState 
    {
        /// <summary> The initial state. </summary>
        JUST_CREATED,
        RUNNING,
        FINISHED,
        DISPOSED
    }



    /// <summary>
    /// Any launchable application.
    /// </summary>
    public interface IApp
    {
        AppState CurrentState { get; }

        Task LaunchAsync();
    }
}
