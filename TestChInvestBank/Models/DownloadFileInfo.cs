using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestChInvestBank.Models
{
    public class DownloadFileInfo
    {
        public int VersionId { get; set; } = 0;
        public string TextVersion { get; set; } = string.Empty;
        public string FiasCompleteDbfUrl { get; set; } = string.Empty;
        public string FiasCompleteXmlUrl { get; set; } = string.Empty;
        public string FiasDeltaDbfUrl { get; set; } = string.Empty;
        public string FiasDeltaXmlUrl { get; set; } = string.Empty;
        public string Kladr4ArjUrl { get; set; } = string.Empty;
        public string Kladr47ZUrl { get; set; } = string.Empty;
        public string GarXMLFullURL { get; set; } = string.Empty;
        public string GarXMLDeltaURL { get; set; } = string.Empty;
        public string ExpDate { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
