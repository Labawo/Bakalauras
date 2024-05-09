import NavBar from "./NavBar";
import Footer from "./Footer";
import Title from "./Title";
import useAuth from "../../hooks/UseAuth";
import Notifications from "./Notifications";

const Home = () => {
    const { auth } = useAuth();

    return (
        <>
            <Title />
            <NavBar />
            <section>                
                <p className="greeting-note">Hello <i><b>{auth.user}</b></i>, we are glad you are here!</p>
                <Notifications />            
            </section>
            <Footer />
        </>
        
    )
}

export default Home