import React from 'react';

interface SquareProps {
    piece: number;
    onClick: () => void;
    isSelected: boolean;
    isDark: boolean;  
}

const Square: React.FC<SquareProps> = ({ piece, onClick, isSelected, isDark }) => {
    const getPieceClass = () => {
        if (piece === 1) return 'player1';
        if (piece === 2) return 'player2';
        return 'empty';
    };

    return (
        <div
            className={`square ${isDark ? 'dark' : 'light'} ${isSelected ? 'selected' : ''}`}
            onClick={onClick}
        >
            {piece !== 0 && <div className={`piece ${getPieceClass()}`}></div>}
        </div>
    );
};

export default Square;
