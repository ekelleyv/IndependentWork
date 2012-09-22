using System;
using System.Collections.Generic;

namespace Motion_Planner
{
//	public struct Auv
//	{
//		//Keeps track of number of nodes
//		public int count;
//		public List<Grid> grid;
//		public List<int> occupied;
//		public List<Waypoint> waypoints;
//		public double x_target;
//		public double y_target;
//	}
	public class Auv {
		public int count;
		private List<Grid> grid;
		private List<int> occupied;
		private List<Waypoint> waypoints;
		public List<Waypoint> output;
		public double x_target;
		public double y_target;
		
		public Auv (int count, double x_target, double y_target) {
			this.count = count;
			this.x_target = x_target;
			this.y_target = y_target;
			grid = new List<Grid>();
			occupied = new List<int>();
			waypoints = new List<Waypoint>();
			
			//Initialize grid
			for (int i = 0; i < MainClass.grid_number; i++) {
				grid[i] = new Grid();
			}
		}
		
		public void add_waypoint (int yaw, double dt, double t_step, double x_est, double y_est, double theta_est, int parent, int index) {
			//Add waypoint
			waypoints.Add (new Waypoint(yaw, dt, t_step,  x_est, y_est, theta_est, parent, index));
			
			//Add to grid
			int grid_number = determine_grid(x_est, y_est);
			if (!occupied.Contains(grid_number)) {
				occupied.Add(grid_number);
				grid[grid_number].new_grid (x_est, y_est);
			}
		}
		public void add_waypoint (Waypoint new_point) {
			//Add waypoint
			waypoints.Add (new_point);
			
			//Add to grid
			int grid_number = determine_grid(new_point.x_est, new_point.y_est);
			if (!occupied.Contains(grid_number)) {
				occupied.Add(grid_number);
				grid[grid_number].new_grid (new_point.x_est, new_point.y_est);
			}
		}
		
		public Waypoint pick_waypoint () {
			Random rand = new Random();
			int rand_grid = rand.Next(0, occupied.Count);
			int rand_waypoint_index = grid[rand_grid].pick_waypoint();
			return waypoints(rand_waypoint_index);
		}
		
		public Waypoint get_waypoint (int waypoint_index) {
			return waypoints(waypoint_index);
		}
		
		
	}
}

