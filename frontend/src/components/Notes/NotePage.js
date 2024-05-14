import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";

const NotePage = () => {
    const { noteId } = useParams();
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
                if (error.response && error.response.status === 404) {
                    navigate(-1);
                }
            }
        };

        fetchNote();
    }, [axiosPrivate, noteId]);

    return (
        <>
            <Title />
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
