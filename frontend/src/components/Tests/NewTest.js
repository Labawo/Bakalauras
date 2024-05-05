import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import useAuth from "../../hooks/UseAuth";
import questionsData from "./questionsData";
import RedirectModal from "../Modals/RedirectModal"; // Import the RedirectModal component
import ErrorModal from "../Modals/ErrorModal"; // Import the ErrorModal component
import "./TestStyle.css";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';

const NewTest = () => {
  const [formData, setFormData] = useState({});
  const [score, setScore] = useState(null);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [currentPage, setCurrentPage] = useState(0);
  const [answers, setAnswers] = useState(Array(Math.ceil(questionsData.length / 3)).fill(null));
  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUserTests = async () => {
      try {
        const response = await axiosPrivate.get('/tests', {
          params: { pageNumber: 1, patientId: null },
        });
        const tests = response.data;
        if (tests.length > 0) {
          const latestTestDate = new Date(tests[0].time);
          const currentDate = new Date();
          const differenceInDays = Math.floor((currentDate - latestTestDate) / (1000 * 60 * 60 * 24));
          if (differenceInDays < 7) {
            navigate(-1);
            return;
          }
        }
      } catch (error) {
        console.error("Error fetching user tests:", error);
      }
    };
  
    fetchUserTests();
  }, [axiosPrivate, navigate]);

  useEffect(() => {
    // If there are previous answers, update the form data with them
    if (answers[currentPage]) {
      setFormData(answers[currentPage]);
    } else {
      setFormData({});
    }
  }, [currentPage, answers]);

  const handleInputChange = (questionIndex, optionIndex) => {
    const newFormData = { ...formData, [questionIndex]: optionIndex };
    setFormData(newFormData);
    const newAnswers = [...answers];
    newAnswers[currentPage] = newFormData;
    setAnswers(newAnswers);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      // Check if all questions are answered
      const isAllAnswered = answers.every(answer => answer !== null);
  
      if (!isAllAnswered) {
        setErrorMessage("Please answer all questions before submitting.");
        console.log(answers);
        return;
      }
  
      let totalScore = 0;
      for (let i = 0; i < answers.length; i++) {
        const answer = answers[i];
        if (answer) {
          for (const [questionIndex, optionIndex] of Object.entries(answer)) {
            totalScore += questionsData[questionIndex].options[optionIndex].points;
          }
        }
      }
      setScore(totalScore);
  
      const response = await axiosPrivate.post("/tests", { score: totalScore });
      setSuccessMessage("Test created successfully!");
      setFormData({});
    } catch (error) {
      console.error("Error creating test:", error);
      setErrorMessage("Failed to create test. Please try again.");
    }
  };
  

  const handleNextPage = () => {
    if (currentPage < Math.ceil(questionsData.length / 3) - 1) {
      setCurrentPage(prevPage => prevPage + 1);
    }
  };

  const handlePrevPage = () => {
    setCurrentPage(prevPage => Math.max(prevPage - 1, 0));
  };

  return (
    <>
      <NavBar />
      <section>        
        <div className="form-container">
          <h2>BDI Test</h2>
          {errorMessage && <ErrorModal show={errorMessage !== ""} onClose={() => setErrorMessage("")} message={errorMessage} />}
          <form onSubmit={handleSubmit} className="test-form">
            {questionsData.slice(currentPage * 3, currentPage * 3 + 3).map((question, questionIndex) => (
              <div className="form-group" key={questionIndex}>
                <label className="test-label">{question.question}</label><br />
                <div>
                  {question.options.map((option, optionIndex) => (
                    <div key={optionIndex}>
                      <label>
                        <input
                          type="radio"
                          name={`question${questionIndex}`}
                          value={optionIndex}
                          checked={formData[questionIndex] === optionIndex}
                          onChange={() => handleInputChange(questionIndex, optionIndex)}
                          required
                        />
                        {option.text}
                      </label>
                      <br />
                    </div>
                  ))}
                </div>
              </div>
            ))}
            <div className="pagination-buttons">
              {currentPage > 0 && (
                <button type="button" className="previous-button" onClick={handlePrevPage}>
                  <FontAwesomeIcon icon={faAngleLeft} />
                </button>
              )}
              {currentPage < Math.ceil(questionsData.length / 3) - 1 && (
                <button type="button" className="next-button" onClick={handleNextPage}>
                  <FontAwesomeIcon icon={faAngleRight} />
                </button>
              )}
            </div>
            <button type="submit" className="submit-button-test">
              Submit Test
            </button>
          </form>
          <RedirectModal 
            show={successMessage !== ""} 
            onClose={() => setSuccessMessage("")} 
            message={successMessage} 
            buttonText="Go to tests"
            score={score}
            destination="/tests" />
        </div>
      </section>
      <Footer />
    </>
    
  );
};

export default NewTest;
