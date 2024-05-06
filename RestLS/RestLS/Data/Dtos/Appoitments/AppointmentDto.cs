namespace RestLS.Data.Dtos.Appoitments;

public record AppointmentDto(int Id, DateTime Time, decimal Price, string? PatientId, string DoctroName);

public record AppointmentForDoctorDto(int Id, DateTime Time, decimal Price, string? PatientName, string? PatientId, string DoctroName);
public record AppointmentForPatientDto(int Id, DateTime Time, decimal Price, string DoctorName);
public record CreateAppointmentDto(decimal Price, string Time);
public record UpdateAppointmentDto(string Time, decimal Price);