          using                    System;        
using     System.Collections.Generic; // HAHA !

// PROUT
public class Player
{
    private Dictionary<Direction, char> _charMovement = new Dictionary<Direction, char>
    {
        { Direction.Left, 'W' },
        { Direction.Right, 'E' },
        { Direction.Up, 'N' },
        { Direction.Down, 'S' }
    };

    private Direction _favorite;

    public Position Position { get; set; }
    public bool FollowOpponent { get; set; }
    public Direction OpponentDirection { get; set; }
    public bool NoMoveThisTurn { get; set; }
    public Direction PreviousDirection { get; private set; }
    public Direction CurrentDirection { get; private set; }
    public int Life { get; set; }
    public int PreviousTurnLife { get; set; }
    public bool HasShoot { get; set; }

    public Player(int x, int y)
    {
        Position = new Position { X = x, Y = y };
        PreviousDirection = Direction.NoDirection;
        CurrentDirection = Direction.NoDirection;
        FollowOpponent = false;
        OpponentDirection = Direction.NoDirection;
        _favorite = Direction.NoDirection;
        NoMoveThisTurn = false;
        Life = -1;
        PreviousTurnLife = -1;
        HasShoot = false;
    }

    public void ResetPreviousDirection()
    {
        PreviousDirection = Direction.NoDirection;
    }
    
    public string GetFormattedPosition()
    {
        return Position.X.ToString() + " " + Position.Y.ToString();
    }

    public void ResetFavoriteDirection()
    {
        _favorite = Direction.NoDirection;
    }

    public bool Touched()
    {
        return PreviousTurnLife > -1 && PreviousTurnLife > Life;
    }

    public char DropMineDirection()
    {
        char direction = 'A';

        switch (PreviousDirection)
        {
            case Direction.Left:
                direction = 'E';
                break;
            case Direction.Right:
                direction = 'W';
                break;
            case Direction.Up:
                direction = 'S';
                break;
            case Direction.Down:
                direction = 'N';
                break;
        }

        return direction;
    }

    public char ChooseDirection()
    {
        // We go to the _opponent direction, if possible
        char direction = TryToFollowOpponent();
        if (direction != 'A')
        {
            return direction;
        }

        // The direction that should be checked first
        direction = TryFavoriteDirection();
        if (direction != 'A')
        {
            return direction;
        }

        direction = GetBestDirection();
        if (direction != 'A')
        {
            return direction;
        }

        return RandomGoOrSurface();
    }

    private void Go(Direction direction)
    {
        PreviousDirection = direction;

        if (!NoMoveThisTurn)
        {
            switch (direction)
            {
                case Direction.Left:
                    Position.X--;
                    CurrentDirection = Direction.Left;
                    break;
                case Direction.Right:
                    Position.X++;
                    CurrentDirection = Direction.Right;
                    break;
                case Direction.Up:
                    Position.Y--;
                    CurrentDirection = Direction.Up;
                    break;
                case Direction.Down:
                    Position.Y++;
                    CurrentDirection = Direction.Down;
                    break;
            }
        }
    }

    private bool CanGo(Direction direction)
    {
        bool condition = false;
        int newX = Position.X, newY = Position.Y;

        switch (direction)
        {
            case Direction.Left:
                newX = Position.X - 1;
                condition = Position.X >= 0 && newX >= 0 && MapInfo.GridInfo[Position.Y, newX] == '.' && !MapInfo.PositionsDone[Position.Y, newX];
                break;
            case Direction.Right:
                newX = Position.X + 1;
                condition = Position.X < MapInfo.Width - 1 && newX < MapInfo.Width && MapInfo.GridInfo[Position.Y, newX] == '.' && !MapInfo.PositionsDone[Position.Y, newX];
                break;
            case Direction.Up:
                newY = Position.Y - 1;
                condition = Position.Y >= 0 && newY >= 0 && MapInfo.GridInfo[newY, Position.X] == '.' && !MapInfo.PositionsDone[newY, Position.X];
                break;
            case Direction.Down:
                newY = Position.Y + 1;
                condition = Position.Y < MapInfo.Height - 1 && newY < MapInfo.Height && MapInfo.GridInfo[newY, Position.X] == '.' && !MapInfo.PositionsDone[newY, Position.X];
                break;
        }

        if (condition)
        {

            // Check for dead end

            Console.Error.WriteLine("newY : " + newY);
            Console.Error.WriteLine("newX : " + newX);
            Console.Error.WriteLine("direction : " + direction);

            bool technicalCondition = newY < 0 || newY > MapInfo.Height - 1 || newX < 0 || newX > MapInfo.Width - 1;

            if ((technicalCondition || newY - 1 < 0 || (MapInfo.GridInfo[newY - 1, newX] == 'x' || MapInfo.PositionsDone[newY - 1, newX]))
            && (technicalCondition || newY + 1 > MapInfo.Height - 1 || (MapInfo.GridInfo[newY + 1, newX] == 'x' || MapInfo.PositionsDone[newY + 1, newX]))
            && (technicalCondition || newX - 1 < 0 || (MapInfo.GridInfo[newY, newX - 1] == 'x' || MapInfo.PositionsDone[newY, newX - 1]))
            && (technicalCondition || newX + 1 > MapInfo.Width - 1 || (MapInfo.GridInfo[newY, newX + 1] == 'x' || MapInfo.PositionsDone[newY, newX + 1])))
            {
                condition = false;
            }


            // Check again
            if (condition)
            {
                switch (direction)
                {
                    case Direction.Left:
                        newX = Position.X - 2;
                        break;
                    case Direction.Right:
                        newX = Position.X + 2;
                        break;
                    case Direction.Up:
                        newY = Position.Y - 2;
                        break;
                    case Direction.Down:
                        newY = Position.Y + 2;
                        break;
                }

                Console.Error.WriteLine("newY : " + newY);
                Console.Error.WriteLine("newX : " + newX);
                Console.Error.WriteLine("direction : " + direction);

                technicalCondition = newY < 0 || newY > MapInfo.Height - 1 || newX < 0 || newX > MapInfo.Width - 1;

                if (!technicalCondition && MapInfo.GridInfo[newY, newX] == '.' && !MapInfo.PositionsDone[newY, newX])
                {
                    if ((direction == Direction.Down || technicalCondition || newY - 1 < 0 || (MapInfo.GridInfo[newY - 1, newX] == 'x' || MapInfo.PositionsDone[newY - 1, newX]))
                    && (direction == Direction.Up || technicalCondition || newY + 1 > MapInfo.Height - 1 || (MapInfo.GridInfo[newY + 1, newX] == 'x' || MapInfo.PositionsDone[newY + 1, newX]))
                    && (direction == Direction.Right || technicalCondition || newX - 1 < 0 || (MapInfo.GridInfo[newY, newX - 1] == 'x' || MapInfo.PositionsDone[newY, newX - 1]))
                    && (direction == Direction.Left || technicalCondition || newX + 1 > MapInfo.Width - 1 || (MapInfo.GridInfo[newY, newX + 1] == 'x' || MapInfo.PositionsDone[newY, newX + 1])))
                    {
                        condition = false;
                    }
                }
            }
        }

        return condition;
    }

    private char GetBestDirection()
    {
        bool goUp = ShouldGoUp();

        // First Up, if not possible Down
        if (goUp)
        {
            if (CanGo(Direction.Up))
            {
                Go(Direction.Up);
                return _charMovement[Direction.Up];
            }

            if (CanGo(Direction.Down))
            {
                Go(Direction.Down);
                return _charMovement[Direction.Down];
            }
        }
        // First Down, if not possible Up
        else
        {
            if (CanGo(Direction.Down))
            {
                Go(Direction.Down);
                return _charMovement[Direction.Down];
            }

            if (CanGo(Direction.Up))
            {
                Go(Direction.Up);
                return _charMovement[Direction.Up];
            }
        }

        if (_favorite == Direction.NoDirection)
        {
            bool goLeft = ShouldGoLeft();

            // First Left, if not possible Right
            if (goLeft)
            {
                if (CanGo(Direction.Left))
                {
                    _favorite = Direction.Left;
                    Go(Direction.Left);
                    return _charMovement[Direction.Left];
                }

                if (CanGo(Direction.Right))
                {
                    _favorite = Direction.Right;
                    Go(Direction.Right);
                    return _charMovement[Direction.Right];
                }
            }
            // First Right, if not possible Left
            else
            {
                if (CanGo(Direction.Right))
                {
                    _favorite = Direction.Right;
                    Go(Direction.Right);
                    return _charMovement[Direction.Right];
                }

                if (CanGo(Direction.Left))
                {
                    _favorite = Direction.Left;
                    Go(Direction.Left);
                    return _charMovement[Direction.Left];
                }
            }
        }

        return 'A';
    }

    private char TryToFollowOpponent()
    {
        if (FollowOpponent && OpponentDirection != Direction.NoDirection)
        {
            if (CanGo(OpponentDirection))
            {
                Go(OpponentDirection);
                return _charMovement[OpponentDirection];
            }
            else
            {
                FollowOpponent = false;
                OpponentDirection = Direction.NoDirection;
                PreviousDirection = Direction.NoDirection;
            }
        }

        return 'A';
    }

    private char TryFavoriteDirection()
    {
        if (_favorite != Direction.NoDirection)
        {
            if (CanGo(_favorite))
            {
                Go(_favorite);
                return _charMovement[_favorite];
            }
        }

        return 'A';
    }

    private char RandomGoOrSurface()
    {
        // Random choice
        if (CanGo(Direction.Left))
        {
            Go(Direction.Left);
            return _charMovement[Direction.Left];
        }

        if (CanGo(Direction.Up))
        {
            Go(Direction.Up);
            return _charMovement[Direction.Up];
        }

        if (CanGo(Direction.Right))
        {
            Go(Direction.Right);
            return _charMovement[Direction.Right];
        }

        if (CanGo(Direction.Down))
        {
            Go(Direction.Down);
            return _charMovement[Direction.Down];
        }

        // Need to "SURFACE"
        return 'R';
    }

    private bool ShouldGoUp()
    {
        // Check Up or Down - Best choice
        int y = Position.Y;
        bool obstacleEncoutered = false;
        int cptUp;
        int cpt = 0;

        // Check Up choice
        while (y > 0 && !obstacleEncoutered)
        {
            y--;
            if (MapInfo.GridInfo[y, Position.X] == '.' && !MapInfo.PositionsDone[y, Position.X])
            {
                cpt++;
            }
            else
            {
                obstacleEncoutered = true;
            }
        }

        cptUp = cpt;
        y = Position.Y;
        obstacleEncoutered = false;
        cpt = 0;

        // Check Down choice
        while (y < MapInfo.Height - 1 && !obstacleEncoutered)
        {
            y++;
            if (MapInfo.GridInfo[y, Position.X] == '.' && !MapInfo.PositionsDone[y, Position.X])
            {
                cpt++;
            }
            else
            {
                obstacleEncoutered = true;
            }
        }

        return cptUp > cpt;
    }

    private bool ShouldGoLeft()
    {
        // Check Left or Right - Best choice
        int x = Position.X;
        bool obstacleEncoutered = false;
        int cptLeft;
        int cpt = 0;

        // Check Left choice
        while (x > 0 && !obstacleEncoutered)
        {
            x--;
            if (MapInfo.GridInfo[Position.Y, x] == '.' && !MapInfo.PositionsDone[Position.Y, x])
            {
                cpt++;
            }
            else
            {
                obstacleEncoutered = true;
            }
        }

        cptLeft = cpt;
        x = Position.X;
        obstacleEncoutered = false;
        cpt = 0;

        // Check Right choice
        while (x < MapInfo.Width - 1 && !obstacleEncoutered)
        {
            x++;
            if (MapInfo.GridInfo[Position.Y, x] == '.' && !MapInfo.PositionsDone[Position.Y, x])
            {
                cpt++;
            }
            else
            {
                obstacleEncoutered = true;
            }
        }

        return cptLeft > cpt;
    }
}