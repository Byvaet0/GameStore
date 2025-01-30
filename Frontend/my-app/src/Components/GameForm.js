import { useState, useEffect } from 'react';

function GameForm({ game, genres, onSave, onClose }) {
  const [formData, setFormData] = useState({
    name: game ? game.name : '',
    genreId: game ? game.genreId : '',
    price: game ? game.price : '',
    releaseDate: game ? game.releaseDate : '',
  });
  
  const [errors, setErrors] = useState({});
  
  useEffect(() => {
    if (game) {
      setFormData({
        name: game.name,
        genreId: game.genreId,
        price: game.price,
        releaseDate: game.releaseDate,
      });
    }
  }, [game]);
  
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({ ...prevData, [name]: value }));
  };
  
  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.name.trim()) {
      newErrors.name = 'Name is required';
    }
    
    if (!formData.genreId) {
      newErrors.genreId = 'Genre is required';
    }
    
    if (!formData.price || isNaN(formData.price) || formData.price <= 0) {
      newErrors.price = 'Price must be a positive number';
    }
    
    if (!formData.releaseDate) {
      newErrors.releaseDate = 'Release date is required';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };
  
  const handleSubmit = (e) => {
    e.preventDefault();
    
    // Проверка на ошибки
    if (!validateForm()) {
      return;
    }
    
    onSave(formData);
    onClose();
  };
  
  return (
    <div className="modal">
      <h2>{game ? 'Edit Game' : 'Create Game'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Name</label>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleChange}
            className={errors.name ? 'error' : ''}
          />
          {errors.name && <div className="error-message">{errors.name}</div>}
        </div>

        <div className="form-group">
          <label>Genre</label>
          <select
            name="genreId"
            value={formData.genreId}
            onChange={handleChange}
            className={errors.genreId ? 'error' : ''}
          >
            <option value="">Select Genre</option>
            {genres.map((genre) => (
              <option key={genre.id} value={genre.id}>
                {genre.name}
              </option>
            ))}
          </select>
          {errors.genreId && <div className="error-message">{errors.genreId}</div>}
        </div>

        <div className="form-group">
          <label>Price</label>
          <input
            type="number"
            name="price"
            value={formData.price}
            onChange={handleChange}
            className={errors.price ? 'error' : ''}
          />
          {errors.price && <div className="error-message">{errors.price}</div>}
        </div>

        <div className="form-group">
          <label>Release Date</label>
          <input
            type="date"
            name="releaseDate"
            value={formData.releaseDate}
            onChange={handleChange}
            className={errors.releaseDate ? 'error' : ''}
          />
          {errors.releaseDate && <div className="error-message">{errors.releaseDate}</div>}
        </div>

        <div className="form-buttons">
          <button type="button" onClick={onClose}>Cancel</button>
          <button type="submit">{game ? 'Save Changes' : 'Create Game'}</button>
        </div>
      </form>
    </div>
  );
}

export default GameForm;

//