using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace card_shuffler
{
    /* @brief This keeps a track of each tableau. Because of the more complicated
     *        nature of tableaus than the stock, waste, or foundations (which 
     *        usually only allow onec ard movement and does not have a hidden 
     *        element), this class handles all characteristics and functions.
     */
    class Tableau
    {
        /* @brief List of cards that are in this tableau. */
        private List<Card> cards;

        /* @brief Accessor to get Cards. */
        public List<Card> Cards
        {
            get { return cards; }
        }

        /* @brief How many cards deep are still hidden. */
        private int hiddenDepth;

        /* @brief Accessor to get hidden depth. */
        public int HiddenDepth
        {
            get { return hiddenDepth; }
        }

        /* @brief Default constructor */
        public Tableau()
        {
            cards = new List<Card>();
            cards.Capacity = 52;
            hiddenDepth = 0;
        }

        /* @brief Constructor that specifies this tableau's hidden depth (this 
         *        can't increase so... ) */
        public Tableau(int HiddenDepth)
        {
            cards = new List<Card>();
            cards.Capacity = 52;
            this.hiddenDepth = HiddenDepth;
        }

        /* @brief Inserts a card into the tableau 
         * @param NewCard- The card to be inserted into this tableau*/
        public void InsertCard(Card NewCard)
        {
            cards.Insert(cards.Count, NewCard);
        }

        /* @brief Generates a list of cards to move. 
         * @param   Depth - How many cards to take out of this tableau
         * @return  A copy of the list of cards to move. */
        public List<Card> GetCards(int Depth)
        {
            List<Card> tempCards = new List<Card>();

            if (Depth <= cards.Count)
            {
                for (int i = cards.Count; i > (cards.Count - Depth); i--)
                {
                    tempCards.Insert(0, cards[i - 1]);
                }
            }

            return tempCards;
        }

        /* @brief Removes cards from the tableau.
         * @note  If the top most card is hidden, reveal it.*
         * @return Boolean if card was revealed or not.*/
        public bool RemoveCards(int Depth)
        {
            bool cardRevealed = false;
            for (int i = 0; i < Depth; i++)
            {
                cards.RemoveAt(cards.Count - 1);

                if (cards.Count == hiddenDepth)
                {
                    hiddenDepth--;
                    cardRevealed = true;
                }
            }

            return cardRevealed;
        }
    }

    /* @brief The class that handles all Klondike solitaire based functions. */
    class SolitaireEnigne
    {
        /* @brief This game's deck of cards. */
        private DeckOfCards deck;

        /* @brief An array of Tableau objects to hold tableaus. */
        private Tableau[] tableaus;

        /* @brief A list of list<cards> to hold the foundations. */
        private List<List<Card>> foundations;

        /* @brief A list of cards to hold the stock pile. */
        private List<Card> stock;

        /* @brief A list of cards to hold the waste pile. */
        private List<Card> waste;

        /* @brief How many cards are visible in the waste pile. */
        private int wasteVisible;

        /* @brief Points in this game (TBI) */
        private int points;

        /* @brief How many cards the player can draw from the stock pile. */
        private int stockDraw;

        /* @brief Is the game cleared? */
        private bool gameCleared;

        /* @brief Accessor to gameCleared */
        public bool GameCleared
        {
            get { return gameCleared; }
        }

        /* @brief Accessor to points */
        public int Points
        {
            get { return points; }
        }

        /* @brief Default constructor. */
        public SolitaireEnigne()
        {
            tableaus = new Tableau[7];
            foundations = new List<List<Card>>();
            stock = new List<Card>();
            waste = new List<Card>();
            waste.Capacity = 28;

            foundations.Add(new List<Card>());
            foundations.Add(new List<Card>());
            foundations.Add(new List<Card>());
            foundations.Add(new List<Card>());

            deck = new DeckOfCards(true);
            wasteVisible = 0;
            points = 0;
            gameCleared = false;
            stockDraw = 1;
            NewGame();
        }

        /* @brief Constructor to set the draw from stock count. */
        public SolitaireEnigne(int DrawStockCount)
        {
            tableaus = new Tableau[7];
            foundations = new List<List<Card>>();
            stock = new List<Card>();
            waste = new List<Card>();
            waste.Capacity = 28;

            foundations.Add(new List<Card>());
            foundations.Add(new List<Card>());
            foundations.Add(new List<Card>());
            foundations.Add(new List<Card>());

            deck = new DeckOfCards(true);
            wasteVisible = 0;
            points = 0;
            gameCleared = true;
            stockDraw = DrawStockCount;
            NewGame();
        }

        /* @brief Puts cards from the stock pile to the waste pile. */
        public void DrawFromStock()
        {
            if (stock.Count == 0)
            {
                while (waste.Count > 0)
                {
                    stock.Add(waste[waste.Count - 1]);
                    waste.RemoveAt(waste.Count - 1);
                }

                if (stockDraw == 1)
                {
                    points -= 100;
                }
            }
            else
            {
                for (int i = 0; (i < stockDraw) && (stock.Count > 0); i++)
                {
                    if (wasteVisible < 3)
                    {
                        wasteVisible++;
                    }
                    waste.Insert(0, stock[0]);
                    stock.RemoveAt(0);
                }
            }
        }

        /* @brief   Prints the game out. 
         * @return  A string prinout of the game. */
        public string PrintGame()
        {
            string printout = "";
            printout += PrintStock() + Environment.NewLine;
            printout += "Waste: " + PrintWaste() + Environment.NewLine;

            for (int i = 0; i < 7; i++)
            {
                printout += PrintTableau(i) + Environment.NewLine;
            }

            printout += PrintFoundations();
            return printout;
        }

        public string PrintGameFancy()
        {
            string output = "";
            int mostTableauCount = 0;

            output += string.Format(
                "(S)tock - {0,2}                                       (F)oundations" + Environment.NewLine, 
                stock.Count);
            output += string.Format(
                "(W)aste - {0}                     [1]     [2]     [3]     [4]" + Environment.NewLine, 
                PrintWaste()
                );
            output += "                                            ";

            for (int foundation = 0; foundation < 4; foundation++)
            {
                if (foundations[foundation].Count > 0)
                {
                    output += foundations[foundation][0] + "     ";
                }
                else
                {
                    output += "Emp     ";
                }
            }

            output += Environment.NewLine;
            output += "Tableaus" + Environment.NewLine;
            output += "    [1]       [2]       [3]       [4]       [5]       [6]       [7]" + Environment.NewLine;

            /* Look for the tableau with the most cards in it. */
            foreach (Tableau tableau in tableaus)
            {
                if (tableau.Cards.Count > mostTableauCount)
                {
                    mostTableauCount = tableau.Cards.Count;
                }
            }

            for (int i = 0; i < mostTableauCount; i++)
            {
                output += "    ";
                foreach (Tableau tableau in tableaus)
                {
                    if (tableau.Cards.Count > i)
                    {
                        if (tableau.HiddenDepth > i)
                        {
                            output += " XX       ";
                        }
                        else
                        {
                            output += tableau.Cards[i].ToString() + "       ";
                        }
                    }
                    else
                    {
                        output += "          ";
                    }
                }
                output += Environment.NewLine;
            }

            return output;
        }

        /* @brief   Moves cards from one tableau to another. 
         * @param   SrcTableauIdx - Index of the source tableau.
         * @param   Depth - How many cards to move in the source tableau.
         * @param   DestTableauIdx - Index of the destination tableau. 
         * @return  Boolean indicating if the move was valid or invalid. */
        public bool MoveCards(int SrcTableauIdx, int Depth, int DestTableauIdx)
        {
            bool validMove = false;
            Tableau SrcTableau = tableaus[SrcTableauIdx];
            Tableau DestTableau = tableaus[DestTableauIdx];
            Card srcCard = SrcTableau.Cards[( SrcTableau.Cards.Count - 1) - (Depth-1)];

            /* Check if we can even move from the source */
            if ((SrcTableau.HiddenDepth + Depth) <= (SrcTableau.Cards.Count))
            {
                /* Check if destination tableau is empty, only a king can go in there */
                if (DestTableau.Cards.Count == 0 &&
                            srcCard.Value == 13)
                {
                    MoveBetweenTableaus(SrcTableau, Depth, DestTableau);
                    validMove = true;
                }
                /* Check if different suit color and the value is 1 lower. */
                else
                {
                    Card destCard = DestTableau.Cards[DestTableau.Cards.Count - 1];
                    if (CheckSuit(srcCard, destCard, true) &&
                            CheckCardValue(srcCard, destCard, true))
                    {
                        MoveBetweenTableaus(SrcTableau, Depth, DestTableau);
                        validMove = true;
                    }
                }
            }

            return validMove;
        }

        /* @brief   Moves all stacked cards from one tableau to another.
         * @param   SrcTableauIdx - Index of the source tableau.
         * @param   DestTableauIdx - Index of the destination tableau. 
         * @return  Boolean indicating if the move was valid or invalid. */
        public bool MoveWholeTableau(int SrcTableauIdx, int DestTableauIdx)
        {
            bool validMove = false;
            Tableau SrcTableau = tableaus[SrcTableauIdx];
            Tableau DestTableau = tableaus[DestTableauIdx];
            Card srcCard;
            int depth = 0;

            if (SrcTableau.HiddenDepth == -1)
            {
                srcCard = SrcTableau.Cards[0];
                depth = SrcTableau.Cards.Count;
            }
            else
            {
                srcCard = SrcTableau.Cards[SrcTableau.HiddenDepth];
                depth = SrcTableau.Cards.Count - SrcTableau.HiddenDepth;
            }
            

            /* Check if destination tableau is empty, only a king can go in there */
            if (DestTableau.Cards.Count == 0 &&
                        srcCard.Value == 13)
            {
                MoveBetweenTableaus(SrcTableau, depth, DestTableau);
                validMove = true;
            }
            /* Check if different suit color and the value is 1 lower. */
            else
            {
                Card destCard = DestTableau.Cards[DestTableau.Cards.Count - 1];
                if (CheckSuit(srcCard, destCard, true) &&
                        CheckCardValue(srcCard, destCard, true))
                {
                    MoveBetweenTableaus(SrcTableau, depth, DestTableau);
                    validMove = true;
                }

            }

            return validMove;
        }

        /* @brief   Moves a card from a tableau to a foundation. 
         * @param   SrcTableauIdx - Index of the source tableau.
         * @param   DestFoundationIdx - Foundation to put the card into.
         * @return  Boolean indicating if the move was valid or invalid. */
        public bool MoveToFoundation(int SrcTableauIdx, int DestFoundationIdx)
        {
            bool validMove = false;
            Tableau SrcTableau = tableaus[SrcTableauIdx];
            Card srcCard = SrcTableau.Cards[SrcTableau.Cards.Count - 1];

            /* If this foundation is empty, check if the source card is an ace.*/
            if (foundations[DestFoundationIdx].Count == 0 && srcCard.Value == 1)
            {
                foundations[DestFoundationIdx].Add(srcCard);
                validMove = true;
            }
            /* If the foundation is not empty, check if same suit and value is one higher*/
            else
            {
                Card destCard = foundations[DestFoundationIdx][0];
                if (CheckSuit(srcCard, destCard, false) &&
                        CheckCardValue(srcCard, destCard, false))
                {
                    AddToFoundation(srcCard, DestFoundationIdx);
                    validMove = true;
                }
            }

            if (validMove)
            {
                if (SrcTableau.RemoveCards(1))
                {
                    points += 5;
                }
                points += 10;
            }
            return validMove;
        }

        /* @brief Moves a card from the waste pile to a tableau
         * @param DestTableauIdx - Index to destination tableau.
         * @return  Boolean indicating if the move was valid or invalid. */
        public bool MoveFromWaste(int DestTableauIdx)
        {
            bool validMove = false;
            Tableau DestTableau = tableaus[DestTableauIdx];
            Card srcCard = waste[0];
            
            /* Check if we can even move from the source */
            if (waste.Count > 0 && wasteVisible > 0)
            {
                if (DestTableau.Cards.Count == 0 &&
                             srcCard.Value == 13)
                {
                    validMove = true;
                }
                /* Check if different suit color and the value is 1 lower. */
                else
                {
                    Card destCard = DestTableau.Cards[DestTableau.Cards.Count - 1];
                    if (CheckSuit(srcCard, destCard, true) &&
                                 CheckCardValue(srcCard, destCard, true))
                    {
                        validMove = true;
                    }
                }
            }

            if (validMove)
            {
                points += 5;
                DestTableau.InsertCard(waste[0]);
                waste.RemoveAt(0);

                if (wasteVisible > 0)
                {
                    wasteVisible--;
                }
            }
            return validMove;
        }

        /* @brief Moves a card from the waste pile to a foundation
         * @param DestFoundationIdx - Index to destination foundation.
         * @return  Boolean indicating if the move was valid or invalid. */
        public bool MoveFromWasteToFoundation(int DestFoundationIdx)
        {
            bool validMove = false;
            Card srcCard = waste[0];

            /* If this foundation is empty, check if the source card is an ace.*/
            if (foundations[DestFoundationIdx].Count == 0)
            {
                /* Check if this is an ace */
                if (srcCard.Value == 1)
                {
                    foundations[DestFoundationIdx].Add(srcCard);
                    validMove = true;
                }
            }
            /* If the foundation is not empty, check if same suit and value is one higher*/
            else
            {
                Card destCard = foundations[DestFoundationIdx][0];
                if (CheckSuit(srcCard, destCard, false) &&
                        CheckCardValue(srcCard, destCard, false))
                {
                    AddToFoundation(srcCard, DestFoundationIdx);
                    validMove = true;
                }
            }

            if(validMove)
            {
                points += 10;
                waste.RemoveAt(0);
                if (wasteVisible > 0)
                {
                    wasteVisible--;
                }
            }
            return validMove;
        }

        /* @brief Moves a card from the foundation to a tableau
         * @param SrcFoundationIdx - Index of source foundation to move a card from.
         * @param DestTableauIdx - Index to destination tableau.
         * @return  Boolean indicating if the move was valid or invalid. */
        public bool MoveFromFoundation(int SrcFoundationIdx, int DestTableauIdx)
        {
            bool validMove = false;
            Tableau DestTableau = tableaus[DestTableauIdx];

            /* Check if we can even move from the source */
            if (foundations[SrcFoundationIdx].Count > 0)
            {
                Card srcCard = foundations[SrcFoundationIdx][0];

                /* Check if destination tableau is empty, only a king can go in there */
                if (DestTableau.Cards.Count == 0 &&
                        srcCard.Value == 13)
                {
                    DestTableau.InsertCard(srcCard);
                    validMove = true;
                }
                /* Check if different suit color and the value is 1 lower. */
                else
                {
                    Card destCard = DestTableau.Cards[DestTableau.Cards.Count - 1];
                    if (CheckSuit(srcCard, destCard, true) &&
                            CheckCardValue(srcCard, destCard, true))
                    {
                        DestTableau.InsertCard(srcCard);
                        validMove = true;
                    }
                }
            }

            if (validMove)
            {
                points -= 15;
                foundations[SrcFoundationIdx].RemoveAt(0);
            }
            return validMove;
        }

        /* @brief Performs the move of one or more cards from one tableau to another.
         * @param SrcTableau - Tableau where the cards are moving from.
         * @param Depth - How many cards are being moved.
         * @param DestTableau - Tableau where the cards are moving to.
         * */
        private void MoveBetweenTableaus(Tableau SrcTableau, int Depth, Tableau DestTableau)
        {
            List<Card> cardsToMove = new List<Card>();
            cardsToMove = SrcTableau.GetCards(Depth);

            foreach (Card card in cardsToMove)
            {
                DestTableau.InsertCard(card);
            }

            if (SrcTableau.RemoveCards(Depth))
            {
                points += 5;
            }
        }

        private void AddToFoundation(Card SrcCard, int FoundationIdx)
        {
            int fullFoundations = 0;
            foundations[FoundationIdx].Insert(0, SrcCard);

            foreach (List<Card> foundation in foundations)
            {
                if (foundation.Count == 13)
                {
                    fullFoundations++;
                }
            }

            if (fullFoundations >= 4)
            {
                gameCleared = true;
            }
        }

        /* @brief   Checks the suit between two cards.
         * @param   SrcCard - Source card
         * @param   DestCard - Destination card
         * @param   Stacking - If this is stacking onto a tableau or not.
         * @return  Boolean that returns if a match was found or not. */
        private bool CheckSuit(Card SrcCard, Card DestCard, bool Stacking)
        {
            bool match = false;

            /* If this is moving between tableaus, make sure the suits are alternating color. */
            if (Stacking)
            {
                if (SrcCard.Suit == CardSuit.HEART && (DestCard.Suit == CardSuit.SPADE || DestCard.Suit == CardSuit.CLUB) ||
                        SrcCard.Suit == CardSuit.DIAMOND && (DestCard.Suit == CardSuit.SPADE || DestCard.Suit == CardSuit.CLUB) ||
                        SrcCard.Suit == CardSuit.CLUB && (DestCard.Suit == CardSuit.HEART || DestCard.Suit == CardSuit.DIAMOND) ||
                        SrcCard.Suit == CardSuit.SPADE && (DestCard.Suit == CardSuit.HEART || DestCard.Suit == CardSuit.DIAMOND))
                {
                    match = true;
                }
            }
            else
            {
                match = SrcCard.Suit == DestCard.Suit;
            }

            return match;
        }

        /* @brief   Checks the value between two cards.
         * @param   SrcCard - Source card
         * @param   DestCard - Destination card
         * @param   Stacking - If this is stacking onto a tableau or not.
         * @return  Boolean that returns if a match was found or not. */
        private bool CheckCardValue(Card SrcCard, Card DestCard, bool Stacking)
        {
            /* If this moving between tableaus, make sure the card's value is one less. */
            if (Stacking)
            {
                return SrcCard.Value == DestCard.Value - 1; /* This looks weird, but it works. */
            }
            else
            {
                return SrcCard.Value - 1 == DestCard.Value;
            }
        }

        /* @brief   Prints a tableau
         * @param   Tableau - Index to the tableau in question
         * @return  String printout of the tableau */
        private string PrintTableau(int Tableau)
        {
            string printout = string.Format("Tableau {0}: ", Tableau + 1 );

            for (int i = tableaus[Tableau].Cards.Count - 1; i >= 0; i--)
            {
                if (tableaus[Tableau].HiddenDepth > i)
                {
                    printout += " XX ";
                }
                else
                {
                    printout += tableaus[Tableau].Cards[i].ToString() + " ";
                }
            }

            return printout;
        }

        /* @brief   Prints the foundations
         * @return  String printout of the foundations */
        private string PrintFoundations()
        {
            string printout = "";

            for (int foundation = 0; foundation < 4; foundation++)
            {
                if (foundations[foundation].Count > 0)
                {
                    printout += string.Format("Foundation {0}: {1}" + Environment.NewLine, foundation + 1, foundations[foundation][0]);
                }
                else
                {
                    printout += string.Format("Foundation {0}: Empty" + Environment.NewLine, foundation + 1);
                }
            }

            return printout;
        }

        /* @brief   Prints the waste pile
         * @return  String printout of the waste pile */
        private string PrintWaste()
        {
            string prinout = "";
            
            for (int i = 0; (i < wasteVisible) && (i < waste.Count); i++)
            {
                prinout += string.Format("{0}, ", waste[i]);
            }

            if (prinout.Length > 7)
            {
                prinout = prinout.Substring(0, prinout.Length - 2);
            }

            while (prinout.Length < 13)
            {
                prinout += " ";
            }
            return prinout;
        }

        /* @brief   Prints the stock pile
         * @return  String printout of the stock pile */
        private string PrintStock()
        {
            return string.Format("Stock Count: {0}", stock.Count);
        }

        /* @brief   Starts a new game. */
        private void NewGame()
        {
            int cardCount = 0;

            for (int tableau = 0; tableau < 7; tableau++)
            {
                tableaus[tableau] = new Tableau(tableau);
                for (int tableauCard = 0; tableauCard < tableau + 1; tableauCard++)
                {
                    tableaus[tableau].InsertCard(deck.Deck[cardCount++]);
                }
            }

            for (; cardCount < deck.Deck.Length; cardCount++)
            {
                stock.Add(deck.Deck[cardCount]);
            }
        }
    }
}
