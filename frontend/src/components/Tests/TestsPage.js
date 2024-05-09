import React, { useState, useEffect } from 'react';
import Tests from './Tests';
import { useNavigate } from 'react-router-dom';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import useAuth from "../../hooks/UseAuth";

const TestsPage = () => {
    const navigate = useNavigate();
    const axiosPrivate = useAxiosPrivate();
    const [timer, setTimer] = useState([]);
    const { auth } = useAuth();

    const canAccess = () => {
        if (timer.timer == null) return false;
        
        console.log(new Date(timer.timer) > new Date());
        return new Date(timer.timer) > new Date();
    };

    const canAccessDoctor = auth.roles.includes("Doctor") && !auth.roles.includes("Admin");

    useEffect(() => {
        const fetchTimer = async () => {
            try {
                const response = await axiosPrivate.get('/testTimer');
                setTimer(response.data);
            } catch (error) {
                console.error("Error fetching tests:", error);
            }
        };

        fetchTimer();
    }, []);

    const createTest = () => {
        // Navigate to the Create Test page
        navigate('/tests/newTest');
    };

    return (
        <>
            <Title />
            <NavBar />
            <section>                
                <div className="page-header">
                    {canAccess() && (
                        <button onClick={createTest} className="create-button-v1">
                            New Test
                        </button>
                    )}
                </div>
                {canAccessDoctor && (
                    <Tests />
                )}                
            </section>
            <Footer />
        </>
        
    );
};

export default TestsPage;
