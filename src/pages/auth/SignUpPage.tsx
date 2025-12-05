import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Particles } from '../../components/Particles';
import { PasswordInput } from '../../components/ui/PasswordInput';
import authService from '../../services/authService';
import { sanitizeInput } from '../../utils/validation';
import '../../styles/auth.css';

export const SignUpPage: React.FC = () => {
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [submitted, setSubmitted] = useState(false);

    const handleSignup = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitted(true);

        if (password !== confirmPassword) {
            return;
        }

        const success = await authService.signup({ username, password });

        if (success) {
            navigate('/main');
        }
    };

    return (
        <>
            <Particles />
            <div className="login-container">
                <div className="login-box">
                    <h2>Sign Up</h2>
                    <p className="welcome-text">Create your account ✨</p>

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
                            placeholder="Password"
                            required
                        />

                        <PasswordInput
                            value={confirmPassword}
                            onChange={(value: string) => setConfirmPassword(sanitizeInput(value))}
                            placeholder="Confirm Password"
                            required
                        />

                        {submitted && password !== confirmPassword && (
                            <p className="error-text">Password doesn't match</p>
                        )}

                        <button onClick={handleSignup} className="login-button">
                            Sign Up
                        </button>
                    </div>

                    <p className="signup-text">
                        Already have an account?{' '}
                        <button onClick={() => navigate('/login')} className="link">
                            Login
                        </button>
                    </p>
                </div>
            </div>
        </>
    );
};