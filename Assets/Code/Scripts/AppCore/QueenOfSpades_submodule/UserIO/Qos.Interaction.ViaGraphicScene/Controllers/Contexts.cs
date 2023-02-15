using CommonTools;
using Qos.Domain.Entities;
using Qos.Interaction.ViaGraphicScene.Controllers.CardsRealated;
using Qos.Interaction.ViaGraphicScene.Controllers.PlayersRelated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Qos.Interaction.ViaGraphicScene
{
    public class Contexts
    {
        public TimeContext TimeContext { get; private set; }
        public PlayersInfo PlayersInfo { get; private set; }
        public IReadOnlyDictionary<CardId, CardModel> CardModels { get; private set; }
        public IReadOnlyList<CardId> CardIdOrder;


        public Contexts(PlayersInfo playersInfo, IReadOnlyDictionary<CardId, CardModel> cardModels, out Control control)
        {
            PlayersInfo = playersInfo;
            CardModels = cardModels;

            CardIdOrder = new ReadOnlyCollection<CardId>(Shaffle(CardModels.Keys));

            TimeContext = new TimeContext(out var updateTimeContext);
            control = new Control(updateTimeContext);
        }


        private List<CardId> Shaffle(IEnumerable<CardId> cards)
        {
            var rnd = new Random();
            var result = cards.ToList();

            for (int i = 1; i < result.Count; i++)
            {
                if (rnd.NextDouble() > 0.5f)
                {
                    var tmp = result[0];
                    result[0] = result[i];
                    result[i] = tmp;
                }
            }

            return result;
        }



        public class Control
        {
            private readonly Action _UpdateTimeContext;


            public Control(Action UpdateTimeContext)
            {
                _UpdateTimeContext = UpdateTimeContext;
            }


            public void UpdateTimeContext() => _UpdateTimeContext();
        }


    }
}