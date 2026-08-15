using DocQuery;
using DocQuery.Core;
namespace DocQuery.Tests;

public class VectorMathTests
{
    [Fact]
    public void IdenticalVectors_ReturnsOne()
    {
        // Arrange
        float[] a = { 1, 2, 3 };
        float[] b = { 1, 2, 3 };

        // Act
        double result = VectorMath.CosineSimilarity(a, b);

        // Assert
        Assert.Equal(1.0, result, precision: 6);
    }
    [Fact]
    public void OrthogonalVectors_ReturnsZero()
    {
        // Arrange
        float[] a = { 1, 0 };
        float[] b = { 0, 1 };
        // Act
        double result = VectorMath.CosineSimilarity(a, b);
        // Assert
        Assert.Equal(0.0, result, precision: 6);
    }
    [Fact]
    public void OppositeVectors_ReturnsNegativeValue()
    {
        // Arrange
        float[] a = { 1, 1 };
        float[] b = { -1, -1 };
        // Act
        double result = VectorMath.CosineSimilarity(a, b);
        // Assert
        Assert.Equal(-1.0, result, precision: 6);
    }
    [Fact]
    public void DifferentLengthVectors_ThrowsArgumentException()
    {
        // Arrange
        float[] a = { 1, 2, 3 };
        float[] b = { 1, 2 };
        // Act & Assert
        Assert.Throws<ArgumentException>(() => VectorMath.CosineSimilarity(a, b));
    }
    [Fact]
    public void NullVectors_ThrowsArgumentNullException()
    {
        // Arrange
        float[] a = null;
        float[] b = null;


        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => VectorMath.CosineSimilarity(a, b));
    }
    [Fact]
    public void DoubleLengthVectors_SameDirection_ReturnsExpectedValue()
    {
        // Arrange
        float[] a = { 1, 2, 3 };
        float[] b = { 2, 4, 6 };
        // Act
        double result = VectorMath.CosineSimilarity(a, b);
        // Assert
        Assert.Equal(1.0, result, precision: 6);
    }
}