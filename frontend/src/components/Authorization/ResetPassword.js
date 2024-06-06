import { useRef, useState, useEffect } from "react";
import { faCheck, faTimes, faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import NavBar from "../Main/NavBar";
import Title from "../Main/Title";
import Footer from "../Main/Footer";
import SuccessSelectModal from "../Modals/SuccessSelectModal";

const PWD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%]).{8,24}$/;
const REGISTER_URL = '/resetPassword';

const ResetPassword = () => {
    const errRef = useRef();

    const axiosPrivate = useAxiosPrivate();

    const [cpwd, setcPwd] = useState('');
    const [validcPwd, setValidcPwd] = useState(false);
    const [cpwdFocus, setcPwdFocus] = useState(false);

    const [pwd, setPwd] = useState('');
    const [validPwd, setValidPwd] = useState(false);
    const [pwdFocus, setPwdFocus] = useState(false);

    const [matchPwd, setMatchPwd] = useState('');
    const [validMatch, setValidMatch] = useState(false);
    const [matchFocus, setMatchFocus] = useState(false);

    const [errMsg, setErrMsg] = useState('');
    const [success, setSuccess] = useState(false);
    const [successMsg, setSuccessMsg] = useState('');

    useEffect(() => {
        setValidcPwd(PWD_REGEX.test(cpwd));
    }, [cpwd])

    useEffect(() => {
        setValidPwd(PWD_REGEX.test(pwd));
        setValidMatch(pwd === matchPwd);
    }, [pwd, matchPwd])

    useEffect(() => {
        setErrMsg('');
    }, [cpwd, pwd, matchPwd])

    const resetForm = () => {
        setSuccess(false);
        setErrMsg('');
        setSuccessMsg('Password changed successfully');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const v1 = PWD_REGEX.test(cpwd);
        const v2 = PWD_REGEX.test(pwd);
        if (!v1 || !v2 ) {
            setErrMsg("Invalid Entry");
            return;
        }
        try {
            const response = await axiosPrivate.put(REGISTER_URL,
                JSON.stringify({ CurrentPassword : cpwd, NewPassword : pwd }),
                {
                    headers: { 'Content-Type': 'application/json' },
                    withCredentials: true
                }
            );
            console.log(response?.data);
            console.log(response?.accessToken);
            setSuccess(true);
            setcPwd('');
            setPwd('');
            setMatchPwd('');
            resetForm();
        } catch (err) {
            if (!err?.response) {
                setErrMsg('No Server Response');
            } else if (err.response?.status === 409) {
                setErrMsg('Username Taken');
            } else {
                setErrMsg('Password change Failed')
            }
            errRef.current.focus();
        }
    }

    return (
        <>
            <Title />
            <NavBar />
            <section>                  
                    <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">{errMsg}</p>
                    <h1>Change password</h1>
                    <form onSubmit={handleSubmit} className = "input_form">
                        <label htmlFor="currentpassword">
                            Current password:
                            <FontAwesomeIcon icon={faCheck} className={validcPwd ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validcPwd || !cpwd ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="password"
                            id="currentpassword"
                            onChange={(e) => setcPwd(e.target.value)}
                            value={cpwd}
                            required
                            aria-invalid={validcPwd ? "false" : "true"}
                            aria-describedby="cpwdnote"
                            onFocus={() => setcPwdFocus(true)}
                            onBlur={() => setcPwdFocus(false)}
                        />
                        <p id="cpwdnote" className={cpwdFocus && !validcPwd ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            8 to 24 characters.<br />
                            Must include uppercase and lowercase letters, a number and a special character.<br />
                            Allowed special characters: <span aria-label="exclamation mark">!</span> <span aria-label="at symbol">@</span> <span aria-label="hashtag">#</span> <span aria-label="dollar sign">$</span> <span aria-label="percent">%</span>
                        </p>

                        <label htmlFor="password">
                            Password:
                            <FontAwesomeIcon icon={faCheck} className={validPwd ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validPwd || !pwd ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="password"
                            id="password"
                            onChange={(e) => setPwd(e.target.value)}
                            value={pwd}
                            required
                            aria-invalid={validPwd ? "false" : "true"}
                            aria-describedby="pwdnote"
                            onFocus={() => setPwdFocus(true)}
                            onBlur={() => setPwdFocus(false)}
                        />
                        <p id="pwdnote" className={pwdFocus && !validPwd ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            8 to 24 characters.<br />
                            Must include uppercase and lowercase letters, a number and a special character.<br />
                            Allowed special characters: <span aria-label="exclamation mark">!</span> <span aria-label="at symbol">@</span> <span aria-label="hashtag">#</span> <span aria-label="dollar sign">$</span> <span aria-label="percent">%</span>
                        </p>
                        <label htmlFor="confirm_pwd">
                            Confirm Password:
                            <FontAwesomeIcon icon={faCheck} className={validMatch && matchPwd ? "valid" : "hide"} />
                            <FontAwesomeIcon icon={faTimes} className={validMatch || !matchPwd ? "hide" : "invalid"} />
                        </label>
                        <input
                            type="password"
                            id="confirm_pwd"
                            onChange={(e) => setMatchPwd(e.target.value)}
                            value={matchPwd}
                            required
                            aria-invalid={validMatch ? "false" : "true"}
                            aria-describedby="confirmnote"
                            onFocus={() => setMatchFocus(true)}
                            onBlur={() => setMatchFocus(false)}
                        />
                        <p id="confirmnote" className={matchFocus && !validMatch ? "instructions" : "offscreen"}>
                            <FontAwesomeIcon icon={faInfoCircle} />
                            Must match the first password input field.
                        </p>

                        <button disabled={!validcPwd || !validPwd || !validMatch ? true : false} className="auth_button">Change Password</button>
                    </form>
                </section>
                <SuccessSelectModal
                    show={successMsg !== ""}
                    onClose={() => setSuccessMsg("")}
                    message={successMsg}
                />
                <Footer />
        </>
    )
}

export default ResetPassword
