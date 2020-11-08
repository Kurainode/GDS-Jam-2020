public class TriggerOnSpeak : AkTriggerBase
{
    public void Speak()
    {
        if (triggerDelegate != null)
        {
            triggerDelegate(null);
        }
    }
}
