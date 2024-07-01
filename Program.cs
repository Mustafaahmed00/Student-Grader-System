using System;
using System.Collections.Generic;
using System.IO;

namespace StudentGradingSystem
{
    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, double> Grades { get; set; }

        public Student()
        {
            Grades = new Dictionary<string, double>();
        }

        public double CalculateGPA()
        {
            double total = 0;
            foreach (var grade in Grades.Values)
            {
                total += grade;
            }
            return Grades.Count > 0 ? total / Grades.Count : 0;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, GPA: {CalculateGPA():F2}";
        }

        public string GetReportCard()
        {
            string report = $"Report Card for {Name} (ID: {Id})\n";
            report += "----------------------------------\n";
            foreach (var grade in Grades)
            {
                report += $"{grade.Key}: {grade.Value}\n";
            }
            report += $"GPA: {CalculateGPA():F2}\n";
            return report;
        }
    }

    class StudentManager
    {
        private List<Student> students;
        private string filePath = "students.txt";

        public StudentManager()
        {
            students = new List<Student>();
            LoadStudents();
        }

        public void AddStudent(string name)
        {
            int id = students.Count > 0 ? students[^1].Id + 1 : 1;
            students.Add(new Student { Id = id, Name = name });
            SaveStudents();
        }

        public void UpdateStudent(int id, string name)
        {
            var student = students.Find(s => s.Id == id);
            if (student != null)
            {
                student.Name = name;
                SaveStudents();
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        public void DeleteStudent(int id)
        {
            students.RemoveAll(s => s.Id == id);
            SaveStudents();
        }

        public void AddGrade(int id, string subject, double grade)
        {
            var student = students.Find(s => s.Id == id);
            if (student != null)
            {
                student.Grades[subject] = grade;
                SaveStudents();
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        public void DisplayStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("No students available.");
                return;
            }

            foreach (var student in students)
            {
                Console.WriteLine(student);
            }
        }

        public void GenerateReportCard(int id)
        {
            var student = students.Find(s => s.Id == id);
            if (student != null)
            {
                Console.WriteLine(student.GetReportCard());
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        private void SaveStudents()
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.Id}|{student.Name}|{string.Join(",", student.Grades)}");
                }
            }
        }

        private void LoadStudents()
        {
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 2)  // Ensure there are enough parts
                        {
                            var student = new Student
                            {
                                Id = int.Parse(parts[0]),
                                Name = parts[1]
                            };

                            if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2]))
                            {
                                var grades = parts[2].Split(',');
                                foreach (var grade in grades)
                                {
                                    var gradeParts = grade.Split(':');
                                    if (gradeParts.Length == 2)  // Ensure the gradeParts are in correct format
                                    {
                                        if (double.TryParse(gradeParts[1], out double gradeValue))
                                        {
                                            student.Grades[gradeParts[0]] = gradeValue;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Invalid grade value for {gradeParts[0]}");
                                        }
                                    }
                                }
                            }
                            students.Add(student);
                        }
                        else
                        {
                            Console.WriteLine($"Invalid student record: {line}");
                        }
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StudentManager manager = new StudentManager();
            while (true)
            {
                Console.WriteLine("Student Grading System");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Update Student");
                Console.WriteLine("3. Delete Student");
                Console.WriteLine("4. Add Grade");
                Console.WriteLine("5. Display Students");
                Console.WriteLine("6. Generate Report Card");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter Name: ");
                        string name = Console.ReadLine();
                        manager.AddStudent(name);
                        break;
                    case 2:
                        Console.Write("Enter Student ID to update: ");
                        int updateId = int.Parse(Console.ReadLine());
                        Console.Write("Enter New Name: ");
                        string newName = Console.ReadLine();
                        manager.UpdateStudent(updateId, newName);
                        break;
                    case 3:
                        Console.Write("Enter Student ID to delete: ");
                        int deleteId = int.Parse(Console.ReadLine());
                        manager.DeleteStudent(deleteId);
                        break;
                    case 4:
                        Console.Write("Enter Student ID to add grade: ");
                        int gradeId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Subject: ");
                        string subject = Console.ReadLine();
                        Console.Write("Enter Grade: ");
                        double grade = double.Parse(Console.ReadLine());
                        manager.AddGrade(gradeId, subject, grade);
                        break;
                    case 5:
                        manager.DisplayStudents();
                        break;
                    case 6:
                        Console.Write("Enter Student ID to generate report card: ");
                        int reportId = int.Parse(Console.ReadLine());
                        manager.GenerateReportCard(reportId);
                        break;
                    case 7:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}
