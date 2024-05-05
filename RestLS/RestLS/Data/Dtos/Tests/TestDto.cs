namespace RestLS.Data.Dtos.Tests;

public record TestDto(int Id, string Name, int DepressionScore, int AnxietyScore, DateTime Time, string PatientId);
public record CreateTestDto(int DepressionScore, int AnxietyScore, string DepressionResults, string AnxietyResults);