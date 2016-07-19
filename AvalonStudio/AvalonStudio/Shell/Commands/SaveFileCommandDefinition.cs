namespace AvalonStudio.Shell.Commands
{
	using System;
	using System.ComponentModel.Composition;
	using Avalonia.Controls.Shapes;
	using Avalonia.Input;
	using Avalonia.Media;
	using Extensibility;
	using Extensibility.Commands;
	using ReactiveUI;

	[CommandDefinition]
	public class SaveFileCommandDefinition : CommandDefinition
	{
		public SaveFileCommandDefinition()
		{
			_command = ReactiveCommand.Create();

			_command.Subscribe(_ =>
			{
				var shell = IoC.Get<IShell>();
				shell?.Save();
			});
		}

		public override string Text => "Save";

		public override string ToolTip => "Saves the current changes.";

		public override Path IconPath => new Path() { Fill = Brush.Parse("#FF7AC1FF"), UseLayoutRounding = false, Stretch = Stretch.Uniform, Data = StreamGeometry.Parse("M15,9H5V5H15M12,19A3,3 0 0,1 9,16A3,3 0 0,1 12,13A3,3 0 0,1 15,16A3,3 0 0,1 12,19M17,3H5C3.89,3 3,3.9 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V7L17,3Z") };

		public override Uri IconSource => new Uri("");

		readonly ReactiveCommand<object> _command;
		public override System.Windows.Input.ICommand Command => _command;

		[Export]
		public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<SaveFileCommandDefinition>(new KeyGesture() { Key = Key.S, Modifiers = InputModifiers.Control });
	}
}