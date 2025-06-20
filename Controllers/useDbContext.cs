//using Microsoft.EntityFrameworkCore;
//using ResDb;
//using ResDb.Controllers;
//using System;

//namespace useDb;
//public class Programs
//{
//    public static void Main(string[] args)
//    {
//        using (var db = new DatabaseContext())
//        {                   
//            Console.WriteLine("Test Run in database kub:");
            
         
//            var wordWithId1 = db.MasterProjectReservedWord.FirstOrDefault(word => word.Id == "2");
//            if (wordWithId1 != null)
//            {
//                Console.WriteLine($" - ID: {wordWithId1.Id}, Name: {wordWithId1.WordName}, IsActive:{wordWithId1.IsActive}");
//            }
//            else
//            {
//                Console.WriteLine("No Word found with ID");
//            }

//            // ตัวอย่างโค้ดเดิม (แสดงทั้งหมด)
//            //Console.WriteLine("\n--- All Words in Database ---");
//            //foreach (var word in db.MasterProjectReservedWord)
//            //{
//            //    Console.WriteLine($" - ID: {word.Id}, Name: {word.WordName}, IsActive:{word.IsActive}");
//            //}
//        }
//    }
//}