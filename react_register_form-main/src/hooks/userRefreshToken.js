import axios from '../api/axios';
import useAuth from './UseAuth';

const useRefreshToken = () => {
    const { setAuth, auth } = useAuth();

    const refresh = async () => {
        const response = await axios.post('/accessToken', {
            refreshToken : auth.refreshToken
        }, {
            withCredentials: true,
            headers: {
                'Content-Type': 'application/json' // Set Content-Type header
            }
        });
        setAuth(prev => ({
            ...prev,
            accessToken: response.data.accessToken
        }));
        return response.data.accessToken;
    }
    return refresh;
};

export default useRefreshToken;