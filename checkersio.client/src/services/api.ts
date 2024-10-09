export const getBoardFromAPI = async (): Promise<number[][]> => {
    const response = await fetch('http://localhost:5187/api/Game/peek');
    const data = await response.json();
    return data;
};

export const movePieceAPI = async (from: [number, number], to: [number, number]): Promise<string> => {
    const response = await fetch('http://localhost:5187/api/Game/move', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            FromRow: from[0],
            FromCol: from[1],
            ToRow: to[0],
            ToCol: to[1],
        }),
    });

    if (!response.ok) {
        throw new Error("Failed to execute move");
    }

    const data = await response.text();
    return data;
};

export const startNewGameAPI = async (): Promise<string> => {
    const response = await fetch('http://localhost:5187/api/Game/start', {
        method: 'GET',
    });

    if (!response.ok) {
        throw new Error("Failed to start new game");
    }

    const data = await response.text();
    return data;
};