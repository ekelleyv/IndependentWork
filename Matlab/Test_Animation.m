function [ ] = Test_Animation( max_time )
%TEST_ANIMATION Summary of this function goes here
%   Detailed explanation goes here
time_start = clock;
figure('color', 'blue');
axis off, axis equal
hold on 

auv_scale = .5;

current_state = [0, 0, 0];

while ((clock - time_start) < max_time)
    current_state(1) = current_state(1) + 1;
    current_state(2) = current_state(2) + 2;
    current_state(3) = current_state(3) + 15;
    off_x = current_state(1);
    off_y = current_state(2);
    rotation = current_state(3);
    [auv, ~, auv_alpha] = imread('AUV.png');
    auv = imresize(auv, auv_scale);
    auv_alpha = imresize(auv_alpha, auv_scale);
    auvh = image(off_x, off_y, imrotate(auv, rotation));
    set(auvh, 'AlphaData', imrotate(auv_alpha, rotation));
    refreshdata(auvh);
    drawnow
    
end


end

