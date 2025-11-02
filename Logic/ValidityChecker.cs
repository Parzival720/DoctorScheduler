using DoctorScheduler.Models;

namespace DoctorScheduler.Logic
{
    public static class ValidityChecker
    {
        public static bool AppointmentIsValid(Clinic clinic, Appointment appointment)
        {
            // Check if the date is valid
            if (!DateIsValid(appointment.DateTime))
            {
                Console.WriteLine($"Date is invalid: {appointment.DateTime}");
                return false;
            }
            // Check if Patient has a conflict
            if (CheckPatientConflict(clinic, appointment))
            {
                return false;
            }
            // Check if Doctor has a conflict
            if (CheckDoctorConflict(appointment))
            {
                return false;
            }
            return true;
        }

        private static bool CheckDoctorConflict(Appointment appointment)
        {
            if (appointment.Doctor.Schedule.ContainsKey(appointment.DateTime.ToString()))
            {
                Console.WriteLine("Doctor already has an appointment at the scheduled time.");
                return true;
            }
            return false;
        }

        private static bool CheckPatientConflict(Clinic clinic, Appointment appointment)
        {
            if (!clinic.Patients.Contains(appointment.Patient) && !(appointment.DateTime.Hour == 15 || appointment.DateTime.Hour == 16))
            {
                Console.WriteLine("New Patient must be scheduled at 3pm or 4pm");
                return true;
            }
            // Check if Patient has an appointment within one week
            return false;
        }

        public static bool DateIsValid(DateTime dateTime)
        {
            return YearIsValid(dateTime.Year) && MonthIsValid(dateTime.Month) && DayIsValid(dateTime) && HourIsValid(dateTime.Hour) && MinutesIsValid(dateTime.Minute);
        }

        private static bool YearIsValid(int year)
        {
            if (year == 2021)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Year is invalid: {year}");
                return false;
            }
        }

        private static bool MonthIsValid(int month)
        {
            if (month == 11 || month == 12)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Month is invalid: {month}");
                return false;
            }
        }

        private static bool DayIsValid(DateTime date)
        {
            int MaxDay = date.Month == 11 ? 30 : 31;
            if (date.Day >= 0 && date.Day <= MaxDay && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Day is invalid: {date.DayOfWeek} the {date.Day}");
                return false;
            }
        }

        private static bool HourIsValid(int hour)
        {
            if (hour >= 8 && hour <= 16)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Hour is invalid: {hour}");
                return false;
            }
        }
        
        private static bool MinutesIsValid(int minutes)
        {
            if (minutes == 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Minutes is invalid: {minutes}");
                return false;
            }
        }
    }
}