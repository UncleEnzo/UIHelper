namespace Nevelson.UIHelper
{
    public interface IManageScreens
    {
        void ChangeToNextScreen(UIScreenBase nextScreen);
        void ChangeToNextScreenNoAnim(UIScreenBase nextScreen);
    }
}