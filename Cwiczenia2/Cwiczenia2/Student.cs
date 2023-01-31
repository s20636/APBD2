using System.Diagnostics.CodeAnalysis;

namespace Cwiczenia2
{
    public class Student : IEqualityComparer<Student>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public Studies studies { get; set; }

        public bool Equals(Student x, Student y)
        {
            return (x.FirstName == y.FirstName) && (x.LastName == y.LastName) && (x.IndexNumber == y.IndexNumber);
        }

        public int GetHashCode([DisallowNull] Student obj)
        {
            return obj.IndexNumber.GetHashCode();
        }

        public class Studies
        {
            public string Department { get; set; }
            public string LearningMode { get; set; }
        }
    }
}
