using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.src
{
    public class Weapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public Weapon Clone() => new Weapon { Name = Name, Damage = Damage };
    }

    public abstract class Enemy
    {
        private string _modelData;
        public string Name { get; set; }
        public int Health { get; set; }
        public Weapon Weapon { get; set; }
        public List<string> Abilities { get; set; } = new();
        public string ModelId => _modelData;
        protected Enemy()
        {
            Console.WriteLine("   ...loading 3D model (slow)...");
            Thread.Sleep(500);
            _modelData = "MODEL_" + Guid.NewGuid().ToString("N")[..6];
        }
        protected Enemy(Enemy other)
        {
            var copy = (Enemy)other.MemberwiseClone();

            this._modelData = copy._modelData;
            this.Name = copy.Name;
            this.Health = copy.Health;

            this.Weapon = other.Weapon?.Clone();
            this.Abilities = new List<string>(other.Abilities);
        }

        public abstract Enemy Clone();
    }

    public class Orc : Enemy
    {
        public Orc()
        {
            Name = "Orc";
            Health = 100;
            Weapon = new Weapon { Name = "Axe", Damage = 25 };
            Abilities.Add("Rage");
        }

        protected Orc(Orc other) : base(other)
        {
        }

        public override Enemy Clone() => new Orc(this);
    }
    public class Elf : Enemy
    {
        public Elf()
        {
            Name = "Elf";
            Health = 70;
            Weapon = new Weapon { Name = "Bow", Damage = 18 };
            Abilities.Add("Stealth");
        }
        protected Elf(Elf other) : base(other) { }

        public override Enemy Clone() => new Elf(this);
    }

    // Bonus (R5): Prototype Registry
    public class EnemyRegistry
    {
        private readonly Dictionary<string, Enemy> _prototypes = new();

        public void Register(string key, Enemy prototype)
        {
            _prototypes[key] = prototype;
        }

        public Enemy Create(string key)
        {
            if (_prototypes.TryGetValue(key, out var prototype))
            {
                return prototype.Clone();
            }
            throw new KeyNotFoundException($"Enemy prototype '{key}' not found.");
        }
    }
}
