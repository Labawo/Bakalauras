import { useState, useEffect, useCallback } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash, faSearch, faEdit } from '@fortawesome/free-solid-svg-icons';
//import "./Notes.css"; // Import CSS file for styling

const Notes = () => {
    const [notes, setNotes] = useState([]);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();

    const handleInspect = (noteId) => {
        // Navigate to the InspectPage with the noteId parameter
        navigate(`/notes/${noteId}`);
    };

    const fetchNotes = useCallback(async (pageNumber) => {
        try {
            const response = await axiosPrivate.get('/notes', {
                params: { pageNumber: pageNumber }, // Pass the page number as a query parameter
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
        setPage(prevPage => prevPage + 1); // Move to the next page for the next load
        setIsLoading(false);
    };

    useEffect(() => {
        loadNotes();
    }, []); // Load notes only once on initial mount

    const createNote = () => {
        // Navigate to the Create Note page
        navigate(`/notes/createNote`);
    };

    const updateNote = (noteId) => {
        // Navigate to the Edit Note page
        navigate(`/notes/${noteId}/editNote`);
    };

    const removeNote = async (noteId) => {
        try {
            await axiosPrivate.delete(`/notes/${noteId}`);
            setNotes(prevNotes =>
                prevNotes.filter(note => note.id !== noteId)
            );
        } catch (error) {
            console.error(`Error removing note ${noteId}:`, error);
            // Handle error as needed
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
                                            onClick={() => removeNote(note.id)} // Call remove function on click
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
        </article>
    );
};

export default Notes;
