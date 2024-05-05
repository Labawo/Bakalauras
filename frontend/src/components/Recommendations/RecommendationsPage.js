import Recommendations from './Recommendations';
import { useParams } from 'react-router-dom';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";

const RecommendationsPage = () => {
    const { therapyId, appointmentId } = useParams();

    return (
        <>
            <NavBar />
            <section>               
                <Recommendations therapyId = {therapyId} appointmentId = {appointmentId} />
            </section>
            <Footer />
        </>
        
    )
}

export default RecommendationsPage