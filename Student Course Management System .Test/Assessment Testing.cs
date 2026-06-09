using System;
using System.IO;
using Assessments;
using StudentRegistry;
using NUnit.Framework;

namespace Student_Course_Management_Testing
{
	[TestFixture]
	public class AssessmentsTesting
	{
		[Test]
		public void AddStudentForAssessment_StudentsAwaitingAssessment_PrintsQueuedStudents()
		{
			var assessor = new Assessment();
			var s1 = new Student(1, "Alice", "Smith", "CS101");
			var s2 = new Student(2, "Bob", "Jones", "CS101");

			assessor.AddStudentForAssessment(s1);
			assessor.AddStudentForAssessment(s2);

			var sw = new StringWriter();
			var originalOut = Console.Out;
			try
			{
				Console.SetOut(sw);
				assessor.StudentsAwaitingAssessment();
			}
			finally
			{
				Console.SetOut(originalOut);
			}

			string output = sw.ToString();
			Assert.That(output, Does.Contain("Students awaiting assessment"));
			Assert.That(output, Does.Contain("Alice Smith (ID: 1)"));
			Assert.That(output, Does.Contain("Bob Jones (ID: 2)"));
		}

		[Test]
		public void ConductAssessment_RecordsScores_DisplayResultsContainScores()
		{
			var assessor = new Assessment();
			var s1 = new Student(10, "Charlie", "Day", "MATH");
			var s2 = new Student(11, "Dana", "Lee", "MATH");
			assessor.AddStudentForAssessment(s1);
			assessor.AddStudentForAssessment(s2);

			// Provide two scores via Console.In
			var input = new StringReader("85.5\n92\n");
			var originalIn = Console.In;
			var sw = new StringWriter();
			var originalOut = Console.Out;
			try
			{
				Console.SetIn(input);
				Console.SetOut(sw);
				assessor.ConductAssessment();
				assessor.DisplayResults();
			}
			finally
			{
				Console.SetIn(originalIn);
				Console.SetOut(originalOut);
			}

			string output = sw.ToString();
			// Check that score recording messages were printed and final results contain the IDs and scores
			Assert.That(output, Does.Contain("Recorded score of 85.5"));
			Assert.That(output, Does.Contain("Recorded score of 92"));
			Assert.That(output, Does.Contain("Student ID: 10, Score: 85.5"));
			Assert.That(output, Does.Contain("Student ID: 11, Score: 92"));
		}
	}
}
