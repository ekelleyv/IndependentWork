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
		private List<GridCell> grid;
		private List<int> occupied;
		public List<Waypoint> waypoints;
		public List<Waypoint> output;
        public List<double> output_time;
		public double x_target;
		public double y_target;
        public double path_length;
		
		public Auv (int count, double x_target, double y_target) {
			this.count = count;
			this.x_target = x_target;
			this.y_target = y_target;
			grid = new List<GridCell>();
			occupied = new List<int>();
			waypoints = new List<Waypoint>();
            output = new List<Waypoint>();
            output_time = new List<double>();
			
			//Console.WriteLine("Initializing AUV");
			//Console.WriteLine("Gridnumber = " + MainClass.grid_number);
			
			//Initialize grid
			for (int i = 0; i < MainClass.grid_number; i++) {
				grid.Add (new GridCell());
			}
		}
		
		public void add_waypoint (int yaw, double dt, double t_step, double x_est, double y_est, double theta_est, int parent, int index) {
			//Add waypoint
			waypoints.Add (new Waypoint(yaw, dt, t_step,  x_est, y_est, theta_est, parent, index));
			//Console.WriteLine("Inside Add");
			//Add to grid
			int grid_number = GridCell.determine_grid(x_est, y_est);
			//Console.WriteLine("Grid = " + grid_number);
			if (!occupied.Contains(grid_number)) {
				occupied.Add(grid_number);
			}
			grid[grid_number].add_to_grid(waypoints.Count-1);
		}
		public void add_waypoint (Waypoint new_point) {
			//Add waypoint
			waypoints.Add (new_point);
			
			//Add to grid
			int grid_number = GridCell.determine_grid(new_point.x_est, new_point.y_est);
			if (!occupied.Contains(grid_number)) {
				occupied.Add(grid_number);
			}
			grid[grid_number].add_to_grid(waypoints.Count-1);
		}
		
		public Waypoint pick_waypoint (Random rand) {
			int rand_grid = occupied[rand.Next(0, occupied.Count)];
			//Console.WriteLine("Occupied Size = " + occupied.Count);
			//Console.WriteLine ("Random Grid = " + rand_grid);
			int rand_waypoint_index = grid[rand_grid].pick_waypoint();
			//Console.WriteLine ("Rand Waypoint Index = " + rand_waypoint_index);
			return waypoints[rand_waypoint_index];
		}
		
		public Waypoint get_waypoint (int waypoint_index) {
			return waypoints[waypoint_index];
		}
		
        //public static List<Waypoint> generate_output(int start) {
        //    int count = start;
        //    while (count != 0) {
				
				
				
				
        //    }
				
        //}
		
	}
}

