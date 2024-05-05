import axios from 'axios';
const BASE_URL = 'http://localhost:5014/api'

export default axios.create({
    baseURL: BASE_URL
});

export const axiosPrivate = axios.create({
    baseURL: BASE_URL,
    headers: { 'Conten-Type' : 'application/json' },
    withCredentials : true
});