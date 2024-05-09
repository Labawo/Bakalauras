import Recommendations from './Recommendations';
import { useParams } from 'react-router-dom';
import NavBar from "../Main/NavBar";
import Footer from "../Main/Footer";
import Title from "../Main/Title";

const RecommendationsPage = () => {
    const { therapyId, appointmentId } = useParams();

    return (
        <>
            <Title />
            <NavBar />
            <section>               
                <Recommendations therapyId = {therapyId} appointmentId = {appointmentId} />
            </section>
            <Footer />
        </>
        
    )
}

export default RecommendationsPage