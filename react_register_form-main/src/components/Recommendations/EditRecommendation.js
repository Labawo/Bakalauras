import React, { useState, useEffect } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import { useParams, useNavigate } from "react-router-dom";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const EditRecommendation = () => {
  const { therapyId, appointmentId, recommendationId } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    description: "",
  });

  const axiosPrivate = useAxiosPrivate();

  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [isLoading, setIsLoading] = useState(true);

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
      const response = await axiosPrivate.put(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/${recommendationId}`, formData);
      setSuccessMessage("Recommendation updated successfully!");
    } catch (error) {
      console.error("Error updating recommendation:", error);
      setErrorMessage("Failed to update recommendation. Please try again.");
    }
  };

  useEffect(() => {
    const fetchRecommendationData = async () => {
      try {
        const response = await axiosPrivate.get(`/therapies/${therapyId}/appointments/${appointmentId}/recommendations/${recommendationId}`);
        const { description } = response.data;
        setFormData({ description });
        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching recommendation:", error);
        if (error.response && error.response.status === 404) {
          navigate(-1);
        } else if (error.response && error.response.status === 403) {
          navigate("/therapies"); // Redirect to unauthorized page
        }
      }
    };
    fetchRecommendationData();
  }, [axiosPrivate, therapyId, appointmentId, recommendationId, navigate]);

  if (isLoading) {
    return <p>Loading...</p>;
  }

  return (
    <>
      <NavBar />
      <section>       
        <div className="form-container">
          <h2>Edit recommendation</h2>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="description">Description:</label><br />
              <textarea
                id="description"
                name="description"
                value={formData.description}
                onChange={handleInputChange}
                required
                className="textarea-field"
              />
            </div>
            <button type="submit" className="submit-button">
              Update
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

export default EditRecommendation;
