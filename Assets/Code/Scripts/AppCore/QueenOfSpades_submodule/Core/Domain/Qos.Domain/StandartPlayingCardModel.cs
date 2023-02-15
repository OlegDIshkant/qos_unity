using Qos.Domain.Entities;


namespace Qos.Domain
{
    /// <summary>
    /// Модель описывающая стандартную игровую карту.
    /// Четыре масти: пики, червы, бубны и трефы.
    /// Значения: от "2" до "туза"
    /// </summary>
    public class StandartPlayingCardModel : CardModel
    {
        public static readonly string DECK_NAME = "standart_playing_cards";

        public enum Suits
        {
            CLUBS, DIAMONDS, HEARTS, SPADES
        }

        public enum Values
        {
            TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN,
            JACK, QUEEN, KING, ACE
        }


        public static string ToCardName(Suits suit, Values value) => $"{suit}_{value}";


        public Suits Suit { get; private set; }
        public Values Value { get; private set; }


        public StandartPlayingCardModel(Suits suit, Values value) : 
            base(DECK_NAME, ToCardName(suit, value))
        {
            Suit = suit;
            Value = value;
        }
    }


}
