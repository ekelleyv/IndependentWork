using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Motion_Planner
{
    public partial class Motion_GUI : Form
    {
        Thread t;
        int x = 10;
        int y = 10;
        int offset = 100;
        int formscale = 4;
        bool replan = false;
        int num_replans = 0;
        List<Obstacle> obstacles;
        List<double> bounding_box;
        int grid_width = 20;
        bool whole_path_intersect;
        int iterations;
        List<GPS> start = new List<GPS>();
        List<GPS> end = new List<GPS>();
        int num_auvs = 2;
        int scenario = 1;
        int num_obstacles;

        bool first_sim = true;
        List<List<double>> locations = new List<List<double>>();
        List<int> target_waypoints = new List<int>();
        List<bool> finished = new List<bool>();

        bool actual_first_sim = true;
        List<List<double>> actual_locations = new List<List<double>>();
        List<List<double>> last_locations = new List<List<double>>();
        List<int> actual_target_waypoints = new List<int>();
        List<bool> actual_finished = new List<bool>();

        Button run_button = new Button();
        CheckBox simulate = new CheckBox();
        CheckBox whole_path = new CheckBox();
        CheckBox replans_on = new CheckBox();
        TextBox iteration_input = new TextBox();


        public Motion_GUI()
        {
            InitializeComponent();
            t = new Thread(new ThreadStart(run_simulation));
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new System.Drawing.Size(1100, 800);
            BackColor = System.Drawing.Color.White;

            Controls.Add(simulate);
            simulate.Location = new System.Drawing.Point(900, 20);
            simulate.Text = "Simulate";

            Controls.Add(replans_on);
            replans_on.Location = new System.Drawing.Point(900, 60);
            replans_on.Text = "Replans";

            Controls.Add(whole_path);
            whole_path.Location = new System.Drawing.Point(900, 100);
            whole_path.Text = "Whole Path Intersection";

            Controls.Add(iteration_input);
            iteration_input.Location = new System.Drawing.Point(900, 140);
            iteration_input.Text = "10";
            


            Controls.Add(run_button);
            run_button.Text = "Run";
            run_button.Location = new System.Drawing.Point(900, 200);
            run_button.Click += new EventHandler(run_button_click);

            //Uncomment to run tests
            //this.Load += new EventHandler(Motion_GUI_Load);

        }

        protected void Motion_GUI_Load(object sender, EventArgs e)
        {
            int test_type = 3;

            //ITERATIONS
            if (test_type == 0)
            {
                //int[] nums = new int[] { 1, 2, 4, 8, 10, 20, 40, 50, 100 };
                int[] steps = new int[]{1, 2, 3, 4, 5, 10, 15, 20, 50, 100};
                foreach (int j in steps)
                {
                    //int i = nums[a];
                    for (int a = 0; a < 50; a++)
                    {
                        Console.Write(j + ", ");
                        grid_width = 20;
                        num_auvs = 2;
                        iteration_input.Text = j.ToString();
                        whole_path.Checked = false;
                        run_button.PerformClick();
                        Console.WriteLine(MainClass.max_length);
                    }
                }
            }
            
            //GRID WIDTH
            else if (test_type == 1)
            {
                //int[] nums = new int[] { 1, 2, 4, 8, 10, 20, 40, 50, 100, 200 };
                int[] nums = new int[] { 100, 200 };
                foreach (int j in nums)
                {
                    //int i = nums[a];
                    for (int a = 0; a < 100; a++)
                    {
                        Console.Write(j + ", ");
                        grid_width = j;
                        num_auvs = 2;
                        iteration_input.Text = "1";
                        whole_path.Checked = false;
                        run_button.PerformClick();
                        Console.WriteLine(MainClass.max_length);
                    }
                }
            }

            //NUM AUVS
            else if (test_type == 2)
            {
                //int[] nums = new int[] { 1, 2, 4, 8, 10, 20, 40, 50, 100 };
                for (int j = 9; j <= 10; j++)
                {
                    //int i = nums[a];
                    for (int a = 0; a < 100; a++)
                    {
                        Console.Write(j + ", ");
                        grid_width = 20;
                        num_auvs = j;
                        iteration_input.Text = "1";
                        whole_path.Checked = false;
                        run_button.PerformClick();
                        Console.WriteLine(MainClass.max_length);
                    }
                }
            }

             //NUM OBSTACLES
            else if (test_type == 3)
            {
                for (int j = 35; j <= 50; j += 5)
                {
                    num_obstacles = j;
                    for (int a = 0; a < 100; a++)
                    {
                        Console.Write(j + ", ");
                        grid_width = 20;
                        iteration_input.Text = "1";
                        whole_path.Checked = false;
                        run_button.PerformClick();
                        Console.WriteLine(MainClass.max_length);
                    }
                }
            }

        }

        protected bool overlaps(List<int> x_rand, List<int> y_rand, int x_pos, int y_pos, int width, int height) 
        {
            for (int i = 0; i < x_rand.Count; i++)
            {
                int x_diff = Math.Abs(x_rand[i] - x_pos);
                int y_diff = Math.Abs(y_rand[i] - y_pos);

                if ((x_diff < width) && (y_diff < height))
                {
                    return true;
                }

            }

            return false;

        }

        protected void load_scenario()
        {
            bounding_box.Add(0); //x1
            bounding_box.Add(0); //y1
            bounding_box.Add(200); //x2
            bounding_box.Add(200); //y2

            if (scenario == 0)
            {
                //Define obstacles
                List<Point> obstaclepoints = new List<Point>();
                //obstaclepoints.Add(new Point(75, 75));
                //obstaclepoints.Add(new Point(75, 125));
                //obstaclepoints.Add(new Point(125, 125));
                //obstaclepoints.Add(new Point(125, 75));
                //obstaclepoints.Add(new Point(75, 75));
                //obstacles.Add(new Obstacle(obstaclepoints));

                //Easy Path for Real World
                //obstaclepoints.Add(new Point(22, 3));
                //obstaclepoints.Add(new Point(22, 7));
                //obstaclepoints.Add(new Point(27, 7));
                //obstaclepoints.Add(new Point(27, 3));
                //obstaclepoints.Add(new Point(22, 3));
                //obstacles.Add(new Obstacle(obstaclepoints));

                //Harder Path for Real World
                obstaclepoints.Add(new Point(21, 0));
                obstaclepoints.Add(new Point(21, 7));
                obstaclepoints.Add(new Point(27, 7));
                obstaclepoints.Add(new Point(27, 0));
                obstaclepoints.Add(new Point(21, 0));
                obstacles.Add(new Obstacle(obstaclepoints));
            }
            else if (scenario == 1)
            {
                //Define obstacles
                List<Point> obstaclepoints = new List<Point>();

                Random rand = new Random();

                List<int> x_rand = new List<int>();
                List<int> y_rand = new List<int>();

                if (num_auvs > 0)
                {
                    x_rand.Add(50); y_rand.Add(50);
                    x_rand.Add(150); y_rand.Add(150);
                }
                if (num_auvs > 1)
                {
                    x_rand.Add(50); y_rand.Add(150);
                    x_rand.Add(150); y_rand.Add(50);
                }
                if (num_auvs > 2)
                {
                    x_rand.Add(50); y_rand.Add(100);
                    x_rand.Add(150); y_rand.Add(100);
                }
                if (num_auvs > 3)
                {
                    x_rand.Add(50); y_rand.Add(75);
                    x_rand.Add(150); y_rand.Add(125);
                }

                //For use with random obstacles
                //for (int i = 0; i < num_obstacles; i++)
                //{
                //    obstaclepoints = new List<Point>();
                //    int width = 10;
                //    int height = 10;
                //    int x_pos = rand.Next(0, 180);
                //    int y_pos = rand.Next(0, 180);
                    
                //    while (overlaps(x_rand, y_rand, x_pos, y_pos, width, height))
                //    {
                //        x_pos = rand.Next(0, 180);
                //        y_pos = rand.Next(0, 180);
                //    }

                //    x_rand.Add(x_pos);
                //    y_rand.Add(y_pos);
                //    obstaclepoints.Add(new Point(x_pos, y_pos));
                //    obstaclepoints.Add(new Point(x_pos, y_pos + height));
                //    obstaclepoints.Add(new Point(x_pos + width, y_pos + height));
                //    obstaclepoints.Add(new Point(x_pos + width, y_pos));
                //    obstaclepoints.Add(new Point(x_pos, y_pos));
                //    obstacles.Add(new Obstacle(obstaclepoints));


                //}


                obstaclepoints = new List<Point>();
                obstaclepoints.Add(new Point(60, 65));
                obstaclepoints.Add(new Point(60, 100));
                obstaclepoints.Add(new Point(80, 100));
                obstaclepoints.Add(new Point(80, 65));
                obstaclepoints.Add(new Point(60, 65));
                obstacles.Add(new Obstacle(obstaclepoints));

                obstaclepoints = new List<Point>();
                obstaclepoints.Add(new Point(105, 55));
                obstaclepoints.Add(new Point(105, 90));
                obstaclepoints.Add(new Point(130, 90));
                obstaclepoints.Add(new Point(130, 55));
                obstaclepoints.Add(new Point(105, 55));
                obstacles.Add(new Obstacle(obstaclepoints));

                obstaclepoints = new List<Point>();
                obstaclepoints.Add(new Point(90, 110));
                obstaclepoints.Add(new Point(90, 150));
                obstaclepoints.Add(new Point(120, 150));
                obstaclepoints.Add(new Point(120, 110));
                obstaclepoints.Add(new Point(90, 110));
                obstacles.Add(new Obstacle(obstaclepoints));
            }

            if (!replan)
            {
                if (scenario == 0)
                {
                    //EASY SCENARIO
                    //start.Add(new GPS(50, 50, 0));
                    //end.Add(new GPS(150, 150, 0));
                    //start.Add(new GPS(50, 150, 0));
                    //end.Add(new GPS(150, 50, 0));
                    start.Add(new GPS(0, 0, 0));
                    end.Add(new GPS(55, 10, 0));
                }
                else if (scenario == 1)
                {
                    if (num_auvs > 0)
                    {
                        start.Add(new GPS(50, 50, 0));
                        end.Add(new GPS(150, 150, 0));
                    }
                    if (num_auvs > 1)
                    {
                        start.Add(new GPS(50, 150, 0));
                        end.Add(new GPS(150, 50, 0));
                    }
                    if (num_auvs > 2)
                    {
                        start.Add(new GPS(50, 100, 0));
                        end.Add(new GPS(150, 100, 0));
                    }
                    if (num_auvs > 3)
                    {
                        start.Add(new GPS(50, 75, 0));
                        end.Add(new GPS(150, 125, 0));
                    }
                    if (num_auvs > 4)
                    {
                        start.Add(new GPS(50, 125, 0));
                        end.Add(new GPS(150, 75, 0));
                    }
                    if (num_auvs > 5)
                    {
                        start.Add(new GPS(50, 130, 0));
                        end.Add(new GPS(150, 70, 0));
                    }
                    if (num_auvs > 6)
                    {
                        start.Add(new GPS(50, 70, 0));
                        end.Add(new GPS(150, 130, 0));
                    }
                    if (num_auvs > 7)
                    {
                        start.Add(new GPS(50, 65, 0));
                        end.Add(new GPS(150, 120, 0));
                    }
                    if (num_auvs > 8)
                    {
                        start.Add(new GPS(50, 120, 0));
                        end.Add(new GPS(150, 65, 0));
                    }
                    if (num_auvs > 9)
                    {
                        start.Add(new GPS(50, 40, 0));
                        end.Add(new GPS(150, 160, 0));
                    }
                }

            }
        }

        protected void run_button_click(object sender, EventArgs e)
        {
            replan = false;
            num_replans = 0;
            if (t.IsAlive)
            {
                t.Abort();
            }
            run_plan();
            
        }

        public void run_plan()
        {

           
            //Invalidate();
            //Console.WriteLine("In Run Plan");
            Thread old = t;
            t = new Thread(new ThreadStart(run_simulation));
            //Console.WriteLine("Making stuff");

            if (!replan)
            {
                bounding_box = new List<double>();
                obstacles = new List<Obstacle>();
            }
            locations = new List<List<double>>();
            target_waypoints = new List<int>();
            finished = new List<bool>();
            
            actual_finished = new List<bool>();
            whole_path_intersect = whole_path.Checked;
            iterations = Convert.ToInt32(iteration_input.Text);
            start = new List<GPS>();
            end = new List<GPS>();

            load_scenario();

            if (replan)
            {
                for (int j = 0; j < MainClass.auvs.Count; j++)
                {
                    Auv curr = MainClass.auvs[j];
                    double theta = Math.Atan2(curr.output[actual_target_waypoints[j]].y_est - actual_locations[j][1], curr.output[actual_target_waypoints[0]].x_est - actual_locations[j][0])*(180/Math.PI);
                    start.Add(new GPS(last_locations[j][0], last_locations[j][1], theta)); //CHANGE TO NOT ZERO
                    end.Add(new GPS(MainClass.auvs[j].x_target, MainClass.auvs[j].y_target, 0));

                }
            }

            //Reset actual_locations
            actual_locations = new List<List<double>>();
            actual_target_waypoints = new List<int>();


            MainClass.run_motion_planner(whole_path_intersect, iterations, start, end, bounding_box, obstacles, grid_width);

            if (simulate.Checked)
            {
                //Console.WriteLine("Simulate Checked");
                int count = 0;
                foreach (Auv curr_auv in MainClass.auvs)
                {
                    List<double> current_list = new List<double>();
                    current_list.Add(curr_auv.waypoints[0].x_est);
                    current_list.Add(curr_auv.waypoints[0].y_est);
                    finished.Add(false);
                    target_waypoints.Add(0);
                    locations.Add(current_list);

                    current_list = new List<double>();
                    current_list.Add(curr_auv.waypoints[0].x_est);
                    current_list.Add(curr_auv.waypoints[0].y_est);
                    actual_finished.Add(false);
                    actual_target_waypoints.Add(0);
                    actual_locations.Add(current_list);
                    //Console.WriteLine("Adding Point (" + locations[count][0] + ", " + locations[count][1] + ")");
                    count++;
                }
                t.Start();
                old.Abort();
            }
            else
            {
                Invalidate();
            }

        }

        public bool on_last_segment()
        {
            for (int i = 0; i < MainClass.curr_auvs.Count; i++)
            {
                if (actual_target_waypoints[i] >= MainClass.curr_auvs[i].output.Count-1)
                {
                    return true;
                }
            }
            return false;

        }
        public void run_simulation()
        {
            Random rand = new Random();
            //Console.WriteLine("Simulation Running");
            while (!is_finished())
            {
                int count = 0;
                if (replans_on.Checked)
                {
                    if (check_error() && !on_last_segment())
                    {
                        replan = true;
                        last_locations = actual_locations;
                        num_replans++;
                        run_plan();
                    }
                }
                foreach (Auv curr_auv in MainClass.auvs)
                {
                    if (!finished[count])
                    {

                        //Console.WriteLine("Auv #" + count);
                        //Console.WriteLine("(" + locations[count][0] + ", " + locations[count][1] + ")");
                        Waypoint curr_waypoint = curr_auv.output[target_waypoints[count]];
                        if (first_sim)
                        {
                            locations[count][0] = curr_waypoint.x_est;
                            locations[count][1] = curr_waypoint.y_est;
                            first_sim = false;
                        }
                        else
                        {
                            double distance = get_distance(locations[count][0], locations[count][1], curr_waypoint.x_est, curr_waypoint.y_est);
                            if (distance < 1)
                            {
                                //Console.WriteLine("Hit Target");
                                target_waypoints[count]++;
                                if (target_waypoints[count] == MainClass.auvs[count].output.Count)
                                {
                                    //Console.WriteLine("AUV #" + count + " hit target ");
                                    finished[count] = true;
                                    //Console.WriteLine("1 Finished = " + finished[0] + " 2 Finsihed " + finished[1]);
                                }

                                //Console.WriteLine("Now going to index " + target_waypoints[count]);
                            }
                            else
                            {
                                //Console.WriteLine("Still traveling");
                                double direction = Math.Atan2(curr_waypoint.y_est - locations[count][1], curr_waypoint.x_est - locations[count][0]);

                                double this_distance = 1.0/20.0;
                                double delta_x = this_distance * Math.Cos(direction);
                                double delta_y = this_distance * Math.Sin(direction);
                                //Console.WriteLine("Direction = " + direction + ", this_distance = " + this_distance + ", deltax, deltay = " + delta_x + ", " + delta_y);
                                locations[count][0] += delta_x*formscale;
                                locations[count][1] += delta_y*formscale;


                            }
                        }
                    }

                    if (!actual_finished[count])
                    {
                        Waypoint curr_waypoint = curr_auv.output[actual_target_waypoints[count]];
                        if (actual_first_sim)
                        {
                            actual_locations[count][0] = curr_waypoint.x_est;
                            actual_locations[count][1] = curr_waypoint.y_est;
                            actual_first_sim = false;
                        }
                        else
                        {
                            double distance = get_distance(actual_locations[count][0], actual_locations[count][1], curr_waypoint.x_est, curr_waypoint.y_est);
                            if (distance < 1)
                            {
                                actual_target_waypoints[count]++;
                                if (actual_target_waypoints[count] == MainClass.auvs[count].output.Count)
                                {
                                    actual_finished[count] = true;
                                }

                            }
                            else
                            {
                                double direction = Math.Atan2(curr_waypoint.y_est - actual_locations[count][1], curr_waypoint.x_est - actual_locations[count][0]);
                                double this_distance = 1.0 / 20.0 * get_rand_gain(rand);
                                double delta_x = this_distance * Math.Cos(direction);
                                double delta_y = this_distance * Math.Sin(direction);
                                actual_locations[count][0] += delta_x * formscale;
                                actual_locations[count][1] += delta_y * formscale;


                            }
                        }
                    }
                    count++;
                }
                Thread.Sleep(100);
                Invalidate();


            }
            //Console.WriteLine("I am finished");
            //t.Abort();
        }

        public bool check_error()
        {
            for (int i = 0; i < locations.Count; i++)
            {
                double distance = get_distance(locations[i][0], locations[i][1], actual_locations[i][0], actual_locations[i][1]);
                if (distance > 5)
                {
                    return true;

                }

            }
            return false;

        }
        //Adapted from http://stackoverflow.com/questions/218060/random-gaussian-variables
        public double get_rand_gain(Random rand)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double mean = 1.0;
            double stddev = 2.0;
            double randNormal =
                         mean + stddev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;

        }

        public bool is_finished()
        {
            bool temp = true;

            foreach (bool curr in finished)
            {
                temp = (temp && curr);
            }
            return temp;
        }

        public static double get_distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return distance;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            List<Pen> pens = new List<Pen>();
            pens.Add(new Pen(Color.Red, 2));
            pens.Add(new Pen(Color.Blue, 2));
            pens.Add(new Pen(Color.Green, 2));
            pens.Add(new Pen(Color.Black, 2));
            pens.Add(new Pen(Color.Orange, 2));
            pens.Add(new Pen(Color.Purple, 2));
            pens.Add(new Pen(Color.HotPink, 2));
            pens.Add(new Pen(Color.IndianRed, 2));
            pens.Add(new Pen(Color.Magenta, 2));
            pens.Add(new Pen(Color.Navy, 2));

            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 11);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            foreach (Obstacle curr_ob in MainClass.obstacles)
            {
                for (int i = 1; i < curr_ob.coord.Count; i++)
                {

                    g.DrawLine(pens[3], (int)curr_ob.coord[i - 1].X * formscale, (int)curr_ob.coord[i - 1].Y * formscale, (int)curr_ob.coord[i].X * formscale, (int)curr_ob.coord[i].Y * formscale);

                }

            }
            int count = 0;
            //Draw each AUV
            foreach (Auv curr_auv in MainClass.auvs)
            {
                g.DrawRectangle(pens[3], (int)curr_auv.x_target * formscale, (int)curr_auv.y_target * formscale, 4, 4);
                Pen curr_pen;
                if (count == 0)
                {
                    curr_pen = pens[3];
                }
                else
                {
                    curr_pen = pens[count];
                }
                count += 1;
                
                //Draw every waypoint
                foreach (Waypoint curr_waypoint in curr_auv.waypoints)
                {
                    
                    //g.DrawRectangle(curr_pen, (int)curr_waypoint.x_est * formscale, (int)curr_waypoint.y_est * formscale, 4, 4);

                    if (curr_waypoint.parent >= 0)
                    {
                        Waypoint parent_waypoint = curr_auv.waypoints[curr_waypoint.parent];
                        //g.DrawLine(curr_pen, (int)parent_waypoint.x_est * formscale, (int)parent_waypoint.y_est * formscale, (int)curr_waypoint.x_est * formscale, (int)curr_waypoint.y_est * formscale);
                    }

                }

                //Draw ouput paths
                foreach (Waypoint curr_waypoint in curr_auv.output)
                {
                    if (curr_waypoint.parent != -1)
                    {

                        Waypoint parent_waypoint = curr_auv.waypoints[curr_waypoint.parent];
                        //Write t-step
                        g.DrawString(((int)curr_waypoint.t_step).ToString() + "s", drawFont, drawBrush, (int)curr_waypoint.x_est * formscale, (int)curr_waypoint.y_est * formscale);
                        //Console.WriteLine(curr_waypoint.x_est + ", " + curr_waypoint.y_est);
                        g.DrawRectangle(curr_pen, (int)curr_waypoint.x_est * formscale, (int)curr_waypoint.y_est * formscale, 4, 4);
                        g.DrawLine(curr_pen, (int)parent_waypoint.x_est * formscale, (int)parent_waypoint.y_est * formscale, (int)curr_waypoint.x_est * formscale, (int)curr_waypoint.y_est * formscale);
                    }
                }
                int lastpoint = curr_auv.waypoints.Count - 1;
                g.DrawLine(curr_pen, (int)curr_auv.waypoints[lastpoint].x_est * formscale, (int)curr_auv.waypoints[lastpoint].y_est * formscale, (int)curr_auv.x_target * formscale, (int)curr_auv.y_target * formscale);

            }

            //Simulate

            if (simulate.Checked)
            {
                if (locations.Count > 0)
                {
                    for (int i = 0; i < locations.Count; i++)
                    {
                        g.DrawRectangle(pens[0], (float)locations[i][0] * formscale - 4, (float)locations[i][1] * formscale - 4, 8, 8);
                        g.DrawRectangle(pens[1], (float)actual_locations[i][0] * formscale - 4, (float)actual_locations[i][1] * formscale - 4, 8, 8);


                    }
                }

            }
            
            base.OnPaint(e);
        }

       

    }

}
