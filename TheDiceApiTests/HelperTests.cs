using FluentAssertions;
using TheDiceApi.Extensions;
using TheDiceApi.Models;
using Xunit;

namespace TheDiceApiTests
{
    public class HelperTests
    {
        [Theory]
        [InlineData(DieType.D4, 4)]
        [InlineData(DieType.D6, 6)]
        [InlineData(DieType.D8, 8)]
        [InlineData(DieType.D10, 10)]
        [InlineData(DieType.D12, 12)]
        [InlineData(DieType.D20, 20)]
        [InlineData(DieType.D100, 100)]
        public void ToFacetCount_Returns_Correct_Value(DieType die, int expected)
        {
            //Arrange

            //Act
            var actual = die.ToFacetCount();

            //Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(4, DieType.D4)]
        [InlineData(6, DieType.D6)]
        [InlineData(8, DieType.D8)]
        [InlineData(10, DieType.D10)]
        [InlineData(12, DieType.D12)]
        [InlineData(20, DieType.D20)]
        [InlineData(100, DieType.D100)]
        public void ToDieType_Returns_Correct_Value(int facetCount, DieType expected)
        {
            //Arrange

            //Act
            var actual = facetCount.ToDieType();

            //Assert
            actual.Should().Be(expected);
        }
    }
}
