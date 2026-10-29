using DesignPatterns.src;
using System.Diagnostics;
using static DesignPatterns.src.CourseRegistrationBuilder;

namespace DesignPatterns
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("=== SINGLETON: BEFORE ===\n");

            var db = new DatabaseService();
            var ui = new UiService();

            Console.WriteLine();

            db.Config.Theme = "Dark";
            Console.WriteLine("Admin changed theme to Dark.");

            ui.Render();

            Console.WriteLine($"\nSame config object? {ReferenceEquals(db.Config, ui.Config)}");
            Console.WriteLine($"Times config was loaded from disk: {AppConfig.LoadCount}");

            Console.WriteLine("\n=== PROTOTYPE PATTERN DEMO ===\n");

            var sw = Stopwatch.StartNew();
            var army = new List<Enemy>();

            var prototypeOrc = new Orc();

            for (int i = 1; i <= 5; i++)
            {
                var orcCopy = (Orc)prototypeOrc.Clone(); 
                orcCopy.Name = $"Orc-{i}";
                army.Add(orcCopy);
            }

            sw.Stop();
            Console.WriteLine($"\nCreated 5 orcs using Prototype in {sw.ElapsedMilliseconds} ms\n");

            Enemy original = new Orc();
            original.Name = "Boss Orc";

            Enemy copy = original.Clone();

            copy.Weapon.Damage = 999;
            Console.WriteLine($"\nWe changed the COPY's weapon damage to 999.");
            Console.WriteLine($"Original's weapon damage is now: {original.Weapon.Damage} (Should be safe & unchanged!)");

            copy.Abilities.Add("Fire Breath");
            Console.WriteLine($"Original abilities count: {original.Abilities.Count} (Should be safe & unchanged!)");

            Console.WriteLine("\n=== BUILDER: BEFORE ===\n");
            Console.WriteLine(RegistrationCallSites.CreateLiveStudentUgly());
            Console.WriteLine(RegistrationCallSites.CreateVideosOnlyUgly());

        }
    }
    
}
