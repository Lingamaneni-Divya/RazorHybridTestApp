using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

public class BatchServiceTests
{
    [Fact]
    public void Batch_ShouldSplitListIntoCorrectSizedBatches()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 10); // [1,2,3,4,5,6,7,8,9,10]
        int batchSize = 3;

        // Act
        var result = numbers.Batch(batchSize).ToList();

        // Assert
        Assert.Equal(4, result.Count); // 10 elements should be split into 4 batches: [1,2,3], [4,5,6], [7,8,9], [10]
        Assert.Equal(new List<int> { 1, 2, 3 }, result[0]);
        Assert.Equal(new List<int> { 4, 5, 6 }, result[1]);
        Assert.Equal(new List<int> { 7, 8, 9 }, result[2]);
        Assert.Equal(new List<int> { 10 }, result[3]); // Last batch has only 1 element
    }

    [Fact]
    public void Batch_ShouldHandleExactBatchSize()
    {
        // Arrange
        var numbers = new List<int> { 1, 2, 3, 4 };
        int batchSize = 2;

        // Act
        var result = numbers.Batch(batchSize).ToList();

        // Assert
        Assert.Equal(2, result.Count); // Should split into two batches: [1,2], [3,4]
        Assert.Equal(new List<int> { 1, 2 }, result[0]);
        Assert.Equal(new List<int> { 3, 4 }, result[1]);
    }

    [Fact]
    public void Batch_ShouldHandleSingleElement()
    {
        // Arrange
        var numbers = new List<int> { 5 };
        int batchSize = 3;

        // Act
        var result = numbers.Batch(batchSize).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(new List<int> { 5 }, result[0]);
    }

    [Fact]
    public void Batch_ShouldReturnEmpty_WhenSourceIsEmpty()
    {
        // Arrange
        var numbers = new List<int>();
        int batchSize = 3;

        // Act
        var result = numbers.Batch(batchSize).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Batch_ShouldThrowException_WhenBatchSizeIsZeroOrNegative()
    {
        // Arrange
        var numbers = Enumerable.Range(1, 5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => numbers.Batch(0).ToList());
        Assert.Throws<ArgumentException>(() => numbers.Batch(-1).ToList());
    }

    [Fact]
    public void Batch_ShouldThrowException_WhenSourceIsNull()
    {
        // Arrange
        IEnumerable<int> numbers = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => numbers.Batch(3).ToList());
    }
}