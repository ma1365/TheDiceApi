using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using TheDiceApi.Managers;
using TheDiceApi.Models;
using Xunit;

namespace TheDiceApiTests
{
    public class DiceRollerManagerTests
    {
        [Fact]
        public void Roll_D4()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D4, 4);
        }

        [Fact]
        public void Roll_D6()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D6, 6);
        }

        [Fact]
        public void Roll_D8()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D8, 8);
        }

        [Fact]
        public void Roll_D10()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D10, 10);
        }

        [Fact]
        public void Roll_D12()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D12, 12);
        }

        [Fact]
        public void Roll_D20()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D20, 20);
        }

        [Fact]
        public void Roll_D100()
        {
            //Arrange
            var manager = new DiceRollManager();

            CheckRoll(manager, DieType.D100, 100);
        }

        private static void CheckRoll(DiceRollManager manager, DieType die, ushort upperBound)
        {
            //Act
            var rolls = manager.RollDie(die, 1000);

            //Assert
            rolls.ForEach(roll => roll.RollValue.Should().BeInRange(0, upperBound));
        }

        [Theory]
        [InlineData("D4", 10, 4)]
        [InlineData("D6", 10, 6)]
        [InlineData("D8", 10, 8)]
        [InlineData("D10", 10, 10)]
        [InlineData("D12", 10, 12)]
        [InlineData("D20", 10, 20)]
        [InlineData("D100", 10, 100)]
        public void RollDice_Returns_Rolls(string dieTypeStr, uint numberOfRolls, ushort upperBound)
        {
            //Arrange
            var manager = new DiceRollManager();
            var results = new List<DiceRolls>();
            for (int i = 0; i < 1000; i++)
            {
                var input = new Dictionary<string, uint>
                {
                    {dieTypeStr, numberOfRolls}
                };

                //Act
                var result = manager.RollDice(input);
                results.Add(result);
            }

            //Assert
            var values = results.SelectMany(x => x.DiceRollResults.Values).SelectMany(x => x).ToList();
            values.ForEach(x => x.RollValue.Should().BeInRange(1, upperBound));
        }
    }
}
