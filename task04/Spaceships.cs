namespace task04;

public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
}

// Карта на которой расположены космические корабли
public static class SpaceshipMap
{
    private static List<SimpleSpaceship> _ships = new();

    public static void AddShip(SimpleSpaceship ship) => _ships.Add(ship);
    public static void RemoveShip(SimpleSpaceship ship) => _ships.Remove(ship);

    public static void ProcessFire(SimpleSpaceship shooter)
    {
        foreach (var target in _ships.Where(s => s != shooter && s.Health > 0))
        {
            double dx = target.X - shooter.X;
            double dy = target.Y - shooter.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) continue;

            double actualSin = Math.Abs(dy) / distance;
            double shooterSin = Math.Abs(Math.Sin(shooter.Angle * Math.PI / 180));

            // Погрешность 3 градуса (иначе сложно получить урон, можно считать как хитбокс корабля)
            if (Math.Abs(actualSin - shooterSin) < Math.Sin(3))
            {
                target.Health -= shooter.FirePower;
            }
        }
    }
}

//Простейшая реализация корабля с помощью интерфейса
public abstract class SimpleSpaceship : ISpaceship
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; } = 150;
    public int Angle { get; set; }

    // делаем поля абстрактными чтобы в крейсерах/истребителях прописать конкретные значения
    public abstract int Speed { get; }
    public abstract int FirePower { get; }

    public void MoveForward()
    {
        X += (int)(Speed * Math.Cos(Angle * Math.PI / 180));
        Y += (int)(Speed * Math.Sin(Angle * Math.PI / 180));
    }

    public void Rotate(int angle) => Angle = (Angle + angle);
    public void Fire() => SpaceshipMap.ProcessFire(this);
}

public class Cruiser : SimpleSpaceship
{
    public override int Speed => 50;
    public override int FirePower => 100;
}

public class Fighter : SimpleSpaceship
{
    public override int Speed => 100;
    public override int FirePower => 50;
}