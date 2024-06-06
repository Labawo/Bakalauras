import React, { useRef } from "react";
import "./TestInspectStyle.css"; // Import the CSS file
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCopy } from '@fortawesome/free-solid-svg-icons';

const TestInspect = ({ show, onClose, message, message2, user }) => {

  const divRef = useRef(null);

  const renderMessageLines = (message) => {
    const lines = message.split("\n");
    const reversedLines = lines.reverse();
    const reversedWithoutFirst = [reversedLines[0], ...reversedLines.slice(1).reverse()];

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

  const copyToClipboard = () => {
    if (divRef.current) {
      const text = divRef.current.innerText;
      navigator.clipboard.writeText(text).then(() => {
        alert('Content copied to clipboard!');
      }).catch(err => {
        console.error('Failed to copy: ', err);
      });
    }
  };

  return (
    <div className={`inspect ${show ? "show" : ""}`}>
      <div className="inspect-content">
        <span className="close" onClick={onClose}>
          &times;
        </span>
        <h2>{user} results.</h2>
        <div className="copy-button-div">
          <button onClick={copyToClipboard} className="copy-button"><FontAwesomeIcon icon={faCopy} /> Copy</button>
        </div>
        <div className="results-div-written" ref={divRef}>
          <div className="anx-div">{renderMessageLines(message)}</div>
          <br />
          <div className="dep-div">{renderMessageLines(message2)}</div>
        </div>
        <div className="inspect-buttons">
          <button className="secondary-button" onClick={onClose}>Close</button>
        </div>
      </div>
    </div>
  );
};

export default TestInspect;