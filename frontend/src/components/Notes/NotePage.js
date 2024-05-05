import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
//import "./NotePage.css"; // Import a CSS file for styling (create NotePage.css in the same directory)

const NotePage = () => {
    const { noteId } = useParams(); // Get the noteId from the URL params
    const [note, setNote] = useState(null);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    
    useEffect(() => {
        const fetchNote = async () => {
            try {
                const response = await axiosPrivate.get(`/notes/${noteId}`);
                setNote(response.data.resource);
            } catch (error) {
                console.error(error);
                // Handle error, e.g., show a message or navigate to an error page
                if (error.response && error.response.status === 404) {
                    navigate(-1);
                }
            }
        };

        fetchNote();
    }, [axiosPrivate, noteId]);

    return (
        <>
            <NavBar />
            <section className="note-page">               
                {note ? (
                    <div className="note-details">
                        <h2>{note.name}</h2>
                        <p>{note.content}</p>
                    </div>
                ) : (
                    <p>Loading note details...</p>
                )}
            </section>
            <Footer />
        </>
        
    );
};

export default NotePage;
