namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public interface IGameManager
    {
        void ShowMenu();
        void ShowDeathScreen();
        void HideDeathScreen();
        void ShowWonScreen();
        void HideWonScreen();
        void SetDeathReason(string reason);
    }
}
