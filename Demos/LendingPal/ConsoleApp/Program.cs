using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static IRepository<LendingRecord> Records = new TextFileRepository();
        static bool inputMode, inquiryMode, programRunning = true;
        static void Main(string[] args)
        {
            Console.WriteLine("===== Welcome to LendingPal =====");

            while (programRunning)
            {
                Console.WriteLine("Add New Record Press A, Inquiry Press I, Quit press Q");
                string c = Console.ReadLine();
                if (c.ToLower() == "a")
                {
                    GetRecord();
                }
                else if (c.ToLower() == "i")
                {
                    Inquiry();
                }
                else if (c.ToLower() == "q")
                {
                    programRunning = false;
                }
            }
        }

        private static void Inquiry()
        {
            inquiryMode = true;
            while (inquiryMode)
            {
                Console.Write("Whom do you want to Inquiry? : ");
                string p = Console.ReadLine();

                var records = Records.GetAll();

                //one query takes care of all scenarios
                var query = from lenders in records.Where(r=> r.Borrower.ToLower() == p.ToLower())
                            join borrowers in records.Where(r=> r.Lender.ToLower() == p.ToLower()) 
                                on lenders.Lender equals borrowers.Borrower
                            into lendingRecordGroup
                            select new LendingRecord{Borrower = p, Lender = lenders.Lender, Amount = lenders.Amount - lendingRecordGroup.Sum(i=>i.Amount) };
                            
                            

                if(query.Any())
                {
                    foreach (var record in query)
                    {
                        if(record.Amount > 0)
                        {
                            Console.WriteLine($"{p} Owes {record.Lender} total ${record.Amount}");
                        }
                        else if (record.Amount < 0)
                        {
                            Console.WriteLine($"{record.Lender} Owes {p} total ${-record.Amount}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Can't find any records!");
                }


                Console.WriteLine("More Inquiry? Yes or No");
                if (Console.ReadLine().ToLower().StartsWith("n")) inquiryMode = false;
            }
        }

        private static void GetRecord()
        {
            inputMode = true;
            while (inputMode)
            {
                LendingRecord record = new();
                Console.Write("Who is Borrowing? : ");
                record.Borrower = Console.ReadLine();
                Console.Write("Who is Lending? : ");
                record.Lender = Console.ReadLine();
                Console.Write("What is the Amount? : ");
                record.Amount = Convert.ToDecimal(Console.ReadLine()); //Input needs to be checked. Potential runtime error!
                record.Date = DateTime.Now;
                Records.Add(record);

                Console.WriteLine("Add More? Yes or No");
                if (Console.ReadLine().ToLower().StartsWith("n")) inputMode = false;
            }
        }
    }
}
