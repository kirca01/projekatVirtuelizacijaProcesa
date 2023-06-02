using Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDataBase
{
    public class InMemoryAudit
    {
        private InMemoryAudit() { }

        private static readonly Lazy<InMemoryAudit> lazyInstance = new Lazy<InMemoryAudit>(() =>
        {
            return new InMemoryAudit();
        });

        public static InMemoryAudit Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        private ConcurrentDictionary<int, Audit> dB = new ConcurrentDictionary<int, Audit>();

        public List<Audit> GetAuditData()
        {
            List<int> keys = dB.Keys.ToList();
            Dictionary<int, Audit> auditData = new Dictionary<int, Audit>();
            keys.ForEach(t => AddToDict(auditData, t));
            return auditData.Values.ToList();
        }

        private void AddToDict(Dictionary<int, Audit> auditData, int l)
        {
            Audit audit;
            if (dB.TryGetValue(l, out audit))
            {
                auditData.Add(l, audit);
            }
        }

        public bool InsertAudit(int id, Audit audit)
        {
            return dB.TryAdd(id, audit);
        }
    }
}
