using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem
{
    public class Menu
    {
        public string menuHeader;
        public string menuMessage;

        public Menu()
        {
            this.menuHeader = "DOTNET Hospital Management System";
            this.menuMessage = "Login";
        }

        public static void MenuBox(string menuMessage)
        {
            string menuHeader = "DOTNET Hospital Management System";

            //horizontal length of menu box - menuHeader + 2
            string menuLength = new string('─', 35);

            //alternates between spaces and dashes
            string divider = string.Join("", Enumerable.Range(0, menuLength.Length).Select(i => (i % 2 == 0) ? ' ' : '-'));

            //centers the menu message
            string message = Utils.centerText(menuMessage);

            //prints menu box using the alt codes
            Console.WriteLine("┌" + menuLength + "┐");
            Console.WriteLine("│ " + menuHeader + " │"); //DOTNET Hospital Management System
            Console.WriteLine("│" + divider + "│"); //dividing line
            Console.WriteLine("│ " + message + " │"); //variable message depending on menu displayed
            Console.WriteLine("└" + menuLength + "┘");
        }

        //main menu for DOTNET Hospital Management System
        public static void MainMenu()
        {
            bool flag = true;

            while (flag)
            {
                Console.WriteLine("ID: ");
                Console.WriteLine("Password: ");

                Console.SetCursorPosition(4, 5);
                string ID = Console.ReadLine();

                Console.SetCursorPosition(10, 6);
                string password = Utils.MaskedPassword(); //masked password

                User currentUser = User.UserLogin(ID, password);

                Console.ReadKey();
                Console.Clear(); //console is emptied to display new data

                switch (currentUser.Role) //starts menu based on user role. e.g. Administrator, Patient...
                {
                    case User.UserRole.Administrator:
                        Menu.MenuBox("Administrator Menu");
                        Administrator.MainMenu((Administrator)currentUser);
                        break;
                    case User.UserRole.Doctor:
                        Menu.MenuBox("Doctor Menu");
                        Doctor.MainMenu((Doctor)currentUser);
                        break;
                    case User.UserRole.Patient:
                        Menu.MenuBox("Patient Menu");
                        Patient.MainMenu((Patient)currentUser);
                        break;
                    case User.UserRole.Receptionist:
                        Menu.MenuBox("Receptionist Menu");
                        Receptionist.MainMenu((Receptionist)currentUser);
                        break;
                    case User.UserRole.None:
                        Console.WriteLine("Invalid user role");
                        MainMenu();
                        break;
                }
                break;

            }
        }

        //clears the console, refreshes menu
        public static void NewMenu(string message)
        {
            Console.Clear();
            Menu.MenuBox(message);
            Menu.MainMenu();
        }

        //clears the console and creates a new title in message box
        public static void NewTitle(string message)
        {
            Console.Clear();
            Menu.MenuBox(message);
        }

        //formats the header for doctor information to be displayed
        public static void DoctorFormat()
        {
            Console.WriteLine(); //line break
            Console.WriteLine(string.Format("{0,-18} | {1,-25} | {2,-12} | {3,-40}", "Name", "Email Address", "Phone", "Address"));
            Console.WriteLine(new string('-', 120)); //dashed divider
        }

        //formats the header for patient information to be displayed
        public static void PatientFormat()
        {
            Console.WriteLine(); //line break
            Console.WriteLine(string.Format("{0,-18} | {1,-18} | {2,-25} | {3,-12} | {4,-25}",
               "Patient", "Doctor", "Email Address", "Phone", "Address"));
            Console.WriteLine(new string('-', 120)); //dashed divider
        }

        //formats the header for appointment information to be displayed
        public static void AppointmentFormat()
        {
            Console.WriteLine(); //line break
            Console.WriteLine("{0, -18} | {1, -18} | {2, -30}", "Doctor", "Patient", "Description");
            Console.WriteLine(new string('-', 100));
        }


    }
}
