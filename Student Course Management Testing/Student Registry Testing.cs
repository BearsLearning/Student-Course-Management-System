using StudentRegistry;
namespace Student_Course_Management_Testing
{
    public class StudentRegistryTesting
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SetupAndGetters()
        {
            Student testStudent = new Student(1, "John", "Smith", "C100");
            var result = testStudent.StudentID;
            Assert.That(result, Is.EqualTo(1));
        }
    }
}
