using System;
using System.Collections.Generic;
using StudentRegistry;
using Courses;

public static class SampleInitialiser
{
    private static readonly Random rng = new Random();

    private static readonly string[] firstNames = new[] { "Alex", "Sam", "Jamie", "Taylor", "Jordan", "Morgan", "Casey", "Riley", "Chris", "Pat" };
    private static readonly string[] lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson" };
    private static readonly string[] courseNames = new[] { "Computer Science", "Mathematics", "Physics", "Chemistry", "Biology", "History", "English", "Economics" };

    public static SampleData Seed(StudentRegister registry, int courseCount = 6, int studentsPerCourse = 20)
    {
        if (registry == null) throw new ArgumentNullException(nameof(registry));
        // generate some courses and keep track of created instances
        var courseInstances = new List<Course>();
        var modulesByCourse = new Dictionary<string, List<string>>();
        for (int i = 0; i < courseCount; i++)
        {
            string code = GenerateCourseCode(i);
            string name = courseNames[rng.Next(courseNames.Length)] + " " + (rng.Next(1, 4));
            int credits = rng.Next(10, 31);
            // modules
            int moduleCount = rng.Next(3, 7);
            var modules = new List<string>();
            for (int m = 0; m < moduleCount; m++) modules.Add($"{code}-MOD{m + 1}");
            Course.RegisterCourse(code, name, credits, modules);
            courseInstances.Add(new Course(code, credits));
            modulesByCourse[code] = modules;
        }

        // generate students
        int idStart = 1000;
        var registeredCourses = new List<string>();
        for (int i = 0; i < courseCount; i++) registeredCourses.Add(GenerateCourseCode(i));

        for (int c = 0; c < registeredCourses.Count; c++)
        {
            string courseCode = registeredCourses[c];
            for (int s = 0; s < studentsPerCourse; s++)
            {
                int id = idStart++;
                string first = firstNames[rng.Next(firstNames.Length)];
                string last = lastNames[rng.Next(lastNames.Length)];
                var student = new Student(id, first, last, courseCode);
                // add some grades across modules
                var modules = Course.GetModules(courseCode);
                foreach (var mod in modules)
                {
                    // each module gets 1-3 grades
                    int gradesCount = rng.Next(1, 4);
                    for (int g = 0; g < gradesCount; g++)
                    {
                        double grade = Math.Round(rng.NextDouble() * 60 + 40, 2); // range ~40-100
                        student.AddGrade(mod, grade);
                    }
                }
                registry.AddStudent(student);
            }
        }

        return new SampleData(registry, courseInstances, modulesByCourse);
    }

    private static string GenerateCourseCode(int idx)
    {
        return $"C{100 + idx}";
    }
}

public class SampleData
{
    public StudentRegister Registry { get; }
    public IReadOnlyList<Course> Courses { get; }
    public IReadOnlyDictionary<string, List<string>> ModulesByCourse { get; }

    public SampleData(StudentRegister registry, IEnumerable<Course> courses, IDictionary<string, List<string>> modulesByCourse)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Courses = new List<Course>(courses ?? Array.Empty<Course>());
        ModulesByCourse = new Dictionary<string, List<string>>(modulesByCourse ?? new Dictionary<string, List<string>>());
    }
}
