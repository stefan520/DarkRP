using GameSystems.Player;

namespace GameSystems.Jobs
{
	public class JobSystem : Component
	{
		public Dictionary<string, JobResource> Jobs { get; private set; } = new();
		public Dictionary<string, JobGroupResource> JobGroups { get; private set; } = new();

		// On Start load all jobs
		public JobSystem()
		{
			Log.Info( "Loading groups..." );
			// Get all JobGroup resources
			foreach ( var group in ResourceLibrary.GetAll<JobGroupResource>( "data/jobs/groups" ) )
			{
				Log.Info( $"Loading group: {group.Name}" );
				JobGroups[group.Name] = group;
			}

			Log.Info( "Loading jobs..." );
			// Get all Job resources
			foreach ( var job in ResourceLibrary.GetAll<JobResource>( "data/jobs" ) )
			{
				Log.Info( $"Loading job: {job.Name}" );
				Jobs[job.Name] = job;
			}
		}

		// Get default job when player spawns
		public static JobResource GetDefault()
		{
			return ResourceLibrary.Get<JobResource>( "data/jobs/citizen.job" );
		}

		// Set selected job
		public void SelectJob( JobResource job, GameObject Player )
		{
			// Get all players in the game
			var players = GameController.Instance.GetAllPlayers();

			// Check how many players have this job
			var count = 0;
			foreach ( var player in players )
			{
				if ( player.Value.GameObject.Components.Get<Stats>().Job == job )
				{
					count++;
				}
			}

			Log.Info( $"Count: {count}" );

			// Compare the number of players having this job with the MaxWorkers and there is a possitive value on MaxWorkers
			// If MaxWorkers is set to -1 then there is no limit 
			if ( count >= job.MaxWorkers && job.MaxWorkers >= 0 )
			{
				Player.Components.Get<Stats>().SendMessage( $"We've reached the maximum amount of workers ({job.MaxWorkers})" );
				return;
			}

			// Set player job
			Player.Components.Get<Stats>().SelectJob( job );
		}
	}
}
