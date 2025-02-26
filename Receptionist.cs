using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem
{
    public class Receptionist : User
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        public Receptionist() : base("", "", "", "", UserRole.Receptionist)
        {
            this.Email = "";
            this.PhoneNumber = "";
        }

        public Receptionist(string ID, string password, string firstName, string lastName, string email, string phoneNumber)
            : base(ID, password, firstName, lastName, UserRole.Receptionist)
        {
            this.ID = ID;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
        }

        //Receptionist main menu
        public static void MainMenu(User user)
        {
            bool exit = false;

            {
                while (!exit) //loops until an exit condition is met
                {
                    Console.Clear();
                    Menu.MenuBox("Receptionist Menu");
                    ShowMenu(user);

                    if (int.TryParse(Console.ReadLine(), out int input)) //parses the input and, if int, outputs to variable input
                    {
                        Console.Clear();
                        Menu.MenuBox("Receptionist Menu");
                        exit = HandleMenu(user, input);
                    }
                    else
                    {
                        Console.SetCursorPosition(25, 7);
                        Console.WriteLine("Invalid input. Please enter a number.");
                        Console.ReadKey();
                    }
                }
            }

            static void ShowMenu(User user) //displays menu items for receptionists
            {
                Console.WriteLine("Welcome to DOTNET Hospital Management System, {0} {1}\n", user.FirstName, user.LastName);
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Check doctor details");
                Console.WriteLine("2. Check patient details");
                Console.WriteLine("3. Book appointment");
                Console.WriteLine("4. Logout");
                Console.WriteLine("5. Exit");
                Console.SetCursorPosition(25, 7);
            }

            static bool HandleMenu(User user, int input) //switch case for receptionist menu items
            {
                Receptionist receptionist = (Receptionist)user;
                switch (input)
                {
                    case 1:
                        //checks doctor details
                        CheckUser(receptionist, User.UserRole.Doctor);
                        break;
                    case 2:
                        //checks patient details
                        CheckUser(receptionist, User.UserRole.Patient);
                        break;
                    case 3:
                        //books an appointment on behalf of a patient
                        Appointment.BookAppointment(receptionist);
                        break;
                    case 4:
                        //exits to login menu
                        Console.WriteLine("Returning to login...");
                        Console.ReadKey();
                        Menu.NewMenu("Login");
                        return true;
                    case 5:
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


    }
}
