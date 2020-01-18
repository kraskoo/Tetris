using System;
using System.Collections.Generic;

public class IntComparer : IComparer<int>
{
    private readonly Comparison<int> comparison;

    public IntComparer(Comparison<int> comparison) => this.comparison = comparison;

    public int Compare(int x, int y)
    {
        return this.comparison(x, y);
    }
}