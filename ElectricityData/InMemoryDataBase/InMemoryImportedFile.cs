using Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDataBase
{
    public class InMemoryImportedFile
    {
        private InMemoryImportedFile() { }

        private static readonly Lazy<InMemoryImportedFile> lazyInstance = new Lazy<InMemoryImportedFile>(() =>
        {
            return new InMemoryImportedFile();
        });

        public static InMemoryImportedFile Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        private ConcurrentDictionary<int, ImportedFile> dB = new ConcurrentDictionary<int, ImportedFile>();

        public List<ImportedFile> GetImportedFileData()
        {
            List<int> keys = dB.Keys.ToList();
            Dictionary<int, ImportedFile> importedFileData = new Dictionary<int, ImportedFile>();
            keys.ForEach(t => AddToDict(importedFileData, t));
            return importedFileData.Values.ToList();
        }

        private void AddToDict(Dictionary<int, ImportedFile> importedFileData, int l)
        {
            ImportedFile importedFile;
            if (dB.TryGetValue(l, out importedFile))
            {
                importedFileData.Add(l, importedFile);
            }
        }
        public bool InsertImportedFile(int id, ImportedFile importedFile)
        {
            return dB.TryAdd(id, importedFile);
        }
        
    }
}
