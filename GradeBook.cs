using System;
using System.Collections.Generic;
using System.Linq;
using Assessments;
using StudentRegistry;
using Courses;

namespace GradeBook
{
    public class Gradebook
    {
        private StudentRegister registry = new StudentRegister();
        private Assessment assessor = new Assessment();
        private Stack<string> actionLog = new Stack<string>();

        public Gradebook()
        {
        }

        public Gradebook(StudentRegister registry)
        {
            this.registry = registry;
        }

        public void Run()
        {
            Console.WriteLine("Gradebook dispatcher started. Type 'q' at any prompt to quit to main menu.");
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Select an action:");
                Console.WriteLine("1) Add student");
                Console.WriteLine("2) View all students");
                Console.WriteLine("3) Search student by ID");
                Console.WriteLine("4) Search student by name");
                Console.WriteLine("5) Remove student by ID");
                Console.WriteLine("6) Add grade to student");
                Console.WriteLine("7) View student grades");
                Console.WriteLine("8) View student averages");
                Console.WriteLine("9) Display all course codes");
                Console.WriteLine("10) Display course modules");
                Console.WriteLine("11) Display students enrolled on a course");
                Console.WriteLine("12) Add student to assessment queue");
                Console.WriteLine("13) Show students awaiting assessment");
                Console.WriteLine("14) Conduct assessments");
                Console.WriteLine("15) Display assessment results");
                Console.WriteLine("16) View performance table");
                Console.WriteLine("17) View action log");
                Console.WriteLine("q) Quit gradebook");

                string choice = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (string.Equals(choice, "q", StringComparison.OrdinalIgnoreCase)) return;

                switch (choice)
                {
                    case "1": AddStudent(); break;
                    case "2": ViewAllStudents(); break;
                    case "3": SearchByID(); break;
                    case "4": SearchByName(); break;
                    case "5": RemoveByID(); break;
                    case "6": AddGradeToStudent(); break;
                    case "7": ViewStudentGrades(); break;
                    case "8": ViewStudentAverages(); break;
                    case "9": DisplayCourseCodes(); break;
                    case "10": DisplayCourseModules(); break;
                    case "11": DisplayStudentsOnCourse(); break;
                    case "12": QueueStudentForAssessment(); break;
                    case "13": assessor.StudentsAwaitingAssessment(); break;
                    case "14": assessor.ConductAssessment(); break;
                    case "15": assessor.DisplayResults(); break;
                    case "16": registry.PerformanceTable(); break;
                    case "17": DisplayActionLog(); break;
                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }
                // After handling the choice, pause and clear the console so the menu is shown fresh
                PauseThenClear();
            }
        }

        private void LogAction(string action)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";
            actionLog.Push(entry);
        }

        private void AddStudent()
        {
            Console.Write("Enter student ID: ");
            if (!ReadInt(out int id)) return;
            if (registry.GetStudentByID(id) != null)
            {
                Console.WriteLine("A student with that ID already exists.");
                return;
            }
            Console.Write("Enter first name: ");
            string first = ReadString(); if (first == null) return;
            Console.Write("Enter last name: ");
            string last = ReadString(); if (last == null) return;
            Console.Write("Enter course code: ");
            string course = ReadString(); if (course == null) return;

            var student = new Student(id, first, last, course);
            registry.AddStudent(student);
            Console.WriteLine($"Added student: {first} {last} (ID: {id})");
            LogAction($"Added student {first} {last} (ID: {id}, Course: {course})");
        }

        private void ViewAllStudents()
        {
            var list = registry.SortStudents();
            if (list.Count == 0)
            {
                Console.WriteLine("No students available.");
                return;
            }
            Console.WriteLine("Students:");
            foreach (var s in list)
            {
                Console.WriteLine($"{s.LastName}, {s.FirstName} - ID: {s.StudentID} - Course: {s.CourseCode}");
            }
            LogAction("Viewed all students");
        }

        private void SearchByID()
        {
            Console.Write("Enter student ID to search: ");
            if (!ReadInt(out int id)) return;
            var s = registry.GetStudentByID(id);
            if (s == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            Console.WriteLine($"Found: {s.FirstName} {s.LastName} (ID: {s.StudentID}) - Course: {s.CourseCode}");
            LogAction($"Searched student by ID {id}");
        }

        private void SearchByName()
        {
            Console.Write("Enter first or last name to search: ");
            string term = ReadString(); if (term == null) return;
            var matches = registry.SortStudents()
                .Where(s => s.FirstName.Equals(term, StringComparison.OrdinalIgnoreCase)
                         || s.LastName.Equals(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count == 0)
            {
                Console.WriteLine("No matching students found.");
                return;
            }
            foreach (var s in matches)
            {
                Console.WriteLine($"{s.FirstName} {s.LastName} (ID: {s.StudentID}) - Course: {s.CourseCode}");
            }
            LogAction($"Searched students by name '{term}' ({matches.Count} results)");
        }

        private void RemoveByID()
        {
            Console.Write("Enter student ID to remove: ");
            if (!ReadInt(out int id)) return;
            var s = registry.GetStudentByID(id);
            if (s == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            registry.RemoveStudent(s);
            Console.WriteLine($"Removed {s.FirstName} {s.LastName} (ID: {s.StudentID})");
            LogAction($"Removed student {s.FirstName} {s.LastName} (ID: {s.StudentID})");
        }

        private void AddGradeToStudent()
        {
            Console.Write("Enter student ID to add grade to: ");
            if (!ReadInt(out int id)) return;
            var s = registry.GetStudentByID(id);
            if (s == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            Console.Write("Enter subject name: ");
            string subject = ReadString(); if (subject == null) return;
            Console.Write("Enter grade (numeric): ");
            if (!ReadDouble(out double grade)) return;
            s.AddGrade(subject, grade);
            Console.WriteLine($"Added grade {grade} for {s.FirstName} {s.LastName} in {subject}");
            LogAction($"Added grade {grade} to {s.FirstName} {s.LastName} (ID: {id}) for {subject}");
        }

        private void ViewStudentGrades()
        {
            Console.Write("Enter student ID to view grades: ");
            if (!ReadInt(out int id)) return;
            var s = registry.GetStudentByID(id);
            if (s == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            s.DisplayGrades();
            LogAction($"Viewed grades for {s.FirstName} {s.LastName} (ID: {id})");
        }

        private void ViewStudentAverages()
        {
            Console.Write("Enter student ID to view averages: ");
            if (!ReadInt(out int id)) return;
            var s = registry.GetStudentByID(id);
            if (s == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            Console.WriteLine("1) Average by subject\n2) Overall average");
            string opt = Console.ReadLine()?.Trim();
            if (opt == "1")
            {
                Console.Write("Enter subject: ");
                string subject = ReadString(); if (subject == null) return;
                s.AvgGradeBySubject(subject);
                LogAction($"Viewed average for {s.FirstName} {s.LastName} (ID: {id}) subject {subject}");
            }
            else
            {
                s.AvgGradeOverall();
                LogAction($"Viewed overall average for {s.FirstName} {s.LastName} (ID: {id})");
            }
        }

        private void DisplayCourseCodes()
        {
            Console.WriteLine("Course codes:");
            registry.DisplayAllUniqueCourseCodes();
            LogAction("Displayed course codes");
        }

        private void DisplayCourseModules()
        {
            Console.WriteLine("Enter course code to view modules: ");
            string courseCode = ReadString();
            if (courseCode == null) return;
            var course = new Course(courseCode);
            course.DisplayModules();
        }

        private void QueueStudentForAssessment()
        {
            Console.Write("Enter student ID to queue for assessment: ");
            if (!ReadInt(out int id)) return;
            var s = registry.GetStudentByID(id);
            if (s == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            assessor.AddStudentForAssessment(s);
            Console.WriteLine($"Queued {s.FirstName} {s.LastName} for assessment.");
            LogAction($"Queued {s.FirstName} {s.LastName} (ID: {id}) for assessment");
        }

        private void ConductAssessments()
        {
            assessor.ConductAssessment();
            LogAction("Conducted assessments");
        }

        private void DisplayActionLog()
        {
            if (actionLog.Count == 0)
            {
                Console.WriteLine("Action log is empty.");
                return;
            }
            Console.WriteLine("Action log (most recent first):");
            foreach (var entry in actionLog)
            {
                Console.WriteLine(entry);
            }
        }

        private void DisplayStudentsOnCourse()
        {
            Console.WriteLine("Enter course code: ");
            string courseCode = ReadString();
            if (courseCode == null) return;
            var course = new Course(courseCode);
            course.DisplayEnrolledStudents(registry);
            return;
        }

        // Helper input readers
        private static bool ReadInt(out int value)
        {
            string input = Console.ReadLine()?.Trim();
            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) { value = 0; return false; }
            if (int.TryParse(input, out value)) return true;
            Console.WriteLine("Invalid integer input.");
            value = 0; return false;
        }

        private static bool ReadDouble(out double value)
        {
            string input = Console.ReadLine()?.Trim();
            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) { value = 0; return false; }
            if (double.TryParse(input, out value)) return true;
            Console.WriteLine("Invalid numeric input.");
            value = 0; return false;
        }

        private static string ReadString()
        {
            string input = Console.ReadLine();
            if (input == null) return null;
            input = input.Trim();
            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) return null;
            return input;
        }

        private void PauseThenClear()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to the menu or type 'q' then Enter to quit.");
            string input = Console.ReadLine()?.Trim();
            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            { 
                Environment.Exit(0);
            }
            Console.Clear();
        }
    }
}
