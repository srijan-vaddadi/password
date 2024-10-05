using CsvHelper;
using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public class User
{
    public string Username { get; set; }
    public string password { get; set; }
}

public class Exercise16
{
    public static void Main()
    {
        string filePath = "accounts.csv";
        string action = AskForAction();
        if (action == "create")
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            string csvLine = $"{username},{HashAlg(password)}";

            AppendToCsv(filePath, csvLine);

            Console.WriteLine("Account saved successfully.");
        }
        else
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            ValidateLogin(filePath, username, password);
        }
    }

    static string AskForAction()
    {
        string action;
        do
        {
            Console.Write("Enter 'create' to create an account or 'login' to login: ");
            action = Console.ReadLine().ToLower();
        } while (action != "create" && action != "login");
        return action;
    }
    static void AppendToCsv(string filePath, string csvLine)
    {
        using (StreamWriter sw = File.AppendText(filePath))
        {
            sw.WriteLine(csvLine);
        }
    }
    static bool ValidateLogin(string filePath, string username, string password)
    {
        foreach (var line in File.ReadLines(filePath))
        {
            string[] credentials = line.Split(',');

            if (credentials[0] == username && credentials[1] == password)
            {
                return true;
            }
        }
        return false;
    }
    static string HashAlg(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

// int ctr = 0; 
// var users = ReadUsersFromCsv(filepath);

// foreach (var user in users)
// {
//     Console.WriteLine($"Username: {user.Username} \tPassword: {user.password}");
// }
// Console.Write("\n\nCheck username and password :\n");

// do
// {
//     Console.Write("Input a username: ");
//     username = Console.ReadLine();

//     Console.Write("Input a password: ");
//     password = Console.ReadLine();

//     if (username != "abcd" || password != "1234")
//     {
//         ctr++;
//     }
//     else
//     {
//         ctr = 1;
//     }

// } while ((username != "abcd" || password != "1234") && (ctr != 3)); 

// if (ctr == 3)
// {
//     Console.Write("\nLogin attempt three or more times. Try later!\n\n");
// }
// else
// {
//     Console.Write("\nThe password entered successfully!\n\n");
// }