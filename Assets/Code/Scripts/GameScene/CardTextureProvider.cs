using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Qos.Domain.StandartPlayingCardModel;

namespace GameScene
{
    internal interface IStandartPlayingCardTexturesProvider
    {
        Texture GetSuitDecal(Suits suit);
        Texture GetValueDecal(Values value);
    }


    internal class StandartPlayingCardTexturesProvider : IStandartPlayingCardTexturesProvider
    {
        private readonly string _pathToSuitTextures = "3dModels/card/SuitDecals";
        private readonly string _pathToValueTextures = "3dModels/card/ValueDecals";


        private Dictionary<Suits, string> _suitTextureNames = new Dictionary<Suits, string>()
        {
            { Suits.HEARTS,     "decal_suit_hearts" },
            { Suits.SPADES,     "decal_suit_spades" },
            { Suits.DIAMONDS,   "decal_suit_diamonds" },
            { Suits.CLUBS,      "decal_suit_clubs" },
        };


        private Dictionary<Values, string> _valueTextureNames = new Dictionary<Values, string>()
        {
            { Values.TWO,   "decal_value_2" },
            { Values.THREE, "decal_value_3" },
            { Values.FOUR,  "decal_value_4" },
            { Values.FIVE,  "decal_value_5" },
            { Values.SIX,   "decal_value_6" },
            { Values.SEVEN, "decal_value_7" },
            { Values.EIGHT, "decal_value_8" },
            { Values.NINE,  "decal_value_9" },
            { Values.TEN,   "decal_value_10" },
            { Values.JACK,  "decal_value_j" },
            { Values.QUEEN, "decal_value_q" },
            { Values.KING,  "decal_value_k" },
            { Values.ACE,   "decal_value_a" },
        };




        public Texture GetSuitDecal(Suits suit)
        {
            return Resources.Load<Texture>(Path.Combine(_pathToSuitTextures, _suitTextureNames[suit]));
        }


        public Texture GetValueDecal(Values value)
        {
            return Resources.Load<Texture>(Path.Combine(_pathToValueTextures, _valueTextureNames[value]));
        }
    }
}
