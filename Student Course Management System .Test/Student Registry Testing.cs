using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StudentRegistry;
using Courses;

namespace Student_Course_Management_Testing
{
    public class StudentRegistryTesting
    {
        private TextWriter? originalOut;

        [SetUp]
        public void Setup()
        {
            // Reset private static courseCodes HashSet<string> to isolate tests
            var t = typeof(StudentRegister);
            var field = t.GetField("courseCodes", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, new System.Collections.Generic.HashSet<string>());

            // Save Console.Out so tests can redirect and restore
            originalOut = Console.Out;
        }

        [TearDown]
        public void TearDown()
        {
            if (originalOut != null)
            {
                Console.SetOut(originalOut);
            }
        }

        private string CaptureOutput(Action action)
        {
            var sw = new StringWriter();
            Console.SetOut(sw);
            action();
            Console.Out.Flush();
            return sw.ToString();
        }

        [Test]
        public void SetupAndGetters()
        {
            Student testStudent = new Student(1, "John", "Smith", "C100");
            Assert.That(testStudent.StudentID, Is.EqualTo(1));
            Assert.That(testStudent.FirstName, Is.EqualTo("John"));
            Assert.That(testStudent.LastName, Is.EqualTo("Smith"));
            Assert.That(testStudent.CourseCode, Is.EqualTo("C100"));
            //Course course = new Course("C100");
            //Student newStudent = new Student(2, "Jane", "Doe", course);
            //Assert.That(newStudent.CourseCode, Is.EqualTo("C100"));
        }

        [Test]
        public void AddGradeAndOverallAverage_ComputesPerSubjectAndOverallCorrectly()
        {
            var s = new Student(1, "A", "B", "C1");
            s.AddGrade("Math", 100);
            s.AddGrade("Math", 80);
            s.AddGrade("Eng", 70);
            // Math avg = 90, Eng avg = 70 => overall = (90+70)/2 = 80
            Assert.That(Math.Round(s.OverallAverageValue(), 6), Is.EqualTo(80.0).Within(1e-6));
        }

        [Test]
        public void AddGrade_InvalidRange_Throws()
        {
            var s = new Student(1, "A", "B", "C1");
            Assert.Throws<ArgumentOutOfRangeException>(() => s.AddGrade("X", -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => s.AddGrade("X", 101));
        }

        [Test]
        public void AvgGradeBySubject_PrintsAverage_WhenSubjectExists()
        {
            var s = new Student(1, "A", "B", "C1");
            s.AddGrade("Sci", 85);
            var outText = CaptureOutput(() => s.AvgGradeBySubject("Sci"));
            Assert.That(outText, Does.Contain("Sci Average: 85.00"));
        }

        [Test]
        public void AvgGradeBySubject_PrintsNotFound_WhenMissing()
        {
            var s = new Student(1, "A", "B", "C1");
            var outText = CaptureOutput(() => s.AvgGradeBySubject("Nope"));
            Assert.That(outText.Trim(), Is.EqualTo("Subject not found."));
        }

        [Test]
        public void DisplayGrades_PrintsSubjectsAndValues()
        {
            var s = new Student(1, "Jane", "Doe", "C2");
            s.AddGrade("Hist", 60);
            s.AddGrade("Hist", 80);
            var outText = CaptureOutput(() => s.DisplayGrades());
            Assert.That(outText, Does.Contain("Grades for Jane Doe (ID: 1):"));
            Assert.That(outText, Does.Contain("Hist: 60, 80").Or.Contain("Hist: 60, 80"));
        }

        [Test]
        public void AvgGradeOverall_NoGrades_PrintsNoGradesAvailable()
        {
            var s = new Student(1, "X", "Y", "C3");
            var outText = CaptureOutput(() => s.AvgGradeOverall());
            Assert.That(outText.Trim(), Is.EqualTo("No grades available."));
        }

        [Test]
        public void AvgGradeOverall_WithZeroGrade_PrintsNoGradesAvailable_BehaviourConfirmed()
        {
            // This test documents current behaviour: an overall average of 0 is treated as 'No grades available.'
            var s = new Student(1, "Z", "Y", "C4");
            s.AddGrade("G", 0);
            var outText = CaptureOutput(() => s.AvgGradeOverall());
            Assert.That(outText.Trim(), Is.EqualTo("No grades available."));
        }

        [Test]
        public void StudentRegister_AddRemove_Get_CourseExistsBehavesAsExpected()
        {
            var reg = new StudentRegister();
            var s = new Student(11, "Sam", "Hill", "CS101");
            reg.AddStudent(s);
            Assert.That(reg.GetStudentByID(11), Is.Not.Null);
            Assert.That(reg.CourseExists("CS101"), Is.True);
            reg.RemoveStudentByID(11);
            Assert.That(reg.GetStudentByID(11), Is.Null);
        }

        [Test]
        public void StudentRegister_RemoveStudentWithDuplicateCourseCode_ShouldKeepCourseCode_WhenOtherStudentStillEnrolled()
        {
            // This test asserts the expected behaviour (course code should remain if another student uses it)
            // The current implementation removes the course code unconditionally and this test will fail,
            // exposing that bug.
            var reg = new StudentRegister();
            var s1 = new Student(21, "A", "One", "DUP1");
            var s2 = new Student(22, "B", "Two", "DUP1");
            reg.AddStudent(s1);
            reg.AddStudent(s2);
            reg.RemoveStudentByID(21);
            // Expected: course still exists because s2 is enrolled. If this assertion fails, it surfaces a bug.
            Assert.That(reg.CourseExists("DUP1"), Is.True);
        }

        [Test]
        public void SortStudents_SortsByLastName()
        {
            var reg = new StudentRegister();
            var s1 = new Student(1, "A", "Cee", "X");
            var s2 = new Student(2, "B", "Aye", "Y");
            var s3 = new Student(3, "C", "Bee", "Z");
            reg.AddStudent(s1);
            reg.AddStudent(s2);
            reg.AddStudent(s3);
            var sorted = reg.SortStudents();
            var lastNames = sorted.Select(s => s.LastName).ToArray();
            Assert.That(lastNames, Is.EqualTo(new[] { "Aye", "Bee", "Cee" }));
        }

        [Test]
        public void PerformanceTable_OrdersByHighestAverageFirst()
        {
            var reg = new StudentRegister();
            var s1 = new Student(10, "Low", "One", "P1");
            var s2 = new Student(20, "High", "Two", "P2");
            var s3 = new Student(30, "Mid", "Three", "P3");
            s1.AddGrade("G", 50);
            s2.AddGrade("G", 80);
            s3.AddGrade("G", 70);
            reg.AddStudent(s1);
            reg.AddStudent(s2);
            reg.AddStudent(s3);
            var outText = CaptureOutput(() => reg.PerformanceTable());
            var lines = outText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Find the index of the header line that contains 'ID'
            var headerIndex = Array.FindIndex(lines, l => l.Contains("ID\tName"));
            Assert.That(headerIndex, Is.GreaterThanOrEqualTo(0));
            // The first data line should follow the header. Find the next line after header that looks like data
            var dataLine = lines.Skip(headerIndex + 1).FirstOrDefault(l => l.Trim().Length > 0 && char.IsDigit(l.Trim()[0]));
            Assert.That(dataLine, Is.Not.Null);
            // Expect the first data line to contain the ID for the highest average (20)
            Assert.That(dataLine, Does.Contain("20\t"));
        }

        [Test]
        public void PerformanceTable_NoStudents_PrintsHeaderOnly()
        {
            var reg = new StudentRegister();
            var outText = CaptureOutput(() => reg.PerformanceTable());
            Assert.That(outText, Does.Contain("Student Performance Table:"));
            Assert.That(outText, Does.Contain("ID\tName"));
        }

        [Test]
        public void DisplayAllUniqueCourseCodes_PrintsCodes()
        {
            var reg = new StudentRegister();
            var s1 = new Student(1, "A", "A", "C-A");
            var s2 = new Student(2, "B", "B", "C-B");
            reg.AddStudent(s1);
            reg.AddStudent(s2);
            var outText = CaptureOutput(() => reg.DisplayAllUniqueCourseCodes());
            Assert.That(outText, Does.Contain("C-A"));
            Assert.That(outText, Does.Contain("C-B"));
        }

        [Test]
        public void GetStudentByID_ReturnsNullWhenNotFound()
        {
            var reg = new StudentRegister();
            Assert.That(reg.GetStudentByID(9999), Is.Null);
        }
    }
}
