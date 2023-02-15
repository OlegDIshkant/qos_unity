using Qos.Domain.Entities;
using Qos.Domain.Events;
using System;
using System.Collections.Immutable;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated
{

    /// <summary>
    /// ������������� ��������� ������ (��������������) ��� ������� ��������.
    /// </summary>
    public interface IModelsProvider<Key, Model>
    {
        public ImmutableDictionary<Key, Model> Models { get; }
    }



    /// <summary>
    /// ����������, ��������������� ��������� ������ (��������������) ��� ����.
    /// </summary>
    public class CardModelsProvider : EventController, IModelsProvider<CardId, CardModel>
    {

        public ImmutableDictionary<CardId, CardModel> Models { get; private set; } = ImmutableDictionary.Create<CardId, CardModel>();


        public CardModelsProvider(Contexts contexts, Func<IEvent> GetCurrentEvent) : base(contexts, GetCurrentEvent)
        {
        }


        public override void Update()
        {
            if (CurrentEvent is CardCreatedEvent ccEvent)
            {
                Models = Models.SetItem(ccEvent.CardId, ccEvent.CardModel);
            }
        }
    }
}