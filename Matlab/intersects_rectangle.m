function [ output ] = intersects_rectangle( target_x, target_y, current_x, current_y, r)
%INTERSECTS_RECTANGLE Summary of this function goes here
%   Detailed explanation goes here
% 


% curr_line = [target_x - current_x, target_y - current_y];
% norm_line = curr_line/norm(curr_line);
% 
% steps = 100;
% size = norm(curr_line)/steps;
% output = 0;
% 
% for i=1:steps
%     %plot(current_x + norm_line(1)*i*size, current_y + norm_line(2)*i*size, 'Marker', 's','MarkerSize', 10);
%     if (inside_rectangle(current_x + norm_line(1)*i*size, current_y + norm_line(2)*i*size, r))
%         output = 1;
%     end
% end

current = [current_x, current_y, 0];
target = [target_x, target_y, 0];
path = [current_x, current_y, target_x, target_y];

output = 0;

 for i=1:length(r)
    rect = r{i};
    xmin = rect(1);
    ymin = rect(2);
    xmax = xmin + rect(3);
    ymax = ymin + rect(4);
    
%     c1 = [xmin, ymin, 0];
%     c2 = [xmin, ymax, 0];
%     c3 = [xmax, ymax, 0];
%     c4 = [xmax, ymin, 0];
      l1 = [xmin, ymin, xmin, ymax];
      l2 = [xmin, ymax, xmax, ymax];
      l3 = [xmax, ymax, xmax, ymin];
      l4 = [xmax, ymin, xmin, ymin];
      out1 = lineSegmentIntersect(l1, path);
      out2 = lineSegmentIntersect(l2, path);
      out3 = lineSegmentIntersect(l3, path);
      out4 = lineSegmentIntersect(l4, path);
      
      if (out1.intAdjacencyMatrix || out2.intAdjacencyMatrix || out3.intAdjacencyMatrix || out4.intAdjacencyMatrix)
          output = 1;
          return;
      end
    
    
%     if (DistBetween2Segment(c1, c2, current, target) < 2)
%         output = 1;
%         return
%     end
%     if (DistBetween2Segment(c2, c3, current, target) < 2)
%         output = 1;
%         return
%     end
%     if (DistBetween2Segment(c3, c4, current, target) < 2)
%         output = 1;
%         return
%     end
%     if (DistBetween2Segment(c4, c1, current, target) < 2)
%         output = 1;
%         return
%     end
    
 end


        
end
