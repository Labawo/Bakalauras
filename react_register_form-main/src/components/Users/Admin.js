import Users from './Users';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";

const Admin = () => {
    return (
        <>
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