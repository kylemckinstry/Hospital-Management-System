using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;
using System.Numerics;

namespace HospitalManagementSystem
{
    public class FileManager
    {
        //reads the .txt files - HospitalDB.txt for user data, AppointmentDB.txt for appointment data.
        public static List<string[]> readDB(string file)
        {
            string database = file;

            //saves the file data in a list of string arrays
            List<string[]> dbList = new List<string[]>();

            try
            {
                //using 'using' to automatically dispose of the StreamReader
                using (StreamReader reader = new StreamReader(database))
                {
                    string data = reader.ReadLine();

                    while (data != null)
                    {
                        string[] dbArray = data.Split(',');
                        dbList.Add(dbArray);
                        data = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<string[]>();
            }

            GC.Collect(); //explicit garbage collection
            GC.WaitForPendingFinalizers();

            return dbList;
        }

        //writes user record to HospitalDB.txt file
        public static void WriteToDB(string userType, string firstName, string lastName, string email, string phone, string streetNumber, string street, string city, string state, string doctor, List<string> patientIDs)
        {
            try
            {
                string ID = Utils.GenerateID();
                string password = "password";

                //combines all the elements in patientIDs into a single string separated by semicolons
                string patients = patientIDs != null ? string.Join(";", patientIDs) : "";

                //ensures that null values do not appear in the final string
                string record = $"{ID},{password},{userType},{firstName ?? ""},{lastName ?? ""},{email ?? ""},{phone ?? ""},{streetNumber ?? ""},{street ?? ""},{city ?? ""},{state ?? ""},{doctor ?? ""},{patients ?? ""}";

                //uses StreamWriter to write to the file in append mode (true)
                using (StreamWriter writer = new StreamWriter("HospitalDB.txt", true))
                {
                    writer.WriteLine(record);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to database: " + e.Message);
            }
        }

        //writes appointment record to AppointmentDB.txt file
        public static void WriteToDB(string doctorID, string doctorFirstName, string doctorLastName, string patientID, string patientFirstName, string patientLastName, string description)
        {
            try
            {
                string record = $"{doctorID},{doctorFirstName},{doctorLastName},{patientID},{patientFirstName},{patientLastName},{description}";

                //uses StreamWriter to write to the file in append mode (true)
                using (StreamWriter writer = new StreamWriter("AppointmentDB.txt", true))
                {
                    writer.WriteLine(record);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to database: " + e.Message);
            }
        }

        //updates database file
        public static void UpdateDB(string fileName, List<string[]> data)
        {
            try
            {
                //uses StreamWriter to write back to the file in overwrite mode (false)
                using (StreamWriter writer = new StreamWriter(fileName, false))
                {
                    foreach (var row in data)
                    {
                        //joins the array elements into a comma separated string and write it as a line
                        writer.WriteLine(string.Join(",", row));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to database: " + e.Message);
            }
        }

        //updates doctor in patient record
        public static void UpdateDoctor(Patient currentUser, string doctorID)
        {
            try
            {
                List<string[]> data = FileManager.readDB("HospitalDB.txt");

                foreach (var row in data)
                {
                    if (row[0] == currentUser.ID)
                    {
                        //updates the doctor ID field (row[11]) with a doctorID
                        row[11] = doctorID;
                        break;
                    }
                }

                //writes the updated data back to the file
                FileManager.UpdateDB("HospitalDB.txt", data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error updating doctor in patient record: " + e.Message);
            }
        }

        //updates patients field in a doctor record
        public static void UpdatePatients(Patient currentUser, string doctorID)
        {
            try
            {
                List<string[]> data = FileManager.readDB("HospitalDB.txt");

                foreach (var row in data)
                {
                    if (row[0] == doctorID)
                    {
                        //if row[12] is not empty, splits the existing patient IDs by semicolon
                        if (!string.IsNullOrEmpty(row[12]))
                        {
                            //splits the existing IDs into an array
                            string[] existingPatientIDs = row[12].Split(';');

                            //checks if the current patient's ID is already in the list
                            if (!existingPatientIDs.Contains(currentUser.ID))
                            {
                                //appends the current patient's ID if it does not already exist
                                row[12] += ";" + currentUser.ID;
                            }
                        }
                        else
                        {
                            //if row[12] is empty, assigns the current patient's ID
                            row[12] = currentUser.ID;
                        }

                        break;
                    }
                }

                //writes the updated data back to the file
                FileManager.UpdateDB("HospitalDB.txt", data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error appending to field in patient record: " + e.Message);
            }
        }

        //gets patients from patient field in doctor record, split by ';' delimiter
        public static List<string> getPatientList(Doctor currentUser)
        {
            List<string> patientList = new List<string>();

            try
            {
                List<string[]> data = FileManager.readDB("HospitalDB.txt");

                foreach (var row in data)
                {
                    if (row[0] == currentUser.ID)
                    {
                        string patientIDs = row[12];

                        //splits the string by ';' to get individual patient IDs
                        string[] patientArray = patientIDs.Split(';');

                        //adds each patient ID to the list
                        foreach (string patientID in patientArray)
                        {
                            //ensures no empty strings or invalid IDs are added
                            if (!string.IsNullOrEmpty(patientID))
                            {
                                patientList.Add(patientID.Trim());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting patient list: " + e.Message);
            }

            return patientList;
        }


    }

}
