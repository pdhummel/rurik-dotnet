using rurik;


var game = new RurikMonoGame();
Globals.Log("Game starting");
try
{
    game.Run();
}
catch(ObjectDisposedException eIgnore)
{
}
Globals.Log("Game exited");
