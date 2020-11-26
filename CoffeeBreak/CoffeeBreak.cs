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

        public Vector3[] coffeeMachines = new Vector3[2]
        {
            new Vector3(24.82f, -1342.447f, 29.497f),
            new Vector3(-45.865f, -1754.764f, 29.421f)
        };

        public Vector3[] coffeeLocations = new Vector3[1]
        {
            new Vector3(29.085f, -1350.303f, 29.332f)
        };


        internal CoffeeBreak()
        {
            playerPos = base.LocalPlayer.Character.Position;
            
            CoffeeMenu();


            foreach (Vector3 loc in coffeeLocations)
            {
                Blip CoffeeLoc = World.CreateBlip(loc);                
                CoffeeLoc.Sprite = BlipSprite.Business;
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
            MenuItem clear = new MenuItem("Clear Anim", "Finish your coffee, you've got work to do!")
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

                    
                }
                if (_index == 1) //clear
                {
                    player.Task.ClearAnimation("amb@world_human_drinking@coffee@male@idle_a", "idle_a");
                    CitizenFX.Core.Native.API.DeleteEntity(ref coffeecup);

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

                        if (dist < 10f)
                        {
                            menu.Visible = !menu.Visible;
                        }
                    }
                    
                }
            };
        }

    }
}
