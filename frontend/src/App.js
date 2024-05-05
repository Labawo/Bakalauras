import Register from './components/Authorization/Register';
import RegisterDoctor from './components/Authorization/RegisterDoctor';
import Login from './components/Authorization/Login';
import Home from './components/Main/Home';
import Layout from './components/Main/Layout';
import Editor from './components/Appointments/Editor';
import Admin from './components/Users/Admin';
import Missing from './components/Main/Missing';
import Unauthorized from './components/Authorization/Unauthorized';
import TherapiesPage from './components/Therapies/TherapiesPage';
import CreateTherapyPage from './components/Therapies/CreateTherapy';
import EditTherapyPage from './components/Therapies/EditTherapy';
import CreateAppointment from './components/Appointments/CreateAppointment';
import MyAppointmentsPage from './components/Appointments/MyAppointments';
import EditAppointment from './components/Appointments/EditAppointment';
import AppointmentsPage from './components/Appointments/AppointmentsPage';
import AppointmentPage from './components/Appointments/AppointmentPage';
import RecommendationsPage from './components/Recommendations/RecommendationsPage';
import RecommendationPage from './components/Recommendations/RecommendationPage';
import CreateRecommendation from './components/Recommendations/CreateRecommendation';
import EditRecommendation from './components/Recommendations/EditRecommendation';
import TherapyPage from './components/Therapies/TherapyPage';
import RequireAuth from './components/Authorization/RequireAuth';
import NotesPage from './components/Notes/NotesPage';
import CreateNotePage from './components/Notes/CreateNote';
import EditNotePage from './components/Notes/EditNote';
import NotePage from './components/Notes/NotePage';
import TestsPage from './components/Tests/TestsPage';
import NewTestPage from './components/Tests/NewTest';
import { Routes, Route } from 'react-router-dom';

const ROLES = {
  'User': "Patient",
  'Editor': "Doctor",
  'Admin': "Admin"
}

function App() {

  return (
    <Routes>
      <Route path='/' element={<Layout />}>
        {/* public routes */}
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route path="unauthorized" element={<Unauthorized />} />

        {/* we want to protect these routes */}
        <Route element={<RequireAuth allowedRoles={[ROLES.User]} doNotPassAdmin = {true}/>}>
          <Route path="/notes" element={<NotesPage />} />
          <Route path="/notes/:noteId" element={<NotePage />} />
          <Route path="/notes/createNote" element={<CreateNotePage />} />
          <Route path="/notes/:noteId/editNote" element={<EditNotePage />} />
          <Route path="/tests/newTest" element={<NewTestPage />} />
        </Route>

        <Route element={<RequireAuth allowedRoles={[ROLES.User, ROLES.Editor]} />}>
          <Route path="/" element={<Home />} />
          <Route path="/therapies" element={<TherapiesPage />} />
          <Route path="/myAppointments" element={<MyAppointmentsPage />} />
          <Route path="/therapies/:therapyId" element={<TherapyPage />} />
          <Route path="/therapies/:therapyId/appointments" element={<AppointmentsPage />} />
          <Route path="/therapies/:therapyId/appointments/:appointmentId" element={<AppointmentPage />} />
          <Route path="/tests" element={<TestsPage />} />
        </Route>

        <Route element={<RequireAuth allowedRoles={[ROLES.Editor]} />}>
          <Route path="/therapies/:therapyId/appointments/:appointmentId/recommendations" element={<RecommendationsPage />} />
          <Route path="/therapies/:therapyId/appointments/:appointmentId/recommendations/:recommendationId" element={<RecommendationPage />} />
          <Route path="/therapies/createTherapy" element={<CreateTherapyPage />} />
          <Route path="/therapies/:therapyId/editTherapy" element={<EditTherapyPage />} />
          <Route path="/therapies/:therapyId/appointments/createAppointment" element={<CreateAppointment />} />
          <Route path="/therapies/:therapyId/appointments/:appointmentId/editAppointment" element={<EditAppointment />} />
          <Route path="/therapies/:therapyId/appointments/createAppointment" element={<CreateAppointment />} />
          <Route path="/therapies/:therapyId/appointments/:appointmentId/editAppointment" element={<EditAppointment />} />
          <Route path="/therapies/:therapyId/appointments/:appointmentId/recommendations/createRecommendation" element={<CreateRecommendation />} />
          <Route path="/therapies/:therapyId/appointments/:appointmentId/recommendations/:recommendationId/editRecommendation" element={<EditRecommendation />} />
        </Route>

        <Route element={<RequireAuth allowedRoles={[ROLES.Editor]} />}>
          <Route path="editor" element={<Editor />} />
        </Route>


        <Route element={<RequireAuth allowedRoles={[ROLES.Admin]} />}>
          <Route path="admin" element={<Admin />} />
          <Route path="registerDoctor" element={<RegisterDoctor />} />
        </Route>

        {/* catch all */}
        <Route path="*" element={<Missing />} />
      </Route>
    </Routes>
  );
}

export default App;