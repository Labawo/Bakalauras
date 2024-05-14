import { useState, useEffect, useCallback } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch, faEdit } from '@fortawesome/free-solid-svg-icons';
import ConfirmationModal from "../Modals/ConfirmationModal";

const Notes = () => {
    const [notes, setNotes] = useState([]);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();
    const [deleteId, setDeleteId] = useState("");

    const handleInspect = (noteId) => {
        navigate(`/notes/${noteId}`);
    };

    const fetchNotes = useCallback(async (pageNumber) => {
        try {
            const response = await axiosPrivate.get('/notes', {
                params: { pageNumber: pageNumber },
            });
            return response.data;
        } catch (err) {
            console.error(err);
            navigate('/login', { state: { from: location }, replace: true });
            return [];
        }
    }, [axiosPrivate, navigate, location]);

    const loadNotes = async () => {
        if (isLoading) return;

        setIsLoading(true);
        const data = await fetchNotes(page);
        console.log(data)
        setNotes(prevNotes => [...prevNotes, ...data]);
        setPage(prevPage => prevPage + 1);
        setIsLoading(false);
    };

    useEffect(() => {
        loadNotes();
    }, []);

    const createNote = () => {
        navigate(`/notes/createNote`);
    };

    const updateNote = (noteId) => {
        navigate(`/notes/${noteId}/editNote`);
    };

    const removeNote = async (noteId) => {
        try {
            await axiosPrivate.delete(`/notes/${noteId}`);
            setNotes(prevNotes =>
                prevNotes.filter(note => note.id !== noteId)
            );
            setDeleteId("");
        } catch (error) {
            console.error(`Error removing note ${noteId}:`, error);
        }
    };

    return (
        <article className="notes-container">
            <div className="table-container">
                <h2 className="list-headers">Notes List</h2>
                <button onClick={createNote} className="create-button-v1"> New Note </button>
                {notes.length ? (
                    <table className="my-table">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Content</th>
                            </tr>
                        </thead>
                        <tbody>
                            {notes.map((note, i) => (
                                <tr key={i}>
                                    <td>{note?.name}</td>
                                    <td>{note?.content.length > 20 ? note?.content[20] : note?.content}...</td>
                                    <td>
                                        <button
                                            className="table-buttons-blue"
                                            onClick={() => handleInspect(note.id)}
                                        >
                                            <FontAwesomeIcon icon={faSearch} />
                                        </button>
                                        <button
                                            className="table-buttons-blue"
                                            onClick={() => updateNote(note.id)}
                                        >
                                            <FontAwesomeIcon icon={faEdit} />
                                        </button>
                                        <button
                                            className="table-buttons-red"
                                            onClick={() => setDeleteId(note.id)}
                                        >
                                            <FontAwesomeIcon icon={faTrash} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p className="no-list-items-p">No notes to display</p>
                )}
                {isLoading ? (
                    <p>Loading...</p>
                ) : notes.length > 2 ? (
                    <button onClick={loadNotes} className="load-button-v1">Load More</button>
                ) : null}
            </div>
            <ConfirmationModal 
                show={deleteId !== ""}
                onClose={() => setDeleteId("")}
                onConfirm={() => removeNote(deleteId)}
                message={"Are you sure you want to delete note?"}
            />
        </article>
    );
};

export default Notes;
