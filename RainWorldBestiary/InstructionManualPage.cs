using Menu;
using Menu.Remix.MixedUI;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class InstructionManualPage : ManualPage
    {
        public FSprite headingSeparator;

        public readonly int rectHeight = 550;

        public readonly int rectWidth = 600;

        public Vector2 belowHeaderPos;

        public Vector2 belowHeaderPosCentered;

        public float textWidth;

        public float textLeftMargin;

        public float spaceBuffer;

        public Vector2 verticalBuffer;

        public InstructionManualPage(Menu.Menu menu, MenuObject owner)
            : base(menu, owner)
        {
            InstructionManualDialog instructionMenu = menu as InstructionManualDialog;

            topicName = menu.Translate(instructionMenu.TopicName(instructionMenu.currentTopic));
            MenuLabel item = new MenuLabel(menu, owner, topicName, new Vector2(15f + instructionMenu.contentOffX, 475f), Vector2.one, bigText: true)
            {
                label =
            {
                alignment = FLabelAlignment.Left
            }
            };
            subObjects.Add(item);
            headingSeparator = new FSprite("pixel")
            {
                scaleX = 594f,
                scaleY = 2f,
                color = new Color(0.7f, 0.7f, 0.7f)
            };
            Container.AddChild(headingSeparator);
            belowHeaderPos = new Vector2(-2f + instructionMenu.contentOffX, 448f);
            belowHeaderPosCentered = new Vector2(rectWidth / 2f + instructionMenu.contentOffX, 448f);
            textWidth = rectWidth * 0.92f;
            textLeftMargin = (rectWidth - textWidth) / 2f + instructionMenu.contentOffX;
            spaceBuffer = 40f;
            if (menu.CurrLang == InGameTranslator.LanguageID.Spanish)
            {
                spaceBuffer *= 0.9f;
            }

            verticalBuffer = new Vector2(0f, spaceBuffer);
        }

        public float AddManualText(string text, float startY, bool bigText = true, bool centered = true, float? customTextWidth = null)
        {
            float num = customTextWidth ?? textWidth;
            float num2 = (rectWidth - num) / 2f;
            float num3 = 0f;
            string[] array = Regex.Split(text.WrapText(bigText, num), "\n");
            for (int i = 0; i < array.Length; i++)
            {
                num3 = startY - 25f * i;
                MenuLabel menuLabel = new MenuLabel(menu, owner, array[i], new Vector2(centered ? (num2 + num * 0.5f) : num2, num3), default, bigText);
                menuLabel.label.SetAnchor(centered ? 0.5f : 0f, 0.5f);
                menuLabel.label.color = new Color(0.7f, 0.7f, 0.7f);
                subObjects.Add(menuLabel);
            }

            return num3 - 5f - LabelTest.LineHeight(bigText);
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            headingSeparator.x = page.pos.x + 295f + (menu as InstructionManualDialog).contentOffX;
            headingSeparator.y = page.pos.y + 450f;
        }

        public override void RemoveSprites()
        {
            base.RemoveSprites();
            headingSeparator.RemoveFromContainer();
        }
    }
}