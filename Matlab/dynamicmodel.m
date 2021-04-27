%Dirichlet BC

clc, clf
clear

xlength=4; %length of area
nx=20; %number of grids per side
delta_x=xlength/nx; %spatial step
n=(nx-1)^2;
nt=100;
t=100;
delta_t=t/nt;
D=.006;
d=D*delta_t/(delta_x^2);
beta=2*(1+d);
gamma=2*(1-d);
nodes=nx+1;
nodesspace=linspace(-2,2,nodes);
L=@(x,y)7*exp(-norm([x,y])^2/3);
C0=zeros(nx+1);
C=C0;

A=zeros(n)+beta*eye(n);
A(1,2)=-d; A(n,n-1)=-d;
k=2;
for i=2:n-1
    if k==(nodes-2)
        A(i,i-1)=-d;
        k=k+1;
    else if k==(nodes-1)
            A(i,i+1)=-d;
            k=2;
        else
            A(i,i-1)=-d; A(i,i+1)=-d;
            k=k+1;
        end
    end
end
  

B=zeros(n)+gamma*eye(n);

for i=1:n
    if i<=(nodes-2)
        B(i,i+nodes-2)=d;
    else if i>(n-nodes+2)
            B(i,i-(nodes-2))=d;
        else
            B(i,i+nodes-2)=d;
            B(i,i-(nodes-2))=d;
        end        
    end
end

for i=2:nx
    for j=2:nx
    C(i,j)=L(nodesspace(i),nodesspace(j));
    end
end

uA=zeros(n-1,1);
for m=1:n-1
    uA(m)=A(m+1,m);
end
lA=uA;
mA=diag(A);

%iteration t=[0,0.5*delta_t]  A*c_(n+1)=B*c_n

for q=1:t
[X,Y]=meshgrid(linspace(-2,2,nodes),linspace(-2,2,nodes));
surf(X,Y,C)
%surf(linspace(-2,2,nodes),linspace(-2,2,nodes),C)
axis([-2 2 -2 2 0 7])
pause(0.5)
i=1;
for j=2:(nodes-1)
    for k=2:(nodes-1)
        dd(i,1)=C(j,k);
        i=i+1;
    end
end

y0=B*dd;

c_prim=zeros(n-1,1);
c_prim(1)=uA(1)/mA(1);

for i=2:n-1
    c_prim(i)=uA(i)/(mA(i)-lA(i-1)*c_prim(i-1));
end

d_prim=zeros(n,1);
d_prim(1)=y0(1)/mA(1);

for i=2:n
    d_prim(i)=(y0(i)-lA(i-1)*d_prim(i-1))/(mA(i)-lA(i-1)*c_prim(i-1));
end

x=zeros(n,1);
x(n)=d_prim(n);
for i=1:n-1
    x(n-i)=d_prim(n-i)-c_prim(n-i)*x(n-i+1);
    
end
dd=x;

i=1;
for j=2:(nodes-1)
    for k=2:(nodes-1)
        C(j,k)=dd(i);
        i=i+1;
    end
end

%iteration t=[0.5*delta_t, delta_t]

i=1;
for j=2:(nodes-1)
    for k=2:(nodes-1)
        dd(i)=C(k,j);
        i=i+1;
    end
end

y0=B*dd;   
c_prim=zeros(n,1);
c_prim(1)=uA(1)/mA(1);

for i=2:n-1
    c_prim(i)=uA(i)/(mA(i)-lA(i-1)*c_prim(i-1));
end

d_prim=zeros(n,1);
d_prim(1)=y0(1)/mA(1);

for i=2:n
    d_prim(i)=(y0(i)-lA(i-1)*d_prim(i-1))/(mA(i)-lA(i-1)*c_prim(i-1));
end
x=zeros(n,1);
x(n)=d_prim(n);
for i=1:n-1
    x(n-i)=d_prim(n-i)-c_prim(n-i)*x(n-i+1);
    
end
dd=x;

i=1;
for j=2:(nodes-1)
    for k=2:(nodes-1)
        C(j,k)=dd(i);
        i=i+1;
    end
end
end
%% 
% Neumann BC
clc, clf
clear

xlength=4; %length of area
nx=20; %number of grids per side
delta_x=xlength/nx; %spatial step
n=(nx+1)^2;
nt=100;
t=100;
delta_t=t/nt;
D=.005;
d=D*delta_t/(delta_x^2);
beta=2*(1+d);
gamma=2*(1-d);
nodes=nx+1;
nodesspace=linspace(-2,2,nodes);
L=@(x,y)7*exp(-norm([x,y])^2/3);
C0=zeros(nx+1);
C=C0;

T_rot=@(phi)[cos(phi), -sin(phi) ; sin(phi), cos(phi)];
bacteria=10;
speed=0.035;
W=5*ones(1,bacteria);
%U=zeros(n,bacteria);
for i=1:bacteria
    px=-2+(4)*rand;
    py=-2+(4)*rand;
    p(:,i)=[px;py];
    theta_n=2*pi*rand;
    vx=speed*sin(theta_n);
    vy=speed*cos(theta_n);
    v(:,i)=[vx;vy];
    R(i)=10*rand;
end
%p=[-2,-2];


%h=@(x).02+.5*x;



A=zeros(n)+beta*eye(n);
A(1,2)=-2*d; A(n,n-1)=-2*d;
k=2;
for i=2:n-1
    if k==nodes
        A(i,i-1)=-2*d;
        k=k+1;
    elseif k==(nodes+1)
            A(i,i+1)=-2*d;
            k=2;
        else
            A(i,i-1)=-d; A(i,i+1)=-d;
            k=k+1;
        end
end
  
B=zeros(n)+gamma*eye(n);
B(1,nodes+1)=2*d; B(n,n-nodes)=2*d;
%B(nodes,2*nodes)=d; B(n-nodes,n-2*nodes)=d;

k=2;
for i=2:n-1
    if i>nodes && i<=n-nodes
        B(i,i-nodes)=d; B(i,i+nodes)=d;
    elseif i<=nodes
        B(i,i+nodes)=2*d;
    elseif i>n-nodes
        B(i,i-nodes)=2*d;
    end
end
   


%for i=2:nodes-2
%    B(i*nodes,(i-1)*nodes)=2*d;
%    B(i*nodes,(i+1)*nodes)=0;
%    B(i*nodes+1,(i+1)*nodes+1)=2*d;
%    B(i*nodes+1,(i-1)*nodes+1)=0;
%end
    
    

for i=1:nodes
    for j=1:nodes
    C(i,j)=L(nodesspace(i),nodesspace(j));
    
    end
end
U=zeros(bacteria,2);

for m=1:bacteria

for j=1:nodes
    for k=1:nodes
        %if sqrt((nodesspace(j)-p(1,m))^2+(nodesspace(k)-p(2,m))^2)<.5*delta_x
        %    U(m,1)=C(j,k);
        %end
        
        U(m,1)=U(m,1)+C(j,k)*exp(-((nodesspace(j)-p(1,m))^2+(nodesspace(k)-p(2,m))^2));
    end
end
end


uA=zeros(n-1,1);
lA=zeros(n-1,1);
for m=1:n-1
    uA(m)=A(m,m+1);
    lA(m)=A(m+1,m);
end

mA=diag(A);

%iteration t=[0,0.5*delta_t]  A*c_(n+1)=B*c_n

for q=1:2*nt
[X,Y]=meshgrid(linspace(-2,2,nodes),linspace(-2,2,nodes));
hold off
surf(X,Y,C)
hold on
plot(p(2,:),p(1,:),'.')
axis([-2 2 -2 2 0 7])
pause(0.5)
f=@(x,y,pos)exp(-30*((x-pos(1))^2+(y-pos(2))^2));
F=zeros(n,1);
for m=1:bacteria
    k=1;
    for i=1:nodes
        for j=1:nodes
            H(k,1)=.15*f(nodesspace(i),nodesspace(j),p(:,m));
            %G(i,j)=.05*f(nodesspace(i),nodesspace(j),p(:,m));
            k=k+1;
        end
    end
    F=F+H;
end

i=1;
for j=1:nodes
    for k=1:nodes
        dd(i,1)=C(j,k);
        i=i+1;
    end
end


y0=B*dd-F;

for i=1:length(y0)
    if y0(i)<0
        y0(i)=0;
    end
end

c_prim=zeros(n-1,1);
c_prim(1)=uA(1)/mA(1);

for i=2:n-1
    c_prim(i)=uA(i)/(mA(i)-lA(i-1)*c_prim(i-1));
end

d_prim=zeros(n,1);
d_prim(1)=y0(1)/mA(1);

for i=2:n
    d_prim(i)=(y0(i)-lA(i-1)*d_prim(i-1))/(mA(i)-lA(i-1)*c_prim(i-1));
end

x=zeros(n,1);
x(n)=d_prim(n);
for i=1:n-1
    x(n-i)=d_prim(n-i)-c_prim(n-i)*x(n-i+1);
    
end
dd=x;

i=1;
for j=1:nodes
    for k=1:nodes
        C(j,k)=dd(i);
        i=i+1;
    end
end

%iteration t=[0.5*delta_t, delta_t]

i=1;
for j=1:nodes
    for k=1:nodes
        dd(i)=C(k,j);
        i=i+1;
    end
end
F=zeros(n,1);
for m=1:bacteria
    k=1;
    for i=1:nodes
        for j=1:nodes
            H(k,1)=.15*f(nodesspace(j),nodesspace(i),p(:,m));
            %G(i,j)=.05*f(nodesspace(i),nodesspace(j),p(:,m));
            k=k+1;
        end
    end
    F=F+H;
end

y0=B*dd-F;
for i=1:length(y0)
    if y0(i)<0
        y0(i)=0;
    end
end
c_prim=zeros(n,1);
c_prim(1)=uA(1)/mA(1);

for i=2:n-1
    c_prim(i)=uA(i)/(mA(i)-lA(i-1)*c_prim(i-1));
end

d_prim=zeros(n,1);
d_prim(1)=y0(1)/mA(1);

for i=2:n
    d_prim(i)=(y0(i)-lA(i-1)*d_prim(i-1))/(mA(i)-lA(i-1)*c_prim(i-1));
end
x=zeros(n,1);
x(n)=d_prim(n);
for i=1:n-1
    x(n-i)=d_prim(n-i)-c_prim(n-i)*x(n-i+1);
    
end
dd=x;

i=1;
for j=1:nodes
    for k=1:nodes
        C(k,j)=dd(i);
        i=i+1;
    end
end

for m=1:bacteria
    for j=1:nodes
        for k=1:nodes
            %if ((nodesspace(j)-p(1,m))^2+(nodesspace(k)-p(2,m))^2)<.5*delta_x
            %    U(m,2)=C(j,k);
            %else
            %    U(m,2)=U(m,1);
            %end
            
            U(m,2)=U(m,2)+C(j,k)*exp(-((nodesspace(j)-p(1,m))^2+(nodesspace(k)-p(2,m))^2));
        end
    end
end

for i=1:bacteria
    if U(i,2)>U(i,1)
        W(i)=3;
    else
        W(i)=7;
    end
    if W(i)>R(i)
        angle=(2*(rand>.5) - 1)*(pi/10+(49*pi/90)*rand);
        v(:,i)=T_rot(angle)*v(:,i);
    end
    R(i)=10*rand;
    W(i)=5;
end
U(:,1)=U(:,2);
U(:,2)=zeros(bacteria,1);
p=p+v;

for i=1:bacteria
    if p(1,i)>2 
        p(1,i)=2;
        v(1,i)=-v(1,i);
    end
    if p(2,i)>2
        p(2,i)=2;
        v(2,i)=-v(2,i);
    end
    if p(2,i)<-2
        p(2,i)=-2;
        v(2,i)=-v(2,i);
    end
    if p(1,i)<-2
        p(1,i)=-2;
        v(1,i)=-v(1,i);
    end
end
end


