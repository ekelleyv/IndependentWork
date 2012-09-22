using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Motion_Planner
{
		
	class MainClass
	{
		//Constants
		public static double MAX_TPS = 10; //Max degrees per second turn
		public static double VELOCITY = 1; //Velocity in m/s
		public static int MAX_DT = 10; //Max t-step in seconds
        public static int num_auvs;
		public static double grid_width = 20; //in meters
		public static int width = 200;
		public static int height = 200;
		public static int grid_number = (int)((width/grid_width)*(height/grid_width));
		public static char[] colors = new char[] {'r', 'g', 'b'};
		public static List<Auv> auvs = new List<Auv>();
        public static List<Auv> curr_auvs = new List<Auv>();
        public static List<Obstacle> obstacles = new List<Obstacle>();
        public static double max_length = double.MaxValue;
		
		public static Waypoint sim_motion(Waypoint rand_point, double yaw_rand, double dt_rand, double t_step, int count) {
			double v = VELOCITY;
            double x_est = 0;
            double y_est = 0;
            double d_theta = 0;
            double theta_est = 0;

            //Console.WriteLine("YAW RAND = " + yaw_rand);
            d_theta = (yaw_rand - 128.0) * MAX_TPS / 128.0;
            theta_est = rand_point.theta_est + d_theta * dt_rand;
            x_est = rand_point.x_est + dt_rand * v * Math.Cos(theta_est * Math.PI / 180);
            y_est = rand_point.y_est + dt_rand * v * Math.Sin(theta_est * Math.PI / 180);
			
			
			Waypoint new_waypoint = new Waypoint((int)yaw_rand, dt_rand, t_step, x_est, y_est, theta_est, rand_point.index, count);
			
			return new_waypoint;
		}
		
        [STAThread]
		public static void Main (string[] args)
		{
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Motion_GUI());
			
		}

        public static void init_objects(List<GPS> start, List<GPS> end, List<double> bounding_box, double new_grid_width)
        {
            //Initialize global variables
            width = (int)Math.Abs(bounding_box[0] - bounding_box[2]);
            height = (int)Math.Abs(bounding_box[1] - bounding_box[3]);
            num_auvs = start.Count;

            //Initialize auvs		
            for (int i = 0; i < start.Count; i++)
            {
                //Console.WriteLine ("Adding " + i);
                //Declare new auv and first waypoint
                int j = num_auvs - i;
                Auv current = new Auv(i, end[i].long_coord, end[i].lat_coord);
                current.add_waypoint(128, 0, 0, start[i].long_coord, start[i].lat_coord, start[i].heading, -1, 0);
                //Auv current = new Auv(i, 180, j * 40 + 5);
                //current.add_waypoint(128, 0, 0, 20, (i + 1) * 40 + 20, 0, -1, 0);
                //Console.WriteLine ("Added first waypoint");
                //Add current auv to auvs
                curr_auvs.Add(current);
                //Console.WriteLine ("Add new auv to list");

            }

            
        }



        public static void run_motion_planner(bool whole_path, int iterations, List<GPS> start, List<GPS> end, List<double> bounding_box, List<Obstacle> new_obstacles, int new_grid_width)
        {
            //Reset grid width
            grid_width = new_grid_width;

            grid_number = (int)((width / grid_width) * (height / grid_width));

            auvs = new List<Auv>();
            obstacles = new_obstacles;
            max_length = Double.MaxValue;

            Random rand = new Random();

            Stopwatch sw = Stopwatch.StartNew();
            for (int iteration = 0; iteration < iterations; iteration++)
            {
            newiteration:
                curr_auvs = new List<Auv>();
                //Initialize everything
                init_objects(start, end, bounding_box, new_grid_width);

                
                //Prob Roadmap
                for (int a = 0; a < curr_auvs.Count; a++)
                {
                    //Start with next waypoint after start
                    int count = 1;
                    int last_point = 0;

                    //Create current line to target
                    List<Point> current_line = new List<Point>();
                    current_line.Add(new Point(curr_auvs[a].get_waypoint(last_point).x_est, curr_auvs[a].get_waypoint(last_point).y_est));
                    current_line.Add(new Point(curr_auvs[a].x_target, curr_auvs[a].y_target));


                    //While there is no path to target, keep adding to tree
                    while (intersect.intersect_obstacles(obstacles, current_line) || !intersect.is_reachable(whole_path, last_point, curr_auvs[a]))
                    {
                        //if (curr_auvs[a].waypoints.Count > 100000)
                        //{
                        //    goto newiteration;
                        //}
                        //Console.WriteLine("Adding again #" + howmany++);
                        double x_est = -1;
                        double y_est = -1;
                        int dt_rand = -1;

                        Waypoint rand_point = new Waypoint();
                        Waypoint new_point = new Waypoint();


                        while (x_est < 0 || x_est > width || y_est < 0 || y_est > height)
                        {
                            int yaw_rand = rand.Next(0, 255);

                            dt_rand = rand.Next(0, MAX_DT);

                            rand_point = curr_auvs[a].pick_waypoint(rand);

                            double prev_t = rand_point.t_step;
                            double t_step = prev_t + dt_rand;

                            new_point = sim_motion(rand_point, yaw_rand, dt_rand, t_step, count);

                            x_est = new_point.x_est;
                            y_est = new_point.y_est;

                        }

                        List<Point> new_line = new List<Point>();
                        new_line.Add(new Point(rand_point.x_est, rand_point.y_est));
                        new_line.Add(new Point(new_point.x_est, new_point.y_est));

                        if (!intersect.intersect_obstacles(obstacles, new_line) && !intersect.intersects_path(whole_path, curr_auvs[a], rand_point.index, new_point.x_est, new_point.y_est, rand_point.t_step, rand_point.t_step + dt_rand))
                        {
                            //Console.WriteLine(grid_width + ", " + new_point.x_est + ", " + new_point.y_est);
                            curr_auvs[a].add_waypoint(new_point);
                            last_point = count;
                            current_line = new List<Point>();
                            current_line.Add(new Point(curr_auvs[a].get_waypoint(last_point).x_est, curr_auvs[a].get_waypoint(last_point).y_est));
                            current_line.Add(new Point(curr_auvs[a].x_target, curr_auvs[a].y_target));
                            count++;
                        }

                    }

                    //Generate output array
                    int current_point = last_point;
                    Waypoint last_waypoint = curr_auvs[a].waypoints[last_point];

                    //Get dt to target
                    double target_dt = Math.Sqrt((last_waypoint.x_est - curr_auvs[a].x_target) * (last_waypoint.x_est - curr_auvs[a].x_target) + (last_waypoint.y_est - curr_auvs[a].y_target) * (last_waypoint.y_est - curr_auvs[a].y_target));
                    double target_theta_est = Math.Atan2((curr_auvs[a].y_target - last_waypoint.y_est), (curr_auvs[a].x_target - last_waypoint.x_est));
                    curr_auvs[a].path_length = (target_dt + last_waypoint.t_step) * VELOCITY;
                    Waypoint target_waypoint = new Waypoint(128, target_dt, target_dt + last_waypoint.t_step, curr_auvs[a].x_target, curr_auvs[a].y_target, target_theta_est, last_point, last_point + 1);
                    curr_auvs[a].output.Add(target_waypoint);
                    curr_auvs[a].output_time.Add(target_waypoint.t_step);

                    while (current_point != -1)
                    {
                        curr_auvs[a].output.Add(curr_auvs[a].waypoints[current_point]);
                        curr_auvs[a].output_time.Add(curr_auvs[a].waypoints[current_point].t_step);
                        current_point = curr_auvs[a].waypoints[current_point].parent;
                    }


                    curr_auvs[a].output.Reverse();
                    curr_auvs[a].output_time.Reverse();
                    //Still inside auv loop
                }
               
               //Determine Length
                double new_length = 0;
                for (int i = 0; i < curr_auvs.Count; i++)
                {
                    if (i == 0)
                    {
                        new_length = curr_auvs[i].path_length;
                    }
                    else
                    {
                        if (curr_auvs[i].path_length > new_length)
                        {
                            new_length = curr_auvs[i].path_length;
                        }
                    }
                }

                if (new_length < max_length)
                {
                    max_length = new_length;
                    auvs = curr_auvs;
                }

                //Determine number of waypoints planned for tests
                int total_waypoints = 0;
                foreach (Auv curr_auv in curr_auvs)
                {
                    total_waypoints += curr_auv.waypoints.Count;
                    
                }

                Console.Write(total_waypoints + ", ");

            }

            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds.ToString() + ", ");

        }
		
		
	}
}
