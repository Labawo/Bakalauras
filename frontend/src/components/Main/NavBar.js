import { useNavigate, Link, useLocation } from "react-router-dom";
import { useContext, useState } from "react";
import AuthContext from "../../context/AuthProvider";
import useAuth from "../../hooks/UseAuth";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt, faKey } from '@fortawesome/free-solid-svg-icons';

const NavBar = () => {

    const { setAuth } = useContext(AuthContext);
    const { auth } = useAuth();
    const navigate = useNavigate();

    const location = useLocation();

    const isActiveLink = (path) => {
        return location.pathname === path;
    };

    const logout = async () => {
        setAuth({});
        navigate('/login');
    }

    const changePassword = async () => {
        navigate('/resetPassword');
    }

    const canAccessAdmin = auth.roles.includes("Admin");
    const canAccessDoctor = auth.roles.includes("Doctor") && !auth.roles.includes("Admin");
    const canAccessPatient = auth.roles.includes("Patient") && !auth.roles.includes("Admin");
    const canAccessPatientDoctor = (auth.roles.includes("Doctor") || auth.roles.includes("Patient")) && !auth.roles.includes("Admin");

    return (
        <div className="navbar">
            <div className="navbar-allign">
                <div className="logout-div">
                    <span>
                        <button onClick={changePassword} className="password-btn">
                            <FontAwesomeIcon icon={faKey} />
                        </button>
                    </span>
                    <span>
                        <button onClick={logout} className="logout-btn">
                            <FontAwesomeIcon icon={faSignOutAlt} />
                        </button>
                    </span>             
                </div>   

                <div className="navbar-links">
                    <span className={`nav-link-span ${isActiveLink('/') ? 'active-span' : ''}`}>
                        <Link to="/" className='nav-link'>Home</Link>
                    </span>
                    <span className={`nav-link-span ${isActiveLink('/therapies') ? 'active-span' : ''}`}>
                        <Link to="/therapies" className='nav-link'>Therapies</Link>
                    </span>
                    <span className={canAccessDoctor ? `nav-link-span ${isActiveLink('/editor') ? 'active-span' : ''}` : 'hidden'}>
                        <Link to="/editor" className={canAccessDoctor ? 'nav-link' : 'hidden'}>Weekly Appointments</Link>
                    </span>
                    <span className={canAccessAdmin ? `nav-link-span ${isActiveLink('/admin') ? 'active-span' : ''}` : 'hidden'}>
                        <Link to="/admin" className={canAccessAdmin ? 'nav-link' : 'hidden'}>Admin</Link>
                    </span>
                    <span className={canAccessAdmin ? `nav-link-span ${isActiveLink('/registerDoctor') ? 'active-span' : ''}` : 'hidden'}>
                        <Link to="/registerDoctor" className={canAccessAdmin ? 'nav-link' : 'hidden'}>Register Doctor</Link>
                    </span>
                    <span className={canAccessPatient ? `nav-link-span ${isActiveLink('/myAppointments') ? 'active-span' : ''}` : 'hidden'}>
                        <Link to="/myAppointments" className={canAccessPatient ? 'nav-link' : 'hidden'}>My Appointments</Link>
                    </span>
                    <span className={canAccessPatient ? `nav-link-span ${isActiveLink('/notes') ? 'active-span' : ''}` : 'hidden'}>
                        <Link to="/notes" className={canAccessPatient ? 'nav-link' : 'hidden'}>Notes</Link>
                    </span>
                    <span className={canAccessPatientDoctor ? `nav-link-span ${isActiveLink('/tests') ? 'active-span' : ''}` : 'hidden'}>
                        <Link to="/tests" className={canAccessPatientDoctor ? 'nav-link' : 'hidden'}>Tests</Link>
                    </span>
                </div>
            </div>
        </div>
    );
};

export default NavBar;