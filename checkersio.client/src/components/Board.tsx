import React from 'react';
import Square from './Square';

interface BoardProps {
    board: number[][];
    handleSquareClick: (row: number, col: number) => void;
    selectedPiece: [number, number] | null;  // Pass the selected piece
}

const Board: React.FC<BoardProps> = ({ board, handleSquareClick, selectedPiece }) => {
    return (
        <div className="board">
            {board.map((row, rowIndex) => (
                <div className="row" key={rowIndex}>
                    {row.map((piece, colIndex) => {
                        // Determine if the square should be light or dark
                        const isDark = (rowIndex + colIndex) % 2 === 1;
                        return (
                            <Square
                                key={colIndex}
                                piece={piece}
                                onClick={() => handleSquareClick(rowIndex, colIndex)}
                                isSelected={selectedPiece && selectedPiece[0] === rowIndex && selectedPiece[1] === colIndex}
                                isDark={isDark}  // Pass whether the square is dark
                            />
                        );
                    })}
                </div>
            ))}
        </div>
    );
};

export default Board;
