using Menu;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal interface IOverlappingMenuOwner
    {
        /// <summary>
        /// Called when the submenu starts closing
        /// </summary>
        void ReturningToThisMenu();
        /// <summary>
        /// Called when the submenu is shut down
        /// </summary>
        void ClosingSubMenu();
    }

    internal abstract class OverlappingMenu : Dialog
    {
        protected bool Opening { get; private set; } = false;
        protected bool Closing { get; private set; } = false;
        private float targetAlpha;

        private readonly IOverlappingMenuOwner owningMenu;

        private readonly List<MovingObject> MovingObjects = new List<MovingObject>();

        public OverlappingMenu(ProcessManager manager, IOverlappingMenuOwner parentMenu) : base(manager)
        {
            Opening = true;
            targetAlpha = 1f;
            owningMenu = parentMenu;
        }

        public virtual void CloseMenu()
        {
            PlaySound(SoundID.MENU_Switch_Page_Out);

            Opening = false;
            Closing = true;
            targetAlpha = 0f;
            owningMenu?.ReturningToThisMenu();
        }

        public void AddMovingObject(PositionedMenuObject @object, Vector2 secondPosition)
        {
            AddMovingObject(@object, @object.pos, secondPosition);
        }
        public void AddMovingObject(PositionedMenuObject @object, Vector2 firstPosition, Vector2 secondPosition)
        {
            pages[0].subObjects.Add(@object);
            MovingObjects.Add(new MovingObject(@object, firstPosition, secondPosition));
        }

        public void RemoveMovingObject(PositionedMenuObject @object)
        {
            for (int i = 0; i < MovingObjects.Count; i++)
            {
                if (MovingObjects[i].MenuObject == @object)
                {
                    MovingObjects.RemoveAt(i);
                    break;
                }
            }
        }

        private float lastAlpha, currentAlpha, uAlpha;
        public override void Update()
        {
            base.Update();

            lastAlpha = currentAlpha;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10);

            if (Opening && currentAlpha >= 0.999f)
            {
                Opening = false;
            }
            if (Closing && Math.Abs(currentAlpha - targetAlpha) < 0.09f)
            {
                Closing = false;
                owningMenu?.ClosingSubMenu();
                manager.StopSideProcess(this);
            }
        }
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            if (Opening || Closing)
            {
                uAlpha = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastAlpha, currentAlpha, timeStacker)), 1.5f);
                darkSprite.alpha = uAlpha * 0.8f;

                foreach (MovingObject @object in MovingObjects)
                {
                    if (@object.MenuObject.pos != (Closing ? @object.FirstPosition : @object.SecondPosition))
                    {
                        @object.MenuObject.pos = Vector2.Lerp(@object.FirstPosition, @object.SecondPosition, (uAlpha < 0.999f) ? uAlpha : 1f);
                    }
                }
            }
        }

        public class MovingObject
        {
            public PositionedMenuObject MenuObject;
            public Vector2 FirstPosition, SecondPosition;

            public MovingObject(PositionedMenuObject value, Vector2 firstPosition, Vector2 secondPosition)
            {
                MenuObject = value;
                FirstPosition = firstPosition;
                SecondPosition = secondPosition;
            }
        }
    }
}
