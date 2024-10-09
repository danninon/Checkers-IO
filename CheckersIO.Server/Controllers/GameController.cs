using Microsoft.AspNetCore.Mvc;
using CheckersGame.GameLogic;

namespace CheckersGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {

        private readonly GameEngine engine;

        public GameController(GameEngine gameEngine)
        {
            Console.WriteLine("on GameController ctor");
            engine = gameEngine;



        }

        [HttpGet("peek")]
        public IActionResult GetBoard()
        {
            Console.WriteLine("At peek");
            int[,] board = engine.getBoard();

            // Convert the 2D array to a JSON-serializable format (int[][])
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            int[][] boardRepresentation = new int[rows][];

            for (int i = 0; i < rows; i++)
            {
                boardRepresentation[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    boardRepresentation[i][j] = board[i, j];
                }
            }

            return Ok(boardRepresentation);
        }

        [HttpGet("start")]
        public IActionResult StartNewGame()
        {
            Console.WriteLine("initializaing...");
            engine.initGame();

            return Ok("Game started. Board initialized.");
        }

        [HttpPost("move")]
        public IActionResult ProcessMove([FromBody] MoveRequest request)
        {
            var from = Tuple.Create(request.FromRow, request.FromCol);
            var to = Tuple.Create(request.ToRow, request.ToCol);

            string moveResult = engine.Move(from, to);

            if (moveResult == "Move executed successfully.")
            {
                return Ok(moveResult);
            }
            else
            {
                return BadRequest(moveResult);
            }
        }

    }

    public class MoveRequest
    {
        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
    }
}
