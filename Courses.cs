using System;
using System.Collections.Generic;

namespace Courses
{
    public class Course
    {
        private readonly string courseCode;
        private static readonly Dictionary<string, string> courseNames = new Dictionary<string, string>();
        private static readonly Dictionary<string, int> courseCredits = new Dictionary<string, int>();
        private static readonly Dictionary<string, List<string>> courseModules = new Dictionary<string, List<string>>();
        private readonly int credits;
        public Course(string courseCode, int credits)
        {
            this.courseCode = courseCode;
            this.credits = credits;
        }
        public string CourseCode => courseCode;
        public string CourseName => courseNames.GetValueOrDefault(courseCode);
        public int Credits => credits;
        public static void RegisterCourse(string code, string name, int credits, IEnumerable<string> modules = null)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("code");
            courseNames[code] = name ?? string.Empty;
            courseCredits[code] = credits;
            courseModules[code] = modules != null ? new List<string>(modules) : new List<string>();
        }

        public static IReadOnlyList<string> GetModules(string code)
        {
            if (courseModules.TryGetValue(code, out var list)) return list.AsReadOnly();
            return Array.Empty<string>();
        }

        public static bool IsRegistered(string code) => courseNames.ContainsKey(code);

        public void DisplayModules()
        {
            var mods = GetModules(courseCode);
            if (mods.Count == 0)
            {
                Console.WriteLine("No modules registered for this course.");
                return;
            }
            Console.WriteLine($"Modules for {courseCode} ({CourseName}):");
            foreach (var m in mods) Console.WriteLine(" - " + m);
        }
        public void DisplayCourseInfo()
        {
            Console.WriteLine($"Course Code: {courseCode}, Course Name: {CourseName}, Credits: {credits}");
        }
    }
}