clc, clf
clear

xlength=28; %length of area
nx=40; %number of grids per side
delta_x=xlength/nx; %spatial step
n=(nx+1)^2;
nt=500;
t_tot=500;
delta_t=t_tot/nt;
D=0;%.05;
d=D*delta_t/(delta_x^2);
beta=2*(1+d);
gamma=2*(1-d);
nodes=nx+1;
nodesspace=linspace(-xlength/2,xlength/2,nodes);
L=@(x,y)49*exp(-norm([(x),(y)])^2/100);%+7*exp(-norm([(x+1),(y+1)])^2/.4);
C=zeros(nx+1);
f=@(x,y,pos)exp(-30*((x-pos(1))^2+(y-pos(2))^2));
T_rot=@(phi)[cos(phi), -sin(phi) ; sin(phi), cos(phi)];
bacteria=20;
speed=0.14; %bacterial step length
W=0;%.5; %bacterial consumtion constant
conc=0;

k1=48.571; %CheA autophosphorylation
k2=1385.714; %phosphotransfer to CheY 
k3=6; %phosphotransfer to CheB
k4=8.686; %CheY-P deposphorylation by CheZ
k5=1; %Dephosphorylation of CheB-P
k6=0.121; %Dephosphorylation of CheY-P
gammaR=8.57*1e-3; %Methylation by CheR
gammaB=0.352; %Demethylation by CheB-P
alpha1=0.814;
alpha2=28.214;

%Initial values (arbitrary)
Ap0=0;
Yp0=0;
Bp0=0;
m0=5;
S0=0;

tspan=[0 1];
tplot=0;

N=30; %Number of (Tar) receptors
Kaon=0.5*1e3; %Dissociation constant of active (Tar) receptor
Kaoff=0.02*1e3; %Dissociation constant of inactive (Tar) receptor


h=@(x).02+.5*x;

 

for i=1:bacteria
    px=-14+(28)*rand;
    py=-14+(28)*rand;
    p(:,i)=[px;py];
    theta_n=2*pi*rand;
    vx=speed*sin(theta_n);
    vy=speed*cos(theta_n);
    v(:,i)=[vx;vy];
    Y0(i,:)=[m0;Ap0;Yp0;Bp0;S0];
    u(i)=.8*rand;
end

%Create LHS-matrix
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

%create RHS-matrix
B=zeros(n)+gamma*eye(n);
B(1,nodes+1)=2*d; B(n,n-nodes)=2*d;
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
      

for i=1:nodes
    for j=1:nodes
    C(i,j)=L(nodesspace(i),nodesspace(j));
    
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
plottest=Y0(:,3);
for q=1:t_tot
 
%[X,Y]=meshgrid(linspace(-14,14,nodes),linspace(-14,14,nodes));
%hold off
%surf(X,Y,C)
%hold on
%subplot(2,2,[1,2])
plot(p(2,:),p(1,:),'.')
axis([-14 14 -14 14 0 49])

%subplot(2,2,[3,4])
%plot([q-1 q],[plottest Y0(:,3)])
%axis([0 t_tot 0.35 0.65])
%plottest=Y0(:,3);
%hold on
%pause(0.5)

F=zeros(n,1);
for m=1:bacteria
    k=1;
    for i=1:nodes
        for j=1:nodes
            H(k,1)=W*f(nodesspace(i),nodesspace(j),p(:,m));
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
            H(k,1)=W*f(nodesspace(j),nodesspace(i),p(:,m));
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
    for k=1:nodes
        if p(1,m)<nodesspace(k)
           I=k;
           x1=abs(p(1,m)-nodesspace(k));
           x2=delta_x-x1;
           break
        end
    end
    for k=1:nodes
        if p(2,m)<nodesspace(k)
           J=k;
           y1=abs(p(2,m)-nodesspace(k));
           y2=delta_x-y1;
           break
        end
    end
    conc=(y1*(x2*C(I,J-1)+x1*C(I-1,J-1))+y2*(x2*C(I,J)+x1*C(I-1,J)))/(delta_x^2);
    Phi=@(x,y)(1/(1+exp(N*(1-(y/2)+log((1+conc/Kaoff)/(1+conc/Kaon))))));
    %System of ODE:s
    odesystem=@(t,y)[gammaR*(1-Phi(p(:,m),y(1)))-gammaB*(y(4)^2)*Phi(p(:,m),y(1)); %m
    Phi(p(:,m),y(1))*k1*(1-y(2))-k2*(1-y(3))*y(2)-k3*(1-y(4))*y(2);              %CheA-P
    alpha1*k2*(1-y(3))*y(2)-(k4+k6)*y(3);                                          %CheY-P
    alpha2*k3*(1-y(4))*y(2)-k5*y(4);                                               %CheB-P
    h(y(3))*(1-y(5))];    
    [t,y]=ode15s(odesystem,tspan,Y0(m,:));
    Y0(m,:)=y(end,:);
   
    if y(end,5)>u(m)
        angle=(2*(rand>.5) - 1)*(pi/10+(49*pi/90)*rand);
        v(:,m)=T_rot(angle)*v(:,m);
        Y0(m,5)=0;
        u(m)=.8*rand;
    end
    
end

p=p+v;

for i=1:bacteria
    if p(1,i)>14 
        p(1,i)=14;
        v(1,i)=-v(1,i);
    end
    if p(2,i)>14
        p(2,i)=14;
        v(2,i)=-v(2,i);
    end
    if p(2,i)<-14
        p(2,i)=-14;
        v(2,i)=-v(2,i);
    end
    if p(1,i)<-14
        p(1,i)=-14;
        v(1,i)=-v(1,i);
    end
end
end

