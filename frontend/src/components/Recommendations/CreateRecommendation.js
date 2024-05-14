import React, { useState } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
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
    const sanitizedValue = sanitizeInput(value);
    setFormData({
      ...formData,
      [name]: sanitizedValue,
    });
  };

  const sanitizeInput = (value) => {
    return value.replace(/(<([^>]+)>)/gi, "");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axiosPrivate.post(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations`, formData); // Adjust the API endpoint and payload as per your backend

      setSuccessMessage("Recommendation created successfully!");
      setFormData({ description: "" });
    } catch (error) {
      console.error("Error creating recommendation:", error);
      setErrorMessage("Failed to create recommendation. Please try again.");
    }
  };

  return (
    <>
     <Title />
     <NavBar />
      <section>        
        <div className="form-container">
          <h2>Create new recommendation</h2>
          <form onSubmit={handleSubmit} className="input_form">
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
            <button type="submit" className="auth_button">
              Create
            </button>
          </form>
        </div>
        <SuccessModal
          show={successMessage !== ""}
          onClose={() => setSuccessMessage("")}
          message={successMessage}
          buttonText="Go to Recommendations List"
          destination={`/therapies/${therapyId}/appointments/${appointmentId}/recommendations`}
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

export default CreateRecommendation;
