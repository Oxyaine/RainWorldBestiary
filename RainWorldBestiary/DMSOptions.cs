namespace RainWorldBestiary
{
    public class DMSOptions : OptionInterface
    {
        public static readonly DMSOptions Instance = new DMSOptions();

        public readonly Configurable<bool> LoadInactiveMods;

        public DMSOptions()
        {
            LoadInactiveMods = config.Bind("LoadInactiveMods", defaultValue: false);
        }

        public override string ValidationString()
        {
            string text = base.ValidationString();
            Configurable<bool> loadInactiveMods = LoadInactiveMods;
            return text + ((loadInactiveMods != null && loadInactiveMods.Value) ? " LI" : "");
        }

        public override void Initialize()
        {

        }
    }


    //public class PictureButton : SymbolButton
    //{
    //    public PictureButton(Menu.Menu menu, MenuObject owner, string symbolName, string singalText, Vector2 pos, Vector3 size)
    //        : base(menu, owner, symbolName, singalText, pos)
    //    {
    //        this.size = size;
    //        new FSprite(new FAtlas())
    //    }

    //    public override void GrafUpdate(float timeStacker)
    //    {
    //        base.GrafUpdate(timeStacker);
    //        float num = 0.5f - 0.5f * Mathf.Sin(Mathf.Lerp(buttonBehav.lastSin, buttonBehav.sin, timeStacker) / 30f * (float)Math.PI * 2f);
    //        num *= buttonBehav.sizeBump;
    //        symbolSprite.color = (buttonBehav.greyedOut ? Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey) : Color.Lerp(base.MyColor(timeStacker), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.VeryDarkGrey), num));
    //        symbolSprite.x = DrawX(timeStacker) + DrawSize(timeStacker).x / 2f;
    //        symbolSprite.y = DrawY(timeStacker) + DrawSize(timeStacker).y / 2f;
    //        Color color = Color.Lerp(Menu.Menu.MenuRGB(Menu.Menu.MenuColors.Black), Menu.Menu.MenuRGB(Menu.Menu.MenuColors.White), Mathf.Lerp(buttonBehav.lastFlash, buttonBehav.flash, timeStacker));
    //        for (int i = 0; i < 9; i++)
    //        {
    //            roundedRect.sprites[i].color = color;
    //        }
    //    }

    //    public void UpdateSymbol(string newSymbolName)
    //    {
    //        symbolSprite.element = Futile.atlasManager.GetElementWithName(newSymbolName);
    //    }

    //    public override void RemoveSprites()
    //    {
    //        symbolSprite.RemoveFromContainer();
    //        base.RemoveSprites();
    //    }

    //    public override void Clicked()
    //    {
    //        Singal(this, signalText);
    //    }
    //}
}
