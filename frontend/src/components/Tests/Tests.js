import React, { useState, useEffect, useCallback } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import useAuth from "../../hooks/UseAuth";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch } from '@fortawesome/free-solid-svg-icons';
import ConfirmationModal from "../Modals/ConfirmationModal";
import ErrorModal from "../Modals/ErrorModal";
import TestInspect from "./TestInspect";

const Tests = () => {
    const [tests, setTests] = useState([]);
    const [page, setPage] = useState(2);
    const [isLoading, setIsLoading] = useState(false);
    const [patients, setPatients] = useState([]);
    const [selectedPatientId, setSelectedPatientId] = useState("");
    const [selectedPatientUsername, setSelectedPatientUsername] = useState("");
    const axiosPrivate = useAxiosPrivate();
    const { auth } = useAuth();
    const isAdmin = auth.roles.includes("Doctor") && !auth.roles.includes("Admin");
    const [deleteId, setDeleteId] = useState("");
    const [test, setTest] = useState(null);
    const [errorMessage, setErrorMessage] = useState("");

    const fetchTests = useCallback(async (pageNumber, patientId) => {
        try {
            const response = await axiosPrivate.get('/tests', {
                params: { pageNumber: pageNumber, patientId: patientId },
            });
            return response.data;
        } catch (err) {
            console.error(err);
            return [];
        }
    }, [axiosPrivate]);

    const fetchPatients = useCallback(async () => {
        try {
            const response = await axiosPrivate.get("/patients");
            setPatients(response.data);
        } catch (error) {
            console.error("Error fetching patients:", error);
        }
    }, [axiosPrivate]);

    useEffect(() => {
        if (isAdmin) {
            fetchPatients();
        } else {
            loadTests();
        }
    }, [isAdmin, fetchPatients]);

    useEffect(() => {
        if (selectedPatientId) {
            setPage(2); 
            setTests([]); 
            loadTests(); 
        }
    }, [selectedPatientId]);

    const loadTests = useCallback(async () => {
        if (isLoading) return;
    
        setIsLoading(true);
        
        const data = await fetchTests(1, selectedPatientId);
        setTests(data);
        setIsLoading(false);
    }, [fetchTests, isLoading, selectedPatientId]);

    const handleInspect = async (testId) => {
        try {
            const response = await axiosPrivate.get(`/tests/${testId}`);
            console.log(response.data.resource)
            setTest(response.data.resource);
        } catch (error) {
            console.error(error);
            setErrorMessage("Failed to inspect.")
        }
    };
    
    const loadNextPageTests = useCallback(async () => {
        if (isLoading) return;
    
        setIsLoading(true);
    
        const data = await fetchTests(page, selectedPatientId); 
        setTests(prevTests => [...prevTests, ...data]); 
        setPage(prevPage => prevPage + 1); 
        setIsLoading(false);
    }, [fetchTests, isLoading, selectedPatientId, page]);

    const removeTest = async (testId) => {
        try {
            await axiosPrivate.delete(`/tests/${testId}`);
            setTests(prevTests =>
                prevTests.filter(test => test.id !== testId)
            );
            setDeleteId("");
        } catch (error) {
            console.error(`Error removing test ${testId}:`, error);
        }
    };

    const handlePatientSelect = (e) => {
        const selectedOption = e.target.value;
        const [patientId, username] = selectedOption.split('|');
        setSelectedPatientId(patientId);
        setSelectedPatientUsername(username);
    }; 

    return (
        <article className="tests-container">
            {isAdmin && patients.length > 0 && ( 
                <>
                    <div className="patient-selection">
                        <h2 className="list-headers">HAD Tests</h2>
                        <label htmlFor="patientSelect">Select Patient:</label>
                        <select id="patientSelect" onChange={handlePatientSelect}>
                            <option value="">Select Patient</option>
                            {patients.map((patient) => (
                                <option key={patient.id} value={`${patient.id}|${patient.userName}`}>
                                    {patient.userName}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="table-container">
                        
                        {tests.length ? (
                            <table className="my-table">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {tests.map((test, i) => (
                                        <tr key={i}>
                                            <td>{test?.name}</td>
                                            {isAdmin && (
                                                <td>
                                                <button 
                                                    className="table-buttons-blue"
                                                    onClick={() => handleInspect(test.id)}
                                                >
                                                    <FontAwesomeIcon icon={faSearch} />
                                                </button>
                                                    <button
                                                        className="table-buttons-red"
                                                        onClick={() => setDeleteId(test.id)}
                                                    >
                                                        <FontAwesomeIcon icon={faTrash} />
                                                    </button>
                                                </td>
                                            )}
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <p className="no-list-items-p">No tests to display</p>
                        )}
                        {isLoading ? (
                            <p>Loading...</p>
                        ) : tests.length > 2 ? (
                            <button onClick={loadNextPageTests} className="load-button-v1">Load More</button>
                        ) : null}
                    </div>
                </>
            )}
            <ConfirmationModal 
                show={deleteId !== ""}
                onClose={() => setDeleteId("")}
                onConfirm={() => removeTest(deleteId)}
                message={"Are you sure you want to delete test?"}
            />
            <ErrorModal
                show={errorMessage !== ""}
                onClose={() => setErrorMessage("")}
                message={errorMessage}
            />
            <TestInspect
                user= {selectedPatientUsername}
                show={test !== null}
                onClose={() => setTest(null)}
                message={test ? test.depressionScore : ""}
                message2={test ? test.anxietyScore : ""}
            />
        </article>
    );
};

export default Tests;
