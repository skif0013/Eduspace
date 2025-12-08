import React from 'react';

export const Particles: React.FC = () => {
    return (
        <div
            style={{
                position: 'fixed',
                top: 0,
                left: 0,
                width: '100%',
                height: '100%',
                zIndex: 1,
                pointerEvents: 'none',
                background:
                    'radial-gradient(circle at 20% 50%, rgba(68, 124, 215, 0.1) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(138, 43, 226, 0.1) 0%, transparent 50%)',
            }}
        />
    );
};