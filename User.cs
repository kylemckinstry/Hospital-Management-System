using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalManagementSystem
{
    public abstract class User
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; } //e.g. Patient, Doctor, Administrator, Receptionist

        public enum UserRole
        {
            None,
            Patient,
            Doctor,
            Administrator,
            Receptionist
        }

        public User()
        {
            ID = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Role = UserRole.None;
        }

        public User(string ID, string password, string firstName, string lastName, UserRole role)
        {
            this.ID = ID;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Role = role;
        }

        //takes input ID and password 
        public static User UserLogin(string id, string password)
        {
            bool check = Utils.credentialCheck(id, password);

            if (check)
            {
                List<string[]> data = FileManager.readDB("HospitalDB.txt");

                foreach (var row in data)
                {
                    //ensures the minimum number of elements required
                    if (row.Length < 11)
                    {
                        Console.WriteLine("Data row is incomplete.");
                        continue;
                    }

                    string userID = row[0];
                    string userPassword = row[1];

                    if (!Enum.TryParse(row[2], out User.UserRole userRole))
                    {
                        Console.WriteLine($"Invalid role for user ID {userID}");
                        continue;
                    }

                    //assigns variables from the read data
                    string userFirstName = row[3];
                    string userLastName = row[4];
                    string userEmail = row[5];
                    string userPhone = row[6];
                    string userStreetNo = row[7];
                    string userStreet = row[8];
                    string userCity = row[9];
                    string userState = row[10];

                    //handles the possibility of missing columns for Doctor and Patient fields
                    string userDoctor = row.Length > 11 ? row[11] : null;
                    string userPatient = row.Length > 12 ? row[12] : null; 

                    List<string> patientIDs = string.IsNullOrEmpty(userPatient) ? new List<string>() : userPatient.Split(';').ToList(); //stores the split patientIDs by their semicolon delimiter 

                    if (userID == id && userPassword == password) //checks if the ID and password match
                    {
                        switch (userRole) //creates a user based on the UserType
                        {
                            case UserRole.Administrator:
                                return new Administrator(userID, userPassword, userFirstName, userLastName, userEmail, userPhone);

                            case UserRole.Doctor:
                                return new Doctor(userID, userPassword, userFirstName, userLastName, userEmail, userPhone, userStreetNo, userStreet, userCity, userState, patientIDs);

                            case UserRole.Patient:
                                return new Patient(userID, userPassword, userFirstName, userLastName, userEmail, userPhone, userStreetNo, userStreet, userCity, userState, userDoctor);

                            case UserRole.Receptionist:
                                return new Receptionist(userID, userPassword, userFirstName, userLastName, userEmail, userPhone);
                        }
                    }
                }
                return null; //if no match is found, return null or throw an exception
            }
            else
            {
                Console.WriteLine("Invalid login");
                return null;
            }
        }

        //instantiates objects based on the user and input ID
        public static User GetUser(User currentUser, string id)
        {
            List<string[]> data = FileManager.readDB("HospitalDB.txt");

            foreach (var row in data) //loops through the data to find the user by their ID
            {

                if (id == row[0]) //if matching user is found
                {
                    string userID = row[0];
                    string userPassword = row[1];

                    if (!Enum.TryParse(row[2], true, out User.UserRole userRole)) //try parsing the user role, ignoring case to avoid issues
                    {
                        Console.WriteLine($"Invalid role for user ID {userID}");
                        continue; //skips row if the role is invalid
                    }

                    string userFirstName = row[3];
                    string userLastName = row[4];
                    string userEmail = row[5];
                    string userPhone = row[6];
                    string userStreetNo = row[7];
                    string userStreet = row[8];
                    string userCity = row[9];
                    string userState = row[10];

                    //handles assigned Doctor and Patient fields, assigns null if they are missing or empty
                    string userDoctor = row.Length > 11 && !string.IsNullOrWhiteSpace(row[11]) ? row[11] : null;
                    string userPatient = row.Length > 12 && !string.IsNullOrWhiteSpace(row[12]) ? row[12] : null;

                    //handles empty or null patient IDs, creates an empty list if no patient IDs are provided - e.g. Patient has doctor but no patients.
                    List<string> patientIDs = !string.IsNullOrEmpty(userPatient) ? userPatient.Split(';').ToList() : new List<string>();

                    //ensures that required data is not empty before proceeding
                    if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(userFirstName) || string.IsNullOrEmpty(userLastName))
                    {
                        Console.WriteLine($"User data is incomplete for ID {userID}");
                        continue; //skips to the next row if data is missing
                    }

                    switch (userRole) //checks the role of the current user to control access
                    {
                        case User.UserRole.Administrator:
                            return new Administrator(userID, userPassword, userFirstName, userLastName, userEmail, userPhone);

                        case User.UserRole.Doctor:
                            return new Doctor(userID, userPassword, userFirstName, userLastName, userEmail, userPhone, userStreetNo, userStreet, userCity, userState, patientIDs);

                        case User.UserRole.Patient:
                            return new Patient(userID, userPassword, userFirstName, userLastName, userEmail, userPhone, userStreetNo, userStreet, userCity, userState, userDoctor);

                        case User.UserRole.Receptionist:
                            return new Receptionist(userID, userPassword, userFirstName, userLastName, userEmail, userPhone);

                        default:
                            Console.WriteLine("Unknown role. Access denied."); //handles unknown roles
                            return null;
                    }
                }
            }
            return null; //returns null if no user is found with the given ID
        }

        //abstract methods implemented in derived classes - Patient, Doctor, Administrator, and Receptionist

        public virtual void ListDetails(User currentUser)
        {
            //lists details such as my details for Doctor - ListDetails(doctor), ListDetails(patient)
        }

        public virtual void ListDetails(User currentUser, string lookupID) 
        {
            //lists details with a lookup ID such as doctor details for Patient - ListDetails(patient, patient.DoctorID)
        }

        public virtual void ListAll(User currentUser, User.UserRole userRole) //list details with userRole
        {
            //lists all users given a user and a lookup userRole, such as list all for Admin - ListAll(admin, User.UserRole.Patient)
        }

        //checks user details
        public static void CheckUser(User currentUser, User.UserRole userRole)
        {
            Menu.NewTitle("Check");
            string role = "";
            int cursorX = 0; //cursorX and cursorY change the cursor position depending on the 'role' i.e. patient, doctor
            int cursorY = 0;

            if (userRole == User.UserRole.Patient)
            {
                role = "patient";
                cursorX = 92;
                cursorY = 5;
                Menu.NewTitle("Patient Details");

            }
            else
            {
                role = "doctor";
                cursorX = 91;
                cursorY = 5;
                Menu.NewTitle("Doctor Details");
            }
            Console.WriteLine($"Enter the ID of the {role} who's details you are checking. Or press 'n' to return to menu: ");
            Console.SetCursorPosition(cursorX, cursorY);
            string id = Console.ReadLine();

            //if user presses 'n', returns to menu
            if (id.ToLower() == "n")
            {
                Console.WriteLine("Returning to the menu...");
                return;
            }

            User lookupUser = GetUser(currentUser, id);

            if (userRole == User.UserRole.Patient && (currentUser.Role is not UserRole.Patient))
            {

                //check if the lookup user is a Patient before ToString
                if (lookupUser is Patient patient)
                {
                    Console.WriteLine($"\nDetails for {patient.FirstName} {patient.LastName}");
                    Menu.PatientFormat();
                    Console.WriteLine(patient.ToString());
                }
                else
                {
                    Console.WriteLine("Error. Invalid ID lookup.");
                }
            }
            else if (currentUser.Role is UserRole.Administrator || currentUser.Role is UserRole.Receptionist)
            {

                //checks if the lookup user is a Doctor before ToString
                if (lookupUser is Doctor doctor)
                {
                    Console.WriteLine($"\nDetails for {doctor.FirstName} {doctor.LastName}");
                    Menu.DoctorFormat();
                    Console.WriteLine(doctor.ToString());
                }
                else
                {
                    Console.WriteLine("Error. Invalid ID lookup.");
                }
            } 

            Console.WriteLine("\nPress any key to return to menu.");
        }


    }
}


