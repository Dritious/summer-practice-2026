namespace task13;

class Subject
{
    public string Name { get; set; }
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public List<Subject> Grades { get; set; }
}
