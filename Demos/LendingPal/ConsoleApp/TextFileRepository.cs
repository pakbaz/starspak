using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp
{
    public class TextFileRepository : IRepository<LendingRecord>
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "db.txt");
        public void Add(LendingRecord record)
        {
            var records = GetRecords();
            records.Add(record);
            SaveRecords(records);
        }

        public List<LendingRecord> GetAll()
        {
            return GetRecords();
        }

        public void Remove(LendingRecord record)
        {
            var records = GetRecords();
            records.Remove(record);
            SaveRecords(records);
        }

        List<LendingRecord> GetRecords ()
        {
            File.AppendAllText(dbPath,string.Empty);
            var file = File.ReadAllText(dbPath);
            var records = JsonConvert.DeserializeObject<List<LendingRecord>>(file);
            if(records == null) records = new List<LendingRecord>();
            return records;
        }

        void SaveRecords (List<LendingRecord> records)
        {
            var content = JsonConvert.SerializeObject(records);
            File.WriteAllText(dbPath,content);
        }
    }
}