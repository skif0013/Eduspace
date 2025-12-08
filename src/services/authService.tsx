interface LoginCredentials {
    username: string;
    password: string;
}

class AuthService {
    async login(credentials: LoginCredentials): Promise<boolean> {
        try {
            console.log('Login attempt:', credentials);
            localStorage.setItem('token', 'dummy-token');
            return true;
        } catch (error) {
            console.error('Login error:', error);
            return false;
        }
    }

    async signup(credentials: LoginCredentials): Promise<boolean> {
        try {
            console.log('Signup attempt:', credentials);
            localStorage.setItem('token', 'dummy-token');
            return true;
        } catch (error) {
            console.error('Signup error:', error);
            return false;
        }
    }

    logout(): void {
        localStorage.removeItem('token');
    }

    isAuthenticated(): boolean {
        return !!localStorage.getItem('token');
    }
}

export default new AuthService();