import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
import useAuth from "../../hooks/UseAuth";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const CreateTherapy = () => {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    doctorId: "",
    image: null,
  });

  const [doctors, setDoctors] = useState([]);
  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();
  const { auth } = useAuth();
  const canAccessAdmin = auth.roles.includes("Admin");

  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    if (canAccessAdmin) {
      fetchDoctors();
    }
  }, [canAccessAdmin]);

  const fetchDoctors = async () => {
    try {
      const response = await axiosPrivate.get("/doctors");
      setDoctors(response.data);
    } catch (error) {
      console.error("Error fetching doctors:", error);
      navigate(-1);
    }
  };

  const handleInputChange = (e) => {
    const { name, value, files } = e.target;
    const sanitizedValue = sanitizeInput(value);
    setFormData({
      ...formData,
      [name]: name === "image" ? files[0] : sanitizedValue,
    });
  };

  const sanitizeInput = (value) => {
    return value.replace(/(<([^>]+)>)/gi, "");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      let imageData = null;
      if (formData.image) {
        imageData = await readFileAsBase64(formData.image);
      }

      const therapyData = {
        name: formData.name,
        description: formData.description,
        doctorId: formData.doctorId,
        imageData: imageData,
      };

      const response = await axiosPrivate.post("/therapies", therapyData);

      setSuccessMessage("Therapy created successfully!");
      clearForm();
    } catch (error) {
      console.error("Error creating therapy:", error);
      setErrorMessage("Failed to create therapy. Please try again.");
    }
  };

  const clearForm = () => {
    setFormData({ name: "", description: "", doctorId: "", image: null });
    document.getElementById("image").value = "";
  };

  const readFileAsBase64 = (file) => {
    return new Promise((resolve, reject) => {
      const fileReader = new FileReader();
      fileReader.readAsDataURL(file);
      fileReader.onload = () => {
        resolve(fileReader.result);
      };
      fileReader.onerror = (error) => {
        reject(error);
      };
    });
  };

  return (
    <>
      <Title />
      <NavBar />
      <section>
        <div className="form-container">
          <h2>Create New Therapy</h2>
          <form onSubmit={handleSubmit} className = "input_form">
            <div className="form-group">
            <label htmlFor="name">Type:</label><br />
              <select
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                required
                className="select-field"
              >
                <option value="">Select Therapy Type</option>
                <option value="Consulting">Consulting</option>
                <option value="Music therapy">Music therapy</option>
                <option value="Film therapy">Film therapy</option>
                <option value="Psychoeducation">Psychoeducation</option>
              </select>
              {errors.name && <span className="error-message">{errors.name}</span>}
            </div>
            <div className="form-group">
              <label htmlFor="description">Description:</label><br/>
              <textarea
                id="description"
                name="description"
                value={formData.description}
                onChange={handleInputChange}
                placeholder="Enter Therapy Description"
                required
                className="textarea-field"
              />
              {errors.description && (
                <span className="error-message">{errors.description}</span>
              )}
            </div>
            {canAccessAdmin && (
              <div className="form-group">
                <label htmlFor="doctorId">Doctor:</label><br />
                <select
                  id="doctorId"
                  name="doctorId"
                  value={formData.doctorId}
                  onChange={handleInputChange}
                  className="select-field"
                  required
                >
                  <option value="">Select Doctor</option>
                  {doctors.map((doctor) => (
                    <option key={doctor.id} value={doctor.id}>
                      {doctor.userName}
                    </option>
                  ))}
                </select>
              </div>
            )}
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
              {errors.image && <span className="error-message">{errors.image}</span>}
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

export default CreateTherapy;
