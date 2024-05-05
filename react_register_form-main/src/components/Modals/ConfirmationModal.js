import React from "react";
import { FiAlertCircle } from "react-icons/fi"; // Assuming you want to use FiAlertCircle for confirmation
import "./ModalStyles.css"; // Import the CSS file

const ConfirmationModal = ({ show, onConfirm, message }) => {
  return (
    <div className={`modal ${show ? "show" : ""}`}>
      <div className="modal-content">
        <h2 className="confirmation-header"><FiAlertCircle /> Confirmation</h2>
        <p>{message}</p>
        <div className="modal-buttons">
          <button className="primary-button" onClick={onConfirm}>Confirm</button>
          <button className="secondary-button">Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmationModal;
