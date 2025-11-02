namespace DoctorScheduler.Models
{
    public class Person(string firstName, string lastName)
    {
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public Dictionary<string, Appointment> Schedule { get; set; } = [];

        public void AddAppointment(Appointment appointment)
        {
            Schedule.Add(appointment.DateTime.ToString(), appointment);
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }

    public class Doctor(string firstName, string lastName) : Person(firstName, lastName)
    {
        public string GetDoctorName()
        {
            return $"Doctor {LastName}";
        }

        public override string ToString()
        {
            return $"Doctor {base.ToString()}";
        }
    }

    public class Patient(string firstName, string lastName) : Person(firstName, lastName)
    {
        
    }
}