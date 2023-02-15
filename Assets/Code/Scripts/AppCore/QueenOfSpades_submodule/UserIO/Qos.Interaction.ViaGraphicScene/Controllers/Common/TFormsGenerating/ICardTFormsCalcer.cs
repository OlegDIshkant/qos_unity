using CommonTools.Math;
using static CommonTools.Math.NDCPointToPlaneSolver;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    public interface ITFormsCalcer
    {
        /// <summary>
        /// Пересчитать положения объектов.
        /// </summary>
        Transform[] Calc(int objectsAmmount);
    }



    public interface IWithCamParams : ITFormsCalcer
    {
        CameraParams CamParams { get; set; }
    }


    public interface IWithPlayerTForm : ITFormsCalcer
    {
        void ChangePlayerTForm(Transform playerTForm);
    }




    public interface IWithTwoPlayerTForms : ITFormsCalcer
    {
        void ChangeFirstPlayerTForm(Transform playerTForm);
        void ChangeSecondPlayerTForm(Transform playerTForm);
    }



    public interface IWithCamParamsAndPlayerTForm : IWithCamParams, IWithPlayerTForm
    {
    }


}
