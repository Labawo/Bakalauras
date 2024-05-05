import { useNavigate, Link } from "react-router-dom";
import { useContext, useState } from "react";
import AuthContext from "../../context/AuthProvider";
import useAuth from "../../hooks/UseAuth";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt, faBars } from '@fortawesome/free-solid-svg-icons';

const NavBar = () => {

    const { setAuth } = useContext(AuthContext);
    const { auth } = useAuth();
    const navigate = useNavigate();
    const [isMenuOpen, setIsMenuOpen] = useState(false);

    const toggleMenu = () => {
        setIsMenuOpen(!isMenuOpen);
    };

    const menuClassName = isMenuOpen ? "navbar-links open" : "navbar-links";

    const logout = async () => {
        // if used in more components, this should be in context 
        // axios to /logout endpoint 
        setAuth({});
        navigate('/login');
    }

    const canAccessAdmin = auth.roles.includes("Admin");
    const canAccessDoctor = auth.roles.includes("Doctor") && !auth.roles.includes("Admin");
    const canAccessAdminDoctor = auth.roles.includes("Doctor") && auth.roles.includes("Admin");
    const canAccessPatient = auth.roles.includes("Patient") && !auth.roles.includes("Admin");
    const canAccessPatientDoctor = (auth.roles.includes("Doctor") || auth.roles.includes("Patient")) && !auth.roles.includes("Admin");

    return (
        <div className="navbar">
            
            <div className="logout-div">
                <button onClick={logout} className="logout-btn">
                    <FontAwesomeIcon icon={faSignOutAlt} />
                </button>
            </div>   

            <div className="menu-icon" onClick={toggleMenu}>
                <FontAwesomeIcon icon={faBars} />
            </div>

            {isMenuOpen && (
                <div className="navbar-links">
                    <Link to="/" className='nav-link white-bg'>Home</Link>
                    <Link to="/therapies" className='nav-link white-bg'>Therapies</Link>
                    <Link to="/editor" className={canAccessDoctor ? 'nav-link' : 'hidden'}>Weekly Appointments</Link>
                    <Link to="/admin" className={canAccessAdmin ? 'nav-link' : 'hidden'}>Admin</Link>
                    <Link to="/registerDoctor" className={canAccessAdmin ? 'nav-link' : 'hidden'}>Register Doctor</Link>
                    <Link to="/myAppointments" className={canAccessPatient ? 'nav-link' : 'hidden'}>My Appointments</Link>
                    <Link to="/notes" className={canAccessPatient ? 'nav-link' : 'hidden'}>Notes</Link>
                    <Link to="/tests" className={canAccessPatientDoctor ? 'nav-link' : 'hidden'}>Tests</Link>
                </div>
            )}           
        </div>
    );
};

export default NavBar;