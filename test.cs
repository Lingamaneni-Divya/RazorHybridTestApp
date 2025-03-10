using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

public class GenericEqualityComparerTests
{
    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenObjectsHaveSameValues()
    {
        var comparer = new GenericEqualityComparer<TestClass>();

        var obj1 = new TestClass { Id = 1, Name = "Alice" };
        var obj2 = new TestClass { Id = 1, Name = "Alice" };

        bool result = comparer.Equals(obj1, obj2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenObjectsHaveDifferentValues()
    {
        var comparer = new GenericEqualityComparer<TestClass>();

        var obj1 = new TestClass { Id = 1, Name = "Alice" };
        var obj2 = new TestClass { Id = 2, Name = "Bob" };

        bool result = comparer.Equals(obj1, obj2);

        Assert.False(result);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOneObjectIsNull()
    {
        var comparer = new GenericEqualityComparer<TestClass>();

        var obj1 = new TestClass { Id = 1, Name = "Alice" };

        bool result = comparer.Equals(obj1, null);

        Assert.False(result);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenBothObjectsAreNull()
    {
        var comparer = new GenericEqualityComparer<TestClass>();

        bool result = comparer.Equals(null, null);

        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualObjects()
    {
        var comparer = new GenericEqualityComparer<TestClass>();

        var obj1 = new TestClass { Id = 1, Name = "Alice" };
        var obj2 = new TestClass { Id = 1, Name = "Alice" };

        int hash1 = comparer.GetHashCode(obj1);
        int hash2 = comparer.GetHashCode(obj2);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentValue_ForDifferentObjects()
    {
        var comparer = new GenericEqualityComparer<TestClass>();

        var obj1 = new TestClass { Id = 1, Name = "Alice" };
        var obj2 = new TestClass { Id = 2, Name = "Bob" };

        int hash1 = comparer.GetHashCode(obj1);
        int hash2 = comparer.GetHashCode(obj2);

        Assert.NotEqual(hash1, hash2);
    }
}

using System.Collections.Generic;
using System.Linq;
using Xunit;

public class DistinctHelperTests
{
    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Fact]
    public void Distinct_ShouldReturnUniqueObjects_BasedOnProperties()
    {
        var items = new List<TestClass>
        {
            new TestClass { Id = 1, Name = "Alice" },
            new TestClass { Id = 1, Name = "Alice" }, // Duplicate
            new TestClass { Id = 2, Name = "Bob" }
        };

        var distinctItems = items.Distinct().ToList();

        Assert.Equal(2, distinctItems.Count);
        Assert.Contains(distinctItems, x => x.Id == 1 && x.Name == "Alice");
        Assert.Contains(distinctItems, x => x.Id == 2 && x.Name == "Bob");
    }

    [Fact]
    public void Distinct_ShouldReturnEmptyList_WhenSourceIsEmpty()
    {
        var items = new List<TestClass>();

        var distinctItems = items.Distinct().ToList();

        Assert.Empty(distinctItems);
    }
}