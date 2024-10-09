const express = require('express');
const http = require('http');
const cors = require('cors');
const { Server } = require('socket.io');

const app = express(); // Initialize Express
const server = http.createServer(app); // Create an HTTP server

app.use(cors()); // Enable CORS for cross-origin requests (frontend <-> backend)

// Initialize Socket.io with the HTTP server
const io = new Server(server, {
    cors: {
        origin: "http://localhost:3000", // Allow your React frontend to connect
        methods: ["GET", "POST"]
    }
});

let gameState = {
    board: Array(8).fill(null).map(() => Array(8).fill(0)), // Empty board
    currentPlayer: 1 // Player 1 starts
};

// Listen for players connecting
io.on('connection', (socket) => {
    console.log('A player connected:', socket.id);

    // Send the current game state to the connected player
    socket.emit('gameState', gameState);

    // Listen for player moves
    socket.on('playerMove', (move) => {
        console.log('Move received:', move);

        // Update the game state with the move
        gameState.board = move.board;
        gameState.currentPlayer = move.currentPlayer;

        // Broadcast the updated game state to all players
        io.emit('gameState', gameState);
    });

    // Handle disconnections
    socket.on('disconnect', () => {
        console.log('A player disconnected:', socket.id);
    });
});

// Start the server on port 4000
server.listen(4000, () => {
    console.log('Server is listening on port 4000');
});
