using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HospitalManagementSystem
{
    public class Doctor : User
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string StreetNo { get; private set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public List<string> PatientIDs { get; set; } //doctors can have multiple patients

        public Doctor() : base("", "", "", "", UserRole.Doctor)
        {
            this.Email = "";
            this.PhoneNumber = "";
            this.StreetNo = "";
            this.Street = "";
            this.City = "";
            this.State = "";
            this.PatientIDs = null;  // No patient assigned
        }

        public Doctor(string ID, string password, string firstName, string lastName, string email, string phoneNumber, string streetNo, string street, string city, string state, List<string> patientIDs) 
            : base(ID, password, firstName, lastName, UserRole.Doctor)
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
            this.PatientIDs = patientIDs;
        }

        public override string ToString()
        {
            string fullName = this.FormatUserFullName(); //extension method to format fullname
            string fullAddress = $"{StreetNo} {Street}, {City}, {State}";

            //ensures the same format is applied as the header
            return string.Format(
                "{0,-18} | {1,-25} | {2,-12} | {3,-40}",
                fullName, Email, PhoneNumber, fullAddress
            );
        }

        //Doctor main menu
        public static void MainMenu(User user)
        {
            bool exit = false;

            {
                while (!exit) //loops until a valid exit condition is met
                {
                    Console.Clear();
                    Menu.MenuBox("Doctor Menu");
                    showMenu(user);

                    if (int.TryParse(Console.ReadLine(), out int input)) //parses the input and, if int, outputs to variable input
                    {
                        Console.Clear();
                        Menu.MenuBox("Doctor Menu");
                        exit = handleMenu(user, input);
                    }
                    else
                    {
                        Console.SetCursorPosition(25, 7);
                        Console.WriteLine("Invalid input. Please enter a number.");
                        Console.ReadKey();
                    }
                }
            }

            static void showMenu(User user) //displays menu items for Doctors
            {
                Console.WriteLine("Welcome to DOTNET Hospital Management System, {0} {1}\n", user.FirstName, user.LastName);
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. List doctor details");
                Console.WriteLine("2. List patients");
                Console.WriteLine("3. List appointments");
                Console.WriteLine("4. Check particular patient");
                Console.WriteLine("5. List appointments with patient");
                Console.WriteLine("6. Logout");
                Console.WriteLine("7. Exit");
                Console.SetCursorPosition(25, 7);
            }

            static bool handleMenu(User user, int input) //switch case for Doctor menu items
            {
                Doctor doctor = (Doctor)user;
                switch (input)
                {
                    case 1:
                        //lists the doctors (my) details
                        doctor.ListDetails(doctor);
                        break;
                    case 2:
                        //lists all patients assigned to doctor
                        doctor.ListAll(doctor, FileManager.getPatientList(doctor));
                        break;
                    case 3:
                        //list all appointments tied to doctor;
                        Appointment.ListAppointments(doctor);
                        break;
                    case 4:
                        //checks specific patient by ID
                        CheckUser(doctor,User.UserRole.Patient);
                        break;
                    case 5:
                        //lists all appointments with specific patient by ID
                        Appointment.PatientAppointments(doctor);
                        break;
                    case 6:
                        //exits to login menu
                        Console.WriteLine("Returning to login...");
                        Console.ReadKey();
                        Menu.NewMenu("Login");
                        return true;
                    case 7:
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

        }

        public override void ListDetails(User currentUser)
        {
            Doctor doctor = (Doctor)User.GetUser(currentUser, currentUser.ID);

            Menu.NewTitle("My Details"); //sets new title in menu box
            Menu.DoctorFormat(); //sets doctor format header. e.g. Name, Email, Phone, Address and a dashed divider
            Console.WriteLine(doctor.ToString());
            Console.WriteLine("\nPress any key to return to menu.");
        }

        //lists all patients tied to doctor
        public virtual void ListAll(User currentUser, List<string> PatientIDs)
        {
            Menu.NewTitle("My Patients"); //sets new title in menu box

            //if no patients, prints no patients are found
            if (PatientIDs == null || PatientIDs.Count == 0)
            {
                Console.WriteLine("You have no assigned patients.");
            }
            else
            {
                Console.WriteLine("Patients assigned to {0} {1}\n", currentUser.FirstName, currentUser.LastName);
                Menu.PatientFormat(); //sets patient format header.e.g. Patient, Doctor, Email...

                foreach (var patient in PatientIDs)
                {
                    Patient newPatient = (Patient)User.GetUser(currentUser, patient);
                    Console.WriteLine(newPatient.ToString());
                }
            }
            Console.WriteLine("\nPress any key to return to menu.");
        }


    }

}
