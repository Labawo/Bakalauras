import Notes from './Notes';
import Footer from "../Main/Footer";
import NavBar from "../Main/NavBar";
import Title from "../Main/Title";

const NotesPage = () => {

    return (
        <>
            <Title />
            <NavBar />
            <section>                
                <Notes />
            </section>
            <Footer />
        </>        
    )
}

export default NotesPage