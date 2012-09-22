function [ ] = Draw_Simulation( current_state, desired_state )
%DRAW_SIMULATION Summary of this function goes here
%   Detailed explanation goes here


auv_scale = .5;

for i=1:size(current_state, 1)
    
    off_x = current_state(i, 1);
    off_y = current_state(i, 2);
    rotation = current_state(i, 3);
    hold on;
    [auv, ~, auv_alpha] = imread('AUV.png');
    auv = imresize(auv, auv_scale);
    auv_alpha = imresize(auv_alpha, auv_scale);
    label = strcat('AUV ', int2str(i));
    hnd1=text(off_x, off_y, label);
    set(hnd1,'FontSize',16)
    set(hnd1,'Color','White')
    auvh = image(off_x, off_y, imrotate(auv, rotation));
    set(auvh, 'AlphaData', imrotate(auv_alpha, rotation));
    hold off; 
    refreshdata
    drawnow
    
end

for i=1:size(desired_state, 1)
    
    off_x = desired_state(i, 1);
    off_y = desired_state(i, 2);
    rotation = desired_state(i, 3);
    hold on;
    [auv, ~, auv_alpha] = imread('AUV.png');
    auv = imresize(auv, auv_scale);
    auv_alpha = imresize(auv_alpha, auv_scale);
    label = strcat('Desired ', int2str(i));
    hnd1=text(off_x, off_y, label);
    set(hnd1,'FontSize',16)
    set(hnd1,'Color','White')
    auvh = image(off_x, off_y, imrotate(auv, rotation));
    set(auvh, 'AlphaData', imrotate(auv_alpha, rotation));
    hold off; 
end


end

