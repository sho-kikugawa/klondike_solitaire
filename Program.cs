using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace card_shuffler
{
  class Program
  {
    static private SolitaireEnigne game;
    static private bool inGame = false;
    static void Main(string[] args)
    {
      string inString = "";
      game = new SolitaireEnigne();

      while (!inGame) 
      {
        Console.WriteLine("Welcome to Klondike!" + Environment.NewLine);
        try
        {
          CommandInterpreter("N");
          inGame = true;
        }
        catch
        {
          Console.WriteLine("Incorrect input, must be 1 or 3");
          Console.Write("(N)ew game or (Q)uit?: ");
          string input = Console.ReadLine();
          input = input.Substring(0).ToUpper();

          if (input != "N")
          {
            break;
          }
        }
        Console.SetCursorPosition(0, 0);
        Console.Clear();
      }
      
      while (inGame)
      {
        Console.WriteLine("Welcome to Klondike!" + Environment.NewLine);
        /*Console.WriteLine(game.PrintGame());*/
        Console.WriteLine(game.PrintGameFancy());
        Console.Write("Enter Command (H for help): ");
        inString = Console.ReadLine();
        inString = inString.Substring(0).ToUpper();

        try
        {
          CommandInterpreter(inString);
        }
        catch
        {
          /* Silently catche errors. */
        }
        Console.SetCursorPosition(0, 0);
        Console.Clear();
      }

      Console.SetCursorPosition(0, 16);
    }

    static public void CommandInterpreter(string Input)
    {
      bool validMove = false;
      int foundationIdx = 0;
      int srcTableauIdx = 0;
      int depth = 0;
      int destTableauIdx = 0;
      string input = "";

      switch (Input)
      {
        case "H":
          Console.SetCursorPosition(0, 0);
          Console.Clear();
          Console.WriteLine(
            "RULES FOR KLONDIKE SOLITAIRE: " + Environment.NewLine + Environment.NewLine +
            "The goal of klondike solitaire is to get rid of all the cards from the" + Environment.NewLine +
            "tableaus and stock by placing them in order on the foundations by suit." + Environment.NewLine + Environment.NewLine +
            "You can stack cards onto the tableau in descending value, but the suits must" + Environment.NewLine + 
            "alternate. e.g., A heart (♥) or diamond (♦) can be placed on a club (♣) or" + Environment.NewLine +
            "spade(♠) and vice versa. An ace of hearts, for example, can be placed on a" + Environment.NewLine +
            "two of clubs, but not the other way around, or on a two of hearts or diamonds." + Environment.NewLine + Environment.NewLine);

          Console.WriteLine(
            "COMMANDS:" + Environment.NewLine + Environment.NewLine +
            "(H)elp: Displays this message" + Environment.NewLine +
            "(Q)uit: Quit the game." + Environment.NewLine +
            "1-7: Move cards from a tableau to another tableau or foundation." + Environment.NewLine +
            "(F)oundation: Removes the top card from the foundation to a tableau" + Environment.NewLine +
            "(N)New Game: Starts a new game." + Environment.NewLine +
            "(S)tock: Draws cards from the stock pile." + Environment.NewLine +
            "(W)aste: Move the top (lefttmost) card from the waste pile onto a tableau." + Environment.NewLine);
          Console.WriteLine("Press Enter to continue...");
          Console.ReadLine();
          break;

        case "1":
        case "2":
        case "3":
        case "4":
        case "5":
        case "6":
        case "7":
          srcTableauIdx = Int32.Parse(Input);
          Console.WriteLine("Move from tableau {0} to where (tableaus 1-7 or (F)oundation)? ", srcTableauIdx);
          input = Console.ReadLine();
          if (input.ToUpper()[0].Equals('F'))
          {
            Console.WriteLine("Move TO which foundation?");
            foundationIdx = Int32.Parse(Console.ReadLine());
            validMove = game.MoveToFoundation(srcTableauIdx - 1, foundationIdx - 1);
          }
          else
          {
            /*Console.Write("How many cards from the top (leftmost)? ");
            depth = Int32.Parse(Console.ReadLine());
            destTableauIdx = Int32.Parse(input);
            game.MoveCards(srcTableauIdx - 1, depth, destTableauIdx - 1);*/
            destTableauIdx = Int32.Parse(input);
            game.MoveWholeTableau(srcTableauIdx - 1, destTableauIdx - 1);
          }
          break;

        case "F":
          Console.WriteLine("Remove from which foundation?");
          foundationIdx = Int32.Parse(Console.ReadLine());
          Console.WriteLine("Move to which tableau?");
          destTableauIdx = Int32.Parse(Console.ReadLine());
          game.MoveFromFoundation(foundationIdx, destTableauIdx);
          break;

        case "N":
          int stockDrawCount = 0;
          game = new SolitaireEnigne();
          Console.Write("Enter how many cards you'd like to draw from stock (1 or 3): ");
          stockDrawCount = Int32.Parse(Console.ReadLine());

          if (stockDrawCount == 1 || stockDrawCount == 3)
          {
            game = new SolitaireEnigne(stockDrawCount);
          }
          else
          {
            throw new Exception("Did not enter 1 or 3 when starting a new game");
          }
          break;
          
        case "Q":
          inGame = false;
          break;

        case "S":
          game.DrawFromStock();
          break;

        case "W":
          Console.WriteLine("Move top (leftmost) waste card to where (tableaus 1-7 or (F)oundation)? ");
          input = Console.ReadLine();

          if (input.ToUpper()[0].Equals('F'))
          {
            Console.WriteLine("Which foundation? ");
            foundationIdx = Int32.Parse(Console.ReadLine());
            validMove = game.MoveFromWasteToFoundation(foundationIdx - 1);
          }
          else
          {
            destTableauIdx = Int32.Parse(input);
            game.MoveFromWaste(destTableauIdx - 1);
          }
          
          break;

        case "X":
          break;
      }
    }
  }
}
