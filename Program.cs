using System;
using GradeBook;

namespace ConsoleCalculatorCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SampleData sample = SampleInitialiser.Seed();
            sample.Registry.DisplayAllUniqueCourseCodes();
            foreach(var course in sample.Courses)
            {
                course.DisplayCourseInfo();
                course.DisplayModules();
            }
            sample.Registry.PerformanceTable();
            //new Gradebook().Run();
        }
    }
}