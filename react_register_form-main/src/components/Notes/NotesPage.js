import Notes from './Notes';
import Footer from "../Main/Footer";
import NavBar from "../Main/NavBar";

const NotesPage = () => {

    return (
        <>
            <NavBar />
            <section>                
                <Notes />
            </section>
            <Footer />
        </>        
    )
}

export default NotesPage