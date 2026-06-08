using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Courses
{
    public record CourseInfo(string Name, int Credits, IReadOnlyList<string> Modules);

    public static class CourseCatalog
    {
        private static readonly Dictionary<string, CourseInfo> _catalog = new Dictionary<string, CourseInfo>(StringComparer.OrdinalIgnoreCase);

        // Exposed as read-only to callers
        public static readonly IReadOnlyDictionary<string, CourseInfo> Catalog = new ReadOnlyDictionary<string, CourseInfo>(_catalog);

        public static void RegisterCourse(string code, string name, int credits, IEnumerable<string> modules = null)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("code", nameof(code));
            if (name is null) name = string.Empty;
            var mods = modules != null ? new List<string>(modules) : new List<string>();
            var info = new CourseInfo(name, credits, mods.AsReadOnly());

            // allow overwriting during initialization
            _catalog[code] = info;
        }

        public static IReadOnlyList<string> GetModules(string code)
        {
            if (_catalog.TryGetValue(code, out var info))
            {
                return info.Modules;
            }
            return Array.Empty<string>();
        }

        public static bool IsRegistered(string code) => _catalog.ContainsKey(code);
    }

    public class Course
    {
        private readonly string courseCode;

        public Course(string courseCode)
        {
            if (string.IsNullOrWhiteSpace(courseCode)) throw new ArgumentException("courseCode", nameof(courseCode));
            if (!CourseCatalog.IsRegistered(courseCode))
            {
                throw new ArgumentException($"Course code '{courseCode}' is not registered in the catalog.", nameof(courseCode));
            }

            this.courseCode = courseCode;
        }

        public string CourseCode => courseCode;
        public string CourseName => CourseCatalog.Catalog.TryGetValue(courseCode, out var info) ? info.Name : string.Empty;
        public int Credits => CourseCatalog.Catalog.TryGetValue(courseCode, out var info2) ? info2.Credits : 0;

        public static IReadOnlyList<string> GetModulesStatic(string code) => CourseCatalog.GetModules(code);

        public IReadOnlyList<string> GetModules() => CourseCatalog.GetModules(courseCode);

        public void DisplayModules()
        {
            var mods = GetModules();
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
            Console.WriteLine($"Course Code: {courseCode}, Course Name: {CourseName}, Credits: {Credits}");
        }
    }
}