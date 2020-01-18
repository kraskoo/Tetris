using System;

[Serializable]
public class GameScores
{
    public GameScores(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9, int n10) =>
        this.SetScores(n1, n2, n3, n4, n5, n6, n7, n8, n9, n10);

    public GameScores() : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
    {
    }

    public int N1 { get; set; }

    public int N2 { get; set; }

    public int N3 { get; set; }

    public int N4 { get; set; }

    public int N5 { get; set; }

    public int N6 { get; set; }

    public int N7 { get; set; }

    public int N8 { get; set; }

    public int N9 { get; set; }

    public int N10 { get; set; }

    public void SetScores(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9, int n10)
    {
        this.N1 = n1;
        this.N2 = n2;
        this.N3 = n3;
        this.N4 = n4;
        this.N5 = n5;
        this.N6 = n6;
        this.N7 = n7;
        this.N8 = n8;
        this.N9 = n9;
        this.N10 = n10;
    }
}