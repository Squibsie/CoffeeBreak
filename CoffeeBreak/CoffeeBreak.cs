using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FivePD.API;
using CitizenFX.Core;
using MenuAPI;


namespace CoffeeBreak
{
    
    public class CoffeeBreak : FivePD.API.Plugin
    {
        Ped player;
        

        Vector3 playerPos;

        float dist;

        int coffeecup;

        public Vector3[] coffeeMachines = new Vector3[6]
        {
            new Vector3(24.82f, -1342.447f, 29.497f),
            new Vector3(-45.865f, -1754.764f, 29.421f),
            new Vector3(126.259f, -1285.655f, 29.284f),
            new Vector3(1162.985f, -319.451f, 69.205f),
            new Vector3(-707.489f, -910.019f, 19.216f),
            new Vector3(373.901f, 330.734f, 103.566f),
        };

        internal CoffeeBreak()
        {
            playerPos = base.LocalPlayer.Character.Position;
            
            CoffeeMenu();
            Tick += DisplayHint;


            foreach (Vector3 loc in coffeeMachines)
            {
                Blip CoffeeLoc = World.CreateBlip(loc);                
                CoffeeLoc.Sprite = BlipSprite.Store;
                CoffeeLoc.Name = "Coffee Machine";                
            }
            
        }

        public void CoffeeMenu()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            Menu menu = new Menu("Coffee Break Menu");
            MenuController.AddMenu(menu);
            player = LocalPlayer.Character;
            
            MenuItem buyCoffee = new MenuItem("Buy Coffee", "Get a piping hot cup of coffee")
            {
                RightIcon = MenuItem.Icon.INV_DOLLAR
            };
            menu.AddMenuItem(buyCoffee);
            MenuItem clear = new MenuItem("Finish Up", "Finish your drink, you've got work to do!")
            {
                RightIcon = MenuItem.Icon.NONE
            };
            menu.AddMenuItem(clear);

            menu.OnItemSelect += delegate (Menu _menu, MenuItem _item, int _index)
            {
                if (_index == 0) //buy coffee
                {
                    coffeecup = CitizenFX.Core.Native.API.CreateObjectNoOffset(3696781377, 0, 0, 0, true, true, true); //Creates Coffee cup

                    CitizenFX.Core.Native.API.AttachEntityToEntity(coffeecup, CitizenFX.Core.Native.API.GetPlayerPed(-1), CitizenFX.Core.Native.API.GetPedBoneIndex(CitizenFX.Core.Native.API.GetPlayerPed(-1), 28422), 0, 0, 0, 0, 0, 0, true, true, false, false, 2, true);

                    AnimationFlags flags = AnimationFlags.Loop;
                    CitizenFX.Core.Native.API.RequestAnimDict("amb@world_human_drinking@coffee@male@idle_a");
                    player.Task.PlayAnimation("amb@world_human_drinking@coffee@male@idle_a", "idle_a", -1 ,-1, flags);

                    uint streetName = 0u;
                    uint crossing = 0u;
                    CitizenFX.Core.Native.API.GetStreetNameAtCoord(LocalPlayer.Character.Position.X, LocalPlayer.Character.Position.Y, LocalPlayer.Character.Position.Z, ref streetName, ref crossing);

                    BaseScript.TriggerServerEvent("PostToDiscord", CitizenFX.Core.Native.API.GetPlayerName(CitizenFX.Core.Native.API.PlayerId()), CitizenFX.Core.Native.API.GetStreetNameFromHashKey(streetName), "REFRESHMENT BREAK");
                    
                }
                if (_index == 1) //clear
                {
                    player.Task.ClearAnimation("amb@world_human_drinking@coffee@male@idle_a", "idle_a");
                    CitizenFX.Core.Native.API.DeleteEntity(ref coffeecup);

                    uint streetName = 0u;
                    uint crossing = 0u;
                    CitizenFX.Core.Native.API.GetStreetNameAtCoord(LocalPlayer.Character.Position.X, LocalPlayer.Character.Position.Y, LocalPlayer.Character.Position.Z, ref streetName, ref crossing);

                    BaseScript.TriggerServerEvent("PostToDiscord", CitizenFX.Core.Native.API.GetPlayerName(CitizenFX.Core.Native.API.PlayerId()), CitizenFX.Core.Native.API.GetStreetNameFromHashKey(streetName), "ON PATROL");

                }
            };
            base.Tick += async delegate
            {
                await Task.FromResult(0);
                if (Game.IsControlJustReleased(1, Control.SelectCharacterFranklin)) //F5
                {
                    foreach(Vector3 pos in coffeeMachines)
                    {
                        playerPos = LocalPlayer.Character.Position;
                        float dist = World.GetDistance(pos, playerPos);

                        if (dist < 8f)
                        {
                            menu.Visible = !menu.Visible;
                        }
                    }
                    
                }
            };
        }

        public async Task DisplayHint()
        {
            foreach (Vector3 pos in coffeeMachines)
            {
                playerPos = LocalPlayer.Character.Position;
                float dist = World.GetDistance(pos, playerPos);

                if (dist < 8f)
                {
                    CitizenFX.Core.Native.API.SetTextComponentFormat("String");
                    CitizenFX.Core.Native.API.AddTextComponentString("Press F6 to open coffee menu...");
                    CitizenFX.Core.Native.API.DisplayHelpTextFromStringLabel(0, false, true, -1);
                }
            }
        }

    }
}
