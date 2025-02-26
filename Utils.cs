using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem
{
    public static class Utils
    {

        //centers text - used in Menu box variable message
        public static string centerText(string menuMessage)
        {
            int spaces = 35 - 2 - menuMessage.Length; //takes length of box - 2 and subtracts the menu message
            if (spaces < 0) spaces = 0; //prevents negative spaces if text is too long

            int paddingLeft = spaces / 2;
            int paddingRight = spaces - paddingLeft; 

            return new string(' ', paddingLeft) + menuMessage + new string(' ', paddingRight);
        }

        //checks login credentials - including if duplicate IDs are found
        public static bool credentialCheck(string ID, string password)
        {
            List<string[]> data = FileManager.readDB("HospitalDB.txt");
            string inputID = ID;
            string inputPassword = password;

            Dictionary<string, int> idCount = new Dictionary<string, int>(); //stores instances of the ID

            foreach (var row in data)
            {
                if (row.Length > 0)
                {
                    string currentID = row[0];
                    if (idCount.ContainsKey(currentID))
                    {
                        idCount[currentID]++;
                    }
                    else
                    {
                        idCount[currentID] = 1;
                    }
                }
            }

            if (idCount.ContainsKey(inputID) && idCount[inputID] > 1) //if multiple identical IDs are found, error message and close
            {
                Console.WriteLine($"Error! Duplicate IDs found for {inputID}. Exiting program.");
                Environment.Exit(0);
            }

            foreach (var row in data) //if no duplicates, continue to valid credentials
            {
                if (row[0] == inputID && row[1] == inputPassword)
                {
                    Console.WriteLine("Valid credentials");
                    return true;
                }
            }

            Console.WriteLine("Invalid credentials"); //if credentials are invalid, print error.
            Console.ReadKey();
            Menu.NewMenu("Login"); //return to login menu
            return false;
        }

        //masks the input password
        public static string MaskedPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true); //reads key without displaying

                if (keyInfo.Key == ConsoleKey.Backspace) //if the key is a backspace, remove the last character from the password
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b"); //erases the last '*' displayed
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter) //if the key is not 'enter' or 'backspace', add it to the password
                {
                    password += keyInfo.KeyChar;
                    Console.Write('*'); //display '*' for the password character
                }

            } while (keyInfo.Key != ConsoleKey.Enter); //exits the loop on 'enter'

            Console.WriteLine();
            return password;
        }

        //generates IDs
        public static string GenerateID()
        {
            List<string[]> dbList = FileManager.readDB("HospitalDB.txt"); //gets the current database list

            int maxId = 0;

            foreach (var row in dbList)
            {
                if (int.TryParse(row[0], out int id))
                {
                    maxId = Math.Max(maxId, id); //keeps track of the maximum ID (most recent ID)
                }
            }

            maxId++; //increments the maxId for the new unique ID

            return maxId.ToString("D5"); //formats as a 5-digit string with leading zeros (e.g. ID = 00001)
        }

        //extension method to format full names from the first and last names
        public static string FormatUserFullName(this User user)
        {
            return $"{user.FirstName} {user.LastName}".Trim();
        }

        //sends email to confirm booking
        public static void SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                MailMessage email = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                email.From = new MailAddress("hmsdotnet@gmail.com");
                email.To.Add(recipientEmail);
                email.Subject = subject;
                email.Body = body;

                smtpServer.Port = 587; //this is the TLS port for Gmail
                smtpServer.Credentials = new NetworkCredential("hmsdotnet@gmail.com", "hjwo pvro cyon fvcx"); //this is a recently created dummy email with app password
                smtpServer.EnableSsl = true;

                smtpServer.Send(email);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }

        //input validations for when adding a user

        //valid name format (first name and last name)
        public static bool IsValidName(string name)
        {
            var nameFormat = @"^[A-Za-z]+$"; //only alphabetic characters
            return Regex.IsMatch(name, nameFormat);
        }

        //valid email format
        public static bool IsValidEmail(string email)
        {
            var emailFormat = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; //email regex format
            return Regex.IsMatch(email, emailFormat);
        }

        //validation phone number format
        public static bool IsValidPhoneNumber(string phone)
        {
            var phoneFormat = @"^\d{1,12}$"; //set to allow 12 digits
            return Regex.IsMatch(phone, phoneFormat);
        }

        //valid street number format
        public static bool IsValidStreetNo(string streetNumber)
        {
            var streetNoFormat = @"^[1-9]\d*$"; //positive integers only
            return Regex.IsMatch(streetNumber, streetNoFormat);
        }

        //valid street name
        public static bool IsValidStreet(string street)
        {
            var streetPattern = @"^[A-Za-z0-9\s\.]+$"; //alphanumeric, periods and spaces allowed
            return Regex.IsMatch(street, streetPattern);
        }

        //valid city and state
        public static bool IsValidCityOrState(string input)
        {
            var cityOrStatePattern = @"^[A-Za-z\s]+$"; //only alphabetic characters and spaces
            return Regex.IsMatch(input, cityOrStatePattern);
        }


    }
}
