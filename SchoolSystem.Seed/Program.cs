using Microsoft.EntityFrameworkCore;
using SchoolSystem.User.Data;
using SchoolSystem.User.Models;
using SchoolSystem.Academic.Data;
using SchoolSystem.Academic.Models;
using SchoolSystem.Grades.Data;
using SchoolSystem.Grades.Models;

Console.WriteLine("=== SCHOOL SYSTEM SEED STARTED ===");

// Delete old DBs
Delete("..\\SchoolSystem.User\\user.db");
Delete("..\\SchoolSystem.Academic\\academic.db");
Delete("..\\SchoolSystem.Grades\\grades.db");

// Init DB contexts
var userDb = new UserDbContext(
    new DbContextOptionsBuilder<UserDbContext>()
        .UseSqlite("Data Source=..\\SchoolSystem.User\\user.db")
        .Options
);

var academicDb = new AcademicDbContext(
    new DbContextOptionsBuilder<AcademicDbContext>()
        .UseSqlite("Data Source=..\\SchoolSystem.Academic\\academic.db")
        .Options
);

var gradesDb = new GradesDbContext(
    new DbContextOptionsBuilder<GradesDbContext>()
        .UseSqlite("Data Source=..\\SchoolSystem.Grades\\grades.db")
        .Options
);

// Create DBs
userDb.Database.EnsureCreated();
academicDb.Database.EnsureCreated();
gradesDb.Database.EnsureCreated();

// =======================================================
// USERS
// =======================================================
Console.WriteLine("Seeding users...");

var admin = new User {
    Email = "admin@mail.com",
    FirstName = "System",
    LastName = "Admin",
    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
    Role = UserRole.Admin,
    CreatedAt = DateTime.UtcNow
};

var teacher = new User {
    Email = "teacher@mail.com",
    FirstName = "Ivan",
    LastName = "Petrov",
    PasswordHash = BCrypt.Net.BCrypt.HashPassword("teacher123"),
    Role = UserRole.Teacher,
    CreatedAt = DateTime.UtcNow
};

userDb.Users.AddRange(admin, teacher);
userDb.SaveChanges();


// --- Create 10 students ---
var studentNames = new (string First, string Last)[]
{
    ("Oleh", "Sydorenko"),
    ("Mariya", "Koval"),
    ("Andriy", "Tkachenko"),
    ("Dmytro", "Shevchenko"),
    ("Iryna", "Poliakova"),
    ("Serhii", "Melnyk"),
    ("Olena", "Honchar"),
    ("Taras", "Kryvonos"),
    ("Kateryna", "Lytvyn"),
    ("Yurii", "Havryliuk")
};

var students = new List<User>();

foreach (var (first, last) in studentNames)
{
    var st = new User
    {
        Email = $"{first.ToLower()}.{last.ToLower()}@mail.com",
        FirstName = first,
        LastName = last,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("student123"),
        Role = UserRole.Student,
        CreatedAt = DateTime.UtcNow
    };
    students.Add(st);
}

userDb.Users.AddRange(students);
userDb.SaveChanges();

// Sync users into AcademicService
academicDb.Users.Add(new UserRef
{
    Id = teacher.Id,
    FirstName = teacher.FirstName,
    LastName = teacher.LastName
});

foreach (var st in students)
{
    academicDb.Users.Add(new UserRef
    {
        Id = st.Id,
        FirstName = st.FirstName,
        LastName = st.LastName
    });
}

academicDb.SaveChanges();

// =======================================================
// ACADEMIC SERVICE
// =======================================================
Console.WriteLine("Seeding classes and subjects...");

var class10A = new Class { Name = "10-A", Year = 2024 };
var class9B  = new Class { Name = "9-B", Year = 2024 };

academicDb.Classes.AddRange(class10A, class9B);
academicDb.SaveChanges();

var math = new Subject { Name = "Mathematics", Description = "Math lessons" };
var history = new Subject { Name = "History", Description = "History lessons" };

academicDb.Subjects.AddRange(math, history);
academicDb.SaveChanges();


// Assign students: first 5 → 10A, next 5 → 9B
for (int i = 0; i < students.Count; i++)
{
    academicDb.ClassStudents.Add(new ClassStudent
    {
        ClassId = (i < 5 ? class10A.Id : class9B.Id),
        StudentId = students[i].Id
    });
}

academicDb.SaveChanges();


// Teacher teaches both subjects in both classes
academicDb.TeacherSubjects.AddRange(
    new TeacherSubject { TeacherId = teacher.Id, SubjectId = math.Id, ClassId = class10A.Id },
    new TeacherSubject { TeacherId = teacher.Id, SubjectId = history.Id, ClassId = class10A.Id },
    new TeacherSubject { TeacherId = teacher.Id, SubjectId = math.Id, ClassId = class9B.Id },
    new TeacherSubject { TeacherId = teacher.Id, SubjectId = history.Id, ClassId = class9B.Id }
);

academicDb.SaveChanges();


// =======================================================
// GRADES
// =======================================================
Console.WriteLine("Seeding grades...");

var rng = new Random();

void AddRandomGrades(int studentId, int classId, int subjectId)
{
    for (int i = 0; i < rng.Next(5, 10); i++)
    {
        gradesDb.Grades.Add(new Grade
        {
            StudentId = studentId,
            ClassId = classId,
            SubjectId = subjectId,
            TeacherId = teacher.Id,
            Score = rng.Next(6, 13),
            Type = GradeType.Current,
            Date = DateTime.UtcNow.AddDays(-rng.Next(1, 40))
        });
    }
}

foreach (var student in students)
{
    int cls = academicDb.ClassStudents.First(cs => cs.StudentId == student.Id).ClassId;

    AddRandomGrades(student.Id, cls, math.Id);
    AddRandomGrades(student.Id, cls, history.Id);
}

gradesDb.SaveChanges();


// =======================================================
// HOMEWORK
// =======================================================
Console.WriteLine("Seeding homework...");

void AddHomework(int classId, int subjectId)
{
    for (int i = 0; i < rng.Next(5, 10); i++)
    {
        gradesDb.Homework.Add(new Homework
        {
            ClassId = classId,
            SubjectId = subjectId,
            TeacherId = teacher.Id,
            Title = $"Homework #{i + 1}",
            Description = "Complete tasks as described",
            DueDate = DateTime.UtcNow.AddDays(rng.Next(2, 14))
        });
    }
}

AddHomework(class10A.Id, math.Id);
AddHomework(class10A.Id, history.Id);
AddHomework(class9B.Id, math.Id);
AddHomework(class9B.Id, history.Id);

gradesDb.SaveChanges();


// =======================================================
// ATTENDANCE
// =======================================================
Console.WriteLine("Seeding attendance...");

void AddAttendance(int studentId, int classId, int subjectId)
{
    var statuses = new[] { AttendanceStatus.Present, AttendanceStatus.Late, AttendanceStatus.Absent, AttendanceStatus.Excused };

    for (int i = 0; i < rng.Next(5, 10); i++)
    {
        gradesDb.Attendance.Add(new Attendance
        {
            ClassId = classId,
            StudentId = studentId,
            SubjectId = subjectId,     // ДОДАНО !!!
            Status = statuses[rng.Next(statuses.Length)],
            Date = DateTime.UtcNow.AddDays(-rng.Next(1, 30))
        });
    }
}


foreach (var st in students)
{
    int cls = academicDb.ClassStudents.First(cs => cs.StudentId == st.Id).ClassId;
    AddAttendance(st.Id, cls, math.Id);
    AddAttendance(st.Id, cls, history.Id);
}

gradesDb.SaveChanges();

Console.WriteLine("=== SEED COMPLETED ===");

void Delete(string path)
{
    if (File.Exists(path))
    {
        Console.WriteLine($"Deleting {path}");
        File.Delete(path);
    }
}
