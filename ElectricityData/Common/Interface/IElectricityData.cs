using Common.Files;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    [ServiceContract]
    public interface IElectricityData                                               
    {
        [OperationContract]
        void HandleFile(StreamFile streamFile, FileType fileType);
        [OperationContract]
        void CalculateDeviation();
        [OperationContract]
        List<Load> GetLoadData();
        [OperationContract]
        List<Audit> GetAuditData();
        [OperationContract]
        List<ImportedFile> GetImportedFileData();
    }
}
