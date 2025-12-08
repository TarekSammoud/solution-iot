using Riok.Mapperly.Abstractions;
using Domain.Entities;
using Application.DTOs.SeuilAlerte;

namespace Application.Mappers
{
    [Mapper]
    public partial class SeuilAlerteMapper
    {
        public partial SeuilAlerteDto Map(SeuilAlerte seuil);

        public partial SeuilAlerte Map(CreateSeuilAlerteDto dto);

        public partial void UpdateEntity(UpdateSeuilAlerteDto dto, SeuilAlerte entity);
    }
}
