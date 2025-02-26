using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static HospitalManagementSystem.User;

namespace HospitalManagementSystem
{
    public class Appointment
    {
        public Doctor Doctor { get; private set; }
        public Patient Patient { get; private set; }
        public string Description { get; private set; }

        public Appointment() //default constructor
        {
            this.Doctor = new Doctor(); 
            this.Patient = new Patient();
            this.Description = "";
        }

        public Appointment(Doctor doctor, Patient patient, string description)
        {
            this.Doctor = doctor; 
            this.Patient = patient;
            this.Description = description;
        }

        public override string ToString()
        {

            string doctorFullName = $"{Doctor.FirstName} {Doctor.LastName}"; 
            string patientFullName = $"{Patient.FirstName} {Patient.LastName}";
            string description = $"{Description.Replace(";", ",")}"; //changes semicolon back to comma to print (to avoid confusion with delimiter)

            //ensures the same format is applied as the header
            return string.Format(
                "{0, -18} | {1, -18} | {2, -30}",
                doctorFullName, patientFullName, description
            );
        }

        public static void BookAppointment(Patient currentUser)
        {
            Doctor assignedDoctor = new Doctor();

            //if patient has a Doctor assigned, 'gets' the doctor corresponding to the patients DoctorID
            if (!string.IsNullOrEmpty(currentUser.DoctorID))
            {
                assignedDoctor = (Doctor)User.GetUser(currentUser, currentUser.DoctorID);
                Console.WriteLine("You are booking a new appointment with {0} {1}", assignedDoctor.FirstName, assignedDoctor.LastName);

            }
            else //outputs a list of doctors to assign
            {
                bool validInput = false;
                while (!validInput)
                {
                    Console.WriteLine("Please choose a doctor: ");
                    try
                    {
                        Menu.NewTitle("Book Appointment");

                        Console.WriteLine("You are not registered with any doctor! Please choose which doctor you would like to register with: ");
                        List<string[]> data = FileManager.readDB("HospitalDB.txt"); //reads HospitalDB.txt and assigns to string array data
                        int count = 1;
                        List<string> doctors = new List<string>(); //creates a list of string to store doctors

                        foreach (var row in data)
                        {
                            if (row[2] == "Doctor")
                            {
                                doctors.Add(row[0]); //adds the doctor's id to the string list
                                Console.WriteLine("{0}. {1} {2} | {3} | {4} | {5} {6}, {7}, {8}", count, row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10]);
                                count++;
                            }

                        }

                        int choice = int.Parse(Console.ReadLine()); //parse input to make a choice
                        if (choice < 1 || choice > doctors.Count)
                        {
                            throw new ArgumentOutOfRangeException("Invalid choice number");
                        }

                        string selectedDoctor = doctors[choice - 1]; //selected doctor is choice (-1 for index)
                        assignedDoctor = (Doctor)User.GetUser(currentUser, selectedDoctor); //creates new Doctor instance using the selected ID
                        Console.WriteLine("You are booking a new appointment with Dr. {0} {1}", assignedDoctor.FirstName, assignedDoctor.LastName);

                        currentUser.AssignDoctor(assignedDoctor.ID); //assigns doctor ID to patient
                        validInput = true;
                    }
                    catch (Exception ex) //if input is invalid, clears screen and tries again
                    {
                        Console.WriteLine("Invalid selection. Please try again.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }

            }

            Console.WriteLine("Description of the appointment: ");
            (int left, int top) = Console.GetCursorPosition();
            Console.SetCursorPosition(32, top -1);
            string description = Console.ReadLine();
            description = description.Replace(",", ";"); //replaces commas to semicolons to not be confused as delimiters

            //writes information to row in AppointmentDB.txt
            FileManager.WriteToDB(assignedDoctor.ID, assignedDoctor.FirstName, assignedDoctor.LastName, currentUser.ID, currentUser.FirstName, currentUser.LastName, description);

            //updates patient with new doctor
            FileManager.UpdateDoctor(currentUser, assignedDoctor.ID);

            //updates doctor with patient id
            FileManager.UpdatePatients(currentUser, assignedDoctor.ID);

            Console.WriteLine("The appointment has been booked successfully");
            Console.WriteLine("Please wait..."); //asks user to wait while confirmation email processes - takes roughly 2 seconds

            //sends confirmation email
            Utils.SendEmail(currentUser.Email, "Appointment Confirmation", $"Dear {currentUser.FirstName},\n\nYour appointment with Dr. {assignedDoctor.FirstName} {assignedDoctor.LastName} has been booked successfully.\n\nDescription: {description}");

            Console.WriteLine("A confirmation email has been sent.");
            Console.WriteLine("\nPress any key to return to menu.");

        }

        //books an appointment on behalf of the patient
        public static void BookAppointment(Receptionist currentUser)
        {
            Menu.NewTitle("Book Appointment");
            
            //asks for patient ID input which is used as an argument for BookAppointment
            Console.WriteLine($"Enter the patient ID to book an appointment on their behalf. Or press 'n' to return to menu: ");
            Console.SetCursorPosition(93, 5);
            string id = Console.ReadLine();

            //if 'n' is pressed, returns user to menu
            if (id.ToLower() == "n")
            {
                Console.WriteLine("Returning to the menu...");
                return;
            }

            Patient newPatient = (Patient)GetUser(currentUser, id);

            //books appointment on behalf of patient
            BookAppointment(newPatient);
        }

        //lists appointments with currentUser as argument - e.g. ListAppointments(doctor);
        public static void ListAppointments(User currentUser)
        {
            try
            {
                List<string[]> data = FileManager.readDB("AppointmentDB.txt");
                bool appointmentsFound = false; //flag to check if any appointments are found

                foreach (var row in data)
                {
                    if (row[0] == currentUser.ID)
                    {
                        if (!appointmentsFound)
                        {
                            Menu.NewTitle("All Appointments");
                            Menu.AppointmentFormat(); //header format for appointments
                        }

                        appointmentsFound = true;
                    }
                    else if (row[3] == currentUser.ID)
                    {
                        if (!appointmentsFound)
                        {
                            Menu.NewTitle("My Appointments");
                            Console.WriteLine("Appointments for {0} {1}\n", currentUser.FirstName, currentUser.LastName);
                            Menu.AppointmentFormat();
                        }

                        appointmentsFound = true;
                    }
                }

                if (!appointmentsFound)
                {
                    Console.WriteLine("No appointments found.");
                }
                else
                {
                    foreach (var row in data)
                    {
                        if (row[0] == currentUser.ID) //if ID matches doctor row
                        {
                            Appointment appointment = new Appointment((Doctor)currentUser, (Patient)GetUser(currentUser, row[3]), row[6]);
                            Console.WriteLine(appointment.ToString());
                        }
                        else if (row[3] == currentUser.ID) //if ID matches patient row
                        {
                            Appointment appointment = new Appointment((Doctor)GetUser(currentUser, row[0]), (Patient)currentUser, row[6]);
                            Console.WriteLine(appointment.ToString());
                        }
                    }
                }

                Console.WriteLine("\nPress any key to return to menu.");
            }
            catch (Exception ex) //if AppointmentsDB.txt is empty
            {
                Console.WriteLine("No appointments found. This.");
                Console.WriteLine("\nPress any key to return to menu.");
            }
        }
        
        //lists appointments of patient from input ID
        public static void PatientAppointments(User currentUser)
        {
            List<string[]> data = FileManager.readDB("AppointmentDB.txt"); // read the appointment db.txt
            Menu.NewTitle("Appointments With");

            Console.WriteLine($"Enter the ID of the patient you would like to view appointments for: ");
            Console.SetCursorPosition(69, 5);
            string ID = Console.ReadLine();

            Menu.AppointmentFormat(); //header format
            
            bool appointmentFound = false; //bool flag if appointments are found

            foreach (var row in data) // loop through data
            {
                // If the ID matches the patient and the doctor ID matches the current user
                if (row[3] == ID && row[0] == currentUser.ID)
                {
                    Appointment appointment = new Appointment((Doctor)currentUser, (Patient)GetUser(currentUser, row[3]), row[6]);
                    Console.WriteLine(appointment.ToString());
                    appointmentFound = true; // marks that appointments were found
                }
            }

            if (!appointmentFound) //if no appointments are found, print message
            {
                Console.WriteLine("No appointments to show.");
            }

            Console.WriteLine("\nPress any key to return to the menu.");
        }

    }
}
