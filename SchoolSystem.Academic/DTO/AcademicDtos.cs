namespace SchoolSystem.Academic.DTOs;

public record ClassDto(int Id, string Name, int Year);
public record StudentDto(int Id, string FirstName, string LastName);

public record SubjectDto(int Id, string Name, string Description);

public record TeacherSubjectDto(
    int TeacherId,
    int SubjectId,
    int ClassId,
    string SubjectName,
    string ClassName
);
