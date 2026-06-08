using System;
using StudentRegistry;

namespace Teachers
{
	public class Teacher
	{

		private readonly string firstName;
		private readonly string lastName;
		private readonly string courseCode;
		private HashSet<Student> students = new HashSet<Student>();
		public Teacher(string firstName, string lastName, string courseCode)
		{
            this.firstName = firstName;
            this.lastName = lastName;
            this.courseCode = courseCode;
        }

		public string FirstName => firstName;
		public string LastName => lastName;
		public string CourseCode => courseCode;

		public void AddStudent(Student student)
		{
			students.Add(student);
        }

		public void RemoveStudent(Student student)
		{
			students.Remove(student);
        }
    }

	public class StaffRoom
	{
        private HashSet<Teacher> staff = new HashSet<Teacher>();
        public StaffRoom() { }

		public void AddTeacher(Teacher t)
		{
			staff.Add(t);
		}
	}
}
