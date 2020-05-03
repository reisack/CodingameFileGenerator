using System;       
 using   System.Collections.Generic; 
 // using toto
  //using tata
// using   titi
    //using tutu
// using System.Toto;  
 // enum toto
using System.IO;  

public class Opponent /* 
hello */
{
    static private Dictionary<int, Position> _supposedOponnentPosition = new Dictionary<int, Position>
    {
        {
            1,
            new Position { X = 2, Y = 2 }
        },
        {
            2,
            new Position { X = 7, Y = 2 }
        },
        {
            3,
            new Position { X = 12, Y = 2 }
        },
        {
            4,
            new Position { X = 2, Y = 7 }
        },
        {
            5,
            new Position { X = 7, Y = 7 }
        },
        {
            6,
            new Position { X = 12, Y = 7 }
        },
        {
            7,
            new Position { X = 2, Y = 12 }
        },
        {
            8,
            new Position { X = 7, Y = 12 }
        },
        {
            9,
            new Position { X = 12, Y = 12 }
        }
    };

    public Position Position { get; set; }
    public int Life { get; set; }
    public int PreviousTurnLife { get; set; }
    public bool HasSurfacedOrShoot { get; set; }
    public int NbSilences { get; set; }
    public Direction PreviousDirection { get; set; }

    public bool IsSpottedWithSonar { get; set; }

    private int _changeSupposedOponnentPositionIndex = 0;
    private Position[] _changeSupposedOponnentPosition = new Position[5]
    {
            // Up
            new Position { X = 0, Y = -1 },
            // Left
            new Position { X = -1, Y = 1 },
            // Down
            new Position { X = 1, Y = 1 },
            // Right
            new Position { X = 1, Y = -1 },
            // Back to center
            new Position { X = -1, Y = 0 }
    };

    public Opponent()
    {
        Position = new Position { X = 7, Y = 7 };
        Life = -1;
        PreviousTurnLife = -1;
        HasSurfacedOrShoot = false;
        NbSilences = 0;
        PreviousDirection = Direction.NoDirection;
        IsSpottedWithSonar = false;
    }

    public void Touched()
    {
        NbSilences = 0;
        HasSurfacedOrShoot = true;
        _changeSupposedOponnentPositionIndex = 0;
    }

    public void NotTouched()
    {
        int quitLoopWhenZero = _changeSupposedOponnentPosition.Length;

        do
        {
            quitLoopWhenZero--;

            Position toChange = _changeSupposedOponnentPosition[_changeSupposedOponnentPositionIndex];
            Position.X += toChange.X;
            Position.Y += toChange.Y;
            _changeSupposedOponnentPositionIndex++;

            Console.Error.WriteLine("_changeSupposedOponnentPosition");
            Console.Error.WriteLine("Position.X : " + Position.X);
            Console.Error.WriteLine("Position.Y : " + Position.Y);

            if (_changeSupposedOponnentPositionIndex == 4)
            {
                _changeSupposedOponnentPositionIndex = 0;
            }

        } while (quitLoopWhenZero >= 0 && (Position.X < 0 || Position.X > MapInfo.Width - 1
                || Position.Y < 0 || Position.Y > MapInfo.Height - 1));
    }

    public void UpdatePositionBySector(int sector)
    {
        Position = _supposedOponnentPosition[sector];
    }

    public void AnalyseOrders(string[] splittedOpponentOrders, Player player, bool hasCheated)
    {
        // Prendre la partie avec "MOVE"
        foreach (string order in splittedOpponentOrders)
        {
            if (order.Contains("MOVE"))
            {
                HandleMoveOrder(order, player);
            }

            if (order.Contains("SURFACE") && !player.FollowOpponent)
            {
                HandleSurfaceOrder(order);
            }

            if (Game.CHECK_OPPONENT_TORPEDO_ORDER
                && (
                    (!player.FollowOpponent && !IsSpottedWithSonar)
                    || NbSilences > Game.NB_SILENCE_TOLERANCE
                   )
                && (order.StartsWith("TORPEDO") || order.Contains("|TORPEDO")))
            {
                HandleTorpedoOrder(order, player);
            }
             
            if (!hasCheated && order.Contains("SILENCE"))
            {
                HandleSilenceOrder();
            }
        }
    }

    private void HandleMoveOrder(string order, Player player)
    {
        char opponentMove = order[5];

        Console.Error.WriteLine("Before Move Order");
        Console.Error.WriteLine("Move Order : " + order);
        Console.Error.WriteLine("Position.X : " + Position.X);
        Console.Error.WriteLine("Position.Y : " + Position.Y);

        Direction opponentDirection = Direction.NoDirection;

        switch (opponentMove)
        {
            case 'N':
                if (Position.Y > 0) Position.Y--;
                opponentDirection = Direction.Up;
                break;
            case 'S':
                if (Position.Y < MapInfo.Height - 1) Position.Y++;
                opponentDirection = Direction.Down;
                break;
            case 'E':
                if (Position.X < MapInfo.Width - 1) Position.X++;
                opponentDirection = Direction.Right;
                break;
            case 'W':
                if (Position.X > 0) Position.X--;
                opponentDirection = Direction.Left;
                break;
        }

        PreviousDirection = opponentDirection;

        if (player.FollowOpponent)
        {
            player.OpponentDirection = opponentDirection;
        }

        Console.Error.WriteLine("After Move Order");
        Console.Error.WriteLine("Position.X : " + Position.X);
        Console.Error.WriteLine("Position.Y : " + Position.Y);
    }

    private void HandleSurfaceOrder(string order)
    {
        NbSilences = 0;
        HasSurfacedOrShoot = true;
        int opponentSector = int.Parse(order[8].ToString());
        Console.Error.WriteLine("opponentSector : " + opponentSector);
        Position supposedOpponentPosition = _supposedOponnentPosition[opponentSector];
        Position.X = supposedOpponentPosition.X;
        Position.Y = supposedOpponentPosition.Y;
        Console.Error.WriteLine("Position.X : " + Position.X);
        Console.Error.WriteLine("Position.Y : " + Position.Y);
    }

    private void HandleTorpedoOrder(string order, Player player)
    {
        Console.Error.WriteLine("order : " + order);

        string[] torpedoCoordinates = order.Split(' ');

        int torpedoX = int.Parse(torpedoCoordinates[1].ToString());
        int torpedoY = int.Parse(torpedoCoordinates[2].ToString());

        bool checkLeft = player.Position.X > MapInfo.Width / 2;
        bool checkUp = player.Position.Y > MapInfo.Height / 2;
        Position.X = (checkLeft) ? torpedoX - 2 : torpedoX + 2;
        Position.Y = (checkUp) ? torpedoY - 2 : torpedoY + 2;

        NbSilences = 0;
        HasSurfacedOrShoot = true;

        Console.Error.WriteLine("Position.X : " + Position.X);
        Console.Error.WriteLine("Position.Y : " + Position.Y);
    }

    private void HandleSilenceOrder()
    {
        if (NbSilences > Game.NB_SILENCE_TOLERANCE - 1)
        {
            HasSurfacedOrShoot = false;
            IsSpottedWithSonar = false;
            NbSilences = 0;
        }

        NbSilences++;

        // We suppose that _opponent has moved in the same direction as previous
        if (PreviousDirection != Direction.NoDirection)
        {
            switch (PreviousDirection)
            {
                case Direction.Up:
                    if (Position.Y > 0) Position.Y--;
                    break;
                case Direction.Down:
                    if (Position.Y < MapInfo.Height - 1) Position.Y++;
                    break;
                case Direction.Right:
                    if (Position.X < MapInfo.Width - 1) Position.X++;
                    break;
                case Direction.Left:
                    if (Position.X > 0) Position.X--;
                    break;
            }
        }
    }
}
