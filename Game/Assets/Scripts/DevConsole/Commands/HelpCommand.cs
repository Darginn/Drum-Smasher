namespace Assets.Scripts.DevConsole.Commands
{
    public class HelpCommand : BaseCommand
    {
        public override string Command => "help";

        public override void Execute(params string[] args)
        {
            foreach(BaseCommand cmd in DevConsole.Commands.Commands.Values)
            {
                DevConsole.WriteLine(cmd.Command);
            }
        }
    }
}
