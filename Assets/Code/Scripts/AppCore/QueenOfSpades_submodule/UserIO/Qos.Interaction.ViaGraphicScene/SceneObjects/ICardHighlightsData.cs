


namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    public class HighlightSettings
    {
        public HighlightType SelectHighlight { get; private set; }
        public HighlightType OverlayHighlight { get; private set; }


        public HighlightSettings(HighlightType selectHighlight, HighlightType overlayHighlight)
        {
            SelectHighlight = selectHighlight;
            OverlayHighlight = overlayHighlight;
        }
    }


    public enum HighlightType
    {
        LIGHT,
        HEAVY
    }

    public interface IHighlightsData
    {
        //TODO: �������� ���������� ����, ������� ��������� �������������� ��� ���� ��� ������������ ������.
        HighlightType Highlight { get; set; }
    }


    public struct HighlightsData : IHighlightsData
    {
        public HighlightType Highlight { get; set; }
    }
}