using DoctorScheduler.Models;

namespace DoctorScheduler.Logic
{
    public static class ValidityChecker
    {
        public static bool AppointmentIsValid(Appointment appointment)
        {
            // Check if the date is valid
            if (!DateIsValid(appointment.DateTime))
            {
                Console.WriteLine($"Date is invalid: {appointment.DateTime}");
                return false;
            }
            // Check if Patient has a conflict

            // Check if Doctor has a conflict
            
            return true;
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