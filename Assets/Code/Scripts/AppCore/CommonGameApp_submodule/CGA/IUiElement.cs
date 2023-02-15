using System;


namespace CGA.MainMenu.GUI
{
    /// <summary>
    /// ����������� ������� Ui.
    /// </summary>
    public interface IUiElement : IDisposable
    {
        event Action<IUiElement> OnPositionChanged;

        /// <summary>
        /// ������� �� ������ � ������������ ������� ��������� (�� 0 �� 1).
        /// ����� ��������� (0,0) � ������ ����� ���� ������. 
        /// ��� Y ������� �����.
        /// </summary>
        Vector2D Position { get; set; }
        /// <summary>
        /// ������ �� ������ (������, ������) � ������������ ������� ��������� (�� 0 �� 1).
        /// </summary>
        Vector2D Size { get; set; }
        bool LocalyHidden { get; set; }

    }

}