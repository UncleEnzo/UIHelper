public interface IScreen
{
    void Init();
    public bool IsScreenDisplayed { get; }
    void Display(bool useAnims = true);
    void Hide(bool setElementsTransparent, bool useAnims = true);
}
