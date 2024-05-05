import React from "react";
import { FiCheckCircle } from "react-icons/fi";
import "./ModalStyles.css"; // Import the CSS file

const SuccessSelectModal = ({ show, onClose, message }) => {
  return (
    <div className={`modal ${show ? "show" : ""}`}>
      <div className="modal-content">
        <span className="close" onClick={onClose}>
          &times;
        </span>
        
        <h2 className="success-header"><FiCheckCircle /> Success!</h2>
        <p>{message}</p>
        <div className="modal-buttons">
          <button className="primary-button" onClick={onClose}>OK</button>
        </div>
      </div>
    </div>
  );
};

export default SuccessSelectModal;
