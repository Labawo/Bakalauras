import React, { useState, useEffect } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import SuccessSelectModal from "../Modals/SuccessSelectModal";

const Patients = () => {
    const [users, setUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [filter, setFilter] = useState('');
    const [successMessage, setSuccessMessage] = useState("");
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const controller = new AbortController();
        const getUsers = async () => {
            try {
                const response = await axiosPrivate.get('/patients', {
                    signal: controller.signal
                });
                setUsers(response.data);
                console.log(users);
            } catch (err) {
                console.error(err);
                navigate('/login', { state: { from: location }, replace: true });
            }
        };

        getUsers();

        return () => {
            controller.abort();
        };
    }, [axiosPrivate, navigate, location, successMessage]);

    useEffect(() => {
        setFilteredUsers(users.filter(user => user.userName.toLowerCase().includes(filter.toLowerCase())));
    }, [users, filter]);

    const handleTestAllowance = async (patientId, patientName) => {
        try {
            await axiosPrivate.put(`/allowTest/${patientId}`);
            setSuccessMessage(`You allowed tests for patient ${patientName}`);
        } catch (error) {
            console.error("Error alowing test for user:", error);
        }
    };

    const handleTestRestriction = async (patientId, patientName) => {
        try {
            await axiosPrivate.put(`/restrictTest/${patientId}`);
            setSuccessMessage(`You restricted tests for patient ${patientName}`);
        } catch (error) {
            console.error("Error alowing test for user:", error);
        }
    };

    const canDoTest = (time) => {
        return new Date(time) > new Date();
    }

    return (
        <article>
            <div className="table-container">
                <h2 className="list-headers">Users List</h2>
                <div className="filter-container">
                    <input
                        type="text"
                        value={filter}
                        onChange={(e) => setFilter(e.target.value)}
                        placeholder="Filter by username"
                    />
                </div>
                {filteredUsers.length ? (
                    <table className="my-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Test Timer</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.map((user, i) => (
                                <tr key={i}>
                                    <td>{user?.userName}</td>
                                    <td>{user?.email}</td>
                                    <td className={canDoTest(user.testTimer) ? 'green' : 'red'}>{user?.testTimer.split('T')[0] + " " + user?.testTimer.split('T')[1].slice(0, 5)}</td>
                                    <td>
                                        <button className="table-buttons-green allowance" onClick={() => handleTestAllowance(user.id, user.userName)}>
                                            Allow test
                                        </button>
                                        <button className="table-buttons-red restriction" onClick={() => handleTestRestriction(user.id, user.userName)}>
                                            Restrict test
                                        </button>
                                    </td>    
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : <p>No patients to display.</p>}
            </div>
            <SuccessSelectModal
                show={successMessage !== ""}
                onClose={() => setSuccessMessage("")}
                message={successMessage}
            />
        </article>
    );
};

export default Patients;
