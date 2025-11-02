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