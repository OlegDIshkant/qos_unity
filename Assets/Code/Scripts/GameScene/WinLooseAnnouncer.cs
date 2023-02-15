using GUI;
using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System;
using System.Collections;
using UnityEngine;
using Utils;


namespace GameScene
{
    public class WinLooseAnnouncer : IWinLooseAnnouncer
    {
        private readonly static float ANNOUNCE_SPEED = 0.5f;

        private UnityGui _gui;


        public WinLooseAnnouncer()
        {
            _gui = new UnityGui("Win Loose Announcer");
        }


        public void LaunchLooseAnnouncement()
        {
            CoroutinesHelper.CoroutinesRunner.StartCoroutine(ShowingAnnouncement(win: false));            
        }


        public void LaunchWinAnnouncement()
        {
            CoroutinesHelper.CoroutinesRunner.StartCoroutine(ShowingAnnouncement(win: true));
        }


        private IEnumerator ShowingAnnouncement(bool win)
        {
            var startSize = 0.1f;
            var sizeDelta = 0.2f;

            var image = _gui.CreateImage();
            image.SetImage(win ? "announcements/win" : "announcements/lose");
            image.Position = new CGA.Vector2D(0.5f, 0.5f);
            image.Size = new CGA.Vector2D(startSize, startSize);
            image.Alpha = 0;

            for (var t = 0.0f; t <= 1f; t += Time.deltaTime * ANNOUNCE_SPEED)
            {
                var size = startSize + sizeDelta * t;
                image.Size = new CGA.Vector2D(size, size);
                image.Alpha = -(float)Math.Pow(2 * t - 1, 6) + 1;

                yield return null;
            }

            image.Dispose();
        }
    }
}