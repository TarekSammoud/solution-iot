using System.Collections.Generic;

namespace IotPlatform.Application.DTOs.External
{
    public class ImportSondeResultDto
    {
        public int NombreImportees { get; set; }
        public int NombreDoublons { get; set; }
        public List<string> Erreurs { get; set; } = new List<string>();
    }
}