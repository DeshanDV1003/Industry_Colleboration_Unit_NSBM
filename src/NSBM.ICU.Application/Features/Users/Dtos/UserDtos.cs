using NSBM.ICU.Application.Features.Users.Models;

namespace NSBM.ICU.Application.Features.Users.Dtos;

/// <summary>One row of the user list. <see cref="Summary"/> is the role's identifying line, e.g. student number and programme.</summary>
public sealed record UserDto(string UserId, string Role, string FullName, string Email, bool IsActive, string Summary);

/// <summary>An account opened for editing.</summary>
public sealed record UserDetailDto(string UserId, string Role, string Email, bool IsActive, UserProfileInput Profile);

public sealed record UserOptionDto(int Id, string Name);

/// <summary>Choices for the user form: partner companies and the programmes already in use.</summary>
public sealed record UserOptionsDto(IReadOnlyList<UserOptionDto> Companies, IReadOnlyList<string> Programmes);

/// <summary>What a CSV import did, row by row. Row numbers count the header as row 1.</summary>
public sealed record StudentImportResultDto(int Created, IReadOnlyList<StudentImportRowError> Errors);

public sealed record StudentImportRowError(int Row, string? StudentNo, string Message);
