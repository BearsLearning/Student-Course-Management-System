using System;
using System.Collections.Generic;
using System.Linq;
using Courses;

namespace StudentRegistry
{
    public class Student
    {
        private readonly int studentID;
        private readonly string firstName;
        private readonly string lastName;
        private readonly string courseCode;
        private Dictionary<string, double[]> grades = new Dictionary<string, double[]>();

        // Initialize private member variables via constructor
        public Student(int studentID, string firstName, string lastName, string courseCode)
        {
            this.studentID = studentID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.courseCode = courseCode;
        }

        public Student(int studentID, string firstName, string lastName, Course course)
        {
            this.studentID = studentID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.courseCode = course.CourseCode;
        }

        public int StudentID => studentID;
        public string FirstName => firstName;
        public string LastName => lastName;
        public string CourseCode => courseCode;

        public void AddGrade(string subject, double grade)
        {
            if (grade < 0 || grade > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be between 0 and 100.");
            }
            if (grades.ContainsKey(subject))
            {
                grades[subject] = grades[subject].Append(grade).ToArray();
            }
            else
            {
                grades[subject] = new double[] { grade };
            }
        }

        // Need to add Removal and amendment of grades.

        public void DisplayGrades()
        {
            Console.WriteLine($"Grades for {firstName} {lastName} (ID: {studentID}):");
            foreach (var subject in grades.Keys)
            {
                Console.WriteLine($"{subject}: {string.Join(", ", grades[subject])}");
            }
        }

        public void AvgGradeBySubject(string subject)
        {
            if (grades.ContainsKey(subject))
            {
                double average = grades[subject].Average();
                Console.WriteLine($"{subject} Average: {average:F2}");
            }
            else
            {
                Console.WriteLine("Subject not found.");
            }
        }

        public double OverallAverageValue()
        {
            if (grades.Count == 0) return 0.0;
            double overallAverage = 0;
            int cnt = 0;
            foreach (var subject in grades)
            {
                if (subject.Value.Length > 0)
                {
                    overallAverage += subject.Value.Average();
                    cnt++;
                }
            }
            if (cnt == 0) return 0.0;
            return overallAverage / cnt;
        }

        // Print and return the overall average
        public void AvgGradeOverall()
        {
            double avg = OverallAverageValue();
            if (avg > 0)
            {
                Console.WriteLine($"Overall Average: {avg:F2}");
            }
            else
            {
                Console.WriteLine("No grades available.");
            }

        }
    }

    public class StudentRegister
    {
        private static HashSet<string> courseCodes = new HashSet<string>();
        private List<Student> students = new List<Student>();
        public void AddStudent(Student student)
        {
            students.Add(student);
            courseCodes.Add(student.CourseCode);
        }
        public void RemoveStudent(Student student)
        {
            students.Remove(student);
            courseCodes.Remove(student.CourseCode);
        }

        public void RemoveStudentByID(int studentID)
        {
            Student stud = GetStudentByID(studentID);
            if (stud != null)
            {
                RemoveStudent(stud);
            }
        }
        public Student GetStudentByID(int studentID)
        {
            return students.FirstOrDefault(s => s.StudentID == studentID);
        }

        public List<Student> SortStudents()
        {
            return students.OrderBy(s => s.LastName).ToList();
        }

        public void DisplayAllUniqueCourseCodes()
        {
            foreach (var code in courseCodes)
            {
                Console.WriteLine(code);
            }
        }

        public bool CourseExists(string courseCode) { return courseCodes.Contains(courseCode); }

        public void PerformanceTable()
        {
            Console.WriteLine("Student Performance Table:");
            Console.WriteLine("ID\tName\t\tCourse\t\tAverage Grade");
            // PriorityQueue is a min-heap on the priority value. To get highest grades first,
            // enqueue using negative average as the priority.
            PriorityQueue<Student, double> pq = new PriorityQueue<Student, double>();
            foreach (var student in students)
            {
                double avg = student.OverallAverageValue();
                // use -avg so larger averages have higher priority (dequeued first)
                pq.Enqueue(student, -avg);
            }
            // Dequeue in priority order and display
            while (pq.TryDequeue(out var stud, out var negPriority))
            {
                double avg = stud.OverallAverageValue();
                Console.WriteLine("{0}\t{1} {2}\t{3}\t{4:F2}", stud.StudentID, stud.FirstName, stud.LastName, stud.CourseCode, avg);
            }
        }
    }
}
