function [ ] = Track_Shark( max_time )
%TRACK_SHARK Summary of this function goes here
%   Detailed explanation goes here

current_state = [200, 200, 0];
target_state = [500, 500, 0];
% Plot_Position(current_state, 1);
% Plot_Position(target_state, 2);





time_start = clock;
current_state = [400 400 10];

auv_scale = .5;

[auv, ~, auv_alpha] = imread('AUV.png');
auv = imresize(auv, auv_scale);
auv_alpha = imresize(auv_alpha, auv_scale);

bkg = imresize(imread('LakeCarnegie.png'), 1.5);
h = imshow(bkg);
hold on

while ((clock - time_start) < max_time)
    h = imshow(bkg);
    current_state(1) = current_state(1) + 5;
    current_state(2) = current_state(2) + 5;
    %current_state(3) = current_state(3) + 15;
    off_x = current_state(1);
    off_y = current_state(2);
    rotation = current_state(3);
    auvh = image(off_x, off_y, imrotate(auv, rotation));
    set(auvh, 'AlphaData', imrotate(auv_alpha, rotation));
    %refreshdata
    drawnow
end

% while ((clock - time_start) < max_time)
%     
%     shark_state = State_Estimator();
%     
%     %Multi AUV Planner
%     desired_state = AUV_Planner(current_state, shark_state);
%     
%     %Motion Planner
%     [current_state, desired_state] = Motion_Planner(current_state, desired_state);
%     
%     %AUV Controller
%     %current_state = AUV_Controller(current_state, desired_state);
%     
%     %Draw_Simulation(current_state, desired_state);
% end

end

