using Menu;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary.Plugins
{
    internal class ErrorManager : Dialog
    {
        private readonly string BackButtonMessage = "BACK";

        internal static ProcessManager.ProcessID BestiaryErrorMenu => new ProcessManager.ProcessID("BestiaryErrorMenu", register: true);

        private static readonly List<ErrorCategory> ErrorCategories = new List<ErrorCategory>()
        {
            ErrorCategory.Default
        };

        public ErrorManager(ProcessManager manager) : base(manager)
        {
            scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.SubBackground);
            pages[0].subObjects.Add(scene);

            darkSprite = new FSprite("pixel")
            {
                color = new Color(0f, 0f, 0f),
                anchorX = 0f,
                anchorY = 0f,
                scaleX = 1368f,
                scaleY = 770f,
                x = -1f,
                y = -1f,
                alpha = 0.90f
            };
            pages[0].Container.AddChild(darkSprite);

            Vector2 screenSize = manager.rainWorld.screenSize;

            FSprite titleSprite = new FSprite("illustrations\\bestiary\\titles\\Bestiary_Errors_Title")
            {
                scale = 0.3f,
                x = screenSize.x / 2f,
                y = screenSize.y - 25f
            };
            pages[0].Container.AddChild(titleSprite);

            float YPos = screenSize.y - 50;
            const float SpacingBetweenLines = 20f;

            foreach (ErrorCategory category in ErrorCategories)
            {
                MenuLabel catLabel = new MenuLabel(this, pages[0], category.Category, new Vector2(20f + (category.Category.Length * 5f), YPos), Vector2.one, true);
                pages[0].subObjects.Add(catLabel);

                YPos -= SpacingBetweenLines + 10f;

                if (!string.IsNullOrEmpty(category.Message))
                {
                    MenuLabel catMessage = new MenuLabel(this, pages[0], category.Message, new Vector2(25f + (category.Message.Length * 3.05f), YPos), Vector2.one, false);
                    pages[0].subObjects.Add(catMessage);

                    YPos -= SpacingBetweenLines;
                }

                category.SortErrors();
                foreach (Error error in category.Errors)
                {
                    FSprite sprite = new FSprite(GetSpriteName(error.Level))
                    {
                        y = YPos,
                        x = 20f
                    };
                    MenuLabel label = new MenuLabel(this, pages[0], error.Message, new Vector2(25f + (error.Message.Length * 3.05f), YPos), Vector2.one, false);

                    pages[0].Container.AddChild(sprite);
                    pages[0].subObjects.Add(label);

                    YPos -= SpacingBetweenLines;
                }
            }

            SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(screenSize.x - 235f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);
            backObject = backButton;
            backButton.nextSelectable[0] = backButton;
            backButton.nextSelectable[2] = backButton;

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Singal(backObject, BackButtonMessage);

            base.Update();
        }
        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
                return;
            }
        }

        public static void AddError(Error error)
        {
            ErrorCategories[0].Errors.Add(error);
        }
        public static void AddError(string error, ErrorLevel level = ErrorLevel.Unknown)
        {
            ErrorCategories[0].Errors.Add(new Error(error, level));
        }
        public static void AddError(string error, ErrorCategory category, ErrorLevel level = ErrorLevel.Unknown)
        {
            for (int i = 0; i < ErrorCategories.Count; i++)
                if (ErrorCategories[i].Equals(category))
                {
                    ErrorCategories[i].Errors.Add(new Error(error, level));
                    return;
                }

            ErrorCategories.Add(category);
            ErrorCategories[ErrorCategories.Count - 1].Errors.Add(new Error(error, level));
        }

        public static bool HasErrors()
        {
            foreach (ErrorCategory category in ErrorCategories)
            {
                if (category.Errors.Count > 0)
                    return true;
            }

            return false;
        }

        public static ErrorLevel GetHighestErrorLevel()
        {
            ErrorLevel level = ErrorLevel.Unknown;

            foreach (ErrorCategory error in ErrorCategories)
            {
                ErrorLevel cache = error.GetHighestErrorLevel();
                if (cache > level)
                    level = cache;
            }

            return level;
        }
        public static string GetSpriteName(ErrorLevel level)
        {
            switch (level)
            {
                case ErrorLevel.Low:
                    return "illustrations\\bestiary\\icons\\warning-low";
                case ErrorLevel.Medium:
                    return "illustrations\\bestiary\\icons\\warning-medium";
                case ErrorLevel.High:
                    return "illustrations\\bestiary\\icons\\warning-high";
                case ErrorLevel.Fatal:
                    return "illustrations\\bestiary\\icons\\error";
                case ErrorLevel.Unknown:
                default:
                    return "illustrations\\bestiary\\icons\\warning-unknown";
            }
        }
    }

    internal enum ErrorLevel
    {
        Unknown,
        Low,
        Medium,
        High,
        Fatal
    }
    internal class Error : IComparable<Error>
    {
        public string Message;
        public ErrorLevel Level = ErrorLevel.Unknown;

        public Error(string message, ErrorLevel level = ErrorLevel.Unknown)
        {
            Message = message;
            Level = level;
        }

        public int CompareTo(Error other) => Level.CompareTo(other.Level);
    }
    internal class ErrorCategory : IEquatable<ErrorCategory>
    {
        public string Category = string.Empty;
        public string Message = string.Empty;

        public List<Error> Errors = new List<Error>();

        private ErrorCategory() { }
        private ErrorCategory(string category, string message = "") : this()
        {
            Category = category;
            Message = message;
        }

        public ErrorLevel GetHighestErrorLevel()
        {
            ErrorLevel level = ErrorLevel.Unknown;

            foreach (Error error in Errors)
                if (error.Level > level)
                    level = error.Level;

            return level;
        }

        public static readonly ErrorCategory Default = new ErrorCategory();
        public static readonly ErrorCategory CreatureHookFailed = new ErrorCategory("Failed Creature Hooks", "The following interactions will not be recorded as there was a problem creating the necessary hooks.");
        public static readonly ErrorCategory PluginLoadingFailed = new ErrorCategory("Failed Plugins", "The following plugins had an issue while loading, you can check the logs for more details.");

        public void SortErrors() => Errors.Sort();
        public int CompareTo(ErrorCategory other) => Category.CompareTo(other.Category);

        public bool Equals(ErrorCategory other) => Category.Equals(other.Category);
    }
}
