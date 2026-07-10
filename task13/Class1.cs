using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; }
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    [JsonConverter(typeof(DateTimeJsonConverter))]
    public List<Subject> Grades { get; set; }

    public Student()
    {
        Grades = new List<Subject>();
    }
}

// кастомный формат дат при конвертации
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "dd MMMM yyyy";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), DateFormat, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}


public class StudentSerializer
{
    private readonly JsonSerializerOptions _options;

    public StudentSerializer()
    {
        _options = new JsonSerializerOptions
        {
            // игнорирование null значений
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public string Serialize(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        return JsonSerializer.Serialize(student, _options);
    }


    public Student Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON не может быть пустым", nameof(json));

        var student = JsonSerializer.Deserialize<Student>(json, _options);

        // валидация данных на null значения
        ValidateStudent(student);
        return student;
    }

    public void SaveToFile(Student student, string filePath)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));

        var json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public Student LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
    private void ValidateStudent(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student), "Студент не может быть null");

        if (string.IsNullOrWhiteSpace(student.FirstName))
            throw new ArgumentException("Имя не может быть пустым", nameof(student.FirstName));

        if (string.IsNullOrWhiteSpace(student.LastName))
            throw new ArgumentException("Фамилия не может быть пустой", nameof(student.LastName));

        if (student.BirthDate == DateTime.MinValue)
            throw new ArgumentException("Дата рождения не указана", nameof(student.BirthDate));

        if (student.Grades == null)
            throw new ArgumentException("Список оценок не может быть null", nameof(student.Grades));

        foreach (var subject in student.Grades)
        {
            if (string.IsNullOrWhiteSpace(subject.Name))
                throw new ArgumentException("Название предмета не может быть пустым", nameof(subject.Name));
        }
    }
}