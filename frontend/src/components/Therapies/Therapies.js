import { useState, useEffect, useCallback } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import useAuth from "../../hooks/UseAuth";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch, faEdit } from '@fortawesome/free-solid-svg-icons';
import ConfirmationModal from "../Modals/ConfirmationModal";
import ErrorModal from "../Modals/ErrorModal";

const Therapies = () => {
    const [therapies, setTherapies] = useState([]);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();
    const { auth } = useAuth();
    const [deleteId, setDeleteId] = useState("");
    const [errorMessage, setErrorMessage] = useState("");

    const handleInspect = (therapyId) => {
        navigate(`/therapies/${therapyId}`);
    };

    const canAccessDoctor = auth.roles.includes("Doctor") || auth.roles.includes("Admin");
    
    const canAccessAdminOrCreator = (therapy) => {
        const isAdmin = auth.roles.includes("Admin");
      
        const isCreator = therapy.doctorId === auth.id;
      
        return isAdmin || isCreator;
      };

    const fetchTherapies = useCallback(async (pageNumber) => {
        try {
            const response = await axiosPrivate.get('/therapies', {
                params: { pageNumber : pageNumber }, 
            });
            return response.data;
        } catch (err) {
            console.error(err);
            navigate('/login', { state: { from: location }, replace: true });
            return [];
        }
    }, [axiosPrivate, navigate, location]);

    const loadTherapies = async () => {
        if (isLoading) return;

        setIsLoading(true);
        const data = await fetchTherapies(page);
        console.log(data)
        setTherapies(prevTherapies => [...prevTherapies, ...data]);
        setPage(prevPage => prevPage + 1);
        setIsLoading(false);
    };

    useEffect(() => {
        loadTherapies();
    }, []); 

    const createTherapy = () => {
        navigate(`/therapies/createTherapy`);
    };

    const updateTherapy = (therapyId) => {
        navigate(`/therapies/${therapyId}/editTherapy`);
    };

    const removeTherapy = async (therapyId) => {
        try {
          await axiosPrivate.delete(`/therapies/${therapyId}`);
          setTherapies(prevTherapies =>
            prevTherapies.filter(therapy => therapy.id !== therapyId)
          );
          setDeleteId("");
        } catch (error) {
          console.error(`Error removing therapy ${therapyId}:`, error);
          setErrorMessage("Error removing therapy.")
          setDeleteId("");
        }
      };

    return (
        <article className="therapies-container">
            <div className="table-container">
                <h2 className="list-headers">Therapies List</h2>
                {canAccessDoctor && (
                    <div className="therapy-create-btn-div">
                        <button onClick={createTherapy} className="therapy-create-btn"> Create Therapy </button>
                    </div>                    
                )}
                {therapies.length ? (
                    <div className="therapy-list">
                        {therapies.map((therapy, i) => (
                            <div key={i} className="therapy-row">
                                <div className="therapy-info-name">
                                    <p>{therapy?.name}</p> 
                                </div>
                                <div className="therapy-info">
                                    <p>Psych. {therapy?.description}</p>
                                </div>
                                <div className="therapy-actions">
                                    
                                    {canAccessAdminOrCreator(therapy) ? (
                                        <>
                                            <button 
                                                className="table-buttons-blue"
                                                onClick={() => updateTherapy(therapy.id)}
                                            >
                                                <FontAwesomeIcon icon={faEdit} />
                                            </button>
                                        </>
                                    ) : <button className="mock-button"></button>}
                                    <button 
                                        className="table-buttons-blue"
                                        onClick={() => handleInspect(therapy.id)}
                                    >
                                        <FontAwesomeIcon icon={faSearch} />
                                    </button>
                                    {canAccessAdminOrCreator(therapy) && (
                                        <>
                                            <button
                                                className="table-buttons-red"
                                                onClick={() => setDeleteId(therapy.id)}
                                            >
                                                <FontAwesomeIcon icon={faTrash} />
                                            </button>
                                        </>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                ) : (
                    <p className="no-list-items-p">No therapies to display</p>
                )}
                {isLoading ? (
                    <p>Loading...</p>
                ) : therapies.length > 1 ? (
                    <button onClick={loadTherapies} className="load-button-v1">Load More</button>
                ) : null}
            </div>
            <ErrorModal
                show={errorMessage !== ""}
                onClose={() => setErrorMessage("")}
                message={errorMessage}
            />
            <ConfirmationModal 
                show={deleteId !== ""}
                onClose={() => setDeleteId("")}
                onConfirm={() => removeTherapy(deleteId)}
                message={"Are you sure you want to delete therapy?"}
            />
        </article>
    );
};

export default Therapies;
