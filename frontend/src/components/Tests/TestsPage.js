import React, { useState, useEffect } from 'react';
import Tests from './Tests';
import { useNavigate } from 'react-router-dom';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import useAuth from "../../hooks/UseAuth";

const TestsPage = () => {
    const navigate = useNavigate();
    const axiosPrivate = useAxiosPrivate();
    const [timer, setTimer] = useState([]);
    const { auth } = useAuth();

    const canAccess = () => {
        return timer != null && timer > new Date().toUTCString();
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
            <NavBar />
            <section>                
                <div className="page-header">
                    {canAccess() && (
                        <button onClick={createTest} className="create-button-v1"> {/* Button to create a test */}
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
