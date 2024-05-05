import React, { useState } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import { useParams } from "react-router-dom";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const CreateRecommendation = () => {
  const { therapyId, appointmentId } = useParams();

  const [formData, setFormData] = useState({
    description: "",
  });

  const axiosPrivate = useAxiosPrivate();

  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    // Validate input to prevent HTML or script injections
    const sanitizedValue = sanitizeInput(value);
    setFormData({
      ...formData,
      [name]: sanitizedValue,
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

      const response = await axiosPrivate.post(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations`, formData); // Adjust the API endpoint and payload as per your backend

      // Handle successful API response
      setSuccessMessage("Recommendation created successfully!");
      // Clear form fields
      setFormData({ description: "" });
    } catch (error) {
      // Handle API call errors
      console.error("Error creating recommendation:", error);
      setErrorMessage("Failed to create recommendation. Please try again.");
      // Update state to display error messages or handle errors appropriately
    }
  };

  return (
    <>
     <NavBar />
      <section>        
        <div className="form-container">
          <h2>Create new recommendation</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="description">Description:</label><br />
              <textarea
                id="description"
                name="description"
                value={formData.description}
                onChange={handleInputChange}
                placeholder="Enter Therapy Description"
                required
                className="textarea-field"
              />
            </div>
            <button type="submit" className="submit-button">
              Create
            </button>
          </form>
        </div>
        {/* Success Modal */}
        <SuccessModal
          show={successMessage !== ""}
          onClose={() => setSuccessMessage("")}
          message={successMessage}
          buttonText="Go to Recommendations List"
          destination={`/therapies/${therapyId}/appointments/${appointmentId}/recommendations`}
        />
        {/* Error Modal */}
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

export default CreateRecommendation;
