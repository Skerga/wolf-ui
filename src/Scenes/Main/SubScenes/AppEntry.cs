using Godot;
using WolfManagement.Resources;

namespace UI
{
	[GlobalClass][Tool]
	public partial class AppEntry : Button
	{
		[Export]
		ProgressBar AppProgress;
		[Export]
		Label AppLabel;
		[Export]
		public Control DownloadIcon;
		[Export]
		PackedScene AppMenu;

		public Image AppImage;
		public string Title;
		public WolfApp wolfApp;
		public bool ImageOnDisc = true;

		private DockerController docker;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if(Engine.IsEditorHint())
			{
				return;
			}

			var Main = GetNode<Main>("/root/Main");
			docker = Main.docker;
			
			SetIcon();
			AppLabel.Text = Title;
			DownloadIcon.Visible = false;
			AppProgress.Hide();
			Pressed+= OnPressed;
		}

		private void OnPressed()
		{
			if(!ImageOnDisc)
			{
				PullImage();
			} else {
				CenterContainer scene = AppMenu.Instantiate<CenterContainer>();
				if(scene is AppMenu menu)
				{
					AddChild(menu);
					SetProcess(false);
				}
			}
		}

		public async void PullImage()
		{
			if(wolfApp.Runner.Image.Contains(':'))
			{
				var image = wolfApp.Runner.Image.Split(":");
				await docker.PullImage(image[0], image[1], AppProgress, this);
			}
		}

		private void SetChildRefrence(Node parent)
		{
			foreach(Node child in parent.GetChildren())
			{
				if(child is ProgressBar c)
					AppProgress = c;

				if(child is Label b)
					AppLabel = b;

				SetChildRefrence(child);
			}
		}

		public void SetIcon()
		{
			if(AppImage != null)
			{
				var texture = ImageTexture.CreateFromImage(AppImage);
				Icon = texture;
			}
		}

		public string GetFocusPath()
		{
			return GetPath();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}