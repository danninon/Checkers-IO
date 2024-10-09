using System.Text;

namespace CheckersGame.GameLogic
{
    public class GameEngine
    {
        private readonly int[,] board = new int[8, 8];
        private bool isGameStarted = false;
        private bool isGameWinConditionReached = false;

        private int currentPlayer = 1;

        public GameEngine()
        {
        }

        public int[,] getBoard() { return board; }

        public void initGame()
        {
            //init player 1
            board[0, 7] = board[0, 5] = board[0, 3] = board[0, 1] = 1;
            board[1, 6] = board[1, 4] = board[1, 2] = board[1, 0] = 1;
            board[2, 7] = board[2, 5] = board[2, 3] = board[2, 1] = 1;

            //init player 2
            board[5, 6] = board[5, 4] = board[5, 2] = board[5, 0] = 2;
            board[6, 7] = board[6, 5] = board[6, 3] = board[6, 1] = 2;
            board[7, 6] = board[7, 4] = board[7, 2] = board[7, 0] = 2;

            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 8; ++j)
                    board[3 + i, j] = 0;
            }


            this.isGameStarted = true;

            Console.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("Checkers Board:");

            sb.Append("   0 1 2 3 4 5 6 7");
            sb.AppendLine();
            sb.Append("  _ _ _ _ _ _ _ _");
            sb.AppendLine();

            for (int i = 0; i < 8; ++i)
            {
                sb.Append($"{i}|");
                for (int j = 0; j < 8; ++j)
                {
                    sb.Append($"{board[i, j]} ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }


        public string Move(Tuple<int, int> from, Tuple<int, int> to)
        {
            Console.WriteLine(this.ToString());
            Console.WriteLine("Current player: " + currentPlayer);
            Console.WriteLine($"Attempting to move from ({from.Item1}, {from.Item2}) to ({to.Item1}, {to.Item2}).");

            string validationResult = ValidateMovement(from, to);

            Console.WriteLine($"Validation result: {validationResult}");

            if (validationResult != "Valid")
            {
                Console.WriteLine($"Move failed: {validationResult}");
                return validationResult;
            }
            else
            {
                Console.WriteLine($"Current player before move: Player {currentPlayer}");

                currentPlayer = (currentPlayer == 1) ? 2 : 1;
                ExecuteMove(from, to);

                Console.WriteLine($"Move executed successfully. Current player is now Player {currentPlayer}");
                Console.WriteLine("Current board state:");
                Console.WriteLine(this.ToString());

                return "Move executed successfully.";
            }
        }



        private string ValidateMovement(Tuple<int, int> from, Tuple<int, int> to)
        {
            try
            {
                int fromRow = from.Item1;
                int fromCol = from.Item2;
                int toRow = to.Item1;
                int toCol = to.Item2;
                if (isGameStarted == false)
                {
                    throw new Exception("Game hasn't started, initialize the game");
                }

                if (IsOutOfBounds(fromRow, fromCol) || IsOutOfBounds(toRow, toCol))
                    throw new Exception("Move is out of bounds.");

                int piece = board[fromRow, fromCol];
                int target = board[toRow, toCol];

                if (piece == 0)
                    throw new Exception("There is no piece at the starting position.");

                if (piece != currentPlayer)
                    throw new Exception("You are trying to move the opponent's piece.");

                if (target != 0)
                    throw new Exception("The target position is already occupied.");

                int rowDiff = Math.Abs(toRow - fromRow);
                int colDiff = Math.Abs(toCol - fromCol);

                Console.WriteLine("@@@");

                if (rowDiff == 1 && colDiff == 1 && IsForwardMove(piece, fromRow, toRow))
                {
                    // Case 1: Simple movement (non-eating)
                    if (AnyPieceCanEat(piece))
                        throw new Exception("You must eat if a capture is available.");
                }
                // Case 2: Eating move (2 steps diagonal and the opponent's piece in between)
                else if (rowDiff == 2 && colDiff == 2 && IsForwardMove(piece, fromRow, toRow))
                {
                    int midRow = (fromRow + toRow) / 2;
                    int midCol = (fromCol + toCol) / 2;
                    int middlePiece = board[midRow, midCol];

                    if (middlePiece == 0 || middlePiece == piece)
                        throw new Exception("No opponent piece to eat, or trying to eat your own piece.");
                }
                else
                {
                    throw new Exception("Invalid move. Must be a diagonal move of 1 step or a valid capture.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Move is invalid: {e.Message}");
                return e.Message;
            }

            return "Valid";
        }

        private void ExecuteMove(Tuple<int, int> from, Tuple<int, int> to)
        {
            int fromRow = from.Item1;
            int fromCol = from.Item2;
            int toRow = to.Item1;
            int toCol = to.Item2;

            board[toRow, toCol] = board[fromRow, fromCol];
            board[fromRow, fromCol] = 0;

            if (Math.Abs(toRow - fromRow) == 2)
            {
                int midRow = (fromRow + toRow) / 2;
                int midCol = (fromCol + toCol) / 2;
                board[midRow, midCol] = 0;
            }

            if (CheckWinCondition(currentPlayer))
            {
                Console.WriteLine($"Player {currentPlayer} wins!");
                isGameWinConditionReached = true;
                isGameStarted = false;
            }
        }

        private bool CheckWinCondition(int player)
        {
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    if (board[i, j] != 0 && board[i, j] != player)
                    {
                        // Found an opponent piece, so the game isn't won yet
                        return false;
                    }
                }
            }
            // If no opponent pieces are found, the current player has won
            return true;
        }

        private bool IsOutOfBounds(int row, int col)
        {
            return row < 0 || row >= 8 || col < 0 || col >= 8;
        }

        private bool IsForwardMove(int piece, int fromRow, int toRow)
        {
            Console.WriteLine($"player:{piece} | fromRow:{fromRow} | toRow:{toRow}");
            if (piece == 1)
                return toRow > fromRow;
            if (piece == 2)
                return toRow < fromRow;
            return false;
        }

        private bool AnyPieceCanEat(int player)
        {
            Console.WriteLine($"Checking if player {player} can eat...");

            // Loop over the board to find all the player's pieces
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == player)
                    {

                        if (player == 1)
                        {
                            if (CanEatInDirection(player, row, col, 1, 1) ||  // Down-right
                                CanEatInDirection(player, row, col, 1, -1))  // Down-left
                            {
                                Console.WriteLine($"Player {player} can eat with piece at ({row}, {col}).");
                                return true;
                            }
                        }
                        else if (player == 2)
                        {
                            if (CanEatInDirection(player, row, col, -1, 1) ||  // Up-right
                                CanEatInDirection(player, row, col, -1, -1))  // Up-left
                            {
                                Console.WriteLine($"Player {player} can eat with piece at ({row}, {col}).");
                                return true;
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Player {player} cannot eat.");
            return false;
        }

        private bool CanEatInDirection(int player, int fromRow, int fromCol, int rowDir, int colDir)
        {
            int opponent = (player == 1) ? 2 : 1;
            int middleRow = fromRow + rowDir;
            int middleCol = fromCol + colDir;
            int landingRow = fromRow + 2 * rowDir;
            int landingCol = fromCol + 2 * colDir;

            if (IsOutOfBounds(middleRow, middleCol) || IsOutOfBounds(landingRow, landingCol))
            {
                return false;
            }

            if (board[middleRow, middleCol] == opponent && board[landingRow, landingCol] == 0)
            {
                return true;
            }

            return false;
        }

    }
}