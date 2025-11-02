namespace DoctorScheduler.Models
{
    public class Appointment(Doctor doctor, Patient patient, DateTime dateTime)
    {
        public Doctor Doctor = doctor;
        public Patient Patient = patient;
        public DateTime DateTime = dateTime;
        
    }
}