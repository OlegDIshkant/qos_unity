using System.Collections;
using System.Collections.Generic;


namespace Qos.Interaction.ViaGraphicScene.Controllers
{
    /// <summary>
    /// �������� � �������� ������������� �����������, ��������� �� ����� � ������� ����������.
    /// </summary>
    public class ControllersQueue
    {
        private Dictionary<string, AbstractController> _controllers = new Dictionary<string, AbstractController>();
        private Queue<AbstractController> _queue = new Queue<AbstractController>();

        public AbstractController this[string tag]
        {
            get { return _controllers[tag]; }
        }

        public T Get<T>(string tag, object tagSuffix)
        {
            return (T)(object)_controllers[TagWithSuffix(tag, tagSuffix)];
        }


        /// <summary>
        /// ��������� ���������� � ������� ������ � �����, �� �������� ����� ����� �������� ����������� ����������.
        /// </summary>
        public T AddWithTag<T>(string tag, T c)
            where T : AbstractController
        {
            _controllers.Add(tag, c);
            _queue.Enqueue(c);
            return c;
        }


        /// <summary>
        /// ��������� ���������� � ������� ��� ���� (����� ������ ����� �������� ������������ ���������� �� ����).
        /// </summary>
        public T AddWithoutTag<T>(T c)
            where T : AbstractController
        {
            _queue.Enqueue(c);
            return c;
        }



        /// <summary>
        /// ��������� ���������� � ������� ������ � �����, �� �������� ����� ����� �������� ����������� ����������.
        /// </summary>
        /// <param name="tagSuffix"> ��������, ������� ����� ��������� � ����. ��� ������� ���� � ��� ��������� ������������ � ������� �����, ������� �� ������ �� ���������. </param>
        public T AddWithTag<T>(string tag, object tagSuffix, T c)
            where T : AbstractController
        {
            _controllers.Add(TagWithSuffix(tag, tagSuffix), c);
            _queue.Enqueue(c);
            return c;
        }

        private string TagWithSuffix(string tag, object tagSuffix) => tag + "_" + tagSuffix;


        public IEnumerable<AbstractController> GetControllersInExecOrder() => new Queue<AbstractController>(_queue);
    }
}