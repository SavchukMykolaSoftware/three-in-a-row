public class CountdownBeforeDefeat : Countdown
{
    private void Start()
    {
        AfterFinishingCountdown += GameEvents.OnFinishGame.Invoke;
        StartCountdown();
    }

    private void OnApplicationQuit()
    {
        AfterFinishingCountdown -= GameEvents.OnFinishGame.Invoke;
    }
}