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
                var query1 = from lenders in records.Where(r=> r.Borrower.ToLower() == p.ToLower())
                            join borrowers in records.Where(r=> r.Lender.ToLower() == p.ToLower()) 
                                on lenders.Lender equals borrowers.Borrower
                            into borrowingRecordGroup
                            select new {Inquirer = p, Other = lenders.Lender, Amount = lenders.Amount - borrowingRecordGroup.Sum(i=>i.Amount) };
                            
                var query2 = from borrowers in records.Where(r=> r.Lender.ToLower() == p.ToLower()) 
                             join lenders in records.Where(r=> r.Borrower.ToLower() == p.ToLower())
                                on borrowers.Borrower equals lenders.Lender
                            into lendingRecordGroup
                            select new {Inquirer = p, Other = borrowers.Borrower, Amount = -(borrowers.Amount - lendingRecordGroup.Sum(i=>i.Amount)) };
                
                var query = query1.Union(query2).DistinctBy(i=>i.Other);
                if(query.Any())
                {
                    foreach (var record in query)
                    {
                        if(record.Amount > 0)
                        {
                            Console.WriteLine($"{p} Owes {record.Other} total ${record.Amount}");
                        }
                        else if (record.Amount < 0)
                        {
                            Console.WriteLine($"{record.Other} Owes {p} total ${-record.Amount}");
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
