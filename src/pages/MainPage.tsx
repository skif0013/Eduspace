import React from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../services/authService';

export const MainPage: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        authService.logout();
        navigate('/login');
    };

    return (
        <div style={{ padding: '40px', textAlign: 'center' }}>
            <h1>Main Page</h1>
            <p>You are logged in! 🎉</p>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
};