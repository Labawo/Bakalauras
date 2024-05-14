import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const EditTherapy = () => {
  const { therapyId } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    name: "",
    description: "",
    imageData: null,
  });

  const axiosPrivate = useAxiosPrivate();

  const [isLoading, setIsLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    const fetchTherapyData = async () => {
      try {
        const response = await axiosPrivate.get(`/therapies/${therapyId}`);
        const { name, description, imageData } = response.data.resource;
        setFormData({ name, description, imageData });
        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching therapy:", error);
        if (error.response && error.response.status === 404) {
          navigate(-1);
        } else if (error.response && error.response.status === 403) {
          navigate("/therapies");
        }
      }
    };

    fetchTherapyData();
  }, [axiosPrivate, therapyId, navigate]);

  const handleInputChange = (e) => {
    const { name, value, files } = e.target;
    const sanitizedValue = sanitizeInput(value);
    
    if (name === "image") {
      const reader = new FileReader();
      const file = files[0];
      
      reader.onloadend = () => {
        setFormData({
          ...formData,
          imageData: reader.result,
        });
      };
      
      reader.readAsDataURL(file);
    } else {
      setFormData({
        ...formData,
        [name]: sanitizedValue,
      });
    }
  };

  const sanitizeInput = (value) => {
    return value.replace(/(<([^>]+)>)/gi, "");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      let formDataToSend = { ...formData };
      
      if (formDataToSend.imageData instanceof File) {
        const imageDataBase64 = await readFileAsBase64(formDataToSend.imageData);
        formDataToSend = {
          ...formDataToSend,
          imageData: imageDataBase64,
        };
      }

      const response = await axiosPrivate.put(`/therapies/${therapyId}`, formDataToSend);
      setSuccessMessage("Therapy updated successfully!");
    } catch (error) {
      console.error("Error updating therapy:", error);
      setErrorMessage("Failed to update therapy. Please try again.");
    }
  };

  const readFileAsBase64 = (file) => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = (error) => reject(error);
    });
  };

  if (isLoading) {
    return <p>Loading...</p>;
  }

  return (
    <>
      <Title />
      <NavBar />
      <section>     
        <div className="form-container">
          <h2>Edit Therapy</h2>
          <form onSubmit={handleSubmit} className="input_form">            
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
            <div className="form-group">
              <label htmlFor="image">Image:</label><br />
              <input
                type="file"
                id="image"
                name="image"
                onChange={handleInputChange}
                accept="image/*"
                className="input-field"
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
          buttonText="Go to Therapy List"
          destination="/therapies"
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

export default EditTherapy;
