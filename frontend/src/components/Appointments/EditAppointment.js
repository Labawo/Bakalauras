import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
import SuccessModal from "../Modals/SuccessModal";
import ErrorModal from "../Modals/ErrorModal";

const EditAppointment = () => {
  const { therapyId, appointmentId } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    date: "",
    time: "",
    price: 0,
  });

  const axiosPrivate = useAxiosPrivate();

  const [isLoading, setIsLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    const fetchAppointmentData = async () => {
      try {
        const response = await axiosPrivate.get(`/therapies/${therapyId}/appointments/${appointmentId}`);
        const { time: datetime, price } = response.data;
        const serverDate = new Date(datetime);

        const clientOffsetInMinutes = new Date().getTimezoneOffset();
        const clientDate = new Date(serverDate.getTime() - clientOffsetInMinutes * 60000);

        const formattedDateTime = clientDate.toISOString().split('T');
        const date = formattedDateTime[0];
        const time = formattedDateTime[1].slice(0, 5);
        setFormData({ date, time, price });
        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching appointment:", error);
        if (error.response && error.response.status === 404) {
          navigate(-1);
        } else if (error.response && error.response.status === 403) {
          navigate("/therapies");
        }
      }
    };

    fetchAppointmentData();
  }, [axiosPrivate, therapyId, appointmentId, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const datetime = `${formData.date} ${formData.time}:00`;

      const payload = {
        time: datetime,
        price: formData.price,
      };

      const response = await axiosPrivate.put(`/therapies/${therapyId}/appointments/${appointmentId}`, payload);
      setSuccessMessage("Appointment updated successfully!");
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

  if (isLoading) {
    return <p>Loading...</p>;
  }

  return (
    <>
     <Title />
     <NavBar />
      <section>       
        <div className="form-container">
          <h2>Edit Appointment</h2>
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
              Update
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

export default EditAppointment;
