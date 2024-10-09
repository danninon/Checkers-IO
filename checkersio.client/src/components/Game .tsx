import React, { useEffect, useState } from 'react';
import { io } from 'socket.io-client';  // Import Socket.io client
import Board from './Board';
import { getBoardFromAPI, movePieceAPI, startNewGameAPI } from '../services/api';

const socket = io('http://localhost:4000');  

const Game: React.FC = () => {
    const [board, setBoard] = useState<number[][]>([]);
    const [currentPlayer, setCurrentPlayer] = useState<number>(1); 
    const [selectedPiece, setSelectedPiece] = useState<[number, number] | null>(null); 
    const [errorMessage, setErrorMessage] = useState<string | null>(null);  

    
    useEffect(() => {
        const fetchBoard = async () => {
            const initialBoard = await getBoardFromAPI();
            setBoard(initialBoard);
        };

        fetchBoard();
    }, []);

    
    useEffect(() => {
        
        socket.on('gameState', (gameState) => {
            setBoard(gameState.board);
            setCurrentPlayer(gameState.currentPlayer);
        });

      
        return () => {
            socket.off('gameState');
        };
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

                
                socket.emit('playerMove', {
                    board: updatedBoard,
                    currentPlayer: currentPlayer === 1 ? 2 : 1
                });
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

            socket.emit('playerMove', {
                board: updatedBoard,
                currentPlayer: 1
            });
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
