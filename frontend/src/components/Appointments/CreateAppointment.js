import React, { useState } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useParams } from "react-router-dom";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const CreateAppointment = () => {
  const { therapyId } = useParams();

  const [formData, setFormData] = useState({
    date: "",
    time: "",
    price: 0,
  });

  const axiosPrivate = useAxiosPrivate();

  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    let updatedValue = value;

    if (name === "price" && value < 0) {
      updatedValue = 0;
    }

    if (name === "date") {
      const currentDate = new Date().toISOString().split("T")[0];
      if (value < currentDate) {
        updatedValue = currentDate;
      }
    }

    if (name === "price" && isNaN(value)) {
      return; 
    }

    setFormData({
      ...formData,
      [name]: updatedValue,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const datetime = `${formData.date}T${formData.time}:00`;
      const combinedDateTime = new Date(datetime).toISOString();      
      const payload = {
        time: combinedDateTime,
        price: formData.price,
      };
      console.log(payload);
      const response = await axiosPrivate.post(`therapies/${therapyId}/appointments`, payload);
  
      setSuccessMessage("Appointment created successfully!");
      setFormData({ date: "", time: "", price: 0 });
    } catch (error) {
      if (error.response) {
        if (error.response.status === 400) {
          console.error('Bad request: ', error.response.data);
          setErrorMessage("Failed to create appointment. Please try again.");
        } else if (error.response.status === 409) {
          setErrorMessage("Appointment at this time already exists.");
        } else {
          console.error(`Error creating appointment for therapy ${therapyId}:`, error);
          setErrorMessage("Failed to create appointment. Please try again.");
        }
      } else {
        console.error('An unexpected error occurred:', error);
        setErrorMessage("Failed to create appointment. Please try again.");
      }
    }
  };

  return (
    <>
      <Title />
      <NavBar />
      <section>       
        <div className="form-container">
          <h2>Create New Appointment</h2>
          <form onSubmit={handleSubmit} className="input_form">
            <div className="form-group">
              <label htmlFor="date">Date:</label><br />
              <input
                type="date"
                id="date"
                name="date"
                value={formData.date}
                onChange={handleInputChange}
                required
                className="input-field"
                placeholder="YYYY-MM-DD"
              />
            </div>
            <div className="form-group">
              <label htmlFor="time">Time:</label><br />
              <input
                type="time"
                id="time"
                name="time"
                value={formData.time}
                onChange={handleInputChange}
                required
                className="input-field"
                step="60"
              />
            </div>
            <div className="form-group">
              <label htmlFor="price">Price:</label><br />
              <input
                type="number"
                id="price"
                name="price"
                value={formData.price}
                onChange={handleInputChange}
                required
                className="input-field"
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
          buttonText="Go to Appointment List"
          destination={`/therapies/${therapyId}/appointments`}
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

export default CreateAppointment;
