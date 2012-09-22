function [ output ] = is_reachable(auvs, last_point, a, VELOCITY)
%IS_REACHABLE Summary of this function goes here
%   Detailed explanation goes here

%RHO IS INCORRECT
rho = atan2((auvs{a}.y_target-auvs{a}.waypoints(last_point).y_est),(auvs{a}.x_target-auvs{a}.waypoints(last_point).x_est))*180/pi;
if (rho <= 0)
    rho = rho + 360;
end

diff = abs(rho-auvs{a}.waypoints(last_point).theta_est);

%fprintf(1, 'Est: %f, Rho: %f, Diff: %f\n', auvs{a}.waypoints(last_point).theta_est, rho, diff);

output = 0;

if (diff < 30)
    last_position = [auvs{a}.waypoints(last_point).x_est, auvs{a}.waypoints(last_point).y_est];
    target_position = [auvs{a}.x_target, auvs{a}.y_target];
    dist = norm(last_position-target_position);
    prev_t = auvs{a}.waypoints(last_point).t_step;
    final_time = auvs{a}.waypoints(last_point).t_step+dist/VELOCITY;
    if (~intersects_path(auvs, a, last_point, auvs{a}.x_target, auvs{a}.y_target, prev_t, final_time))
        output = 1;
    end
end

end

