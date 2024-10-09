import React, { useEffect, useState } from 'react';
import Board from './Board';
import { getBoardFromAPI, movePieceAPI, startNewGameAPI } from '../services/api';

const Game: React.FC = () => {
    const [board, setBoard] = useState<number[][]>([]);
    const [selectedPiece, setSelectedPiece] = useState<[number, number] | null>(null);  
    const [errorMessage, setErrorMessage] = useState<string | null>(null);  

    useEffect(() => {
        const fetchBoard = async () => {
            const initialBoard = await getBoardFromAPI();
            setBoard(initialBoard);
        };

        fetchBoard();
    }, []);

    const handleSquareClick = async (row: number, col: number) => {
        if (selectedPiece) {
            const [fromRow, fromCol] = selectedPiece;

            try {
                const result = await movePieceAPI([fromRow, fromCol], [row, col]); 
                console.log(result);  

                const updatedBoard = await getBoardFromAPI();  
                setBoard(updatedBoard);
                setSelectedPiece(null);
                setErrorMessage(null);  

            } catch (error: unknown) { 
                console.error("Move failed:", error);

                if (error instanceof Error) {
                    setErrorMessage(error.message); 
                } else {
                    setErrorMessage("An unknown error occurred.");  
                }

                setSelectedPiece(null); 
            }
        } else {
            // Select a piece if it's not empty
            if (board[row][col] !== 0) {
                setSelectedPiece([row, col]);
                setErrorMessage(null);  
            }
        }
    };


    
    const handleStartNewGame = async () => {
        try {
            const result = await startNewGameAPI();
            console.log(result); 

           
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
            <button onClick={handleStartNewGame}>Start New Game</button>  
            {errorMessage && <p className="error">{errorMessage}</p>}  
            {selectedPiece && <p>Selected Piece: {selectedPiece[0]}, {selectedPiece[1]}</p>}
            <Board
                board={board}
                handleSquareClick={handleSquareClick}
                selectedPiece={selectedPiece}  
            />
        </div>
    );
};

export default Game;
