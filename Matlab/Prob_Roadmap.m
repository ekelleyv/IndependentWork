function [ ] = Prob_Roadmap(rectangles)
%PROB_ROADMAP Summary of this function goes here
%   Detailed explanation goes here

MAX_TPS = 10; %Max degrees per second turn
VELOCITY = 2; %Velocity in m/s
MAX_DT = 10;


colors = ['r', 'g', 'b'];
%num_grids = length(grid_sizes);
num_auvs = 3;
grid_width = 10; 

figure;
width = 200;
height = 200;
axis([0 width 0 height]);
hold on;


%Initialize auvs
auvs = struct([]);
for i=1:num_auvs    
    %Initialize starting point
    auvs{i}.waypoints(1).yaw = 128;
    auvs{i}.waypoints(1).dt = 0;
    auvs{i}.waypoints(1).t_step = 0;
    auvs{i}.waypoints(1).x_est = 20;
    auvs{i}.waypoints(1).y_est = i*40 + 20;
    auvs{i}.waypoints(1).theta_est = 0;
    auvs{i}.grid = cell(200/grid_width, 200/grid_width);
    auvs{i}.count = 1;
    [x_grid, y_grid] = determine_grid(grid_width, auvs{i}.waypoints(1).x_est, auvs{i}.waypoints(1).y_est);
    auvs{i}.grid{x_grid, y_grid}(1) = {1};
    h = plot(auvs{i}.waypoints(1).x_est, auvs{i}.waypoints(1).y_est, 'Marker', 's', 'MarkerFaceColor', colors(i), 'MarkerSize', 10);
    j = num_auvs - i + 1;
    auvs{i}.x_target = 180;
    auvs{i}.y_target = j*40 + 20;
    plot(auvs{i}.x_target, auvs{i}.y_target, 'Marker', 's', 'MarkerFaceColor', colors(i), 'MarkerSize', 10);
end

%Read in rectangles
for i=1:rectangles
    x_rect = zeros([1 5]);
    y_rect = zeros([1 5]);                                                                                                                                                                    
    r{i} = getrect();%[85.9447   76.9006   29.9539   43.2749];
    current = r{i};
    x_rect(:)=current(1);
    y_rect(:)=current(2);
    x_rect(2:3)=current(1)+current(3);
    y_rect(3:4)=current(2)+current(4);

    plot(x_rect,y_rect); 
end

%Do prob roadmap
for a=1:num_auvs
    
    count = 2;

    last_point = 1;
    
    
    
    %While there is no path to target, keep adding to tree
    while (intersects_rectangle(auvs{a}.x_target, auvs{a}.y_target, auvs{a}.waypoints(last_point).x_est, auvs{a}.waypoints(last_point).y_est, r)... 
        || ~is_reachable(auvs, last_point, a, VELOCITY)) 
        
        yaw_rand = round(rand(1)*255);


        dt_rand = rand(1)*MAX_DT;
        
        grid_count = 0;
        x_rand = 1;
        y_rand = 1;
        
        while (grid_count == 0) 
            x_rand = round(rand(1)*width/grid_width);
            y_rand = round(rand(1)*height/grid_width);
            if (x_rand == 0)
                x_rand = 1;
            end
            if (y_rand == 0) 
                y_rand = 1;
            end
            if (~isempty(auvs{a}.grid{x_rand, y_rand}))
                grid_count = 1;
            end   
        end
        rand_length = length(auvs{a}.grid{x_rand, y_rand});
        rand_point = auvs{a}.grid{x_rand, y_rand}(ceil(rand(1)*rand_length));
        rand_point = rand_point{1};
        

        if (rand_point == 0)
            rand_point = 1;
        end


        if (yaw_rand < 0)
            yaw_rand = 0;
        end
        if (yaw_rand > 255)
            yaw_rand = 255;
        end
        if (dt_rand < 0)
            dt_rand = 0;
        end

        [x_est, y_est, theta_est] = Sim_Motion(auvs{a}.waypoints(rand_point), yaw_rand, dt_rand, MAX_TPS, VELOCITY);
        
        prev_t = auvs{a}.waypoints(rand_point).t_step;
        
        t_step =  prev_t + dt_rand;

        if (~inside_rectangle(x_est, y_est, r) && ~intersects_rectangle(x_est, y_est, auvs{a}.waypoints(last_point).x_est, auvs{a}.waypoints(last_point).y_est, r) && ~intersects_path(auvs, a, rand_point, x_est, y_est, prev_t, t_step))
            
            [x_grid, y_grid] = determine_grid(grid_width, x_est, y_est);
            
            if ((x_grid <= 0) || (x_grid > width/grid_width) || (y_grid <= 0) || (y_grid >= height/grid_width))
                continue;
            end
            auvs{a}.waypoints(count).yaw = yaw_rand;
            auvs{a}.waypoints(count).dt = dt_rand;
            auvs{a}.waypoints(count).t_step = t_step;
            auvs{a}.waypoints(count).x_est = x_est;
            auvs{a}.waypoints(count).y_est = y_est;
            auvs{a}.waypoints(count).theta_est = theta_est;
            auvs{a}.waypoints(count).parent = rand_point; 
            last_point = count;
            grid_length = length(auvs{a}.grid{x_grid, y_grid});
            auvs{a}.grid{x_grid, y_grid}(grid_length+1) = {count};
            count = count + 1;
        end

        
    end
    
        
    %add last point
    %get time to reach target
    last_position = [auvs{a}.waypoints(count-1).x_est, auvs{a}.waypoints(count-1).y_est];
    target_position = [auvs{a}.x_target, auvs{a}.y_target];
    dist = norm(last_position-target_position);
    auvs{a}.waypoints(count).t_step = auvs{a}.waypoints(count-1).t_step+dist/VELOCITY;
    auvs{a}.waypoints(count).parent = last_point;
    auvs{a}.waypoints(count).x_est = auvs{a}.x_target;
    auvs{a}.waypoints(count).y_est = auvs{a}.y_target;
    auvs{a}.count = count;
    last_point = last_point + 1;
    
    current_point = last_point;
    
    
    %h = scatter(output(:, 1), output(:, 2));
    %set(h, 'MarkerEdgeColor', colors(a));
    auvs{a}.output = [];

    while (current_point ~= 1)
        parent_point = auvs{a}.waypoints(current_point).parent;
        x_plot = [auvs{a}.waypoints(parent_point).x_est; auvs{a}.waypoints(current_point).x_est];
        y_plot = [auvs{a}.waypoints(parent_point).y_est; auvs{a}.waypoints(current_point).y_est];
        auvs{a}.output = [auvs{a}.waypoints(current_point).x_est, auvs{a}.waypoints(current_point).y_est, auvs{a}.waypoints(current_point).t_step; auvs{a}.output];
        text(auvs{a}.waypoints(current_point).x_est, auvs{a}.waypoints(current_point).y_est+3, num2str(auvs{a}.waypoints(current_point).t_step));
        current_point = parent_point;
        plot(x_plot, y_plot, 'Color', colors(a));
        hs = scatter(auvs{a}.waypoints(current_point).x_est, auvs{a}.waypoints(current_point).y_est);
        set(hs, 'MarkerEdgeColor', colors(a));
    end
    
    auvs{a}.output = [auvs{a}.waypoints(1).x_est, auvs{a}.waypoints(1).y_est, 0; auvs{a}.output];
    
    disp(auvs{a}.output);
    disp('-----------');

    x_plot = [auvs{a}.waypoints(last_point).x_est];
    y_plot = [auvs{a}.waypoints(last_point).y_est];
    plot(x_plot, y_plot, 'Color', colors(a));


end
end


