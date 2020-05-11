using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum CardSuit
{
    HEART,
    DIAMOND,
    CLUB,
    SPADE
}

class Card
{
    protected CardSuit suit;
    protected int value;

    public CardSuit Suit
    {
        get { return this.suit; }
    }

    public int Value
    {
        get { return this.value; }
    }

    public Card()
    {
        this.suit = CardSuit.HEART;
        this.value = 1;
    }

    public Card(CardSuit Suit, int Value)
    {
        this.suit = Suit;
        this.value = Value;
    }

    public override string ToString()
    {
        return PrintValue() + PrintSuit();
    }

    public string ToStringFancy()
    {
        string cardImage = "";

        cardImage = "╔═══╗" + Environment.NewLine;

        if (value == 10)
        {
            cardImage += string.Format("║{0} ║", PrintValue());
        }
        else
        {
            cardImage += string.Format("║{0}  ║", PrintValue());
        }

        cardImage += string.Format(Environment.NewLine + "║ {0} ║" + Environment.NewLine, PrintSuit());

        if (value == 10)
        {
            cardImage += string.Format("║ {0}║", PrintValue());
        }
        else
        {
            cardImage += string.Format("║  {0}║", PrintValue());
        }

        cardImage += Environment.NewLine + "╚═══╝";
        return cardImage;
    }

    protected string PrintSuit()
    {
        switch (suit)
        {
            case CardSuit.HEART:
                return "♥";
            case CardSuit.DIAMOND:
                return "♦";
            case CardSuit.CLUB:
                return "♣";
            case CardSuit.SPADE:
                return "♠";
        }

        return "";
    }

    protected string PrintValue()
    {
        switch(value)
        {
            case 1:
                return " A";
            case 11:
                return " J";
            case 12:
                return " Q";
            case 13:
                return " K";
            default:
                return string.Format("{0,2}", value);
        }
    }
}

class DeckOfCards
{
    private Random rand = new Random();
    private Card[] deck;

    public Card[] Deck
    {
        get { return deck; }
    }

    public DeckOfCards(bool Shuffled)
    {
        this.deck = new Card[52];

        for (int i = 0; i < 52; i++)
        {
            switch (i / 13)
            {
                case 0:
                    deck[i] = new Card(CardSuit.HEART, ((i % 13) + 1));
                    break;

                case 1:
                    deck[i] = new Card(CardSuit.DIAMOND, ((i % 13) + 1));
                    break;

                case 2:
                    deck[i] = new Card(CardSuit.CLUB, ((i % 13) + 1));
                    break;

                case 3:
                    deck[i] = new Card(CardSuit.SPADE, ((i % 13) + 1));
                    break;
            }
        }

        if (Shuffled)
        {
            ShuffleDeck();
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < 2000000; i++)
        {
            Shuffle();
        }
    }

    private void Shuffle()
    {
        int first = rand.Next(0, 52);
        int second = rand.Next(0, 52);
        Card temp;

        temp = deck[first];
        deck[first] = deck[second];
        deck[second] = temp;
    }
}