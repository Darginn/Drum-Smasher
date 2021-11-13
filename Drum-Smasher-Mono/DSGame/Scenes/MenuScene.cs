using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace Drum_Smasher_Mono.Scenes
{
    public class MenuScene : Scene
    {
        Entity _uiEntity;

        public MenuScene() : base()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            ClearColor = Color.Black;

            SetDesignResolution(800, 800, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(800, 800);

            _uiEntity = new Entity("UIEntity");
            UICanvas canvas = _uiEntity.AddComponent<UICanvas>();
            Table table = canvas.Stage.AddElement(new Table());
            Label title = table.AddElement(new Label("Drum-Smasher"));
            title.SetFontScale(3);

            title.SetPosition(800.PercentageOf(45), 800.PercentageOf(10));

            Entities.Add(_uiEntity);
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
