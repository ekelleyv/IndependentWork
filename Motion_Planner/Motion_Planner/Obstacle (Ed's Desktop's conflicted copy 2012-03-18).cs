using System;
using System.Collections.Generic;

namespace Motion_Planner
{
	public class Obstacle
	{
		private List<Point> coord;
		public Obstacle (List<Point> coord)
		{
			this.coord = coord;
		}
		public bool intersect(List<Point> line) {
			
			for (int i = 0; i < coord.Count; i++) {
				int j;
				if (i < coord.Count-1) {
					j = i;
				}
				else {
					j = 0;
				}
				if (Motion_Planner.intersect.line_intersect(coord[j].X, coord[j].Y, coord[j+1].X, coord[j+1].Y, line[0].X, line[0].Y, line[1].X, line[1].Y)) {
					return true;
				}
			}
			return false;
		}
		
	}
}

