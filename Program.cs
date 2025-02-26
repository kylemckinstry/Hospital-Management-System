using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace HospitalManagementSystem
{
    internal class Program
    {    
        static void Main(string[] args)
        {
            Menu.MenuBox("Login");
            Menu.MainMenu();
        }
    }
}