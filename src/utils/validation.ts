export const sanitizeInput = (value: string): string => {
    return value.replace(/[^a-zA-Z0-9]/g, '');
};

export const validatePassword = (password: string): boolean => {
    return password.length >= 6;
};

export const validateUsername = (username: string): boolean => {
    return username.length >= 3;
};