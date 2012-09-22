function [ x_est, y_est, theta_est ] = Sim_Motion(current_point, yaw, dt, MAX_TPS, VELOCITY)
%SIM_MOTION Summary of this function goes here
%   Detailed explanation goes here

    %Maximum degree turn per second
    max_tps = MAX_TPS;
    
    %Velocity is 2 m/s
    v = VELOCITY;
    
    %Determine change in theta/second
    d_theta = (yaw-128)*max_tps/128;
    
    theta_est = current_point.theta_est + d_theta*dt; 
    x_est = current_point.x_est + dt*v*cos(theta_est*pi/180);
    y_est = current_point.y_est + dt*v*sin(theta_est*pi/180);
    
    

end

