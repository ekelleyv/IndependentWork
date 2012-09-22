function [ output ] = intersects_path(auvs, a, rand_point, x_est, y_est, prev_t, t_step)
%INTERSECTS_PATH Summary of this function goes here
%   Detailed explanation goes here

% MIN_DISTANCE = 2; %Min distance between 2 paths is 2 meters
% 
% p1 = [auvs{a}.waypoints(rand_point).x_est, auvs{a}.waypoints(rand_point).y_est, 0];
% p2 = [x_est, y_est, 0];

line1 = [auvs{a}.waypoints(rand_point).x_est, auvs{a}.waypoints(rand_point).y_est, x_est, y_est];


output = 0;

% for i=1:a-1
%     current_point = auvs{i}.count;
%     while (current_point ~= 1)
%         parent_point = auvs{i}.waypoints(current_point).parent;
%         curr_step = auvs{i}.waypoints(current_point).t_step;
%         prev_step = auvs{i}.waypoints(parent_point).t_step;
%         if (((prev_step > prev_t) && (prev_step < t_step)) || ((curr_step > prev_t) && (curr_step < t_step)) || ((prev_step < prev_t) && (curr_step > t_step)))
%             p3 = [auvs{i}.waypoints(parent_point).x_est, auvs{i}.waypoints(parent_point).y_est, 0];
%             p4 = [auvs{i}.waypoints(current_point).x_est, auvs{i}.waypoints(current_point).y_est, 0];
%             if (DistBetween2Segment(p1, p2, p3, p4) < MIN_DISTANCE)
%                 output = 1;
%                 return;
%             end
%         end
% 
%         current_point = parent_point;
%     end
%     
%     
% end

for i=1:a-1
    for j = 2:length(auvs{i}.output)
        curr_step = auvs{i}.output(j, 3);
        prev_step = auvs{i}.output(j-1, 3);        
        
        if (((prev_step > prev_t) && (prev_step < t_step)) || ((curr_step > prev_t) && (curr_step < t_step)) || ((prev_step < prev_t) && (curr_step > t_step)))
            line2 = [auvs{i}.output(j-1, 1), auvs{i}.output(j-1, 2), auvs{i}.output(j, 1), auvs{i}.output(j, 2)];
            out = lineSegmentIntersect(line1, line2);
            if (out.intAdjacencyMatrix == 1)
                    output = 1;
                    return;
            end
        end
    end
    
    
end

end

