public class ButtonWhichCanBePressedLimitedQuantityOfTimesForUpdatingPosition : ButtonWhichCanBePressedLimitedQuantityOfTimes
{
    private void Start()
    {
        OnClick += GameLogic.instance.UpdatePosition;
    }
}