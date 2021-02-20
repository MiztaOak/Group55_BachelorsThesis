clear
clc, clf
p=zeros(2,2);
q=zeros(2,2);
v=zeros(2,2);
u=zeros(2,2);
p(:,1)=[50*rand; 50*rand];
q(:,1)=[50*rand; 50*rand];
theta=2*pi*rand;
T_rot=[cos(theta), -sin(theta) ; sin(theta), cos(theta)];
v(:,1)=T_rot*[0.5;0];
u(:,1)=T_rot*[0.5;0];
cnc=@(x) 1/((x(1)-5)^2+(x(2)-5)^2);
plot(5,5,'o')
hold on

for i=2:500
    X=5;
    p(:,i)=p(:,i-1)+v(:,i-1);
    theta=2*pi*rand;
    T_rot=[cos(theta), -sin(theta) ; sin(theta), cos(theta)]
    if (cnc(p(:,i))/cnc(p(:,i-1))<1)
        X=X+2;
    else
        X=X-2;
    end
    if 10*rand<X
        v(:,i)=T_rot*v(:,i-1);
    else
        v(:,i)=v(:,i-1);
    end
end
for i=2:500
    X=5;
    q(:,i)=q(:,i-1)+u(:,i-1);
    theta=2*pi*rand;
    T_rot=[cos(theta), -sin(theta) ; sin(theta), cos(theta)]
    if (cnc(q(:,i))/cnc(q(:,i-1))<1)
        X=X+2;
    else
        X=X-2;
    end
    if 10*rand<X
        u(:,i)=T_rot*u(:,i-1);
    else
        u(:,i)=u(:,i-1);
    end
end

for j=1:500
    axis([-10,50,-10,50])
    %plot(p(1,j),p(2,j),'.')
    %plot(q(1,j),q(2,j),'.')
    plot(p(1,j),p(2,j),'g.',q(1,j),q(2,j),'b.')
    pause(0.05)
end
 

    