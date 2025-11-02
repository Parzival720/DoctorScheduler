namespace DoctorScheduler.Models
{
    public class Person(string firstName, string lastName)
    {
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string GetFullName()
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
    }

    public class Patient(string firstName, string lastName) : Person(firstName, lastName)
    {
        
    }
}