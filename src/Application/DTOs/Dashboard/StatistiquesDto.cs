namespace Application.DTOs.Dashboard;

public class StatistiquesDto
{
    public int TotalSondes { get; set; }
    public int SondesActives { get; set; }
    public int SondesInactives { get; set; }
    
    public int TotalActionneurs { get; set; }
    public int ActionneursActifs { get; set; }
    public int ActionneursInactifs { get; set; }
    
    public int AlertesActives { get; set; }
    public int AlertesAcquittees { get; set; }
    public int AlertesResoluesAujourdHui { get; set; }
    
    public int RelevesAujourdHui { get; set; }
}