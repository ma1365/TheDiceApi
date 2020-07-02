using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TheDiceApi.Controllers.API;
using TheDiceApi.Managers;
using TheDiceApi.Models;
using TheDiceApi.ViewModels;
using Xunit;

namespace TheDiceApiTests
{
    public class DiceRollerControllerTests
    {
        private readonly Mock<IDiceRollManager> _manager;
        private readonly Fixture _fixture;

        private readonly DiceRollerController _sut;

        public DiceRollerControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _manager = _fixture.Freeze<Mock<IDiceRollManager>>();
            _sut = new DiceRollerController(_manager.Object);
        }

        [Theory]
        [InlineData("D4", 2)]
        [InlineData("D6", 6)]
        [InlineData("D8", 8)]
        [InlineData("D10", 10)]
        [InlineData("D12", 12)]
        [InlineData("D20", 20)]
        [InlineData("D100", 100)]
        public void GetDie_Returns_Expected_Result(string dieType, ushort expectedRollValue)
        {
            //Arrange
            var dieTypeEnum = Enum.Parse<DieType>(dieType);

            var expectedRoll = new Die { DieType = dieTypeEnum, RollValue = expectedRollValue };

            _manager.Setup(x => x.RollDie(dieTypeEnum, 1)).Returns(new List<Die>{ expectedRoll });

            //Act
            var actual = _sut.GetSingle(dieTypeEnum, 1);

            //Assert
            actual.Value.Should().BeEquivalentTo(expectedRoll);
        }

        [Fact]
        public void Get_Many_Returns_Expected_Result()
        {
            //Arrange
            var response = _fixture.Create<DiceRolls>();
            _manager.Setup(x => x.RollDice(It.IsAny<Dictionary<string, uint>>())).Returns(response);

            var expected = new List<DiceRolls> {response};
            //Act
            var actual = _sut.GetMultiple(2,0,0,0,0,0,0);

            //Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetSingleDiceRollCommands_Returns_Expected_Result()
        {
            //Arrange
            var expected = new List<DiceRollInfoComponent>();

            foreach (var val in Enum.GetValues(typeof(DieType)))
            {
                expected.Add(new DiceRollInfoComponent
                {
                    AvailableCommands = new List<CommandComponent>{ new CommandComponent
                    {
                        CommandName = "RollDice",
                        FriendlyName = "Roll Dice",
                        IsEnabled = true,
                        IsVisible = true,
                        Payload = new CommandPayload{ ExecutionData = JObject.FromObject(new DiceRollRequest { DieType = ((DieType)val).ToString() })
                    }}
                }});
            }
                    

            //Act
            var actual = _sut.GetSingleDiceRollCommands();

            //Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetMultipleDiceRollCommands_Returns_Expected_Result()
        {
            //Arrange
            var expected = new DiceRollInfoComponent();

            foreach (var val in Enum.GetValues(typeof(DieType)))
            {
                expected.AvailableCommands.Add(new CommandComponent
                {
                    CommandName = "RollDice",
                    FriendlyName = "Roll Dice",
                    IsEnabled = true,
                    IsVisible = true,
                    Payload = new CommandPayload
                    {
                        ExecutionData = JObject.FromObject(new Dictionary<DieType, uint>{ { (DieType)val, 0 } })
                    }
                });
            }

            //Act
            var actual = _sut.GetMultipleDiceRollCommands();

            //Assert
            expected.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PostSingleWithCommand_Returns_expected()
        {
            //Arrange
            var rolls = _fixture.CreateMany<Die>().ToList();
            var command = new CommandComponent
            {
                CommandName = "RollDice",
                FriendlyName = "Roll Dice",
                IsEnabled = true,
                IsVisible = true,
                Payload = new CommandPayload
                    {ExecutionData = JObject.FromObject(new DiceRollRequest {DieType = "D12", RollCount = 1}) }

            };

            var request = new SingleDiceRollComponent
            {
                DieType = "D12",
                SelectedCommand = command,
                AvailableCommands = new List<CommandComponent> {command}
            };

            var expected = request;

            _manager.Setup(x => x.RollDie(It.IsAny<DieType>(), It.IsAny<uint>()))
                .Returns(rolls);

            //Act
            var actual = _sut.PostSingleWithCommand(request);

            //Assert
            expected.RollValues = rolls.Select(x => x.RollValue).ToList();
            actual.Value.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public void PostManyWithCommand_Returns_expected()
        {
            //Arrange
            var availableCommands = new List<CommandComponent>();
            foreach (var val in Enum.GetValues(typeof(DieType)))
            {
                availableCommands.Add(new CommandComponent
                {
                    CommandName = "RollDice",
                    FriendlyName = "Roll Dice",
                    IsEnabled = true,
                    IsVisible = true,
                    Payload = new CommandPayload
                    {
                        ExecutionData = JObject.FromObject(new Dictionary<DieType, uint> { { (DieType)val, 0 } })
                    }
                });
            }
            
            var command = new CommandComponent
            {
                CommandName = "RollDice",
                FriendlyName = "Roll Dice",
                IsEnabled = true,
                IsVisible = true,
                Payload = new CommandPayload
                {
                    ExecutionData = JObject.FromObject(new Dictionary<DieType, uint> { { DieType.D12, 0 } })
                }
            };

            var request = new MultipleDiceRollComponent
            {
                SelectedCommand = command,
                AvailableCommands = availableCommands
            };

            var rolls = _fixture.Create<DiceRolls>();
            _manager.Setup(x => x.RollDice(It.IsAny<Dictionary<string, uint>>())).Returns(rolls);

            var expected = request;

            

            //Act
            var actual = _sut.PostManyWithCommand(request);

            //Assert
            expected.RollValues = rolls;
            actual.Value.Should().BeEquivalentTo(expected);
        }
    }
}
