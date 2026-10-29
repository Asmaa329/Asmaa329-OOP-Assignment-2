namespace LibrarySystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Library System Test Execution ===");

            /*
            Person p = new Person("P1", "Ali", "0123"); // Compilation Error (Protected constructor)
            Member m = new Member("M1", "Ahmed", "0122"); // Compilation Error
            Staff s = new Staff("S1", "Sara", "0111", DateTime.Now, 5000); // Compilation Error
            LibraryItem item = new LibraryItem("I1", "Test", 10); // Compilation Error
            */

            var student = new StudentMember("M001", "Mahmoud Ali", "01012345678");
            var premium = new PremiumMember("M002", "Fatma Omar", "01187654321", 5m);

            var librarian = new Librarian("S001", "Khaled Youssef", "01211112222", new DateTime(2022, 1, 15), 6000m);
            var shelver = new Shelver("S002", "Nourhan Samy", "01533334444", new DateTime(2023, 5, 10), 4500m, "Fiction");
            var headLibrarian = new HeadLibrarian("S003", "Dr. Ibrahim", "01099998888", new DateTime(2018, 8, 1), 9000m);

            var book = new Book("B001", "C# Advanced Programming", 10m);
            var dvd = new DVD("D001", "Inception Movie", 15m);
            var magazine = new Magazine("Mag001", "Tech Monthly", 8m);

            try
            {
                headLibrarian.WithdrawItem(dvd);
                student.BorrowItem(dvd, DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Caught Expected Exception]: {ex.Message}");
            }

            try
            {
                headLibrarian.RestoreItem(dvd);
                student.BorrowItem(dvd, DateTime.Now);
                var anotherStudent = new StudentMember("M003", "Ali Hassan", "01000000000");
                anotherStudent.BorrowItem(dvd, DateTime.Now); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Caught Expected Exception]: {ex.Message}");
            }

            try
            {
                var book2 = new Book("B002", "Clean Code", 10m);
                var book3 = new Book("B003", "Design Patterns", 10m);
                var book4 = new Book("B004", "Refactoring", 10m);

                student.BorrowItem(book, DateTime.Now);
                student.BorrowItem(book2, DateTime.Now);
                student.BorrowItem(book3, DateTime.Now);
                student.BorrowItem(book4, DateTime.Now); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Caught Expected Exception]: {ex.Message}");
            }

            Console.WriteLine("\n--- Staff Monthly Pay ---");
            List<Staff> staffList = new List<Staff> { librarian, shelver, headLibrarian };
            foreach (var staffMember in staffList)
            {
                Console.WriteLine($"{staffMember.FullName} ({staffMember.GetType().Name}): {staffMember.GetMonthlyPay()} EGP");
            }

            Console.WriteLine("\n--- Library Items Info ---");
            List<LibraryItem> itemList = new List<LibraryItem> { book, dvd, magazine };
            foreach (var libItem in itemList)
            {
                Console.WriteLine($"[Item] {libItem.Title} | Loan Period: {libItem.LoanPeriodInDays} days | Daily Late Fee: {libItem.GetDailyLateFee()} EGP");
            }

            Console.WriteLine("\n--- Premium Member Loan & Late Fee Test ---");
            var testDvd = new DVD("D002", "Interstellar", 20m); 
            var loanRecord = premium.BorrowItem(testDvd, new DateTime(2026, 10, 1));
            DateTime simulatedReturnDate = new DateTime(2026, 10, 13);

            librarian.ProcessReturn(loanRecord, simulatedReturnDate);

            Console.WriteLine($"Due Date: {loanRecord.DueDate.ToShortDateString()}");
            Console.WriteLine($"Late Fee: {loanRecord.LateFee} EGP");
            Console.WriteLine($"Premium Member Reading Points: {premium.ReadingPoints}");

            try
            {
                librarian.ProcessReturn(loanRecord, simulatedReturnDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Caught Expected Exception]: {ex.Message}");
            }

            Console.WriteLine("\n=== All Tests Completed Successfully ===");
        }
    }
}
    

