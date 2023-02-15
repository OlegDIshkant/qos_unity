

namespace CGA.MainMenu.GUI
{
    /// <summary>
    /// ����� ����������� �� Ui.
    /// </summary>
    public interface IImage : IUiElement
    {
        /// <param name="imageKey">
        /// ������������� (!) ���� � �����������. (���������� ���� ������� �� ���������� �������������.)
        /// </param>
        void SetImage(string imageKey);

        float Alpha { get; set; }
    }
}