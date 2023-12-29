using Newtonsoft.Json;
using pr;

class Program
{
    private const int MinBirthYear = 1800;
    private const int MaxBirthYear = 2023;

    static void Main()
    {
        var inputPath = Path.Combine("D:", "data.json");
        var resultPath = Path.Combine("D:", "result.json");

        var records = ReadFile(inputPath);
        
        if (records == null)
        {
            throw new Exception("File of data is empty");
        }

        var invalidRecords = new List<Person>();

        using (StreamWriter file = new StreamWriter(resultPath, true))
        {
            file.WriteLine("Start processing\n");

            foreach (var record in records)
            {
                DateTime.TryParse(record.DateofBirth, out var dateofBirth);

                if(dateofBirth == DateTime.MinValue)
                {
                    invalidRecords.Add(record);
                    continue;
                }

                if (dateofBirth.Year < MinBirthYear || dateofBirth.Year > MaxBirthYear)
                {
                    invalidRecords.Add(record);
                    continue;
                }
                
                var genderDigit = CalculateGenderDigit(record.Gender, dateofBirth.Year);
                
                string personalCode = GenerateLithuanianPersonalCode(genderDigit, dateofBirth.Year, dateofBirth.Month, dateofBirth.Day);

                file.WriteLine($"Sugeneruotas asmens kodas of {record.FirstName}: {personalCode}");
            }

            file.WriteLine("Invalid records:\n");
            foreach (var invalidRecord in invalidRecords)
            {
                file.WriteLine(JsonConvert.SerializeObject(invalidRecord));
            }

            file.WriteLine("\nEnd processing\n");
        }
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

    static List<Person> ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new Exception("File of data is not exist");
        }

        using (StreamReader r = new(path))
        {
            string json = r.ReadToEnd();
            try
            {
                var records = JsonConvert.DeserializeObject<List<Person>>(json);
                return records;
            }
            catch (Exception)
            {
                throw;
            }                        
        }
    }

    static int CalculateGenderDigit(char gender, int currentYear)
    {
        int genderDigit = 0;

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

        return genderDigit;
    }
}
