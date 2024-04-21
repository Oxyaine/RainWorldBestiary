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

    internal class OverlappingMenu : Dialog
    {
        private bool opening = false, closing = false;
        private float targetAlpha;

        private readonly IOverlappingMenuOwner owningMenu;

        protected List<MovingObject> MovingObjects = new List<MovingObject>();

        public OverlappingMenu(ProcessManager manager, IOverlappingMenuOwner parentMenu = null) : base(manager)
        {
            opening = true;
            targetAlpha = 1f;
            owningMenu = parentMenu;
        }

        public virtual void CloseMenu()
        {
            opening = false;
            closing = true;
            targetAlpha = 0f;
            owningMenu?.ReturningToThisMenu();
        }

        public void AddMovingObject(PositionedMenuObject @object, Vector2 secondPosition)
        {
            pages[0].subObjects.Add(@object);
            MovingObjects.Add(new MovingObject(@object, @object.pos, secondPosition));
        }
        public void AddMovingObject(PositionedMenuObject @object, Vector2 firstPosition, Vector2 secondPosition)
        {
            @object.pos = firstPosition;
            AddMovingObject(@object, secondPosition);
        }

        private float lastAlpha, currentAlpha, uAlpha;
        public override void Update()
        {
            base.Update();

            lastAlpha = currentAlpha;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10);

            if (opening && currentAlpha >= 0.999f)
            {
                opening = false;
            }
            if (closing && Math.Abs(currentAlpha - targetAlpha) < 0.09f)
            {
                closing = false;
                owningMenu?.ClosingSubMenu();
                manager.StopSideProcess(this);
            }
        }
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            if (opening || closing)
            {
                uAlpha = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastAlpha, currentAlpha, timeStacker)), 1.5f);
                darkSprite.alpha = uAlpha * 0.8f;
            }

            foreach (MovingObject @object in MovingObjects)
                @object.MenuObject.pos = Vector2.Lerp(@object.FirstPosition, @object.SecondPosition, (uAlpha < 0.999f) ? uAlpha : 1f);
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
