namespace task04tests;

using task04;
using Xunit;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }
    [Fact]
    public void MoveForward_ShouldUpdateCoordinates()
    {
        var ship = new Fighter { X = 0, Y = 0, Angle = 45 };
        ship.MoveForward();

        Assert.Equal(70, ship.X);
        Assert.Equal(70, ship.Y);
    }

    [Fact]
    public void Rotate_ShouldChangeAngleCorrectly()
    {
        var ship = new Fighter { Angle = 0 };
        ship.Rotate(45);
        Assert.Equal(45, ship.Angle);

        ship.Rotate(330);
        Assert.Equal(15, ship.Angle);

        ship.Rotate(-30);
        Assert.Equal(345, ship.Angle);
    }

    [Fact]
    public void Fire_ShouldDealDamageToTargetsInLineOfSight()
    {
        var shooter = new Fighter { X = 0, Y = 0, Angle = 45 };
        var target = new Cruiser { X = 10, Y = 10, Health = 150 };

        SpaceshipMap.AddShip(shooter);
        SpaceshipMap.AddShip(target);

        shooter.Fire();

        Assert.Equal(130, target.Health);
    }

}