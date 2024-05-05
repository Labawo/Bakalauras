import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";

const RecommendationPage = () => {
    const { therapyId, appointmentId, recommendationId } = useParams(); // Get the therapyId from the URL params
    const [recommendation, setRecommendation] = useState(null);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchRecommendation = async () => {
            try {
                const response = await axiosPrivate.get(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/${recommendationId}`);
                setRecommendation(response.data);
                console.log(response.data);
            } catch (error) {
                console.error(error);
                // Handle error, e.g., show a message or navigate to an error page
                if (error.response && error.response.status === 404) {
                    navigate(-1);
                }
            }
        };

        fetchRecommendation();
    }, [axiosPrivate, therapyId, appointmentId, recommendationId]);

    return (
        <>
            <NavBar />
            <section>               
                <h2>Recommendation Details</h2>
                {recommendation ? (
                    <div>
                        <p>Description: {recommendation.description}</p>                 
                    </div>
                ) : (
                    <p>Loading therapy details...</p>
                )}
            </section>
            <Footer />
        </>
        
    );
};

export default RecommendationPage;