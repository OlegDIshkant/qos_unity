

namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    /// <summary>
    /// Визуально демонстрирует, что пользователь проиграл или выиграл.
    /// </summary>
    public interface IWinLooseAnnouncer
    {
        void LaunchWinAnnouncement();
        void LaunchLooseAnnouncement();
    }
}