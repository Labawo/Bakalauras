import { useState, useEffect } from "react";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSearch } from '@fortawesome/free-solid-svg-icons';

const AppointmentsPage = () => {

    const [appointments, setAppointments] = useState([]);
    const [recommendations, setRecommendations] = useState([]);
    const [showPopup, setShowPopup] = useState(false); // State to control the popup visibility
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();
    const [startDate, setStartDate] = useState("");

    const endDate = startDate ? new Date(new Date(startDate).getTime() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0] : "";

    useEffect(() => {
        let isMounted = true;
        const controller = new AbortController();

        const getAppointments = async () => {
            try {
                const [appointmentsResponse] = await Promise.all([
                    axiosPrivate.get(`/getMyAppointments`, {
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

    const handleInspect = async (appointmentId) => {
        try {
            const response = await axiosPrivate.get(`/getMyAppointments/${appointmentId}/getMyRecommendations`);
            console.log("Recommendations:", response.data); // Log the recommendations data
            setRecommendations(response.data);
            setShowPopup(true);
        } catch (error) {
            console.error("Error fetching recommendations:", error);
        }
    };

    const closePopup = () => {
        setShowPopup(false); // Close the popup
        setRecommendations([]); // Clear recommendations when closing
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
                    <br />
                    <br />
                    {filteredAppointments.length ? (
                        <table className="my-table">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Time</th>
                                    <th>Price</th>
                                    <th>Doctor</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredAppointments.map((appointment, i) => (
                                    <tr key={i}>
                                        <td>{appointment?.time.split('T')[0]}</td>
                                        <td>{appointment?.time.split('T')[1].slice(0, 5)}</td>
                                        <td>{appointment?.price}€</td>
                                        <td>{appointment?.doctorName}</td>
                                        <td>
                                            <button 
                                                className="table-buttons-blue"
                                                onClick={() => handleInspect(appointment.id)}
                                            >
                                                <FontAwesomeIcon icon={faSearch} />
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <p>No appointments to display</p>
                    )}

                    {/* Popup for recommendations */}
                    {showPopup && (
                        <div className={`popup ${showPopup ? 'active' : ''}`} onClick={closePopup}>
                            <div className="popup-content" onClick={(e) => e.stopPropagation()}>
                                <span className="close" onClick={closePopup}>&times;</span>
                                <h3>Recommendations</h3>
                                <table className="recommendations-table">
                                    <thead>
                                    {recommendations.length === 0 ? (
                                            null
                                        ) : (<tr>
                                            <th>Description</th>
                                            <th>Date</th>
                                            <th>Time</th>
                                        </tr>
                                    )}
                                    </thead>
                                        
                                    <tbody>
                                        {recommendations.length === 0 ? (
                                            <tr>
                                                <td colSpan="2">No recommendations available</td>
                                            </tr>
                                        ) : (
                                            recommendations.map((recommendation, index) => (
                                                <tr key={index}>
                                                    <td>{recommendation.description}</td>
                                                    <td>{recommendation?.time.split('T')[0]}</td>
                                                    <td>{recommendation?.time.split('T')[1].slice(0, 5)}</td>
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}
                </div>                
            </section>
            <Footer />
        </>        
    )
}

export default AppointmentsPage