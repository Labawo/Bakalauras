import NavBar from "./NavBar";
import Footer from "./Footer";
import useAuth from "../../hooks/UseAuth";
import Notifications from "./Notifications";

const Home = () => {
    const { auth } = useAuth();

    return (
        <>
            <NavBar />
            <section>                
                <h1>Human emotional state monitoring system</h1>
                <br />
                <p>Hello {auth.user}, we are glad you are here!</p>
                <Notifications />            
            </section>
            <Footer />
        </>
        
    )
}

export default Home