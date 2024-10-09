using System.Text;

namespace CheckersGame.GameLogic
{
    public class GameEngine
    {
        private readonly int[,] board = new int[8, 8];
        private bool isGameStarted = false;
        private bool isGameWinConditionReached = false;
        private int currentPlayer = 1;

        public GameEngine() { }

        public int[,] getBoard() { return board; }

        public void initGame()
        {
            // Initialize player 1
            board[0, 7] = board[0, 5] = board[0, 3] = board[0, 1] = 1;
            board[1, 6] = board[1, 4] = board[1, 2] = board[1, 0] = 1;
            board[2, 7] = board[2, 5] = board[2, 3] = board[2, 1] = 1;

            // Initialize player 2
            board[5, 6] = board[5, 4] = board[5, 2] = board[5, 0] = 2;
            board[6, 7] = board[6, 5] = board[6, 3] = board[6, 1] = 2;
            board[7, 6] = board[7, 4] = board[7, 2] = board[7, 0] = 2;

            // Clear the middle rows
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

            // Validate the move
            string validationResult = ValidateMovement(from, to);
            Console.WriteLine($"Validation result: {validationResult}");

            if (validationResult != "Valid")
            {
                Console.WriteLine($"Move failed: {validationResult}");
                return validationResult;
            }

            // Check if the current move is a capture
            bool moveInvolvedCapture = DidMoveInvolveCapture(from, to);

            // Check if any other piece of the current player could eat an opponent
            bool anyPieceCanEat = AnyPieceCanEat(currentPlayer);

            // If another piece could eat and the current move did not involve a capture, burn the other pieces
            if (anyPieceCanEat && !moveInvolvedCapture)
            {
                BurnPieceThatCouldEat(excludeRow: from.Item1, excludeCol: from.Item2);
            }

            // Execute the move
            ExecuteMove(from, to);

            // Switch the current player
            currentPlayer = (currentPlayer == 1) ? 2 : 1;

            Console.WriteLine($"Move executed successfully. Current player is now Player {currentPlayer}");
            Console.WriteLine(this.ToString());
            return "Move executed successfully.";
        }

        private string ValidateMovement(Tuple<int, int> from, Tuple<int, int> to)
        {
            try
            {
                int fromRow = from.Item1;
                int fromCol = from.Item2;
                int toRow = to.Item1;
                int toCol = to.Item2;

                if (!isGameStarted)
                    throw new Exception("Game hasn't started, initialize the game");

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

                if (rowDiff == 1 && colDiff == 1 && IsForwardMove(piece, fromRow, toRow))
                {
                    // Case 1: Simple diagonal movement
                    return "Valid";
                }
                else if (rowDiff == 2 && colDiff == 2 && IsForwardMove(piece, fromRow, toRow))
                {
                    int midRow = (fromRow + toRow) / 2;
                    int midCol = (fromCol + toCol) / 2;
                    int middlePiece = board[midRow, midCol];

                    if (middlePiece == 0 || middlePiece == piece)
                        throw new Exception("No opponent piece to eat, or trying to eat your own piece.");

                    // Case 2: Valid capture (jumping over opponent)
                    return "Valid";
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
        }

        private void ExecuteMove(Tuple<int, int> from, Tuple<int, int> to)
        {
            int fromRow = from.Item1;
            int fromCol = from.Item2;
            int toRow = to.Item1;
            int toCol = to.Item2;

            // Move the piece
            board[toRow, toCol] = board[fromRow, fromCol];
            board[fromRow, fromCol] = 0;

            // If it was a capture move, remove the opponent's piece
            if (Math.Abs(toRow - fromRow) == 2)
            {
                int midRow = (fromRow + toRow) / 2;
                int midCol = (fromCol + toCol) / 2;
                board[midRow, midCol] = 0;
            }

            // Check for win condition after the move
            if (CheckWinCondition(currentPlayer))
            {
                Console.WriteLine($"Player {currentPlayer} wins!");
                isGameWinConditionReached = true;
                isGameStarted = false;
            }
        }

        // Helper method to check if the move involved a capture
        private bool DidMoveInvolveCapture(Tuple<int, int> from, Tuple<int, int> to)
        {
            return Math.Abs(from.Item1 - to.Item1) == 2 && Math.Abs(from.Item2 - to.Item2) == 2;
        }

        // This method "burns" any piece of the current player that could have eaten but didn't, excluding the piece that just moved
        private void BurnPieceThatCouldEat(int excludeRow, int excludeCol)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Skip the piece that just moved
                    if (row == excludeRow && col == excludeCol)
                        continue;

                    // Check if the current piece can eat
                    if (board[row, col] == currentPlayer && CanPieceEat(row, col))
                    {
                        Console.WriteLine($"Burning piece at ({row}, {col}) for failing to capture.");
                        board[row, col] = 0; // Remove the piece
                    }
                }
            }
        }


        // Check if a specific piece can capture an opponent
        private bool CanPieceEat(int row, int col)
        {
            int piece = board[row, col];
            int opponent = (piece == 1) ? 2 : 1;

            if (IsValidCapture(row, col, row + 2, col + 2, opponent)) return true;
            if (IsValidCapture(row, col, row + 2, col - 2, opponent)) return true;
            if (IsValidCapture(row, col, row - 2, col + 2, opponent)) return true;
            if (IsValidCapture(row, col, row - 2, col - 2, opponent)) return true;

            return false;
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


        // Check if the move is a valid capture
        private bool IsValidCapture(int fromRow, int fromCol, int toRow, int toCol, int opponent)
        {
            if (IsOutOfBounds(toRow, toCol)) return false;

            int midRow = (fromRow + toRow) / 2;
            int midCol = (fromCol + toCol) / 2;

            return board[midRow, midCol] == opponent && board[toRow, toCol] == 0;
        }

        private bool IsOutOfBounds(int row, int col)
        {
            return row < 0 || row >= 8 || col < 0 || col >= 8;
        }

        private bool IsForwardMove(int piece, int fromRow, int toRow)
        {
            return piece == 1 ? toRow > fromRow : toRow < fromRow;
        }

        private bool AnyPieceCanEat(int player)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == player)
                    {
                        if (CanPieceEat(row, col))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}


