import React from "react";
import "./TestInspectStyle.css"; // Import the CSS file

const TestInspect = ({ show, onClose, message, message2, user }) => {

  const renderMessageLines = (message) => {
    const lines = message.split("\n");
    const reversedLines = lines.reverse();
    const reversedWithoutFirst = [reversedLines[0], ...reversedLines.slice(1).reverse()]; // Reverse all but the first line

    return reversedWithoutFirst.map((line, index) => (
      <p
        key={index}
        style={{
          marginLeft: index % 2 === 0 ? "0" : "5px",
          fontWeight: index === 0 ? "bold" : index % 2 === 0 ? "normal" : "bold",
          fontSize: index === 0 ? "inherit" : "smaller"
        }}
      >
        {line}
      </p>
    ));
  };

  return (
    <div className={`inspect ${show ? "show" : ""}`}>
      <div className="inspect-content">
        <span className="close" onClick={onClose}>
          &times;
        </span>
        <h2>{user} results.</h2>
        <div>{renderMessageLines(message)}</div>
        <br />
        <div>{renderMessageLines(message2)}</div>
        <div className="inspect-buttons">
          <button className="secondary-button" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
};

export default TestInspect;