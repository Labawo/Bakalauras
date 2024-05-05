import { useState, useEffect } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation, useParams } from "react-router-dom";
import useAuth from "../../hooks/UseAuth";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch, faEdit } from '@fortawesome/free-solid-svg-icons';

const Recommendations = () => {
    const [recommendations, setRecommendations] = useState([]);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();
    const { auth } = useAuth();
    const { therapyId, appointmentId } = useParams();

    const handleInspect = (recommendationId) => {
        navigate(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/${recommendationId}`);
    };

    const [therapy, setTherapy] = useState(null);

    useEffect(() => {
        let isMounted = true;
        const controller = new AbortController();

        const getRecommendations = async () => {
            try {
                const response = await axiosPrivate.get(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations`, {
                    signal: controller.signal
                });
                isMounted && setRecommendations(response.data);
            } catch (err) {
                console.error(err);
                navigate('/login', { state: { from: location }, replace: true });
            }
        };

        const getTherapy = async () => {
            try {
                const response = await axiosPrivate.get(`/therapies/${therapyId}`, {
                    signal: controller.signal
                });
                isMounted && setTherapy(response.data.resource);
            } catch (err) {
                console.error(err);
                navigate('/login', { state: { from: location }, replace: true });
            }
        };

        getRecommendations();
        getTherapy();

        return () => {
            isMounted = false;
            controller.abort();
        };
    }, [axiosPrivate, navigate, location, therapyId, appointmentId]);

    const doctorId = therapy ? therapy.doctorId : null;
    const userId = auth.id;
    const canEditDelete = userId === doctorId || auth.roles.includes("Admin");

    const createRecommendation = () => {
        navigate(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/createRecommendation`);
    };

    const updateRecommendation = (recommendationId) => {
        navigate(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/${recommendationId}/editRecommendation`);
    };

    const deleteRecommendation = async (recommendationId) => {
        try {
            await axiosPrivate.delete(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/${recommendationId}`);
            setRecommendations(prevRecommendations =>
                prevRecommendations.filter(recommendation => recommendation.id !== recommendationId)
            );
        } catch (error) {
            console.error(`Error deleting recommendation ${recommendationId}:`, error);
        }
    };

    return (
        <article>
            <div className="table-container">
                <h2 className="list-headers">Recommendations List</h2>
                {canEditDelete && (
                    <button onClick={createRecommendation} className="create-button-v1"> Create Recommendation </button>
                )}
                {recommendations.length ? (
                    <table className="my-table">
                        <thead>
                            <tr>
                                <th>Description</th>
                                <th>Date</th>
                                <th>Time</th>
                            </tr>
                        </thead>
                        <tbody>
                            {recommendations.map((recommendation, i) => (
                                <tr key={i}>
                                    <td>{recommendation?.description}</td>
                                    <td>{recommendation?.time.split('T')[0]}</td>
                                    <td>{recommendation?.time.split('T')[1].slice(0, 5)}</td>
                                    <td>
                                        {canEditDelete && (
                                            <>
                                                <button
                                                    className="table-buttons-blue"
                                                    onClick={() => handleInspect(recommendation.id)}
                                                >
                                                    <FontAwesomeIcon icon={faSearch} />
                                                </button>
                                                <button
                                                    className="table-buttons-blue"
                                                    onClick={() => updateRecommendation(recommendation.id)}
                                                >
                                                    <FontAwesomeIcon icon={faEdit} />
                                                </button>
                                                <button
                                                    className="table-buttons-red"
                                                    onClick={() => deleteRecommendation(recommendation.id)}
                                                >
                                                    <FontAwesomeIcon icon={faTrash} />
                                                </button>
                                            </>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p className="no-list-items-p">No recommendations to display</p>
                )}
            </div>
        </article>
    );
};

export default Recommendations;
