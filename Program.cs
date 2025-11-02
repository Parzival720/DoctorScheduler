
using DoctorScheduler.Logic;
using DoctorScheduler.Models;

Doctor doctor1 = new("Steven", "Strange");
// Console.WriteLine(doctor1.GetDoctorName());

Patient patient1 = new("John", "Doe");
Patient patient2 = new("Jane", "Doe");
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

DateTime invalidMinutes = new(2021, 11, 1, 15, 30, 0);
ValidityChecker.DateIsValid(invalidMinutes);

DateTime invalidHour = new(2021, 11, 1, 17, 0, 0);
ValidityChecker.DateIsValid(invalidHour);

DateTime invalidDay = new(2021, 11, 6, 15, 0, 0);
ValidityChecker.DateIsValid(invalidDay);

DateTime invalidMonth = new(2021, 5, 12, 15, 0, 0);
ValidityChecker.DateIsValid(invalidMonth);

DateTime invalidYear = new(2020, 11, 1, 15, 0, 0);
ValidityChecker.DateIsValid(invalidYear);

Appointment appointment2 = new(doctor1, patient2, nov1_3pm);
success = sampleClinic.ScheduleNewAppointment(appointment2);
Console.WriteLine($"{success}: {appointment2}");

DateTime nov2_8am = new(2021, 11, 2, 8, 0, 0);
Appointment appointment3 = new(doctor1, patient2, nov2_8am);
success = sampleClinic.ScheduleNewAppointment(appointment3);
Console.WriteLine($"{success}: {appointment3}");


// Connect to API and get initial state of schedule

// Get queue of appointments and schedule them