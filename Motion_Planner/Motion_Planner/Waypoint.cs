using System;

namespace Motion_Planner
{
	public class Waypoint
	{
		public int yaw;
		public double dt;
		public double t_step;
		public double x_est;
		public double y_est;
		public double theta_est;
		public int parent;
		public int index;
		
		public Waypoint(int yaw, double dt, double t_step, double x_est, double y_est, double theta_est, int parent, int index) {
			this.yaw = yaw;
			this.dt = dt;
			this.t_step = t_step;
			this.x_est = x_est;
			this.y_est = y_est;
			this.theta_est = theta_est;
			this.parent = parent;
			this.index = index;
		}
		
	}
}

