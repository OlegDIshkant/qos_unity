using UnityEngine;


namespace Utils
{
    /// <summary>
    /// ��������� ��������� �� ���� Unity3D-��������.
    /// </summary>
    public class CoroutinesHelper : MonoBehaviour
    {
        private static CoroutinesHelper p_coroutinesRunner;

        public static CoroutinesHelper CoroutinesRunner
        {
            get
            {
                if (p_coroutinesRunner == null)
                {
                    p_coroutinesRunner = GameObject.Find("Coroutines Keeper")?.GetComponent<CoroutinesHelper>();
                }
                return p_coroutinesRunner;
            }
        }
    }
}