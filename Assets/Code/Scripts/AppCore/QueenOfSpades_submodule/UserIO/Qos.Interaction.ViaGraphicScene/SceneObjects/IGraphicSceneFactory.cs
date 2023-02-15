using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;

namespace Qos.Interaction.ViaGraphicScene
{
    /// <summary>
    /// Should create all objects for a graphical scene.
    /// </summary>
    public interface IGraphicSceneFactory
    {
        IPlayer CreatePlayer(PlayerModel model);
        ICard CreateCard(CardModel cardModel);
        IDeck CreateDeck();
        ICardHeap CreateCardHeap();
        ICamera CreateCamera();
        IDiscModeUi CreateDiscModeUi();
        IExitUi CreateExitUi();
        IWinLooseAnnouncer CreateWinLooseAnnouncer();
        IPlayField CreatePlayField();
        ICursor CreateCursor();
    }
}