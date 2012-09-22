function TimClock
% Tim Farajian, March 2003

%%%%%%%% Create Figure and Graphical objects %%%%%%%%
% Figure %
fclr = .9*[1 1 1];
fHnd = figure('double','on', 'Units', 'Normalized','DeleteFcn',@deletefcn ,'Name', 'TimClock', 'menu','none' ,'numbertitle','off','color',fclr);
% Axis *
aHnd = gca;
set(aHnd, 'Pos', [0 0 1 1], 'Visible', 'off', 'XDir','reverse', 'Units', 'Normalized', 'FontUnits', 'Normalized','XLim',[0 20],'YLim',[0 20]);
hold on
% Edit boxes %
g = 9.8; spin = 2.5; ymax = 15; % Initial values
uicontrol('Style','edit', 'Units', 'Normalized', 'Pos', [.85 .85 .06 .05], 'String', num2str(g)        , 'horiz','center', 'FontUnits', 'Normalized', 'fontsize', 8, 'callb',@EditCallback,'Tag','G');
uicontrol('Style','edit', 'Units', 'Normalized', 'Pos', [.85 .75 .06 .05], 'String', num2str(spin)     , 'horiz','center', 'FontUnits', 'Normalized', 'fontsize', 8, 'callb',@EditCallback,'Tag','R');
uicontrol('Style','edit', 'Units', 'Normalized', 'Pos', [.85 .65 .06 .05], 'String', num2str(ymax - 10), 'horiz','center', 'FontUnits', 'Normalized', 'fontsize', 8, 'callb',@EditCallback,'Tag','M');
% Labels for edit boxes %
uicontrol('Style','text', 'Units', 'Normalized', 'Pos', [.83 .91 .1 .03], 'String','Gravity'     , 'horiz','center', 'FontUnits', 'Normalized','backg',fclr);
uicontrol('Style','text', 'Units', 'Normalized', 'Pos', [.83 .81 .1 .03], 'String','# of Rot''ns', 'horiz','center', 'FontUnits', 'Normalized','backg',fclr);
uicontrol('Style','text', 'Units', 'Normalized', 'Pos', [.83 .71 .1 .03], 'String','Peak height' , 'horiz','center', 'FontUnits', 'Normalized','backg',fclr);
% Stack boundaries %
plot(10+[0 0 2 2], [0 10 10 0],'k')
plot(13+[0 0 2 2], [0 10 10 0],'k')
plot(16+[0 0 2 2], [0 10 10 0],'k')
% Text for minutes and seconds stacks %
text(12.5*[1 1 1 1], 2.5*[1 2 3 4],{'15','30','45','60'}, 'FontUnits', 'Normalized', 'horiz','center')
% Text for hour stack %
HtxtHnd = text(17,3,'1','fontsize',20, 'horiz','center', 'FontUnits', 'Normalized');
% Create block patch %
bHnd = patch([0 2 2 0], [0 0 1 1], [0 0 0 0], 'b');


%%%%%%%% Set constants %%%%%%%%%
sP = [10 13 16]; % Horizontal positions of stacks
sC = [1/6 1/6 5/6]; % Vertical conversion factors of stacks
Xo = [0; 0]; Vo = [0; 0]; % Translation position and velocity
A = 0; W = 0; % Rotation position and velocity
bHt = 0; sHt = 0; stackflag = false; % Seconds stack position and vel
gT = g;  % For memory if the edit box gets changed in mid-flight
cT = clock; % Get current time

% Create stack rectangles %
SstHnd = rectangle('Pos',[sP(1) 0 2 1],'facec','b');
MstHnd = rectangle('Pos',[sP(2) 0 2 1],'facec','b');
HstHnd = rectangle('Pos',[sP(3) 0 2 1],'facec','b');

% Define variables in main function %
fT = [];  sHt0 = []; SecVel = []; To = [];

UpdateStacks
InitializeBlock

T = timer('TimerFcn',@MoveBlock, 'Period', 0.001,'executionmode','fixedSpacing');
start(T)


    function MoveBlock(varargin)
        cT = clock; % Get current time
        t = etime(cT, To); % Time since block started flying
        if t < fT
            % Update block to new position %
            X = Xo + Vo*t - [0; g/2]*t^2; % Calculate new translational pos
            A = W*t; % Calculate new rotational position
            % Set block translational position
            set(bHnd, {'XData', 'YData'}, {X(1) + [0 0 2 2], X(2) + [0 bHt bHt 0]});
            % Set block translational position
            rotate(bHnd, [0; 0; 1], A, [X+[1;bHt/2]; 0]);
            if stackflag
                % If seconds stack is moving %
                sHtmp = sHt0 + SecVel*t; % Calculate new stack height
                set(SstHnd, 'Pos',[sP(1) 0 2 sHtmp]); % Set stack height
            end
            drawnow
        else
            % Block reached stack %
            sHt = sHt + bHt; % Update seconds stack size
            UpdateStacks
            if sHt>60*sC(1); % If seconds stack is more than 60 seconds
                sHt0 = sHt; sHt = 0; % Remember stack height then reset
                stackflag = true; % Indicate seconds stack is moving
            else
                stackflag = false; % Indicate seconds stack is not moving
            end
            InitializeBlock
        end
    end


    function UpdateStacks
        %%%%% Update seconds, minutes, and hours stacks %%%%%
        cT(4) = mod(cT(4) - 1, 12) + 1;  % Ensure hour is not greater than 12
        set(SstHnd,'position',[sP(1) 0 2 max(sHt, .01)]); % Seconds stack
        set(MstHnd,'position',[sP(2) 0 2 max(cT(5)*sC(2), .01)]); % Minutes stack
        set(HstHnd,'position',[sP(3) 0 2 cT(4)*sC(3)]); % Hours stack
        set(HtxtHnd,'String',num2str(cT(4)),'pos',[sP(3)+1; cT(4)*sC(3)+.75]); %Hours text
        drawnow
    end


    function InitializeBlock
        g = gT; % Update if edit box got changed
        if stackflag
            ymax2 = ymax - sHt;
        else
            ymax2 = ymax - sHt;
        end
        fT = sqrt(2/g)*(sqrt(ymax - Xo(2)) + sqrt(ymax2)); % Flight time
        bHt = mod((fT + cT(6))*sC(1) - sHt, 60*sC(1)); % Block height
        Vo = [(sP(1) - Xo(1))/fT; sqrt(2*g*(ymax - Xo(2)))]; % Trans. velocity
        W = -spin*360/fT; % Angular velocity
        SecVel = -sHt0/fT; % Stack velocity
        To = clock; % Restart timer
    end


    function EditCallback(h, varargin)
        %%%% Editbox callback function %%%%
        box = get(h, 'Tag'); % Get tag of calling editbox
        val = str2num(get(h, 'String')); % Get value of calling editbox
        % Set appropriate constant %
        switch box
            case 'R'
                spin = val;
            case 'G'
                gT = val;
            case 'M'
                ymax = val + 10;
        end
    end

    function deletefcn(varargin)
        % Figure's DeleteFcn %
        stop(T) % Stop timer
        delete(T) % Delete timer
    end

end

