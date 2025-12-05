import React, { useState } from 'react';
import { Eye, EyeOff } from 'lucide-react';

interface PasswordInputProps {
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
    required?: boolean;
}

export const PasswordInput: React.FC<PasswordInputProps> = ({
                                                                value,
                                                                onChange,
                                                                placeholder = 'Password',
                                                                required = false,
                                                            }) => {
    const [showPassword, setShowPassword] = useState(false);

    return (
        <div className="password-field">
            <input
                required={required}
                value={value}
                type={showPassword ? 'text' : 'password'}
                placeholder={placeholder}
                onChange={(e) => onChange(e.target.value)}
            />
            <button
                type="button"
                className="eye-btn"
                onClick={() => setShowPassword(!showPassword)}
            >
                {showPassword ? <Eye className="eye-icon" /> : <EyeOff className="eye-icon" />}
            </button>
        </div>
    );
};