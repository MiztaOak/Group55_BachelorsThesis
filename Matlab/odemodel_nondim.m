clear
clc, clf

%kinetic constants
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

N=30; %Number of (Tar) receptors

%L1=100; %Ligand concentration
%L2=0.01;

%Receptor constants
Kaon=0.5*1e3; %Dissociation constant of active (Tar) receptor
Kaoff=0.02*1e3; %Dissociation constant of inactive (Tar) receptor

%Total concentration (micromolar)
At=7.9;
Yt=9.7;
Bt=0.28;
Rt=0.16;
Zt=3.8;

%Initial values (arbitrary)
q=0; %experimental ratio
Ap0=0;
Yp0=0;
Bp0=0;
m0=5;
S0=0;
Y0=[m0;Ap0;Yp0;Bp0;S0];
tspan=[0 1];
tplot=0;

d=1;
%L=@(var1,var2).1+6.9*exp(-(sqrt((var1).^2+(var2).^2))./d);
L=@(var1,var2)7*exp(-norm([var1,var2])^2/3);
T_rot=@(phi)[cos(phi), -sin(phi) ; sin(phi), cos(phi)];
n=500;

px=-2+(4)*rand;
py=-2+(4)*rand;
%px=-2;
%py=0;


c=0.02;
theta_n=2*pi*rand;
vx=c*sin(theta_n);
vy=c*cos(theta_n);
%vx=c;
%vy=0;
v=[vx;vy];

u=.8*rand;

a=1;
b=1;
%h=@(x)a./(1+exp(-b.*(x-(Yp0-.5))));
%h=@(x)1./(1+exp(-20.*(x-.5)));
h=@(x).02+.5*x;
%h=@(x)60*x.^5;
%h=@(x).02+.75*x.^(3);
r(1)=sqrt(px^2+py^2);
X(1)=px;
Y(1)=py;


for i=1:n
%System of ODE:s
odesystem=@(t,y)[gammaR*(1-(1/(1+exp(N*(1-(y(1)/2)+log((1+L(px,py)/Kaoff)/(1+L(px,py)/Kaon)))))))-gammaB*(y(4)^2)*(1/(1+exp(N*(1-(y(1)/2)+log((1+L(px,py)/Kaoff)/(1+L(px,py)/Kaon))))));
    (1/(1+exp(N*(1-(y(1)/2)+log((1+L(px,py)/Kaoff)/(1+L(px,py)/Kaon))))))*k1*(1-y(2))-k2*(1-y(3))*y(2)-k3*(1-y(4))*y(2);
    alpha1*k2*(1-y(3))*y(2)-(k4+k6)*y(3);
    alpha2*k3*(1-y(4))*y(2)-k5*y(4);
    h(y(3))*(1-y(5))];

[t,y]=ode15s(odesystem,tspan,Y0);
Y0=y(end,:);
r(i+1)=sqrt(px^2+py^2);
U(i+1)=u;


subplot(4,4,[1,2]) %m
plot(t+(i-1),y(:,1))
hold on
xlim([0 n])
ylim([4 6])
title('Methylation')

subplot(4,4,[3,4]) %CheA-P
plot(t+(i-1),y(:,2))
hold on
xlim([0 n])
ylim([-.2 .5])
title('CheA-P')

subplot(4,4,[5,6]) % CheyY-P
plot(t+(i-1),y(:,3))
xlim([0 n])
ylim([0.3 .65])
title('CheY-P')
hold on

subplot(4,4,[7,8]) %CheB-P
plot(t+(i-1),y(:,4))
hold on
xlim([0 n])
ylim([-.2 1])
title('CheB-P')

subplot(4,4,[9,10]) %r
plot(t+(i-1),linspace(r(i),r(i+1),length(t)))
hold on
xlim([0 n])
ylim([0 sqrt(8)])
title('Distance from center')






pause(0.005)


if y(end,5)>u
    angle=(2*(rand>.5) - 1)*(pi/10+(49*pi/90)*rand);
    v=T_rot(angle)*v;
    Y0(5)=0;
    u=.8*rand;
end

px=px+v(1);
py=py+v(2);
X(i+1)=px;
Y(i+1)=py;

subplot(4,4,[11,12])
plot(t+(i-1),linspace(L(X(i),Y(i)),L(X(i+1),Y(i+1)),length(t)))
hold on
xlim([0 n])
ylim([-.2 7])
title('Ligand concentration')

%subplot(4,4,[13,14])
%plot(t+(i-1),y(:,5))
%xlim([0,n])
%ylim([0,1])
%hold on
%title('S')

%subplot(4,4,[15,16])
%plot(t+(i-1),linspace(U(i),U(i+1),length(t)))
%xlim([0,n])
%ylim([0,1])
%hold on
%title('u')

if px>2 
    px=2;
    v(1)=-v(1);
end
if py>2
    py=2;
    v(2)=-v(2);
end
if py<-2
    py=-2;
    v(2)=-v(2);
end
if px<-2
    px=-2;
    v(1)=-v(1);
end

%legend('CheA-P','CheY-P','CheB-P','M')
end
%%
clc, clf
clear

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
%Receptor constants
Kaon=0.5; %Dissociation constant of active (Tar) receptor
Kaoff=0.02; %Dissociation constant of inactive (Tar) receptor
N=18; %Number of (Tar) receptors
Y0=[5 0 0 0];
tspan=linspace(0,1,100);
L=7;

odesystem=@(t,y)[gammaR*(1-(1/(1+exp(N*(1-(y(1)/2)+log((1+L/Kaoff)/(1+L/Kaon)))))))-gammaB*(y(4)^2)*(1/(1+exp(N*(1-(y(1)/2)+log((1+L/Kaoff)/(1+L/Kaon))))));
    (1/(1+exp(N*(1-(y(1)/2)+log((1+L/Kaoff)/(1+L/Kaon))))))*k1*(1-y(2))-k2*(1-y(3))*y(2)-k3*(1-y(4))*y(2);
    alpha1*k2*(1-y(3))*y(2)-(k4+k6)*y(3);
    alpha2*k3*(1-y(4))*y(2)-k5*y(4)];

[t,y]=ode15s(odesystem,tspan,Y0);

plot(t,y(:,3))
%%
clc, clf
clear
h=@(x).02+.75*x.^(3);
x1=0.35;
x2=0.45;
odefun1=@(t,y)h(x1)*(1-y);
odefun2=@(t,y)h(x2)*(1-y);
tspan=[0 10];
y0=0;
[t1,y1]=ode45(odefun1,tspan,y0)
[t2,y2]=ode45(odefun2,tspan,y0)
plot(t1,y1,t2,y2)
legend('x=0.35','x=0.45')