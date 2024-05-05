namespace RestLS.Data.Dtos.Recomendation;

public record RecomendationDto(int Id, string Description, DateTime time);
public record CreateRecomendationDto(string Description);
public record UpdateRecomendationDto(string Description);