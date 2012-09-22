using System;
using System.Collections.Generic;

namespace Motion_Planner
{
	class intersect {
		
		
		//Adapted from http://alienryderflex.com/intersect/
		public static bool line_intersect(double Ax, double Ay, double Bx, double By, double Cx, double Cy, double Dx, double Dy) {


            //Console.WriteLine("Intersect (" + Ax + ", " + Ay + ", " + Bx + ", " + By + ") (" + Cx + ", " + Cy + ", " + Dx + ", " + Dy + ") ");
			
			double  distAB, theCos, theSin, newX, ABpos ;

		  //  Fail if either line segment is zero-length.
		  if (Ax==Bx && Ay==By || Cx==Dx && Cy==Dy) return false;
		
		  //  Fail if the segments share an end-point.
		  if (Ax==Cx && Ay==Cy || Bx==Cx && By==Cy
		  ||  Ax==Dx && Ay==Dy || Bx==Dx && By==Dy) {
		    return false; }
		
		  //  (1) Translate the system so that point A is on the origin.
		  Bx-=Ax; By-=Ay;
		  Cx-=Ax; Cy-=Ay;
		  Dx-=Ax; Dy-=Ay;
		
		  //  Discover the length of segment A-B.
		  distAB=Math.Sqrt(Bx*Bx+By*By);
		
		  //  (2) Rotate the system so that point B is on the positive X axis.
		  theCos=Bx/distAB;
		  theSin=By/distAB;
		  newX=Cx*theCos+Cy*theSin;
		  Cy  =Cy*theCos-Cx*theSin; Cx=newX;
		  newX=Dx*theCos+Dy*theSin;
		  Dy  =Dy*theCos-Dx*theSin; Dx=newX;
		
		  //  Fail if segment C-D doesn't cross line A-B.
		  if (Cy<0.0 && Dy<0.0 || Cy>=0.0 && Dy>=0.0) return false;
		
		  //  (3) Discover the position of the intersection point along line A-B.
		  ABpos=Dx+(Cx-Dx)*Dy/(Dy-Cy);
		
		  //  Fail if segment C-D crosses line A-B outside of segment A-B.
		  if (ABpos<0.0 || ABpos>distAB) return false;
		
		  //  (4) Apply the discovered position to line A-B in the original coordinate system.
		  //*X=Ax+ABpos*theCos;
		  //*Y=Ay+ABpos*theSin;
          //Console.WriteLine("TRUE");
		  //  Success.
		  return true;
			
		}
		
		public static bool intersect_obstacles (List<Obstacle> obstacles, List<Point> current_line) {
			for (int i = 0; i < obstacles.Count; i++) {
				if (obstacles[i].intersect_obstacle(current_line)) {
					return true;
				}
			}
			return false;
		}

        public static bool is_reachable(bool whole_path, int last_point, Auv current)
        {
            Waypoint last = current.get_waypoint(last_point);
            double x_target = current.x_target;
            double y_target = current.y_target;
            double rho = Math.Atan2(y_target - last.y_est, x_target - last.x_est) * 180 / Math.PI;
            
            if (rho <= 0)
            {
                rho = rho + 360;
            }

            if (last.theta_est < 0)
            {
                last.theta_est += 360;
            }

            double diff = Math.Abs(rho - last.theta_est);

            if (diff < 30)
            {
                double dist = Math.Sqrt((last.y_est - y_target) * (last.y_est - y_target) + (last.x_est - x_target) * (last.x_est - x_target));
                double prev_t = last.t_step;
                double final_time = last.t_step + dist / MainClass.VELOCITY;
                if (!intersects_path(whole_path, current, last_point, x_target, y_target, prev_t, final_time))
                {
                    return true;
                }
            }
            return false;
        }

        //public static bool intersects_path(bool whole_path, Auv current, int rand_point, double x_est, double y_est, double prev_t, double t_step)
        //{

        //    Waypoint last_point = current.get_waypoint(rand_point);
        //    for (int i = 0; i < current.count; i++)
        //    {
        //        for (int j = 1; j < MainClass.curr_auvs[i].output.Count; j++)
        //        {
        //            Waypoint curr = MainClass.curr_auvs[i].output[j];
        //            Waypoint prev = MainClass.curr_auvs[i].output[j - 1];
        //            double curr_step = curr.t_step;
        //            double prev_step = prev.t_step;

        //            if (whole_path)
        //            {
        //                if (line_intersect(last_point.x_est, last_point.y_est, x_est, y_est, prev.x_est, prev.y_est, curr.x_est, curr.y_est))
        //                {
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                if (((prev_step > prev_t) && (prev_step < t_step)) || ((curr_step > prev_t) && (curr_step < t_step)) || ((prev_step < prev_t) && (curr_step > t_step)))
        //                {
        //                    //Console.WriteLine("COLIDING");
        //                    if (line_intersect(last_point.x_est, last_point.y_est, x_est, y_est, prev.x_est, prev.y_est, curr.x_est, curr.y_est))
        //                    {
        //                        return true;
        //                    }

        //                }
        //            }


        //        }

        //    }
        //    return false;

        //}

        public static bool intersects_path(bool whole_path, Auv current, int rand_point, double x_est, double y_est, double prev_t, double t_step)
        {

            Waypoint last_point = current.get_waypoint(rand_point);
            //for each of the previous auvs
            for (int i = 0; i < current.count; i++)
            {
                int min_index = MainClass.curr_auvs[i].output_time.BinarySearch(prev_t);
                int max_index = MainClass.curr_auvs[i].output_time.BinarySearch(t_step);

                if (min_index < 0)
                {
                    min_index = ~min_index;
                }
                if (max_index < 0)
                {
                    max_index = ~max_index;
                }
                if (min_index != 0)
                {
                    min_index -= 1;
                }
                if (max_index == MainClass.curr_auvs[i].output_time.Count)
                {
                    max_index -= 1;
                }
                //Console.WriteLine("------------------------------");
                //Console.WriteLine("Intersecting (" + prev_t + ", " + t_step + ") against");
                foreach (double time in MainClass.curr_auvs[i].output_time)
                {
                    //Console.WriteLine(time);
                }
                //Console.WriteLine("Min = " + min_index + ", Max = " + max_index + ", " + MainClass.curr_auvs[i].output_time.Count);
                //Console.WriteLine("Time segement (" + prev_t + ", " + t_step + ") overlaps (" + MainClass.curr_auvs[i].output_time[min_index] + ", " + MainClass.curr_auvs[i].output_time[max_index] + ")");
                int num_segments = max_index - min_index;
                for (int j = 0; j < num_segments; j++)
                {
                    Waypoint curr = MainClass.curr_auvs[i].output[min_index + j + 1];
                    Waypoint prev = MainClass.curr_auvs[i].output[min_index + j];
                    double curr_step = curr.t_step;
                    double prev_step = prev.t_step;

                    if (line_intersect(last_point.x_est, last_point.y_est, x_est, y_est, prev.x_est, prev.y_est, curr.x_est, curr.y_est))
                    {
                        return true;
                    }

                }

            }
            return false;

        }
	}
}

