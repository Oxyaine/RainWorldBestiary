using Menu;
using System.Collections;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal interface IAnimatableObject
    {
        IEnumerator Animate();
    }
    internal class AnimatableSimpleButton : SimpleButton, IAnimatableObject
    {
        private Vector2 Position2 = Vector2.zero;

        public AnimatableSimpleButton(Menu.Menu menu, MenuObject owner, string displayText, string singalText, Vector2 currentPosition, Vector2 intendedPosition, Vector2 size)
            : base(menu, owner, displayText, singalText, currentPosition, size)
        {
            Position2 = intendedPosition;
        }

        public IEnumerator Animate()
        {
            pos = Position2;
            yield break;
        }
    }
    internal class AnimatableLabel : MenuLabel, IAnimatableObject
    {
        public AnimatableLabel(Menu.Menu menu, MenuObject owner, string text, Vector2 pos, Vector2 size, bool bigText, FTextParams textParameters = null)
            : base(menu, owner, text, pos, size, bigText, textParameters) { }

        public int RevealSpeed = 15;
        public IEnumerator Animate()
        {
            string text = label.text;
            label.text = string.Empty;
            int currentTextIndex = 0, textLength = text.Length;
            while (currentTextIndex + RevealSpeed < textLength)
            {
                label.text += text.Substring(currentTextIndex, RevealSpeed);
                currentTextIndex += RevealSpeed;

                yield return new WaitTime(0.01f);
            }

            label.text += text.Substring(currentTextIndex);
        }
    }
}
