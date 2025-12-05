export interface LoginCredentials {
    username: string;
    password: string;
}

export interface SignupCredentials extends LoginCredentials {
    confirmPassword: string;
}

export interface User {
    id: string;
    username: string;
}