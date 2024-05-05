import { useState, useEffect, useCallback } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import useAuth from "../../hooks/UseAuth";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch, faEdit } from '@fortawesome/free-solid-svg-icons';

const Therapies = () => {
    const [therapies, setTherapies] = useState([]);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();
    const { auth } = useAuth();

    const handleInspect = (therapyId) => {
        // Navigate to the InspectPage with the therapyId parameter
        navigate(`/therapies/${therapyId}`);
    };

    const canAccessDoctor = auth.roles.includes("Doctor") || auth.roles.includes("Admin");
    
    const canAccessAdminOrCreator = (therapy) => {
        // Check if the authenticated user is an admin
        const isAdmin = auth.roles.includes("Admin");
      
        // Check if the authenticated user is the creator of the therapy
        const isCreator = therapy.doctorId === auth.id;
      
        // Return true if the user is either an admin or the creator of the therapy
        return isAdmin || isCreator;
      };

    const fetchTherapies = useCallback(async (pageNumber) => {
        try {
            const response = await axiosPrivate.get('/therapies', {
                params: { pageNumber : pageNumber }, // Pass the page number as a query parameter
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
        setPage(prevPage => prevPage + 1); // Move to the next page for the next load
        setIsLoading(false);
    };

    useEffect(() => {
        loadTherapies();
    }, []); // Load therapies only once on initial mount

    const createTherapy = () => {
        // Navigate to the Create Therapy page
        navigate(`/therapies/createTherapy`);
    };

    const updateTherapy = (therapyId) => {
        // Navigate to the Create Therapy page
        navigate(`/therapies/${therapyId}/editTherapy`);
    };

    const removeTherapy = async (therapyId) => {
        try {
          await axiosPrivate.delete(`/therapies/${therapyId}`);
          setTherapies(prevTherapies =>
            prevTherapies.filter(therapy => therapy.id !== therapyId)
          );
        } catch (error) {
          console.error(`Error removing therapy ${therapyId}:`, error);
          // Handle error as needed
        }
      };

    return (
        <article className="therapies-container">
            <div className="table-container">
                <h2 className="list-headers">Therapies List</h2>
                {canAccessDoctor && (
                    <button onClick={createTherapy} className="create-button-v1"> Create Therapy </button>
                )}
                {therapies.length ? (
                    <table className="my-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Description</th>
                            </tr>
                        </thead>
                        <tbody>
                            {therapies.map((therapy, i) => (
                                <tr key={i}>
                                    <td>{therapy?.name}</td>
                                    <td>{therapy?.description.length > 20 ? therapy?.description[20] : therapy?.description}...</td>
                                    <td>
                                        <button 
                                            className="table-buttons-blue"
                                            onClick={() => handleInspect(therapy.id)}
                                        >
                                            <FontAwesomeIcon icon={faSearch} />
                                        </button>
                                        {canAccessAdminOrCreator(therapy) && (
                                            <>
                                                <button 
                                                    className="table-buttons-blue"
                                                    onClick={() => updateTherapy(therapy.id)}
                                                >
                                                    <FontAwesomeIcon icon={faEdit} />
                                                </button>
                                                <button 
                                                    className="table-buttons-red"
                                                    onClick={() => removeTherapy(therapy.id)} // Call remove function on click
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
                    <p className="no-list-items-p">No therapies to display</p>
                )}
                {isLoading ? (
                    <p>Loading...</p>
                ) : therapies.length > 1 ? (
                    <button onClick={loadTherapies} className="load-button-v1">Load More</button>
                ) : null}
            </div>
        </article>
    );
};

export default Therapies;
