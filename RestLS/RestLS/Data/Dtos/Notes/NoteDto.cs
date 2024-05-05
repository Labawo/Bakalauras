namespace RestLS.Data.Dtos.Notes;

public record NoteDto(int Id, string Name, string Content, string PatientId);
public record CreateNoteDto(string Content, string Name);
public record UpdateNoteDto(string Content);