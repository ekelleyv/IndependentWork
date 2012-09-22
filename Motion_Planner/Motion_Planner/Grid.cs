using System;
using System.Collections.Generic;

namespace Motion_Planner
{
	
	public class Grid {
		private bool occupied;
		private List<int> waypoint_index;		
		public Grid () {
			occupied = false;
			waypoint_index = new List<int>();
		}
		public void new_grid (double x, double y) {
			occupied = true;
			int grid_number = determine_grid (x, y);
			waypoint_index.Add(grid_number);
		}
		
		public int pick_waypoint() {
			Random random = new Random();
			int rand_waypoint = random.Next (0, waypoint_index.Count);
			return waypoint_index[rand_waypoint];
		}
		
		public static int determine_grid(double x, double y) {
			int y_grid = (int)(y/MainClass.grid_width);
			int x_grid = (int)(x/MainClass.grid_width);
			int number_columns = (int)(MainClass.width/MainClass.grid_width);
			
			int grid_index = (y_grid*number_columns) + x_grid;
			return grid_index;
			
		}
	}
}

