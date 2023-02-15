using System;


namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{

    public interface IDiscModeUi_ReadOnly
    {

    }


    public interface IDiscModeUi : IDiscModeUi_ReadOnly
    {
        IPreDiscModeControl EnablePreDiscModeControls();
        IDiscModeControl EnableDiscModeControls();
    }


    public interface IUiControl<T> : IDisposable
    {
        event Action<T> OnResultKnown;

        T FinalResult { get; }
    }



    public enum PreDiscModeControlResult { NONE, GO_TO_DISC_MODE, WONT_GO_DISC_MODE }

    public interface IPreDiscModeControl : IUiControl<PreDiscModeControlResult>
    {
    }



    public enum DiscModeControlResult { NONE, CONFIRM_CARDS, LEAVE_DISC_MODE }

    public interface IDiscModeControl : IUiControl<DiscModeControlResult>
    {
        bool ConfirmButtonEnabled { get; set; }
    }
}
