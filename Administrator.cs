using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static HospitalManagementSystem.User;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem
{
    public class Administrator : User
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        public Administrator() : base("", "", "", "", UserRole.Administrator)
        {
            this.Email = "";
            this.PhoneNumber = "";
        }

        public Administrator(string ID, string password, string firstName, string lastName, string email, string phoneNumber)
            : base(ID, password, firstName, lastName, UserRole.Administrator)
        {
            this.ID = ID;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
        }
        
        //Administrator main menu
        public static void MainMenu(User user)
        {
            bool exit = false;

            {
                while (!exit) //loops until an exit condition is met
                {
                    Console.Clear();
                    Menu.MenuBox("Administrator Menu");
                    showMenu(user);

                    if (int.TryParse(Console.ReadLine(), out int input)) //parses the input and, if int, outputs to variable input
                    {
                        Console.Clear();
                        Menu.MenuBox("Administrator Menu");
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

            static void showMenu(User user) //displays menu items for Administrators
            {
                Console.WriteLine("Welcome to DOTNET Hospital Management System, {0} {1}\n", user.FirstName, user.LastName);
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. List all doctors");
                Console.WriteLine("2. Check doctor details");
                Console.WriteLine("3. List all patients");
                Console.WriteLine("4. Check patient details");
                Console.WriteLine("5. Add doctor");
                Console.WriteLine("6. Add patient");
                Console.WriteLine("7. Logout");
                Console.WriteLine("8. Exit");
                Console.SetCursorPosition(25, 7);
            }

            static bool handleMenu(User user, int input) //switch case for administrator menu items
            {
                Administrator admin = (Administrator)user;
                switch (input)
                {
                    case 1:
                        //lists all doctors
                        admin.ListAll(admin, User.UserRole.Doctor);
                        break;
                    case 2:
                        //checks specific ID doctor details
                        CheckUser(admin, User.UserRole.Doctor);
                        break;
                    case 3:
                        admin.ListAll(admin, User.UserRole.Patient);
                        break;
                    case 4:
                        //checks specific ID patient details
                        CheckUser(admin, User.UserRole.Patient);
                        break;
                    case 5:
                        //adds a doctor to the HospitalDB.txt file
                        AddUser("Doctor");
                        break;
                    case 6:
                        //adds a patient to the HospitalDB.txt file
                        AddUser("Patient");
                        break;
                    case 7:
                        //exits to login menu
                        Console.WriteLine("Returning to login...");
                        Console.ReadKey();
                        Menu.NewMenu("Login");
                        return true;
                    case 8:
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

        //adds user taking userType e.g. Patient, Doctor
        public static void AddUser(string userType)
        {
            Menu.NewTitle($"Add {userType}"); //variable menu title. e.g. 'Add Patient', 'Add Doctor'
            Console.WriteLine("Registering a new {0} with the DOTNET Hospital Management System", userType);

            //inputs for user properties with validation
            string firstName;
            while (true)
            {
                firstName = InputPrompt("First Name: ");
                if (Utils.IsValidName(firstName))
                    break;
                else
                    Console.WriteLine("Invalid first name. Only alphabetic characters allowed. Please try again.");
            }

            string lastName;
            while (true)
            {
                lastName = InputPrompt("Last Name: ");
                if (Utils.IsValidName(lastName))
                    break;
                else
                    Console.WriteLine("Invalid last name. Only alphabetic characters allowed. Please try again.");
            }

            string email;
            while (true)
            {
                email = InputPrompt("Email: ");
                if (Utils.IsValidEmail(email))
                    break;
                else
                    Console.WriteLine("Invalid email format. Please try again.");
            }

            string phone;
            while (true)
            {
                phone = InputPrompt("Phone: ");
                if (Utils.IsValidPhoneNumber(phone))
                    break;
                else
                    Console.WriteLine("Invalid phone number. Must be up to 12 digits. Please try again.");
            }

            string streetNo;
            while (true)
            {
                streetNo = InputPrompt("Street Number: ");
                if (Utils.IsValidStreetNo(streetNo))
                    break;
                else
                    Console.WriteLine("Invalid street number. Must be a positive integer. Please try again.");
            }

            string street;
            while (true)
            {
                street = InputPrompt("Street: ");
                if (Utils.IsValidStreet(street))
                    break;
                else
                    Console.WriteLine("Invalid street name. Only alphanumeric characters, spaces, and periods are allowed. Please try again.");
            }

            string city;
            while (true)
            {
                city = InputPrompt("City: ");
                if (Utils.IsValidCityOrState(city))
                    break;
                else
                    Console.WriteLine("Invalid city name. Only alphabetic characters allowed. Please try again.");
            }

            string state;
            while (true)
            {
                state = InputPrompt("State: ");
                if (Utils.IsValidCityOrState(state))
                    break;
                else
                    Console.WriteLine("Invalid state name. Only alphabetic characters allowed. Please try again.");
            }

            //writes new user to HospitalDB.txt
            FileManager.WriteToDB(userType, firstName, lastName, email, phone, streetNo, street, city, state, null, null);
            Console.WriteLine("{0} {1} added to the system!", firstName, lastName);
            Console.WriteLine("\nPress any key to return to menu.");
        }

        //input prompts for adding user
        public static string InputPrompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        //lists all patients or doctors depending on userRole argument 
        public override void ListAll(User currentUser, User.UserRole userRole)
        {
            List<string[]> data = FileManager.readDB("HospitalDB.txt");

            if (userRole == User.UserRole.Patient)
            {
                Menu.NewTitle("All Patients");
                Console.WriteLine("All patients registered to the DOTNET Hospital Management System\n");
                Menu.PatientFormat();

            }
            else if (userRole == User.UserRole.Doctor)
            {
                Menu.NewTitle("All Doctors");
                Console.WriteLine("All doctors registered to the DOTNET Hospital Management System\n");
                Menu.DoctorFormat();
            }
            else
            {
                Menu.NewTitle("List All");
                Console.WriteLine("Invalid user role lookup");
            }

            foreach (var row in data) //loops through the data to find the user by their ID
            {
                if (row[2] == userRole.ToString())
                {
                    if (userRole == User.UserRole.Patient)
                    {
                        Patient patient = (Patient)GetUser(currentUser, row[0]);
                        Console.WriteLine(patient.ToString()); //patient ToString to print short line
                    }
                    else if (userRole == User.UserRole.Doctor)
                    {
                        Doctor doctor = (Doctor)GetUser(currentUser, row[0]);
                        Console.WriteLine(doctor.ToString()); //doctor ToString to print short line
                    } 

                }
            }

            Console.WriteLine("\nPress any key to return to menu.");
        }


    }
}
