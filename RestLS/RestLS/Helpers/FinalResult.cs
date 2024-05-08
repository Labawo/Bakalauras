namespace RestLS.Helpers;

public class FinalResult
{
    public int Score { get; set; }
    public string Result { get; set; }

    public FinalResult(int score, string result)
    {
        this.Score = score;
        this.Result = result;
    }
}