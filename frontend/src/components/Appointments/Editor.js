import React, { useState, useEffect } from "react";
import NavBar from "./../Main/NavBar";
import Title from "./../Main/Title";
import Footer from "./../Main/Footer";
import useAxiosPrivate from "./../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch } from '@fortawesome/free-solid-svg-icons';
import ConfirmationModal from "../Modals/ConfirmationModal";
import SuccessSelectModal from "../Modals/SuccessSelectModal";

const Editor = () => {
    const [appointments, setAppointments] = useState([]);
    const [startDate, setStartDate] = useState("");
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();
    const [successMessage, setSuccessMessage] = useState("");
    const [deleteId, setDeleteId] = useState("");
    
    const endDate = startDate ? new Date(new Date(startDate).getTime() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0] : "";

    useEffect(() => {
        let isMounted = true;
        const controller = new AbortController();

        const getAppointments = async () => {
            try {
                const [appointmentsResponse] = await Promise.all([
                    axiosPrivate.get(`/getWeeklyAppointments`, {
                        signal: controller.signal,
                    }),
                ]);
                isMounted && setAppointments(appointmentsResponse.data);
            } catch (err) {
                console.error(err);
                navigate('/login', { state: { from: location }, replace: true });
            }
        };

        getAppointments();

        return () => {
            isMounted = false;
            controller.abort();
        };
    }, [axiosPrivate, navigate, location]);

    const handleDeleteAppointment = async (appointmentId) => {
        try {
            await axiosPrivate.delete(`/getWeeklyAppointments/${appointmentId}`);
            setAppointments(prevAppointments => prevAppointments.filter(appointment => appointment.id !== appointmentId));
            setDeleteId("");
        } catch (error) {
            console.error("Error deleting appointment:", error);
        }
    };

    const handleInspect = (therapyId, appintmentId) => {
        navigate(`/therapies/${therapyId}/appointments/${appintmentId}`);
    };

    const filteredAppointments = appointments.filter(appointment => {
        if (startDate) {
            const appointmentDate = new Date(appointment.time.split('T')[0]);
            return appointmentDate >= new Date(startDate) && appointmentDate <= new Date(endDate);
        }
        return true;
    });

    return (
        <>
            <Title />
            <NavBar />
            <section>               
                <div className="table-container">
                    <h2>My Appointments List</h2>
                    <div className="filter-container">
                        <label htmlFor="startDate">Start Date:</label>
                        <input 
                            type="date" 
                            id="startDate" 
                            value={startDate} 
                            onChange={(e) => setStartDate(e.target.value)} 
                        />
                    </div>
                    {filteredAppointments.length ? (
                        <table className="my-table">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Time</th>
                                    <th>Patient</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredAppointments.map((appointment, i) => (
                                    <tr key={i}>
                                        <td>{appointment?.time.split('T')[0]}</td>
                                        <td>{appointment?.time.split('T')[1].slice(0, 5)}</td>
                                        <td>{appointment?.patientName}</td>
                                        <td>
                                            <button 
                                                className="table-buttons-blue"
                                                onClick={() => handleInspect(appointment.therapyId, appointment.id)}
                                            >
                                                <FontAwesomeIcon icon={faSearch} />
                                            </button>
                                            <button
                                                    className="table-buttons-red"
                                                    onClick={() => setDeleteId(appointment.id)} // Invoke deleteAppointment on click
                                                >
                                                    <FontAwesomeIcon icon={faTrash} />
                                                </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <p>No appointments to display</p>
                    )}
                </div>
                <ConfirmationModal 
                    show={deleteId !== ""}
                    onClose={() => setDeleteId("")}
                    onConfirm={() => handleDeleteAppointment(deleteId)}
                    message={"Are you sure you want to delete appointment?"}
                />
                <SuccessSelectModal
                    show={successMessage !== ""}
                    onClose={() => setSuccessMessage("")}
                    message={successMessage}
                />
            </section>
            <Footer />
        </>
    )
}

export default Editor;
