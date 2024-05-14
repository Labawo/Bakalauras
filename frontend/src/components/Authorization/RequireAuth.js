import { useLocation, Navigate, Outlet } from "react-router-dom";
import useAuth from "../../hooks/UseAuth";

const RequireAuth = ({ allowedRoles, doNotPassAdmin = false }) => {
    const { auth } = useAuth();
    const location = useLocation();

    const userRoles = typeof auth.roles === 'string' ? [auth.roles] : auth.roles;

    const allowAdminToPass = doNotPassAdmin && userRoles ? !userRoles.includes("Admin")  : true;

    return (
        userRoles && (userRoles.includes(allowedRoles) || userRoles.some(role => allowedRoles.includes(role))) &&
        allowAdminToPass
            ? <Outlet />
            : auth?.user
                ? <Navigate to="/unauthorized" state={{ from: location }} replace />
                : <Navigate to="/login" state={{ from: location }} replace />
    );
}

export default RequireAuth;