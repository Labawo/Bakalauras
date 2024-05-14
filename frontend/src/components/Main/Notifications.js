import { useState, useEffect, useCallback } from "react";
import useAxiosPrivate from "../../hooks/UseAxiosPrivate";
import { useNavigate, useLocation } from "react-router-dom";

const Notifications = () => {
    const [notifications, setNotifications] = useState([]);
    const [page, setPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const axiosPrivate = useAxiosPrivate();
    const navigate = useNavigate();
    const location = useLocation();

    const fetchNotifications = useCallback(async (pageNumber) => {
        try {
            const response = await axiosPrivate.get('/notifications', {
                params: { pageNumber: pageNumber }, 
            });
            return response.data;
        } catch (err) {
            console.error(err);
            navigate('/login', { state: { from: location }, replace: true });
            return [];
        }
    }, [axiosPrivate, navigate, location]);

    const loadNotifications = async () => {
        if (isLoading) return;

        setIsLoading(true);
        const data = await fetchNotifications(page);
        console.log(data)
        setNotifications(prevNotifications => [...prevNotifications, ...data]);
        setPage(prevPage => prevPage + 1);
        setIsLoading(false);
    };

    useEffect(() => {
        loadNotifications();
    }, []);


    const removeNotification = async (notificationId) => {
        try {
            await axiosPrivate.delete(`/notifications/${notificationId}`);
            setNotifications(prevNotifications =>
                prevNotifications.filter(note => note.id !== notificationId)
            );
        } catch (error) {
            console.error(`Error removing note ${notificationId}:`, error);
        }
    };

    return (
        <article className="notifications-container">
            <div className="table-container">
                <div className="notificationsdiv-container">
                    <h3 className="notification-header">What is new for you:</h3>
                    {notifications.length ? (
                        <div className="notification-list">
                            {notifications.map((notification, i) => (
                                <div className="notification-item" key={i}>
                                    <div className="close-button-div">
                                        <button
                                                className="close-button"
                                                onClick={() => removeNotification(notification.id)}
                                            >
                                                x
                                        </button>
                                    </div>                               
                                    <div className="notification-content">{notification?.content}</div>
                                    
                                </div>
                            ))}
                        </div>
                    ) : (
                        <p className="no-list-items-np">There is nothing new.</p>
                    )}
                    {isLoading ? (
                        <p>Loading...</p>
                    ) : notifications.length > 4 ? (
                        <button onClick={loadNotifications} className="load-button-v1">Load More</button>
                    ) : null}
                </div>                
            </div>
        </article>
    );
};

export default Notifications;
