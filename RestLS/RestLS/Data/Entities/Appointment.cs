using System.ComponentModel.DataAnnotations;
using RestLS.Auth.Models;

namespace RestLS.Data.Entities;

public class Appointment
{
    public int ID { get; set; }
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
    public Therapy Therapy { get; set; }
    
    public string DoctorName { get; set; }
    
    public string? PatientId { get; set; }
    public ClinicUser? Patien { get; set; }
    
}