// using test1
//using test2
using System; // YO
// using test3
 //using test4
using System.Collections.Generic;
        // using test5
//using test6
using System.Text;

public class Game
{
    // --------------------------------------
    // ----------- GAME CONSTANTS -----------
    // --------------------------------------

    // % of chance of "move 1" when SILENCE
    public const int SILENCE_MOVE_RATE = 30;

    // % of SILENCE in accordance to SONAR
    public const int SILENCE_CHARGE_RATE = 80;

    // Number of opponent's SILENCE we consider loosing his position
    public const int NB_SILENCE_TOLERANCE = 3;

    // We try to relocate _opponent when he TORPEDO, or not
    public const bool CHECK_OPPONENT_TORPEDO_ORDER = false;

    // -------------------------------------
    // ---------------- END ----------------
    // -------------------------------------

    static private int _opponentSector = 0;

    static private int _torpedoCooldown = -1;
    static private int _silenceCooldown = -1;
    static private int _sonarCooldown = -1;
    static private int _mineCooldown = -1;

    static private Player _player;
    static private Opponent _opponent;

    static Position LoadMapAndGetStartPosition(int width, int height)
    {
        // Start position for _player
        Position startPosition = new Position { X = -1, Y = -1 };

        for (int y = 0; y < height; y++)
        {
            string lineInfo = Console.ReadLine();
            for (int x = 0; x < width; x++)
            {
                Console.Error.Write(lineInfo[x]);
                MapInfo.GridInfo[y, x] = lineInfo[x];

                MapInfo.PositionsDone[y, x] = false;

                // We set only one time
                if (startPosition.X == -1 && y > 8 && x > 8)
                {
                    // We want to spawn at odd number for position X and even number for Y
                    // In order to have a weird start position
                    if (MapInfo.GridInfo[y, x] == '.' && y % 2 == 0 && x % 2 == 1)
                    {
                        startPosition.X = x;
                        startPosition.Y = y;
                    }
                }
            }
            Console.Error.WriteLine();
        }

        return startPosition;
    }

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');

        // Map Infos
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);

        int idPlayer = int.Parse(inputs[2]);

        MapInfo.Init(width, height);
        Position startPosition = LoadMapAndGetStartPosition(width, height);
        
        _player = new Player(startPosition.X, startPosition.Y);
        _opponent = new Opponent();

        MapInfo.PositionsDone[startPosition.Y, startPosition.X] = true;

        int torpedoLaunchAttempts = 0;
        bool isPlayerTurn = idPlayer == 0;
        List<Position> mines = new List<Position>();

        // Set _player start position
        Console.WriteLine(_player.GetFormattedPosition());

        // game loop
        while (true)
        {
            // My turn
            if (!isPlayerTurn)
            {
                string turnInfos = Console.ReadLine();
                string sonarResult = Console.ReadLine();
                string opponentOrders = Console.ReadLine();
                inputs = turnInfos.Split(' ');

                // Current position
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);
                _player.Position.X = x;
                _player.Position.Y = y;

                _player.Life = int.Parse(inputs[2]);
                _opponent.Life = int.Parse(inputs[3]);

                _torpedoCooldown = int.Parse(inputs[4]);
                _sonarCooldown = int.Parse(inputs[5]);
                _silenceCooldown = int.Parse(inputs[6]);
                _mineCooldown = int.Parse(inputs[7]);

                if (_player.HasShoot)
                {
                    torpedoLaunchAttempts = 0;
                    CheckOpponentLifeAfterTorpedo();
                }
                _player.HasShoot = false;

                HandleOpponentDectectionWithSonar(sonarResult);

                // We verify both MOVE and SILENCE aren't present at the same turn
                // in order to avoid cheat.
                // Because, it's possible to SILENCE 0 and MOVE
                // (Yes, this is cheat to me...)
                bool opponentHasCheated = opponentOrders.Contains("SILENCE") && opponentOrders.Contains("MOVE");
                string[] splittedOpponentOrders = opponentOrders.Split('|');

                _opponent.AnalyseOrders(splittedOpponentOrders, _player, opponentHasCheated);

                Console.Error.WriteLine("MapInfo.PositionsDone X:" + _player.Position.X + ", Y: " + _player.Position.Y);

                MapInfo.PositionsDone[_player.Position.Y, _player.Position.X] = true;

                char playerMove = _player.ChooseDirection();
                StringBuilder command = new StringBuilder();

                if (playerMove != 'R')
                {
                    // We move with SILENCE in priority
                    if (_silenceCooldown == 0)
                    {
                        HandleSilence(playerMove, command);
                    }
                    else
                    {
                        HandleMove(playerMove, torpedoLaunchAttempts, command);
                    }

                    if (_torpedoCooldown <= 1)
                    {
                        HandleTorpedo(command, playerMove);
                        torpedoLaunchAttempts++;
                    }

                    // Drop a mine
                    if (_mineCooldown == 0)
                    {
                        DropMine(mines, command);
                    }

                    // Do Sonar
                    if (_sonarCooldown == 0 && !_opponent.IsSpottedWithSonar)
                    {
                        HandleSonar(command);
                    }
                }
                else
                {
                    HandleSurface(command);
                }

                HandleMineExplosion(mines, command);

                _opponent.PreviousTurnLife = _opponent.Life;
                _player.PreviousTurnLife = _player.Life;
                DisplaySupposedOpponentPosition(command);

                Console.Error.WriteLine("command : " + command);
                // END OF TURN, NOTHING AFTER THIS LINE !!!
                Console.WriteLine(command);
            }

            isPlayerTurn = !isPlayerTurn;
        }
    }

    static void CheckOpponentLifeAfterTorpedo()
    {
        // If _opponent hasn't been touched, change position
        //  X
        // XXX
        //  X
        if (_opponent.PreviousTurnLife == _opponent.Life)
        {
            _opponent.NotTouched();
        }
        // If _opponent touched, we are good !
        // Don't try to relocate position !
        // And follow him !
        else if (_opponent.PreviousTurnLife > _opponent.Life)
        {
            _opponent.Touched();
            _player.FollowOpponent = true;
        }
    }

    static void HandleOpponentDectectionWithSonar(string sonarResult)
    {
        if (sonarResult == "Y")
        {
            if (!_player.FollowOpponent)
            {
                _opponent.UpdatePositionBySector(_opponentSector);
                _opponent.IsSpottedWithSonar = true;
            }
        }
    }

    static void HandleSurface(StringBuilder command)
    {
        MapInfo.ClearPositionsDone();

        // Set NoDirection
        _player.ResetPreviousDirection();
        _player.ResetFavoriteDirection();

        command.Clear();
        command.Append("SURFACE");
    }

    static void HandleMove(char playerMove, int torpedoLaunchAttempts, StringBuilder command)
    {
        if (torpedoLaunchAttempts > 0 || _torpedoCooldown == 0)
        {
            Random rng = new Random();
            int rngSilenceCharge = rng.Next(0, 100);

            command.Clear();

            if (rngSilenceCharge > SILENCE_CHARGE_RATE && _sonarCooldown > 0)
            {
                command.Append($"MOVE {playerMove} SONAR");
            }
            else
            {
                command.Append($"MOVE {playerMove} SILENCE");
            }
        }
        else
        {
            command.Append($"MOVE {playerMove} TORPEDO");
        }
    }

    static void HandleTorpedo(StringBuilder command, char playerMove)
    {
        // Only TORPEDO if we can really shoot
        // Re-define _opponent position if bugged
        if (_opponent.Position.X < 0) _opponent.Position.X = 0;
        if (_opponent.Position.Y < 0) _opponent.Position.Y = 0;
        if (_opponent.Position.X >= MapInfo.Width) _opponent.Position.X = MapInfo.Width - 1;
        if (_opponent.Position.Y >= MapInfo.Height) _opponent.Position.Y = MapInfo.Height - 1;

        if (MapInfo.GridInfo[_opponent.Position.Y, _opponent.Position.X] == '.'
            && (_opponent.HasSurfacedOrShoot || _player.FollowOpponent
            || _opponent.IsSpottedWithSonar || _player.Touched()))
        {
            int differencePositionX = Math.Abs(_opponent.Position.X - _player.Position.X);
            int differencePositionY = Math.Abs(_opponent.Position.Y - _player.Position.Y);
            int differencePosition = differencePositionX + differencePositionY;
            
            if (differencePosition <= 4)
            {
                command.Clear();

                if (_torpedoCooldown == 0)
                {
                    command.Append($"TORPEDO {_opponent.Position.X} {_opponent.Position.Y}|MOVE {playerMove} TORPEDO");
                }
                else if (_torpedoCooldown == 1)
                {
                    command.Append($"MOVE {playerMove} TORPEDO|TORPEDO {_opponent.Position.X} {_opponent.Position.Y}");
                }
                
                _player.HasShoot = true;
            }
        }
    }

    static void HandleSilence(char playerMove, StringBuilder command)
    {
        Random rng = new Random();
        int rngSilenceMoveOrNot = rng.Next(0, 100);
        int nbTilesMove = 0;

        command.Clear();

        if (rngSilenceMoveOrNot > SILENCE_MOVE_RATE)
        {
            nbTilesMove = 1;
        }
        else
        {
            _player.NoMoveThisTurn = true;
        }

        command.Append($"SILENCE {playerMove} {nbTilesMove}");
    }

    static void HandleSonar(StringBuilder command)
    {
        int playerSector = _player.Position.GetSector();
        int supposedOpponentSector = _opponent.Position.GetSector();

        if (playerSector != supposedOpponentSector)
        {
            _opponentSector = supposedOpponentSector;
        }
        else
        {
            Random rng = new Random();
            do
            {
                _opponentSector = rng.Next(1, 9);
            } while (_opponentSector == playerSector);
        }

        command.Append($"|SONAR {_opponentSector}");
    }

    static void DropMine(List<Position> mines, StringBuilder command)
    {
        char mineDirection = _player.DropMineDirection();
        if (mineDirection != 'A')
        {
            command.Append($"|MINE {mineDirection}");

            // Mine position will be the position of _player, now
            Position minePosition = new Position();
            minePosition.X = _player.Position.X;
            minePosition.Y = _player.Position.Y;

            mines.Add(minePosition);
        }
    }

    static void HandleMineExplosion(List<Position> mines, StringBuilder command)
    {
        // Explode mine if _opponent is nearby
        //bool alreadyTriggered = false
        Position mineChoosed = null;
        foreach (Position mine in mines)
        {
            if (mine.X >= _opponent.Position.X - 1 && mine.X <= _opponent.Position.X + 1
             && mine.Y >= _opponent.Position.Y - 1 && mine.Y <= _opponent.Position.Y + 1)
            {
                mineChoosed = mine;
                command.Append($"|TRIGGER {mine.X} {mine.Y}");
                break;
            }
        }

        if (mineChoosed != null)
        {
            mines.Remove(mineChoosed);
        }
    }

    static void DisplaySupposedOpponentPosition(StringBuilder command)
    {
        command.Append($"|MSG X: {_opponent.Position.X}, Y: {_opponent.Position.Y}");
    }
}