using System;

class Program
{
    private const int MinBirthYear = 1800;
    private const int MaxBirthYear = 2023;

    static void Main()
    {
        Console.WriteLine("Įveskite duomenis norint sugeneruoti asmens kodą:");

        char gender;
        int genderDigit = 0;
        int currentYear;

        do
        {
            Console.Write("Lytis (V/M): ");
            gender = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine(); 

            Console.Write("Metai, kai gimėte (keturi skaičiai): ");
            if (!int.TryParse(Console.ReadLine(), out currentYear) || currentYear < MinBirthYear || currentYear > MaxBirthYear)
            {
                Console.WriteLine($"Neteisingi metai. Pakartokite įvedimą (nuo {MinBirthYear} iki {MaxBirthYear}).");
            }

            if (currentYear >= 1800 && currentYear <= 1900)
            {
                genderDigit = (gender == 'V') ? 1 : 2;
            }
            else if (currentYear >= 1901 && currentYear <= 2000)
            {
                genderDigit = (gender == 'V') ? 3 : 4;
            }
            else if (currentYear >= 2001 && currentYear <= 2023)
            {
                genderDigit = (gender == 'V') ? 5 : 6;
            }

            if (genderDigit == 0)
            {
                Console.WriteLine("Neteisingas įvedimas. Pakartokite įvedimą.");
            }

        } while (genderDigit == 0);

        int birthYear = currentYear;

        Console.Write("Gimimo mėnuo (1-12): ");
        int birthMonth = int.Parse(Console.ReadLine());
        while (birthMonth < 1 || birthMonth > 12)
        {
            Console.WriteLine("Gimimo mėnuo turi būti nuo 1 iki 12. Pakartokite įvedimą:");
            birthMonth = int.Parse(Console.ReadLine());
        }

        Console.Write("Gimimo diena: ");
        int birthDay = int.Parse(Console.ReadLine());
        while (birthDay < 1 || birthDay > DaysInMonth(birthYear, birthMonth))
        {
            Console.WriteLine($"Neteisinga diena pasirinktam mėnesiui. Įveskite dar kartą (1-{DaysInMonth(birthYear, birthMonth)}):");
            birthDay = int.Parse(Console.ReadLine());
        }

        string personalCode = GenerateLithuanianPersonalCode(genderDigit, birthYear, birthMonth, birthDay);

        Console.WriteLine($"Sugeneruotas asmens kodas: {personalCode}");
    }

    static string GenerateLithuanianPersonalCode(int genderDigit, int birthYear, int birthMonth, int birthDay)
    {
        int firstDigit = genderDigit;
        int secondAndThirdDigits = birthYear % 100;
        int fourthAndFifthDigits = birthMonth;
        string sixthAndSeventhDigits = $"{birthDay:D2}";

        Random random = new Random();
        int randomDigits = random.Next(1000); 
        string randomDigitsString = $"{randomDigits:D3}"; 

        int tenthDigit = CalculateControlDigit($"{firstDigit}{secondAndThirdDigits:D2}{fourthAndFifthDigits:D2}{sixthAndSeventhDigits}{randomDigitsString}");

        string personalCode = $"{firstDigit}{secondAndThirdDigits:D2}{fourthAndFifthDigits:D2}{sixthAndSeventhDigits}{randomDigitsString}{tenthDigit}";

        return personalCode;
    }

    static int CalculateControlDigit(string partialCode)
    {
        int sum = 0;
        int[] weights = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2 };
        for (int i = 0; i < partialCode.Length; i++)
        {
            sum += int.Parse(partialCode[i].ToString()) * weights[i % weights.Length];
        }
        int remainder = sum % 11;
        return (remainder == 10) ? 0 : remainder;
    }

    static int DaysInMonth(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }
}
