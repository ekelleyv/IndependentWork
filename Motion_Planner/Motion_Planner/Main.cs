using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Motion_Planner
{
		
	class MainClass
	{
		//Constants
		public static double MAX_TPS = 10; //Max degrees per second turn
		public static double VELOCITY = 2; //Velocity in m/s
		public static int MAX_DT = 10; //Max t-step in seconds
		public static int num_auvs = 3;
		public static double grid_width = 10; //in meters
		public static int grid_number = (int)((width/grid_width)*(height/grid_width));
		public static double width = 200;
		public static double height = 200;
		public static char[] colors = new char[] {'r', 'g', 'b'};
		public static List<Auv> auvs = new List<Auv>();
		
		public static Waypoint sim_motion(Waypoint rand_point, double yaw_rand, double dt_rand, double t_step, int count) {
			double v = VELOCITY;
			
			double d_theta = (yaw_rand-128.0)*MAX_TPS/128.0;
			double theta_est = rand_point.theta_est + d_theta*dt_rand;
			double x_est = rand_point.x_est + dt_rand*v*Math.Cos(theta_est*Math.PI/180);
			double y_est = rand_point.y_est + dt_rand*v*Math.Sin(theta_est*Math.PI/180); 
			
			Waypoint new_waypoint = new Waypoint((int)yaw_rand, dt_rand, t_step, x_est, y_est, theta_est, rand_point.index, count);
			
			return new_waypoint;
		}
		
		public static void Main (string[] args)
		{
			
			//Initialize obstacle array
			List<Obstacle> obstacles = new List<Obstacle>();
			
			//Initialize auvs		
			for (int i = 0; i < num_auvs; i++) {
				//Declare new auv and first waypoint
				int j = num_auvs - i;
				Auv current = new Auv(i, 180, j*40 + 20);
				current.add_waypoint(128, 0, 0, 20, (i+1)*40 + 20, 0, -1, 0);
				
				//Add current auv to auvs
				auvs.Add(current);
				
				//Plot Data
				
			}
			
			//Define obstacles
			List<Point> obstaclepoints = new List<Point>();
			obstaclepoints.Add(new Point(50, 50));
			obstaclepoints.Add(new Point(50, 75));
			obstaclepoints.Add(new Point(75, 75));
			obstaclepoints.Add(new Point(75, 50));
			obstacles.Add(new Obstacle(obstaclepoints));
			
			//Prob Roadmap
			for (int a = 0; a < auvs.Count; a++) {
				//Start with next waypoint after start
				int count = 1;
				int last_point = 0;
				
				//Create current line to target
				List<Point> current_line = new List<Point>();
				current_line.Add (new Point(auvs[a].x_target, auvs[a].y_target));
				current_line.Add (new Point(auvs[a].get_waypoint(last_point).x_est, auvs[a].get_waypoint(last_point).y_est));
				
				//While there is no path to target, keep adding to tree
				while (intersect.intersect_obstacles(obstacles, current_line) || !intersect.is_reachable(last_point, auvs[a])) {
					
					Random rand = new Random();
					int yaw_rand = rand.Next(0, 255);
					
					//Change to double?
					int dt_rand = rand.Next(0, MAX_DT);
					
					Waypoint rand_point = auvs[a].pick_waypoint();
					
					double prev_t = rand_point.t_step;
					double t_step = prev_t + dt_rand;
					
					Waypoint new_point = sim_motion(rand_point, yaw_rand, dt_rand, t_step, count);
					
					List<Point> new_line = new List<Point>();
					new_line.Add (new Point(rand_point.x_est, rand_point.y_est));
					new_line.Add (new Point(new_point.x_est, new_point.y_est));
					
					if (!intersect.intersect_obstacles(obstacles, new_line) && !intersect.intersects_path(auvs[a], rand_point.index, new_point.x_est, new_point.y_est, rand_point.t_step, dt_rand)) {
						auvs[a].add_waypoint(new_point);
						last_point = count;
						count++;
					}	
				}
				
				//Create output
				
			}
			
		}
		
		
	}
}
