import React, { useState, useEffect } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";

const TherapyPage = () => {
    const { therapyId } = useParams(); // Get the therapyId from the URL params
    const [therapy, setTherapy] = useState(null);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchTherapy = async () => {
            try {
                const response = await axiosPrivate.get(`/therapies/${therapyId}`);
                setTherapy(response.data.resource);
            } catch (error) {
                console.error(error);
                if (error.response && error.response.status === 404) {
                    navigate(-1);
                }
            }
        };

        fetchTherapy();
    }, [axiosPrivate, therapyId]);

    return (
        <>
            <Title />
            <NavBar />
            <section className="therapy-page">
                
                {therapy ? (
                    <div className="therapy-details">
                        {therapy.imageData && (
                            <img src={therapy.imageData} alt="Therapy" className="therapy-image" />
                        )}
                        <h2>{therapy.name}</h2>
                        <p>Description: {therapy.description}</p>                    
                        <Link to={`/therapies/${therapyId}/appointments`}>
                            <button className="related-button">See Appointments</button>
                        </Link>
                    </div>
                ) : (
                    <p>Loading therapy details...</p>
                )}
            </section>
            <Footer />
        </>
        
    );
};

export default TherapyPage;
