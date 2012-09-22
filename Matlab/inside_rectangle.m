function [ output ] = inside_rectangle( x, y, r )
%INSIDE_RECTANGLE Summary of this function goes here
%   Detailed explanation goes here

 for i=1:length(r)
    rect = r{i};
    xmin = rect(1);
    ymin = rect(2);
    xmax = xmin + rect(3);
    ymax = ymin + rect(4);
    
    if ((x > xmin) && (x < xmax) && (y > ymin) && (y < ymax))
        output = 1;
        return;
    else
        output = 0;
    end
        
 end

end

