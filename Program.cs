using System.Text;
using System.Security.Cryptography;

public class UserInteraction
{
    private static IEncryptionAlgorithm? _algorithm;

    public static void Main()
    {
        string filePath = "accounts.csv";
        string action = AskForAction();

        _algorithm = SelectEncryptionAlgorithm();

        if (action == "create")
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            string salt = _algorithm.GenerateRandomString(username, username.Length);

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            string csvLine = $"{username},{_algorithm.HashPassword(password + salt)},{salt}";

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

    static IEncryptionAlgorithm SelectEncryptionAlgorithm()
    {
        Console.WriteLine("Select Encryption Algorithm:");
        Console.WriteLine("1: Basic SHA256 (No Salting)");
        Console.WriteLine("2: SHA256 with Salting");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                return new SHA256BasicAlgorithm();
            case "2":
                return new SHA256WithSaltingAlgorithm();
            default:
                Console.WriteLine("Invalid choice. Defaulting to SHA256 (No Salting).");
                return new SHA256BasicAlgorithm();
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

            if (credentials[0] == username)
            {
                if (_algorithm is SHA256WithSaltingAlgorithm)
                {
                    string salt = credentials[2];
                    if (credentials[1] == _algorithm.HashPassword(password + salt))
                    {
                        return true;
                    }
                }
                else
                {
                    if (credentials[1] == _algorithm.HashPassword(password))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

public interface IEncryptionAlgorithm
{
    string HashPassword(string rawData);
    string GenerateRandomString(string inputText, int length);
}

public class SHA256BasicAlgorithm : IEncryptionAlgorithm
{
    public string HashPassword(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (var byteValue in bytes)
            {
                builder.Append(byteValue.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public string GenerateRandomString(string inputText, int length)
    {
        return string.Empty;
    }
}

public class SHA256WithSaltingAlgorithm : IEncryptionAlgorithm
{
    public string HashPassword(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (var byteValue in bytes)
            {
                builder.Append(byteValue.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public string GenerateRandomString(string inputText, int length)
    {
        Random random = new Random();
        char[] randomChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            int randomIndex = random.Next(inputText.Length);
            randomChars[i] = inputText[randomIndex];
        }
        return new string(randomChars);
    }
}
