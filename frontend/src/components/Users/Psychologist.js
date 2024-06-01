import Patients from './Patients';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";

const Psychologist = () => {
    return (
        <>
            <Title />
            <NavBar />
            <section>                
                <br />
                <Patients />
                <br />
            </section>
            <Footer />
        </>
        
    )
}

export default Psychologist