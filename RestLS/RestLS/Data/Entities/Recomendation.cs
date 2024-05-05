namespace RestLS.Data.Entities;

public class Recomendation
{
    public int ID { get; set; }
    public string Description { get; set; }
    
    public DateTime RecomendationDate { get; set; }
    
    public Appointment Appoint { get; set; }
}