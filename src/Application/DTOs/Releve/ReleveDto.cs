using Domain.Enums;
using Application.DTOs.Sonde;
namespace Application.DTOs.Releve;

public class ReleveDto
{
    public Guid Id { get; set; }

    public Guid SondeId { get; set; }

    public decimal Valeur { get; set; }

    public DateTime DateHeure { get; set; }

    public TypeReleve TypeReleve { get; set; }

    public SondeDto? Sonde { get; set; } = null!;

}
public class RelevePageDto
{
    public RelevePageDto(int page, int total, int size, IEnumerable<ReleveDto> releves, DateTime? startDate, DateTime? endDate, TypeReleve? typeReleve)
    {
        this.page = page;
        this.total = total;
        this.size = size;
        this.releves = releves;
        this.startDate = startDate;
        this.endDate = endDate;
        this.typeReleve = typeReleve;
    }
    public RelevePageDto()
    {
        this.page = 0;
        this.total = 0;
        this.size = 0;
        this.releves = new List<ReleveDto>();
        this.startDate = DateTime.MinValue; 
        this.endDate = DateTime.MaxValue;
        this.typeReleve = null;
    }

    public int page { get; set; }
    public int total { get; set; }
    public int size { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    public TypeReleve? typeReleve { get; set; }
    public IEnumerable<ReleveDto> releves { get; set; }
}
