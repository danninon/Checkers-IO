import React, { useEffect, useState } from 'react';
import Board from './Board';
import { getBoardFromAPI, movePieceAPI, startNewGameAPI } from '../services/api';

const Game: React.FC = () => {
    const [board, setBoard] = useState<number[][]>([]);
    const [selectedPiece, setSelectedPiece] = useState<[number, number] | null>(null);  // Store selected piece
    const [errorMessage, setErrorMessage] = useState<string | null>(null);  // Track any error messages

    // Fetch the board when the component loads
    useEffect(() => {
        const fetchBoard = async () => {
            const initialBoard = await getBoardFromAPI();
            setBoard(initialBoard);
        };

        fetchBoard();
    }, []);

    // Handle square clicks
    const handleSquareClick = async (row: number, col: number) => {
        if (selectedPiece) {
            const [fromRow, fromCol] = selectedPiece;

            try {
                const result = await movePieceAPI([fromRow, fromCol], [row, col]);  // Send move to API
                console.log(result);  // Log success message

                const updatedBoard = await getBoardFromAPI();  // Fetch updated board
                setBoard(updatedBoard);
                setSelectedPiece(null);  // Reset selected piece after move
                setErrorMessage(null);   // Clear any error messages

            } catch (error: unknown) {  // Handle the error as unknown
                console.error("Move failed:", error);

                if (error instanceof Error) {
                    setErrorMessage(error.message);  // Safely access the error message
                } else {
                    setErrorMessage("An unknown error occurred.");  // Fallback message
                }

                setSelectedPiece(null);  // Reset the selected piece on failure
            }
        } else {
            // Select a piece if it's not empty
            if (board[row][col] !== 0) {
                setSelectedPiece([row, col]);
                setErrorMessage(null);  // Clear any previous error messages
            }
        }
    };


    // Handle the "Start New Game" button click
    const handleStartNewGame = async () => {
        try {
            const result = await startNewGameAPI();
            console.log(result);  // Log response from the backend

            // Fetch the initialized board
            const updatedBoard = await getBoardFromAPI();
            setBoard(updatedBoard);
            setSelectedPiece(null);
            setErrorMessage(null);
        } catch (error) {
            console.error("Failed to start new game:", error);
        }
    };

    return (
        <div className="game">
            <h1>Checkers Game</h1>
            <button onClick={handleStartNewGame}>Start New Game</button>  {/* New game button */}
            {errorMessage && <p className="error">{errorMessage}</p>}  {/* Display error message */}
            {selectedPiece && <p>Selected Piece: {selectedPiece[0]}, {selectedPiece[1]}</p>}
            <Board
                board={board}
                handleSquareClick={handleSquareClick}
                selectedPiece={selectedPiece}  // Pass the selected piece as a prop
            />
        </div>
    );
};

export default Game;
