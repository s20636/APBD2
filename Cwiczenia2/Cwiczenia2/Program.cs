

using Newtonsoft.Json.Linq;
using System.Text.Json;
using static Cwiczenia2.Student;

namespace Cwiczenia2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ..\..\..\Data\dane.csv "..\..\..\Data" json   -args
            if (args.Length != 3)
            {
                var errorMessage = "Wymagane są 3 argumenty";
                writeToLog(errorMessage);
                throw new ArgumentException(errorMessage);
            }
             var inputFile = args[0];
            if (!File.Exists(inputFile))
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
                var errorMessage = $"Plik {inputFile} nie istnieje";
                writeToLog(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }
            var outputDirectory = args[1];
            if (!Directory.Exists(outputDirectory))
            {
                var errorMessage = $"Scieżka {outputDirectory} jest niepoprawna";
                writeToLog(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }
            var fileFormat = args[2];
            if (fileFormat.ToLower() != "json")
            {
                var errorMessage = $"{fileFormat} nie jest obsługiwany, obecnie można używać jedynie formatu json";
                writeToLog(errorMessage);
                throw new NotImplementedException(errorMessage);
            }
            HashSet<Student> studentsSet = new HashSet<Student>();
            using StreamReader sr = new StreamReader(inputFile);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] students = line.Split(',');
                if (students.Length != 9)
                {
                    writeToLog($"Student {line} został pominięty - brak 9 kolumn");
                }
                else if (students.Any(x => x.Trim() == ""))
                {
                    writeToLog($"Student [{line}] został pominięty - jedno z pul jest puste");
                }
                else
                {
                    Student student = new Student
                    {
                        FirstName = students[0],
                        LastName = students[1],
                        IndexNumber = students[4],
                        DateOfBirth = students[5],
                        Email = students[6],
                        MotherName = students[7],
                        FatherName = students[8],
                        studies = new Studies
                        {
                            Department = students[2],
                            LearningMode = students[3]
                        }
                    };
                    bool isDuplicate = studentsSet.Any(s => s.Equals(s, student));
                    if (!isDuplicate)
                    {
                        studentsSet.Add(student);
                    }
                    else 
                    {
                        writeToLog($"Student [{line}] został pominięty - duplikat");
                    }
                }
                var activeStudiesDictionary = new Dictionary<string, int>();
                var activeStudiesSet = new HashSet<Studies>();

                foreach (var item in studentsSet)
                {
                    activeStudiesSet.Add(item.studies);
                }

                foreach (var item in activeStudiesSet)
                {
                    if (!activeStudiesDictionary.ContainsKey(item.Department))
                    {
                        activeStudiesDictionary.Add(item.Department, 1);
                    }
                    else
                    {
                        activeStudiesDictionary[item.Department]++;
                    }
                }
                ExportJsonToOutputFile(outputDirectory, fileFormat, studentsSet, activeStudiesDictionary);
            }
            Console.WriteLine("koniec");
        }

        private static void ExportJsonToOutputFile(string outputDirectory, string fileFormat, HashSet<Student> studentsSet, Dictionary<string, int> activeStudiesDictionary)
        {
            var studentsJArray = new JArray();
            foreach (var item in studentsSet)
            {
                var itemJson = JObject.FromObject(item);
                studentsJArray.Add(itemJson);
            }

            var activeStudiesJArray = new JArray();
            foreach (var item in activeStudiesDictionary)
            {
                var itemJson = JObject.FromObject(item);
                activeStudiesJArray.Add(itemJson);
            }

            var jProperty = new JProperty("property", studentsJArray);
            var jObject = new JObject(
                new JProperty("uczelnia", new JObject(
                    new JProperty("createdAt", DateTime.Today.ToString("dd.MM.yyyy")),
                    new JProperty("author", "Krzysztof Kozłowski"),
                    new JProperty("studenci", studentsJArray),
                    new JProperty("activeStudies", activeStudiesJArray)
                ))
            );
            File.WriteAllText($"{outputDirectory}/result.{fileFormat.ToLower()}", jObject.ToString());
        }

        public static void writeToLog(string text)
        {
            using (StreamWriter sw = new StreamWriter("..\\..\\..\\Data\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now + ": " + text);
            }
        }
    }
}