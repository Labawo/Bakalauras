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

    const canAccessPatient = auth.roles.includes("Patient") && !auth.roles.includes("Admin");

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
                    {canAccessPatient && (
                        <div className='test-description'>
                            <h2>HAD Test</h2>
                            <div>
                                <p>
                                    The Hospital Anxiety and Depression Scale (HADS) is a widely used self-assessment questionnaire designed to measure levels of anxiety and depression in individuals. Consisting of 14 items, the HADS assesses symptoms such as nervousness, restlessness, sadness, and loss of interest or pleasure. It provides separate scores for anxiety and depression, allowing for the identification of specific mental health concerns. The HADS is commonly utilized in healthcare settings, particularly in hospitals, to screen patients for anxiety and depression. Its brevity, simplicity, and focus on emotional symptoms make it a valuable tool for healthcare professionals in assessing and monitoring mental health conditions. Overall, the HADS facilitates early detection and intervention for anxiety and depression, ultimately contributing to improved patient care and well-being.
                                </p>
                            </div>
                            {canAccess() && (
                                <div>
                                    <p className='center-testp'>Try taking test right now:</p>
                                    <div className='had-create-div'>
                                        <button onClick={createTest} className="create-button-v1 had-test-create-btn">
                                            New Test
                                        </button>
                                    </div>
                                </div>
                            )}
                        </div>
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
