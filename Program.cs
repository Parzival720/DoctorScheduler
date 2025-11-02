
using DoctorScheduler.Models;

Doctor doctor1 = new("Steven", "Strange");
Console.WriteLine(doctor1.GetDoctorName());

Patient patient1 = new("John", "Doe");
Console.WriteLine(patient1.GetFullName());