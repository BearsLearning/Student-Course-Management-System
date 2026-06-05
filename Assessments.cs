using System;
using StudentRegistry;

namespace Assessments
{
    public class Assessment
    {
        private Queue<Student> awaitingAssessment = new Queue<Student>();
        private Dictionary<int, double> assessmentResults = new Dictionary<int, double>();
        public void AddStudentForAssessment(Student student)
        {
            awaitingAssessment.Enqueue(student);
        }
        public void StudentsAwaitingAssessment()
        {
            Console.WriteLine("Students awaiting assessment:");
            foreach (var student in awaitingAssessment)
            {
                Console.WriteLine($"{student.FirstName} {student.LastName} (ID: {student.StudentID})");
            }
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public void ConductAssessment()
        {
            while (awaitingAssessment.Count > 0)
            {
                Student student = awaitingAssessment.Dequeue();
                Console.WriteLine($"Assessing {student.FirstName} {student.LastName} (ID: {student.StudentID})");
                Console.Write("Enter assessment score: ");
                if (double.TryParse(Console.ReadLine(), out double score))
                {
                    assessmentResults[student.StudentID] = score;
                    Console.WriteLine($"Recorded score of {score} for {student.FirstName} {student.LastName}");
                }
                else
                {
                    Console.WriteLine("Invalid score. Assessment skipped.");
                }
            }
        }
        public void DisplayResults()
        {
            Console.WriteLine("Assessment Results:");
            foreach (var result in assessmentResults)
            {
                Console.WriteLine($"Student ID: {result.Key}, Score: {result.Value}");
            }
        }
    }
}
