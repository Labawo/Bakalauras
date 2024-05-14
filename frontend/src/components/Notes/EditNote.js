import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Title from "../Main/Title";
import Footer from "../Main/Footer";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const EditNote = () => {
  const { noteId } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    content: ""
  });

  const axiosPrivate = useAxiosPrivate();

  const [isLoading, setIsLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    const fetchNoteData = async () => {
      try {
        const response = await axiosPrivate.get(`/notes/${noteId}`);
        const { content } = response.data.resource;
        setFormData({ content });
        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching note:", error);
        if (error.response && error.response.status === 404) {
          navigate(-1);
        } else if (error.response && error.response.status === 403) {
          navigate("/notes");
        }
      }
    };

    fetchNoteData();
  }, [axiosPrivate, noteId, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    const sanitizedValue = sanitizeInput(value);
    setFormData({
      ...formData,
      [name]: sanitizedValue
    });
  };

  const sanitizeInput = (value) => {
    return value.replace(/(<([^>]+)>)/gi, "");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axiosPrivate.put(`/notes/${noteId}`, formData);
      setSuccessMessage("Note updated successfully!");
    } catch (error) {
      console.error("Error updating note:", error);
      setErrorMessage("Failed to update note. Please try again.");
    }
  };

  return (
    <>
      <Title />
      <NavBar />
      <section>        
        <div className="form-container">
          <h2>Edit Note</h2>
          <form onSubmit={handleSubmit} className = "input_form">
            <div className="form-group">
              <label htmlFor="content">Content:</label><br />
              <textarea
                id="content"
                name="content"
                value={formData.content}
                onChange={handleInputChange}
                required
                className="textarea-field"
              />
            </div>
            <button type="submit" className="auth_button">
              Update
            </button>
          </form>
        </div>
        <SuccessModal
          show={successMessage !== ""}
          onClose={() => setSuccessMessage("")}
          message={successMessage}
          buttonText="Go to Notes List"
          destination="/notes"
        />
        <ErrorModal
          show={errorMessage !== ""}
          onClose={() => setErrorMessage("")}
          message={errorMessage}
        />
      </section>
      <Footer />
    </>
  );
};

export default EditNote;
