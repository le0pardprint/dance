import React, { useState, useEffect } from 'react';
import './App.css';

const API_URL = 'https://localhost:7127/api';

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [role, setRole] = useState(null);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [data, setData] = useState(null);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const userRole = localStorage.getItem('role');
        if (token) {
            setIsAuthenticated(true);
            setRole(userRole);
        }
    }, []);

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const response = await fetch(`${API_URL}/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });
            const data = await response.json();
            if (response.ok) {
                localStorage.setItem('token', data.token);
                localStorage.setItem('role', data.role);
                localStorage.setItem('userId', data.userId);
                setIsAuthenticated(true);
                setRole(data.role);
            } else {
                setError(data.message || 'Ошибка входа');
            }
        } catch (err) {
            setError('Ошибка подключения к серверу');
        }
    };

    const handleLogout = () => {
        localStorage.clear();
        setIsAuthenticated(false);
        setRole(null);
        setData(null);
    };

    const fetchClientData = async () => {
        const token = localStorage.getItem('token');
        try {
            const response = await fetch(`${API_URL}/client/schedule`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            const data = await response.json();
            setData(data);
        } catch (err) {
            console.error('Ошибка:', err);
        }
    };

    if (!isAuthenticated) {
        return (
            <div className="container">
                <div className="card">
                    <h2>Вход в CRM школу танцев</h2>
                    {error && <div className="error">{error}</div>}
                    <form onSubmit={handleLogin}>
                        <input
                            type="email"
                            placeholder="Email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                        <input
                            type="password"
                            placeholder="Пароль"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                        <button type="submit">Войти</button>
                    </form>
                </div>
            </div>
        );
    }

    return (
        <div>
            <div className="navbar">
                <h2>Танцевальная школа CRM</h2>
                <div>
                    <span>Роль: {role} | </span>
                    <button onClick={fetchClientData}>Мои данные</button>
                    <button onClick={handleLogout}>Выйти</button>
                </div>
            </div>
            <div className="container">
                {data && (
                    <div className="card">
                        <h3>Мои данные</h3>
                        <pre>{JSON.stringify(data, null, 2)}</pre>
                    </div>
                )}
                {!data && (
                    <div className="card">
                        <p>Нажмите "Мои данные", чтобы загрузить информацию</p>
                    </div>
                )}
            </div>
        </div>
    );
}

export default App;