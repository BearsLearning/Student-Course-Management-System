using System;
using System.IO;
using StudentRegistry;
using GradeBook;
using NUnit.Framework;

namespace Student_Course_Management_Testing
{
    [TestFixture]
    public class GradebookTesting
    {
        [Test]
        public void AddStudent_ViewAllStudents_ContainsStudent()
        {
            var registry = new StudentRegister();
            // Add student directly to the registry to avoid driving the interactive AddStudent method
            var student = new Student(101, "John", "Doe", "CS50");
            registry.AddStudent(student);
            var gb = new Gradebook(registry);

            var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(sw);
                // Invoke the private ViewAllStudents to produce output
                var viewMethod = typeof(Gradebook).GetMethod("ViewAllStudents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(viewMethod, Is.Not.Null, "ViewAllStudents method not found");
                viewMethod.Invoke(gb, null);
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            string output = sw.ToString();
            Assert.That(output, Does.Contain("Students:"));
            Assert.That(output, Does.Contain("Doe, John - ID: 101 - Course: CS50"));
        }

        [Test]
        public void AddGrade_ViewGradesAndAverages_Workflow()
        {
            var registry = new StudentRegister();
            var student = new Student(202, "Eve", "Adams", "ENG");
            registry.AddStudent(student);
            var gb = new Gradebook(registry);

            // Add grade to student via private AddGradeToStudent method inputs: id, subject, grade
            var input = new StringReader("202\nMath\n78.5\n");
            var originalIn = Console.In;
            var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetIn(input);
                Console.SetOut(sw);
                var addGradeMethod = typeof(Gradebook).GetMethod("AddGradeToStudent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(addGradeMethod, Is.Not.Null, "AddGradeToStudent method not found");
                addGradeMethod.Invoke(gb, null);

                // View grades
                var viewGradesInput = new StringReader("202\n");
                Console.SetIn(viewGradesInput);
                var viewGradesMethod = typeof(Gradebook).GetMethod("ViewStudentGrades", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(viewGradesMethod, Is.Not.Null, "ViewStudentGrades method not found");
                viewGradesMethod.Invoke(gb, null);

                // View overall average
                var viewAvgInput = new StringReader("202\n2\n");
                Console.SetIn(viewAvgInput);
                var viewAvgMethod = typeof(Gradebook).GetMethod("ViewStudentAverages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(viewAvgMethod, Is.Not.Null, "ViewStudentAverages method not found");
                viewAvgMethod.Invoke(gb, null);
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }

            string output = sw.ToString();
            Assert.That(output, Does.Contain("Added grade 78.5 for Eve Adams in Math"));
            Assert.That(output, Does.Contain("Grades for Eve Adams"));
            Assert.That(output, Does.Contain("Overall Average:"));
        }

        [Test]
        public void RemoveByID_StudentRemoved_ActionLogged()
        {
            var registry = new StudentRegister();
            var student = new Student(303, "Frank", "Miller", "BIO");
            registry.AddStudent(student);
            var gb = new Gradebook(registry);

            var input = new StringReader("303\n");
            var originalIn = Console.In;
            var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetIn(input);
                Console.SetOut(sw);
                var removeMethod = typeof(Gradebook).GetMethod("RemoveByID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(removeMethod, Is.Not.Null, "RemoveByID method not found");
                removeMethod.Invoke(gb, null);

                // Display action log to assert removal logged
                var displayLogMethod = typeof(Gradebook).GetMethod("DisplayActionLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(displayLogMethod, Is.Not.Null, "DisplayActionLog method not found");
                displayLogMethod.Invoke(gb, null);
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }

            string output = sw.ToString();
            Assert.That(output, Does.Contain("Removed Frank Miller (ID: 303)"));
            Assert.That(output, Does.Contain("Removed student Frank Miller (ID: 303)"));
        }

        [Test]
        public void QueueStudentForAssessment_And_ConductAssessments_IntegrateWithAssessment()
        {
            var registry = new StudentRegister();
            var student = new Student(404, "Gina", "Chen", "CHEM");
            registry.AddStudent(student);
            var gb = new Gradebook(registry);

            // Queue student for assessment
            var input = new StringReader("404\n");
            var originalIn = Console.In;
            var sw = new StringWriter();
            var originalOut = Console.Out;
            try
            {
                Console.SetIn(input);
                Console.SetOut(sw);
                var queueMethod = typeof(Gradebook).GetMethod("QueueStudentForAssessment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(queueMethod, Is.Not.Null, "QueueStudentForAssessment method not found");
                queueMethod.Invoke(gb, null);

                // Now conduct assessments - need to feed a score
                Console.SetIn(new StringReader("88\n"));
                var conductMethod = typeof(Gradebook).GetMethod("ConductAssessments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(conductMethod, Is.Not.Null, "ConductAssessments method not found");
                conductMethod.Invoke(gb, null);

                // Display action log
                var displayLogMethod = typeof(Gradebook).GetMethod("DisplayActionLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.That(displayLogMethod, Is.Not.Null, "DisplayActionLog method not found");
                displayLogMethod.Invoke(gb, null);
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }

            string output = sw.ToString();
            Assert.That(output, Does.Contain("Queued Gina Chen for assessment."));
            Assert.That(output, Does.Contain("Conducted assessments"));
        }
    }
}
