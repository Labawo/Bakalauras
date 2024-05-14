import React from "react";
import { useNavigate } from "react-router-dom";
import { FiCheckCircle } from "react-icons/fi";
import "./ModalStyles.css";

const SuccessModal = ({ show, onClose, message, buttonText, destination }) => {
  const navigate = useNavigate();

  const handleNavigation = () => {
    navigate(-1);
    onClose();
  };

  return (
    <div className={`modal ${show ? "show" : ""}`}>
      <div className="modal-content">
        <span className="close" onClick={onClose}>
          &times;
        </span>
        <h2 className="success-header"><FiCheckCircle /> Success!</h2>
        <p>{message}</p>
        <div className="modal-buttons">
          <button className="primary-button" onClick={onClose}>Stay</button>
          <button className="secondary-button" onClick={handleNavigation}>{buttonText}</button>
        </div>
      </div>
    </div>
  );
};

export default SuccessModal;
