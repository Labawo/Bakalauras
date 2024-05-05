import React, { useState } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import useAuth from "../../hooks/UseAuth";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const CreateNote = () => {
  const [formData, setFormData] = useState({
    name: "",
    content: ""
  });

  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const axiosPrivate = useAxiosPrivate();
  const { auth } = useAuth();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    // Validate input to prevent HTML or script injections
    const sanitizedValue = sanitizeInput(value);
    setFormData({
      ...formData,
      [name]: sanitizedValue
    });
  };

  const sanitizeInput = (value) => {
    // Basic sanitation function to prevent HTML/script injections
    // Implement according to your security requirements
    return value.replace(/(<([^>]+)>)/gi, "");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      // Implement form validation logic here
      // For instance, check if fields are not empty, etc.
  
      // Include the note data in the JSON payload
      const noteData = {
        name: formData.name,
        content: formData.content
      };
  
      // Send noteData as JSON payload
      const response = await axiosPrivate.post("/notes", noteData);
  
      // Handle successful API response
      setSuccessMessage("Note created successfully!");
      // Clear form fields
      setFormData({ name: "", content: "" });
    } catch (error) {
      // Handle API call errors
      console.error("Error creating note:", error);
      // Update state to display error messages or handle errors appropriately
      setErrorMessage("Failed to create note. Please try again.");
    }
  };  

  return (
    <>
      <NavBar />
      <section>
        <div className="form-container">
          <h2>Create New Note</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="name">Title:</label><br />
              <input
                type="text"
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                placeholder="Enter Note Title"
                required
                className="input-field"
              />
              {errors.name && <span className="error-message">{errors.name}</span>}
            </div>
            <div className="form-group">
              <label htmlFor="content">Content:</label><br />
              <textarea
                id="content"
                name="content"
                value={formData.content}
                onChange={handleInputChange}
                placeholder="Enter Note Content"
                required
                className="textarea-field"
              />
              {errors.content && (
                <span className="error-message">{errors.content}</span>
              )}
            </div>
            <button type="submit" className="submit-button">
              Create
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

export default CreateNote;
