import React, { useState, useEffect } from "react";
import { Button, Table, Modal, Form, Dropdown, Alert } from "react-bootstrap";
import './App.css'; // Подключаем стили

function App() {
  const [games, setGames] = useState([]);
  const [genres, setGenres] = useState([]);
  const [selectedGame, setSelectedGame] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [gameName, setGameName] = useState("");
  const [selectedGenre, setSelectedGenre] = useState("");
  const [gamePrice, setGamePrice] = useState("");
  const [gameReleaseDate, setGameReleaseDate] = useState("");
  const [errors, setErrors] = useState({}); // Для хранения ошибок

  // Загрузка данных о играх и жанрах
  useEffect(() => {
    fetchGames();
    fetchGenres();
  }, []);

  const fetchGames = async () => {
    const response = await fetch("http://localhost:5074/games");
    const data = await response.json();
    setGames(data);
  };

  const fetchGenres = async () => {
    const response = await fetch("http://localhost:5074/genres");
    const data = await response.json();
    setGenres(data);
  };

  // Открытие модального окна для создания игры
  const handleCreateGame = () => {
    setShowModal(true);
    setSelectedGame(null);
    setGameName("");
    setSelectedGenre("");
    setGamePrice("");
    setGameReleaseDate("");
    setErrors({}); // Очистить ошибки при создании новой игры
  };

  // Открытие модального окна для редактирования игры
  const handleEditGame = (game) => {
    setShowModal(true);
    setSelectedGame(game);
    setGameName(game.name);
    setSelectedGenre(game.genreId);
    setGamePrice(game.price);
    setGameReleaseDate(game.releaseDate);
    setErrors({}); // Очистить ошибки при редактировании игры
  };

  // Закрытие модального окна
  const handleCloseModal = () => setShowModal(false);

  // Валидация данных
  const validateForm = () => {
    const newErrors = {};

    if (!gameName) newErrors.gameName = "Название игры обязательно";
    if (!selectedGenre) newErrors.selectedGenre = "Выберите жанр";
    if (!gamePrice) newErrors.gamePrice = "Цена обязательна";
    if (!gameReleaseDate) newErrors.gameReleaseDate = "Дата релиза обязательна";

    return newErrors;
  };

  // Сохранение новой или отредактированной игры
  const handleSaveGame = async () => {
    const validationErrors = validateForm();

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors); // Устанавливаем ошибки в состояние
      return;
    }

    const gameData = {
      name: gameName,
      genreId: selectedGenre,
      price: parseFloat(gamePrice),
      releaseDate: gameReleaseDate,
    };

    if (selectedGame) {
      // Обновление игры
      await fetch(`http://localhost:5074/games/${selectedGame.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(gameData),
      });
    } else {
      // Создание новой игры
      await fetch("http://localhost:5074/games", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(gameData),
      });
    }

    // После сохранения игры обновляем список игр
    fetchGames();
    handleCloseModal();
  };

  // Удаление игры
  const handleDeleteGame = async (id) => {
    await fetch(`http://localhost:5074/games/${id}`, {
      method: "DELETE",
    });
    fetchGames();
  };

  return (
    <div className="container">
      <h1>GAMESTORE</h1>
      <Button className="mt-4 mb-4" onClick={handleCreateGame}>
        Создать игру
      </Button>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Название игры</th>
            <th>Жанр</th>
            <th>Цена</th>
            <th>Дата релиза</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          {games.map((game) => (
            <tr key={game.id}>
              <td>{game.name}</td>
              <td>{game.genre}</td>
              <td>{game.price}</td>
              <td>{game.releaseDate}</td>
              <td>
                <Button variant="warning" onClick={() => handleEditGame(game)}>
                  Редактировать
                </Button>{" "}
                <Button variant="danger" onClick={() => handleDeleteGame(game.id)}>
                  Удалить
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      {/* Модальное окно для создания/редактирования игры */}
      <Modal show={showModal} onHide={handleCloseModal}>
        <Modal.Header closeButton>
          <Modal.Title>{selectedGame ? "Редактировать игру" : "Создать игру"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            {errors.gameName && <Alert variant="danger">{errors.gameName}</Alert>}
            <Form.Group controlId="formGameName">
              <Form.Label>Название игры</Form.Label>
              <Form.Control
                type="text"
                value={gameName}
                onChange={(e) => setGameName(e.target.value)}
                isInvalid={!!errors.gameName}
              />
            </Form.Group>

            {errors.selectedGenre && <Alert variant="danger">{errors.selectedGenre}</Alert>}
            <Form.Group controlId="formGameGenre">
              <Form.Label>Жанр</Form.Label>
              <Dropdown>
                <Dropdown.Toggle variant="secondary" id="dropdown-basic">
                  {selectedGenre
                    ? genres.find((genre) => genre.id === selectedGenre)?.name
                    : "Выберите жанр"}
                </Dropdown.Toggle>

                <Dropdown.Menu>
                  {genres.map((genre) => (
                    <Dropdown.Item
                      key={genre.id}
                      onClick={() => setSelectedGenre(genre.id)}
                    >
                      {genre.name}
                    </Dropdown.Item>
                  ))}
                </Dropdown.Menu>
              </Dropdown>
            </Form.Group>

            {errors.gamePrice && <Alert variant="danger">{errors.gamePrice}</Alert>}
            <Form.Group controlId="formGamePrice">
              <Form.Label>Цена</Form.Label>
              <Form.Control
                type="number"
                value={gamePrice}
                onChange={(e) => setGamePrice(e.target.value)}
                isInvalid={!!errors.gamePrice}
              />
            </Form.Group>

            {errors.gameReleaseDate && <Alert variant="danger">{errors.gameReleaseDate}</Alert>}
            <Form.Group controlId="formGameReleaseDate">
              <Form.Label>Дата релиза</Form.Label>
              <Form.Control
                type="date"
                value={gameReleaseDate}
                onChange={(e) => setGameReleaseDate(e.target.value)}
                isInvalid={!!errors.gameReleaseDate}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleCloseModal}>
            Закрыть
          </Button>
          <Button variant="primary" onClick={handleSaveGame}>
            {selectedGame ? "Сохранить изменения" : "Создать игру"}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}

export default App;
