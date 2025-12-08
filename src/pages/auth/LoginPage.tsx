import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Particles } from '../../components/Particles';
import { PasswordInput } from '../../components/ui/PasswordInput';
import authService from '../../services/authService';
import { sanitizeInput } from '../../utils/validation';
import '../../styles/auth.css';

export const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();

        const success = await authService.login({ username, password });

        if (success) {
            navigate('/main');
        } else {
            alert('Login failed');
        }
    };

    return (
        <>
            <Particles />
            <div className="login-container">
                <div className="login-box">
                    <h2>Login</h2>
                    <p className="welcome-text">Welcome back 👋</p>

                    <div>
                        <input
                            required
                            value={username}
                            type="text"
                            placeholder="Username"
                            className="input-field"
                            onChange={(e) => setUsername(sanitizeInput(e.target.value))}
                        />

                        <PasswordInput
                            value={password}
                            onChange={(value: string) => setPassword(sanitizeInput(value))}
                            required
                        />

                        <button onClick={handleLogin} className="login-button">
                            Login
                        </button>
                    </div>

                    <p className="signup-text">
                        Don't have an account?{' '}
                        <button onClick={() => navigate('/signup')} className="link">
                            Sign up
                        </button>
                    </p>
                </div>
            </div>
        </>
    );
};