using task13;

namespace task13tests;

public class StudentSerializerTests
{
    private readonly StudentSerializer _serializer;
    private readonly Student _testStudent;

    public StudentSerializerTests()
    {
        _serializer = new StudentSerializer();
        _testStudent = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>
                {
                    new Subject { Name = "Математика", Grade = 85 },
                    new Subject { Name = "Физика", Grade = 90 }
                }
        };
    }

    [Fact]
    public void SerializeAndDeserialize_ShouldWork()
    {
        string json = _serializer.Serialize(_testStudent);

        var student = _serializer.Deserialize(json);

        Assert.Equal(_testStudent.FirstName, student.FirstName);
        Assert.Equal(_testStudent.LastName, student.LastName);
        Assert.Equal(_testStudent.BirthDate, student.BirthDate);
        Assert.Equal(_testStudent.Grades.Count, student.Grades.Count);

        for (int i = 0; i < _testStudent.Grades.Count; i++)
        {
            Assert.Equal(_testStudent.Grades[i].Name, student.Grades[i].Name);
            Assert.Equal(_testStudent.Grades[i].Grade, student.Grades[i].Grade);
        }
    }

    [Fact]
    public void DateFormat_ShouldBeCustom()
    {
        string json = _serializer.Serialize(_testStudent);

        var student = _serializer.Deserialize(json);
        Assert.Equal(new DateTime(2000, 5, 15), student.BirthDate);
    }

    [Fact]
    public void SaveAndLoadFile_ShouldWork()
    {
        string filePath = "test.json";

        _serializer.SaveToFile(_testStudent, filePath);
        Assert.True(File.Exists(filePath));

        var student = _serializer.LoadFromFile(filePath);
        Assert.Equal(_testStudent.FirstName, student.FirstName);
        Assert.Equal(_testStudent.BirthDate, student.BirthDate);

        File.Delete(filePath);
    }

    [Fact]
    public void Validation_ShouldThrowErrors()
    {
        var invalidStudent = new Student
        {
            FirstName = "",
            LastName = "Петров",
            BirthDate = DateTime.Now,
            Grades = new List<Subject>()
        };

        string json = _serializer.Serialize(invalidStudent);
        Assert.Throws<ArgumentException>(() => _serializer.Deserialize(json));
    }
}