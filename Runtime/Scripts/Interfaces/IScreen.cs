public interface IScreen
{
    void Init();
    public bool IsScreenDisplayed { get; }
    void Display();
    void Hide();
}
