import React, { useState, useEffect } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faKey } from '@fortawesome/free-solid-svg-icons';

const Users = () => {
    const [users, setUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [filter, setFilter] = useState('');
    const [selectedUserId, setSelectedUserId] = useState(null);
    const [newPassword, setNewPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const [showResetPassword, setShowResetPassword] = useState({}); // Object to track visibility of reset password div for each user
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const controller = new AbortController();
        const getUsers = async () => {
            try {
                const response = await axiosPrivate.get('/users', {
                    signal: controller.signal
                });
                setUsers(response.data);
                // Initialize showResetPassword state for each user
                const initialShowResetPassword = {};
                response.data.forEach(user => {
                    initialShowResetPassword[user.id] = false;
                });
                setShowResetPassword(initialShowResetPassword);
            } catch (err) {
                console.error(err);
                navigate('/login', { state: { from: location }, replace: true });
            }
        };

        getUsers();

        return () => {
            controller.abort();
        };
    }, [axiosPrivate, navigate, location]);

    useEffect(() => {
        setFilteredUsers(users.filter(user => user.userName.toLowerCase().includes(filter.toLowerCase())));
    }, [users, filter]);

    const validatePassword = (password) => {
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
        return passwordRegex.test(password);
    };

    const handlePasswordChange = (e) => {
        setNewPassword(e.target.value);
        if (repeatPassword && e.target.value !== repeatPassword) {
            setPasswordError("Passwords do not match");
        } else {
            setPasswordError("");
        }
    };

    const handleRepeatPasswordChange = (e) => {
        setRepeatPassword(e.target.value);
        if (newPassword && e.target.value !== newPassword) {
            setPasswordError("Passwords do not match");
        } else {
            setPasswordError("");
        }
    };

    const resetPassword = async () => {
        if (newPassword !== repeatPassword) {
            setPasswordError("Passwords do not match");
            return;
        }
        if (!validatePassword(newPassword)) {
            setPasswordError("Password must contain at least 8 characters, including uppercase, lowercase, number, and special symbol.");
            return;
        }
        try {
            await axiosPrivate.put(`/changePassword/${selectedUserId}`, { newPassword });
            alert("Password changed successfully.");
            setShowResetPassword({ ...showResetPassword, [selectedUserId]: false }); // Hide reset password div after success
            setNewPassword(''); // Clear newPassword state
            setRepeatPassword(''); // Clear repeatPassword state
        } catch (error) {
            console.error(`Error resetting password for user ${selectedUserId}:`, error);
            alert("Error changing password. Please try again.");
        }
    };    

    const deleteUser = async (userId) => {
        const confirmDelete = window.confirm("Are you sure you want to delete this user?");
        if (confirmDelete) {
            try {
                await axiosPrivate.delete(`/users/${userId}`);
                setUsers(prevUsers => prevUsers.filter(user => user.id !== userId));
            } catch (error) {
                console.error(`Error deleting user ${userId}:`, error);
            }
        }
    };

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
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.map((user, i) => (
                                <tr key={i}>
                                    <td>{user?.userName}</td>
                                    <td>{user?.email}</td>
                                    <td>
                                        <button
                                            className="table-buttons-red"
                                            onClick={() => deleteUser(user.id)}
                                        >
                                            <FontAwesomeIcon icon={faTrash} />
                                        </button>
                                        <button
                                            className="table-buttons-gold"
                                            onClick={() => {
                                                setSelectedUserId(user.id);
                                                setShowResetPassword({ ...showResetPassword, [user.id]: !showResetPassword[user.id] }); // Toggle visibility of reset password div
                                            }}
                                        >
                                            <FontAwesomeIcon icon={faKey} />
                                        </button>
                                        
                                    </td>
                                    
                                    {showResetPassword[user.id] && (
                                        <div className="password-reset-container">
                                            <input
                                                type="password"
                                                value={newPassword}
                                                onChange={handlePasswordChange}
                                                placeholder="New Password"
                                            /> <br />
                                            <input
                                                type="password"
                                                value={repeatPassword}
                                                onChange={handleRepeatPasswordChange}
                                                placeholder="Repeat Password"
                                            /> <br />
                                            <button
                                                className="table-buttons-blue"
                                                onClick={resetPassword}
                                            >
                                                Submit
                                            </button>
                                            {passwordError && <p className="error-message">{passwordError}</p>}
                                        </div>
                                    )}    
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : <p>No users to display</p>}
            </div>
        </article>
    );
};

export default Users;
