using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>Fictional campus venues and a sample weekly lecture timetable. Pure data.</summary>
internal static class VenueData
{
    public sealed record VenueSeed(string Code, string Name, string Building, VenueType Type, int Capacity, string? Facilities);

    /// <param name="DayOffset">Days after Monday (0 = Monday).</param>
    public sealed record LectureSeed(string VenueCode, string Title, string Programme, int AcademicYear, int DayOffset, TimeOnly Start, TimeOnly End);

    public const string MainEventHall = "EH-01";
    public const int LectureWeeks = 4;

    public static readonly VenueSeed[] Venues =
    [
        new("LH-101", "Lecture Hall 101", "Faculty of Computing", VenueType.LectureHall, 120, "Projector, microphones"),
        new("LH-102", "Lecture Hall 102", "Faculty of Computing", VenueType.LectureHall, 80, "Projector"),
        new("LH-201", "Lecture Hall 201", "Faculty of Business", VenueType.LectureHall, 150, "Projector, lecture capture"),
        new("LAB-01", "Computer Lab 01", "Faculty of Computing", VenueType.ComputerLab, 60, "60 workstations, projector"),
        new("LAB-02", "Computer Lab 02", "Faculty of Computing", VenueType.ComputerLab, 40, "40 workstations"),
        new("AUD-01", "Main Auditorium", "Main Building", VenueType.Auditorium, 500, "Stage, sound system, live stream"),
        new(MainEventHall, "Innovation Event Hall", "Main Building", VenueType.EventHall, 200, "Movable seating, sound system, Wi-Fi"),
    ];

    public static readonly LectureSeed[] Lectures =
    [
        new("LH-101", "Software Architecture", "BSc (Hons) Software Engineering", 3, 0, new(9, 0), new(11, 0)),
        new("LAB-01", "Cloud Computing (practical)", "BSc (Hons) Software Engineering", 3, 2, new(13, 0), new(15, 0)),
        new("LH-102", "Data Structures and Algorithms", "BSc (Hons) Software Engineering", 2, 1, new(10, 0), new(12, 0)),
        new("LH-102", "Machine Learning", "BSc (Hons) Computer Science", 3, 3, new(9, 0), new(11, 0)),
    ];
}
