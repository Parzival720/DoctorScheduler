
using DoctorScheduler.Models;

Doctor doctor1 = new("Steven", "Strange");
// Console.WriteLine(doctor1.GetDoctorName());

Patient patient1 = new("John", "Doe");
// Console.WriteLine(patient1.GetFullName());

Doctor doctor2 = new("John", "Watson");
Clinic sampleClinic = new("Provo Health Clinic");
sampleClinic.AddNewDoctor(doctor1);
sampleClinic.AddNewDoctor(doctor2);
sampleClinic.PrintDoctorList();

DateTime nov1_3pm = new(2021, 11, 1, 15, 0, 0);
Appointment appointment1 = new(doctor1, patient1, nov1_3pm);
Console.WriteLine(appointment1);

bool success = sampleClinic.ScheduleNewAppointment(appointment1);
Console.WriteLine($"{success}: {appointment1}");

// Connect to API and get initial state of schedule

// Get queue of appointments and schedule them