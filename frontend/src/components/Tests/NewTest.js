import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";
import questionsData from "./questionsData";
import RedirectModal from "../Modals/RedirectModal";
import ErrorModal from "../Modals/ErrorModal";
import "./TestStyle.css";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';

const NewTest = () => {
  const [formData, setFormData] = useState({});
  const [score, setScore] = useState(null);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [currentPage, setCurrentPage] = useState(0);
  const [answers, setAnswers] = useState(Array(Math.ceil(questionsData.length / 2)).fill(null));
  const axiosPrivate = useAxiosPrivate();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUserTimer = async () => {
      try {
        const response = await axiosPrivate.get('/testTimer');
        const timer = response.data;
        if (timer != null && timer < new Date().toUTCString()) {
            navigate(-1);
            return;
        }
      } catch (error) {
        console.error("Error fetching user test timer:", error);
      }
    };
  
    fetchUserTimer();
  }, []);

  useEffect(() => {
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
      const isAllAnswered = answers.every(answer => answer !== null);
  
      if (!isAllAnswered) {
        setErrorMessage("Please answer all questions before submitting.");
        console.log(answers);
        return;
      }
  
      let totalScore = "";
      for (let i = 0; i < answers.length; i++) {
        const answer = answers[i];
        if (answer) {
          for (const [questionIndex, optionIndex] of Object.entries(answer)) {
            totalScore += questionsData[questionIndex].options[optionIndex].index;
          }
        }
      }
      setScore(totalScore);
      console.log(totalScore);
  
      const response = await axiosPrivate.post("/tests", { score: totalScore });
      setSuccessMessage("Test created successfully!");
      setFormData({});
    } catch (error) {
      console.error("Error creating test:", error);
      setErrorMessage("Failed to create test. Please try again.");
    }
  };
  

  const handleNextPage = () => {
    if (currentPage < Math.ceil(questionsData.length / 2) - 1) {
      setCurrentPage(prevPage => prevPage + 1);
    }
  };

  const handlePrevPage = () => {
    setCurrentPage(prevPage => Math.max(prevPage - 1, 0));
  };

  return (
    <>
      <Title />
      <NavBar />
      <section>        
        <div className="form-container">
          <h2>BDI Test</h2>
          {errorMessage && <ErrorModal show={errorMessage !== ""} onClose={() => setErrorMessage("")} message={errorMessage} />}
          <form onSubmit={handleSubmit} className="test-form">
            {questionsData.slice(currentPage * 2, currentPage * 2 + 2).map((question, questionIndex) => (
              <div className="form-group" key={questionIndex}>
                <div className="test-label-div">
                 <label className="test-label">{question.question}</label><br />
                </div>                
                <div className="options-div">
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
              {currentPage < Math.ceil(questionsData.length / 2) - 1 && (
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
            destination="/tests" />
        </div>
      </section>
      <Footer />
    </>
    
  );
};

export default NewTest;
