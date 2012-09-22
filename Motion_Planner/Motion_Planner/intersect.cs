using System;
using System.Collections.Generic;

namespace Motion_Planner
{
	class intersect {
		
		
		//Adapted from http://alienryderflex.com/intersect/
		public static bool line_intersect(double Ax, double Ay, double Bx, double By, double Cx, double Cy, double Dx, double Dy) {
			
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
		
		  //  Success.
		  return true;
			
		}
		
		public static bool intersect_obstacles (List<Obstacle> obstacles, List<Point> current_line) {
			for (int i = 0; i < obstacles.Count; i++) {
				if (obstacles[i].intersect(current_line)) {
					return true;
				}
			}
			return false;
		}
		
		public static bool is_reachable(int last_point, Auv current) {
			Waypoint last = current.get_waypoint(last_point);
			double x_target = current.x_target;
			double y_target = current.y_target;
			double rho = Math.Atan2(last.y_est - y_target, last.x_est - x_target)*180/Math.PI;
			if (rho <= 0) {
				rho = rho + 360;
			}
			
			double diff = Math.Abs(rho-last.theta_est);
			
			if (diff < 30) {
				double dist = Math.Sqrt((last.y_est - y_target)*(last.y_est - y_target)+(last.x_est - x_target)*(last.x_est - x_target));
				double prev_t = last.t_step;
				double final_time = last.t_step + dist/MainClass.VELOCITY;
				if (!intersects_path(current, last_point, x_target, y_target, prev_t, final_time)) {
					return true;
				}
			}
			return false;
		}
		
		public static bool intersects_path (Auv current, int rand_point, double x_est, double y_est, double prev_t, double t_step) {
			Waypoint last_point = current.get_waypoint(rand_point);
			
			for (int i = 0; i < current.count; i++) {
				for (int j = 0; j < MainClass.auvs.Count; j++) {
					Waypoint curr = MainClass.auvs[i].output[j];
					Waypoint prev = MainClass.auvs[i].output[j-1];
					double curr_step = curr.t_step;
					double prev_step = prev.t_step;
					
					if (((prev_step > prev_t) && (prev_step < t_step)) || ((curr_step > prev_t) && (curr_step < t_step)) || ((prev_step < prev_t) && (curr_step > t_step))) {
						
						if (line_intersect (last_point.x_est, last_point.y_est, x_est, y_est, prev.x_est, prev.y_est, curr.x_est, curr.y_est)) {
							return true;
						}
						
					}
					
					
				}
				
			}
			return false;
	
		}
	}
}

