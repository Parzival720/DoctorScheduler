
using DoctorScheduler.Logic;
using DoctorScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

public class Program
{
    private static readonly HttpClient sharedClient = new()
    {
        // BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
        BaseAddress = new Uri("https://scheduling.interviews.brevium.com/"),
    };

    private static string WithToken(string relativeUrl)
    {
        var token = Environment.GetEnvironmentVariable("BREVIUM_API_KEY");
        return relativeUrl + "?token=" + token;
    }
    public static async Task Main(string[] args)
    {
        await SchedulingStart();
        Clinic clinic = new("SampleClinic");
        await GetInitialSchedule(clinic);

        while (true)
        {
            try
            {
                await GetNextRequest(clinic);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                break;
            }
        }

        await SchedulingStop();
    }

    public record class AppointmentData(
        int? PersonId = null,
        int? DoctorId = null,
        string? AppointmentTime = null,
        bool? IsNewPatientAppointment = null);
    public static async Task GetInitialSchedule(Clinic clinic)
    {
        List<AppointmentData>? appointments;
        try
        {
            appointments = await sharedClient.GetFromJsonAsync<List<AppointmentData>>(
                WithToken("api/Scheduling/Schedule"));
        }
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
                Console.WriteLine("Your token is invalid");
            }
            else if (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
                Console.WriteLine("The schedule has already been retrieved for this run");
            }
            throw new Exception(ex.Message);
        }

        Console.WriteLine("GET https://scheduling.interviews.brevium.com/swagger/v1/api/Scheduling/Schedule HTTP/1.1");
        if (appointments == null || appointments.Count == 0)
        {
            Console.WriteLine("No appointments returned from API.");
            return;
        }

        foreach (var apptData in appointments)
        {
            if (apptData.PersonId == null || apptData.DoctorId == null || string.IsNullOrWhiteSpace(apptData.AppointmentTime))
            {
                Console.WriteLine($"Skipping invalid appointment data: {System.Text.Json.JsonSerializer.Serialize(apptData)}");
                continue;
            }

            Patient patient = clinic.GetPatient(apptData.PersonId.Value) ?? new Patient(apptData.PersonId.Value);
            Doctor doctor = clinic.GetDoctor(apptData.DoctorId.Value) ?? new Doctor(apptData.DoctorId.Value);

            if (!DateTimeOffset.TryParse(apptData.AppointmentTime, out var parsedDto))
            {
                Console.WriteLine($"Unable to parse appointment time '{apptData.AppointmentTime}' for patient {apptData.PersonId}. Skipping.");
                continue;
            }
            DateTime parsedDateTime = parsedDto.UtcDateTime;

            Appointment appointment = new(doctor, patient, parsedDateTime);

            bool success = clinic.AddAppointmentToSchedule(appointment);
            Console.WriteLine($"Importing appointment: {appointment} -> scheduled={success}");
        }
    }
    public record class AppointmentRequest(
        int? RequestId = null,
        int? PersonId = null,
        List<string>? PreferredDays = null,
        List<int>? PreferredDocs = null,
        bool? IsNew = null);
    public static async Task GetNextRequest(Clinic clinic)
    {
        AppointmentRequest? appointmentRequest;
        try
        {
            appointmentRequest = await sharedClient.GetFromJsonAsync<AppointmentRequest>(
                WithToken("api/Scheduling/AppointmentRequest"));
        }
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
                Console.WriteLine("Your token is invalid");
            }
            else if (ex.Message.Contains(HttpStatusCode.MethodNotAllowed.ToString()))
            {
                Console.WriteLine("You have already called the stop endpoint for this run");
            }
            else if (ex.Message.Contains(HttpStatusCode.NoContent.ToString()))
            {
                Console.WriteLine("There are no more requests to handle");
            }
            throw new Exception(ex.Message);
        }

        Console.WriteLine("GET https://scheduling.interviews.brevium.com/swagger/v1/api/Scheduling/AppointmentRequest HTTP/1.1");
        if (appointmentRequest == null)
        {
            Console.WriteLine("No appointment request returned from API.");
            return;
        }
        if (appointmentRequest.PersonId == null || appointmentRequest.PreferredDays == null || appointmentRequest.PreferredDocs == null)
        {
            Console.WriteLine($"Skipping invalid appointment request: {System.Text.Json.JsonSerializer.Serialize(appointmentRequest)}");
            return;
        }
        List<string> preferredDays = appointmentRequest.PreferredDays;
        List<int> preferredDocs = appointmentRequest.PreferredDocs;
        Patient patient = clinic.GetPatient(appointmentRequest.PersonId.Value) ?? new Patient(appointmentRequest.PersonId.Value);
        foreach (string day in preferredDays)
        {
            if (!DateTimeOffset.TryParse(day, out var parsedDayDto))
            {
                Console.WriteLine($"Unable to parse appointment time '{day}' for patient {appointmentRequest.PersonId}: {day} Skipping.");
                continue;
            }
            DateTime parsedDateTime = parsedDayDto.UtcDateTime;
            parsedDateTime = parsedDateTime.AddHours(8);
            for (int i = 0; i < 9; i++)
            {
                foreach (int doc in preferredDocs)
                {
                    Doctor d = clinic.GetDoctor(doc) ?? new Doctor(doc);
                    bool IsNewPatientAppointment = clinic.IsNewPatient(patient);
                    Appointment appt = new(d, patient, parsedDateTime);
                    bool success = clinic.ScheduleNewAppointment(appt);
                    if (success)
                    {
                        Console.WriteLine($"Successfully booked appointment for Patient with id {patient.Id} at {parsedDateTime} with Doctor {d.Id}");
                        Random random = new();
                        AppointmentInfo apptInfo = new(d.Id, patient.Id, parsedDateTime.ToString("o"), IsNewPatientAppointment, random.Next());
                        await ScheduleAppointment(apptInfo);
                        return;
                    }
                }
                parsedDateTime = parsedDateTime.AddHours(1);
            }
        }

        Console.WriteLine($"Unable to book appointment for Patient {patient.Id}");
    }

    public static async Task SchedulingStart()
    {
        using HttpResponseMessage response = await sharedClient.PostAsync(
            WithToken("api/Scheduling/Start"),
            null);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Successfully hit Start endpoint");
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Your token is invalid");
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
        }
    }

    public static async Task SchedulingStop()
    {
        using HttpResponseMessage response = await sharedClient.PostAsync(
            WithToken("api/Scheduling/Stop"),
            null);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Successfully hit Stop endpoint");
            var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentData>>();
            Console.WriteLine($"{appointments}\n");

        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Your token is invalid");
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
        }
    }

    public record class AppointmentInfo(
        int? DoctorId = null,
        int? PersonId = null,
        string? AppointmentTime = null,
        bool? IsNewPatientAppointment = null,
        int? RequestId = null);
    public static async Task ScheduleAppointment(AppointmentInfo appointmentInfo)
    {
        using HttpResponseMessage response = await sharedClient.PostAsJsonAsync(
            WithToken("api/Scheduling/Schedule"),
            appointmentInfo);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("The schedule has been updated");

        }
        else if (response.StatusCode == HttpStatusCode.MethodNotAllowed)
        {
            Console.WriteLine("You have already called the stop endpoint for this run");
        }
        else if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            Console.WriteLine("The schedule was unable to accomodate the requested appointment");
        }
        else
        {
            System.Console.WriteLine($"Request failed with status code: {response.StatusCode}");
        }
    }
}

// Connect to API and get initial state of schedule

// Get queue of appointments and schedule them



// Doctor doctor1 = new("Steven", "Strange");
// // Console.WriteLine(doctor1.GetDoctorName());

// Patient patient1 = new("John", "Doe");
// Patient patient2 = new("Jane", "Doe");
// // Console.WriteLine(patient1.GetFullName());

// Doctor doctor2 = new("John", "Watson");
// Clinic sampleClinic = new("Provo Health Clinic");
// sampleClinic.AddNewDoctor(doctor1);
// sampleClinic.AddNewDoctor(doctor2);
// sampleClinic.PrintDoctorList();

// DateTime nov1_3pm = new(2021, 11, 1, 15, 0, 0);
// Appointment appointment1 = new(doctor1, patient1, nov1_3pm);
// Console.WriteLine(appointment1);

// bool success = sampleClinic.ScheduleNewAppointment(appointment1);
// Console.WriteLine($"{success}: {appointment1}");

// DateTime invalidMinutes = new(2021, 11, 1, 15, 30, 0);
// ValidityChecker.DateIsValid(invalidMinutes);

// DateTime invalidHour = new(2021, 11, 1, 17, 0, 0);
// ValidityChecker.DateIsValid(invalidHour);

// DateTime invalidDay = new(2021, 11, 6, 15, 0, 0);
// ValidityChecker.DateIsValid(invalidDay);

// DateTime invalidMonth = new(2021, 5, 12, 15, 0, 0);
// ValidityChecker.DateIsValid(invalidMonth);

// DateTime invalidYear = new(2020, 11, 1, 15, 0, 0);
// ValidityChecker.DateIsValid(invalidYear);

// Appointment appointment2 = new(doctor1, patient2, nov1_3pm);
// success = sampleClinic.ScheduleNewAppointment(appointment2);
// Console.WriteLine($"{success}: {appointment2}");

// DateTime nov2_8am = new(2021, 11, 2, 8, 0, 0);
// Appointment appointment3 = new(doctor1, patient2, nov2_8am);
// success = sampleClinic.ScheduleNewAppointment(appointment3);
// Console.WriteLine($"{success}: {appointment3}");


