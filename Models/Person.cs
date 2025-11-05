using System.Runtime.CompilerServices;

namespace DoctorScheduler.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
        public Dictionary<string, Appointment> Schedule { get; set; } = [];

        public Person(string firstName, string lastName, int id)
        {
            FirstName = firstName;
            LastName = lastName;
            Id = id;
        }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Random random = new();
            Id = random.Next();
        }

        public Person(int id) : this("John", "Doe", id)
        {

        }
        
        public void AddAppointment(Appointment appointment)
        {
            Schedule.Add(appointment.DateTime.ToString(), appointment);
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public bool Equals(Person? other)
        {
            if (other == null)
            {
                return false;
            }
            return GetFullName == other.GetFullName;
        }

        public override bool Equals(object? obj) => Equals(obj as Person);
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }

    public class Doctor : Person
    {

        public Doctor(string firstName, string lastName) : base(firstName, lastName) {}
        
        public Doctor(int id) : base(id) {}


        public string GetDoctorName()
        {
            return $"Doctor {LastName}";
        }

        public override string ToString()
        {
            return $"Doctor {base.ToString()}";
        }
    }

    public class Patient : Person
    {
        public Patient(string firstName, string lastName) : base(firstName, lastName) {}
        
        public Patient(int id) : base(id) {}
    }
}