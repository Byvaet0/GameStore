import React, { useState, useEffect } from 'react';
import GameForm from './GameForm';

const GamesList = () => {
  const [games, setGames] = useState([]);
  const [genres, setGenres] = useState([]);
  const [selectedGame, setSelectedGame] = useState(null);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    fetchGenres();
    fetchGames();
  }, []);

  const fetchGenres = async () => {
    const response = await fetch('http://localhost:5074/genres');
    const data = await response.json();
    setGenres(data);
  };

  const fetchGames = async () => {
    const response = await fetch('http://localhost:5074/games');
    const data = await response.json();
    setGames(data);
  };

  const handleSaveGame = async (gameData) => {
    const method = selectedGame ? 'PUT' : 'POST';
    const url = selectedGame
      ? `http://localhost:5074/games/${selectedGame.id}`
      : 'http://localhost:5074/games';

    const response = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(gameData),
    });

    if (response.ok) {
      fetchGames();
      setShowModal(false);
      setSelectedGame(null);
    } else {
      alert('Произошла ошибка');
    }
  };

  const handleEditGame = (game) => {
    setSelectedGame(game);
    setShowModal(true);
  };

  const handleDeleteGame = async (gameId) => {
    const response = await fetch(`http://localhost:5074/games/${gameId}`, {
      method: 'DELETE',
    });

    if (response.ok) {
      fetchGames();
    } else {
      alert('Ошибка при удалении игры');
    }
  };

  const handleCreateGame = () => {
    setSelectedGame(null);
    setShowModal(true);
  };

  return (
    <div>
      <h1>GameStore</h1>
      <button className="btn btn-success" onClick={handleCreateGame}>
        Создать игру
      </button>

      <table className="game-table">
        <thead>
          <tr>
            <th>Название игры</th>
            <th>Жанр</th>
            <th>Цена</th>
            <th>Дата выпуска</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          {games.map((game) => (
            <tr key={game.id}>
              <td>{game.name}</td>
              <td>{game.genre.name}</td>
              <td>{game.price}</td>
              <td>{game.releaseDate}</td>
              <td>
                <button onClick={() => handleEditGame(game)} className="btn btn-warning">
                  Редактировать
                </button>
                <button onClick={() => handleDeleteGame(game.id)} className="btn btn-danger">
                  Удалить
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal">
          <GameForm selectedGame={selectedGame} genres={genres} onSave={handleSaveGame} />
        </div>
      )}
    </div>
  );
};

export default GamesList;
