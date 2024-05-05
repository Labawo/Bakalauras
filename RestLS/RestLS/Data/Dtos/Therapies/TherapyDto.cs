namespace RestLS.Data.Dtos.Therapies;

public record TherapyDto(int Id, string Name, string Description, string DoctorId);
public record TherapyDtoWithImage(int Id, string Name, string Description, string DoctorId, string? ImageData);
public record CreateTherapyDto(string Name, string Description, string? DoctorId, string? ImageData);
public record UpdateTherapyDto(string Name, string Description, string? ImageData);