using DoctorScheduler.Logic;

namespace DoctorScheduler.Models
{
    public class Clinic(string name)
    {
        public string Name { get; set; } = name;
        public List<Doctor> Doctors { get; set; } = [];
        public List<Patient> Patients { get; set; } = [];

        public void AddNewDoctor(Doctor doctor)
        {
            Doctors.Add(doctor);
        }

        public void AddNewPatient(Patient patient)
        {
            Patients.Add(patient);
        }

        public bool ScheduleNewAppointment(Appointment appointment)
        {
            if (!ValidityChecker.AppointmentIsValid(this, appointment))
            {
                Console.WriteLine("Appointment is Invalid");
                return false;
            }
            if (!Patients.Contains(appointment.Patient))
            {
                AddNewPatient(appointment.Patient);
            }
            if (!Doctors.Contains(appointment.Doctor))
            {
                AddNewDoctor(appointment.Doctor);
            }
            appointment.Patient.Schedule.Add(appointment.DateTime.ToString(), appointment);
            appointment.Doctor.Schedule.Add(appointment.DateTime.ToString(), appointment);
            return true;
        }

        public void PrintDoctorList()
        {
            foreach (Doctor d in Doctors)
            {
                Console.WriteLine(d);
            }
        }

        public void PrintPatientList()
        {
            foreach (Patient p in Patients)
            {
                Console.WriteLine(p);
            }
        }
    }
}