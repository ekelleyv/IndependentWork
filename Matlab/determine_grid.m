function [ x_grid, y_grid ] = determine_grid( grid_width, x_pos, y_pos )
%DETERMINE_GRID Summary of this function goes here
%   Detailed explanation goes here
    x_grid = ceil(x_pos/grid_width);
    y_grid = ceil(y_pos/grid_width);

end

