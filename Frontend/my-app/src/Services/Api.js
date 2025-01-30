import axios from 'axios';

const API_BASE_URL = 'http://localhost:5074';

export const getGames = async () => {
    const response = await axios.get(`${API_BASE_URL}/games`);
    return response.data;
};

export const getGenres = async () => {
    const response = await axios.get(`${API_BASE_URL}/genres`);
    return response.data;
};

export const createGame = async (game) => {
    await axios.post(`${API_BASE_URL}/games`, game);
};

export const updateGame = async (id, game) => {
    await axios.put(`${API_BASE_URL}/games/${id}`, game);
};

export const deleteGame = async (id) => {
    await axios.delete(`${API_BASE_URL}/games/${id}`);
};
