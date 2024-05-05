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
    const [tests, setTests] = useState([]);
    const { auth } = useAuth();

    const canAccess = () => {
        const currentDate = new Date();
        let lastTestDateTime = null;

        // Find the creation date of the last test
        if (tests.length > 0) {
            lastTestDateTime = new Date(tests[0].Time); // Assuming createdAt contains the creation date
        }

        console.log(tests[0].time);

        const differenceInDays = lastTestDateTime
            ? Math.floor((currentDate - lastTestDateTime) / (1000 * 60 * 60 * 24))
            : null;

        console.log(differenceInDays);

        return !auth.roles.includes("Doctor") && (differenceInDays === null || differenceInDays >= 7);
    };

    useEffect(() => {
        const fetchTests = async () => {
            try {
                const response = await axiosPrivate.get('/tests');
                setTests(response.data);
            } catch (error) {
                console.error("Error fetching tests:", error);
            }
        };

        fetchTests();
    }, [axiosPrivate]);

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
                <Tests />
            </section>
            <Footer />
        </>
        
    );
};

export default TestsPage;
