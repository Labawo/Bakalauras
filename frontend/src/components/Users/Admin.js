import Users from './Users';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";

const Admin = () => {
    return (
        <>
            <Title />
            <NavBar />
            <section>                
                <br />
                <Users />
                <br />
            </section>
            <Footer />
        </>
        
    )
}

export default Admin