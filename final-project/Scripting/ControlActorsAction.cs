using System.Collections.Generic;
using Final_Project.Casting;
using Final_Project.Services;
using Final_Project.Interactables;
using Final_Project.GameFlow;
using System;

namespace Final_Project.Scripting
{
    public class ControlActorsAction : Action
    {
        InputService _inputService = new InputService();
        PhysicsService _physicsService = new PhysicsService();
        public int currentRoom = 1;
        Room roomObject = new Room();

        public ControlActorsAction(InputService inputService, PhysicsService physicsService)
        {
            _inputService = inputService;
            _physicsService = physicsService;
            roomObject = new Room();
        }
        
        public override void Execute(Dictionary<string, List<Actor>> cast)
        {
            Player p = (Player)cast["player"][0];
            Lever l = (Lever)cast["levers"][0];
            Door d = (Door)cast["doors"][0];
            //Gravity movement
            p.SetVelocity(new Point(p.GetVelocity().GetX(),p.GetVelocity().GetY()+p.GravityModifier));
            //User input for left/right
            Point direction = _inputService.GetDirection();
            direction = new Point(direction.GetX()*6,p.GetVelocity().GetY());
            p.SetVelocity(direction);
            //Lever Delay so it doesn't keep flipping
            if (l.delay > 0)
            {
                l.delay--;
            }
            //Jump input
            if (_inputService.IsUpPressed() && p.CanJump)
            {
                p.Jump();
            }
            //Interacting with objects input
            if (_inputService.IsDownPressed())
            {
                //Handle Moving Through Doors
                if (_physicsService.IsCollision(d,p) && d.isUnlocked)
                {
                    currentRoom++;
                    switch (currentRoom)
                    {
                        case 1:
                            //Do stuff
                            break;
                        case 2:
                            p.SetPosition(new Point(Constants.MAX_X/2, Constants.MAX_Y-200));
                            break;
                        case 3:
                            p.SetPosition(new Point(120, Constants.MAX_Y-200));
                            break;
                        case 4:
                            p.SetPosition(new Point(Constants.MAX_X/2, Constants.MAX_Y-200));
                            break;
                        case 5:
                            //Do Stuff, idk
                            break;
                        case 6:
                            //Do stuff for enemy + gravity levers
                            break;
                        default:
                            //Say something about the game being over
                            break;
                    }
                    cast["room"].Clear();
                    cast["doors"].Clear();
                    cast["levers"].Clear();
                    cast["spikes"].Clear();
                    foreach (Actor actor in roomObject.rooms[$"room{currentRoom}"])
                    {
                        cast["room"].Add(actor);
                    }
                    foreach (Actor door in roomObject.rooms[$"doors{currentRoom}"])
                    {
                        cast["doors"].Add(door);
                    }
                    foreach (Actor lever in roomObject.rooms[$"levers{currentRoom}"])
                    {
                        cast["levers"].Add(lever);
                    }
                    foreach (Actor spike in roomObject.rooms[$"spikes{currentRoom}"])
                    {
                        cast["levers"].Add(spike);
                    }
                }
                //Handle using levers
                if (_physicsService.IsCollision(p, l) && l.delay == 0)
                {
                    if (l.powerOn)
                    {
                        l.flipState();
                        l.delay+=10;
                        switch (currentRoom)
                        {
                            case 2:
                                d.lockDoor();
                                break;
                            case 3:
                                d.lockDoor();
                                break;
                            case 4:
                                p.GravityModifier*= -1;
                                break;
                            
                        }
                    }
                    else 
                    {
                        l.flipState();
                        l.delay+=10;
                        switch (currentRoom)
                        {
                            case 2:
                                d.unlockDoor();
                                break;
                            case 3:
                                d.unlockDoor();
                                break;
                            case 4:
                                p.GravityModifier*= -1;
                                break;
                            
                        }
                    }
                }
            }
        }
    }
}