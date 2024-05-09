import Therapies from './Therapies';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";

const TherapiesPage = () => {
    return (
        <>
            <Title />
            <NavBar />
            <section>                
                <Therapies />
            </section>
            <Footer />
        </>
        
    );
}

export default TherapiesPage;
