using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;


namespace Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated
{
    /// <summary>
    /// ѕредоставл€ет информацию о том, что в данный момент времени должно происходить с тем или иным игроком,
    /// а также, что должно было с ним происходить в предыдущий момент времени.
    /// </summary>
    public interface IPlayerActionsProvider : IPlayerActionsDefiner // через реалицию этого интерфейса будет предоставл€тьс€ информаци€ о текущем моменте времени
    {
        IPlayerActionsDefiner PreviousActions { get; } // через этот член будет предоставл€тьс€ информаци€ о действи€х в предыдущий момент времени
    }


    /// <summary>
    /// ¬ычисл€ет, что в определенный момент времени должно происходит с тем или иным игроком.
    /// </summary>
    /// <remarks>
    public interface IPlayerActionsDefiner
    {
        DictionaryData<PlayerId, PlayerAction> DictionaryOutput { get; }
    }
}