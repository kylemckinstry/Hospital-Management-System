using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem
{
    public class Patient : User
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string StreetNo { get; private set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string DoctorID { get; private set; } //ID of assigned doctor

        public Patient() : base("", "", "", "", UserRole.Patient)
        {
            this.Email = "";
            this.PhoneNumber = "";
            this.StreetNo = "";
            this.Street = "";
            this.City = "";
            this.State = "";
            this.DoctorID = null;  //no initial doctor assigned
        }

        public Patient(string ID, string password, string firstName, string lastName, string email, string phoneNumber, string streetNo, string street, string city, string state, string doctorID) 
            : base(ID, password, firstName, lastName, UserRole.Patient)
        {
            this.ID = ID;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.StreetNo = streetNo;
            this.Street = street;
            this.City = city;
            this.State = state;
            this.DoctorID = doctorID;
        }

        public override string ToString()
        {
            //checks if DoctorID is not null or empty before attempting to retrieve the doctor
            Doctor doctor = !string.IsNullOrEmpty(DoctorID) ? (Doctor)User.GetUser(this, DoctorID) : null;

            string patientFullName = this.FormatUserFullName(); //extension method to format fullname
            string doctorFullName = doctor != null ? doctor.FormatUserFullName() : "-"; //adds doctors name, or '-' if empty
            string fullAddress = $"{StreetNo} {Street}, {City}, {State}";

            //ensures the same format is applied as the header
            return string.Format(
                "{0,-18} | {1,-18} | {2,-25} | {3,-12} | {4,-25}",
                patientFullName, doctorFullName, Email, PhoneNumber, fullAddress
            );
        }

        //assigns doctor ID to patient
        public void AssignDoctor(string doctorID)
        {
            if (!string.IsNullOrEmpty(doctorID))
            {
                this.DoctorID = doctorID;
            }
            else
            {
                Console.WriteLine("Invalid doctor ID.");
            }
        }

        //Patient main menu
        public static void MainMenu(User user)
        {
            bool exit = false;

            while (!exit) // while not exitting
            {
                Console.Clear();
                Menu.MenuBox("Patient Menu");
                showMenu(user);

                if (int.TryParse(Console.ReadLine(), out int input)) //this parses the input and if int, outputs to variable input
                {
                    Console.Clear();
                    Menu.MenuBox("Patient Menu"); 
                    exit = handleMenu(user,input);
                }
                else
                {
                    Console.SetCursorPosition(25, 7);
                    Console.WriteLine("Invalid input. Please enter a number.");
                    Console.ReadKey();
                }

            }

        }

        static void showMenu(User user) //displays menu items for Patients
        {
            Console.WriteLine("Welcome to DOTNET Hospital Management System, {0} {1}\n", user.FirstName, user.LastName);
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. List patient details");
            Console.WriteLine("2. List my doctor details");
            Console.WriteLine("3. List all appointments");
            Console.WriteLine("4. Book appointment");
            Console.WriteLine("5. Logout");
            Console.WriteLine("6. Exit");
            Console.SetCursorPosition(25, 7);
        }

        static bool handleMenu(User user, int input) //switch case for patient menu items
        {
            Patient patient = (Patient)user;
            switch (input)
            {
                case 1:
                    //lists patient details - unique format as shown in brief
                    patient.ListDetails(patient);
                    break;
                case 2:
                    //lists details for doctor assigned to patient
                    patient.ListDetails(patient, patient.DoctorID);
                    break;
                case 3:
                    //lists all of the patients appointments
                    Appointment.ListAppointments(patient);
                    break;
                case 4:
                    //books appointment
                    Appointment.BookAppointment(patient);
                    break;
                case 5:
                    //exits to login menu
                    Console.WriteLine("Returning to login...");
                    Console.ReadKey();
                    Menu.NewMenu("Login");
                    return true;
                case 6:
                    //exits program
                    Console.WriteLine("Exiting program.");
                    Console.ReadKey();
                    return true;
                default:
                    Console.WriteLine("Please enter a valid input.");
                    break;
            }

            Console.ReadKey();
            return false;
        }

        //lists the patients details
        public override void ListDetails(User currentUser)
        {
            //creates a new patient with explicit casting
            Patient patient = (Patient)currentUser;

            Menu.NewTitle("My Details");
            Console.WriteLine("{0} {1}'s Details\n", patient.FirstName, patient.LastName);
            Console.WriteLine("Patient ID: {0}", patient.ID);
            Console.WriteLine("Full Name: {0} {1}", patient.FirstName, patient.LastName);
            Console.WriteLine("Address: {0} {1}, {2}, {3}", patient.StreetNo, patient.Street, patient.City, patient.State);
            Console.WriteLine("Email: {0}", patient.Email);
            Console.WriteLine("Phone: {0}", patient.PhoneNumber);
            Console.WriteLine("\nPress any key to return to menu.");
        }

        //lists doctor details
        public override void ListDetails(User currentUser, string doctorID)
        {

            if (string.IsNullOrEmpty(doctorID))
            {
                Menu.NewTitle("No Doctor Assigned");

                Console.WriteLine("You currently do not have a doctor assigned.");
                Console.WriteLine("\nPress any key to return to the menu.");
                return;
            }

            Doctor doctor = (Doctor)User.GetUser(currentUser, doctorID);

            Menu.NewTitle("My Doctor");
            Console.WriteLine("Your doctor:\n");
            Menu.DoctorFormat();
            Console.WriteLine(doctor.ToString());

            Console.WriteLine("\nPress any key to return to menu.");
        }


    }
}
