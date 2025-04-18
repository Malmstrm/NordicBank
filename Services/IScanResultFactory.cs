using DataAccessLayer.DTO;
using DataAccessLayer.Models;

namespace Services
{
    public interface IScanResultFactory
    {
        ScanResultDTO Create(ScanLog log, List<SuspiciousTransaction> transactions);
    }
}
