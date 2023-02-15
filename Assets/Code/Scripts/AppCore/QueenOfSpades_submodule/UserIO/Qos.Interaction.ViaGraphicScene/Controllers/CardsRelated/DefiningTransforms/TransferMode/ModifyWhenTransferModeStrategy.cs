using Qos.Domain.Entities;
using Qos.Domain.Events;
using Qos.Interaction.ViaGraphicScene.Controllers.Common;


namespace Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated.DefiningTransforms
{
    /// <summary>
    /// —тратеги€, котора€ просит модифицировать положени€ карт всЄ врем€, пока у игрока, владеющего картами, 
    /// какой-либо другой игрок выбирает карты дл€ передачи себе.
    /// </summary>
    public class ModifyWhenTransferModeStrategy : SwitchableWhenToModifyStategy
    {
        /// <summary>
        /// True - считаем, что процесс выбора началс€, если он началс€ дл€ игрока, отдающего карты (<see cref="_giverId"/>).
        /// False - считаем, что процесс выбора началс€, если он началс€ дл€ игрока, берущего карты (<see cref="_takerId"/>).
        /// </summary>
        private readonly bool _takerIsFixed;

        private PlayerId _giverId;
        private PlayerId? _takerIdToIgnore; // ƒл€ некоторых случаев может пригадитьс€ возможность заглушить работу класса 
        private PlayerId _takerId;


        /// <param name="takerIdToIgnore"> —тратеги€ Ќ≈ модифицирует карты, если игрок c этим id выбирает карты среди карт <paramref name="giverId"/>. </param>
        public ModifyWhenTransferModeStrategy(PlayerId giverId, PlayerId? takerIdToIgnore)
        {
            _takerIsFixed = false;
            _giverId = giverId;
            _takerIdToIgnore = takerIdToIgnore;
        }


        public ModifyWhenTransferModeStrategy(PlayerId takerId)
        {
            _takerIsFixed = true;
            _takerId = takerId;
        }


        protected override bool IfEnterModifiesAllowedState()
        {
            return false; // TransferModeStarted(@event);
        }


        private bool TransferModeStarted(IEvent @event)
        {
            if (_takerIsFixed)
            {
                return @event is PlayersGoingToTrasferModeEvent ev && ev.CardTakerId.Equals(_takerId);
            }
            else
            {
                return @event is PlayersGoingToTrasferModeEvent ev && ev.CardGiverId.Equals(_giverId) && IsAllowedTaker(ev.CardTakerId);
            }
        }


        private bool IsAllowedTaker(PlayerId takerId) => _takerIdToIgnore == null || !_takerIdToIgnore.Equals(takerId);


        protected override bool IfExitModifiesAllowedState()
        {
            return false;// TransferModeFinished(@event);
        }


        private bool TransferModeFinished(IEvent @event)
        {
            if (_takerIsFixed)
            {
                return @event is PlayersGoingOutTrasferModeEvent ev && ev.CardTakerId.Equals(_takerId); ;
            }
            else
            {
                return @event is PlayersGoingOutTrasferModeEvent ev && ev.CardGiverId.Equals(_giverId);
            }
        }
    }
}