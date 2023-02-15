using Qos.Domain.Entities;


namespace Qos.GameLogic.GameWorld.Activities
{
    /// <summary>
    /// Определяет, можно ли игроку "сбросить" те или иные карты.  
    /// </summary>
    internal interface ICardsDiscardPolicy
    {
        bool MayDiscard(CardGroups cardGroups);
    }
}