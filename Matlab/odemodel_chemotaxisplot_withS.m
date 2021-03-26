clear
clc, clf



%kinetic constants
k1=34; %CheA autophosphorylation
k2=100; %phosphotransfer to CheY 
k3=15; %phosphotransfer to CheB
k4=1.6; %CheY-P deposphorylation by CheZ
k5=0.7; %Dephosphorylation of CheB-P
k6=0.085; %Dephosphorylation of CheY-P
gR=0.0375; %Methylation by CheR
gB=3.14; %Demethylation by CheB-P

N=18; %Number of (Tar) receptors



%Ett område på 4X4 med näringscentrum i origo
hold on
%axis([-2,2,-2,2])
%plot(0,0,'o')

d=1;
L=@(var1,var2).1+6.9*exp(-(sqrt((var1).^2+(var2).^2))./d);

T_rot=@(phi)[cos(phi), -sin(phi) ; sin(phi), cos(phi)];


%Receptor constants
Kaon=0.5; %Dissociation constant of active (Tar) receptor
Kaoff=0.02; %Dissociation constant of inactive (Tar) receptor

%Total concentration (micromolar)
k=2;
At=k*7.9;
Yt=k*9.7;
Bt=k*0.28;
Rt=k*0.16;
Zt=k*3.8;

%Initial values (arbitrary)
q=0.1835;
Ap0=q*At;
Yp0=q*Yt;
Bp0=q*Bt;
m0=5;
S0=0;
Y0=[Ap0;Yp0;Bp0;m0;S0];
%m=100;
n=500;
%tspan=linspace(0,1,m);
tspan=[0,1];
%tplot=tspan;

xx=-2+(2+2)*rand;
yy=-2+(2+2)*rand;
u=[xx;yy];

%xx=-2;
%yy=-2;

c=0.03;
theta_n=2*pi*rand;
vx=c*sin(theta_n);
vy=c*cos(theta_n);
v=[vx;vy];

%v=c.*[1,0];
%vx=c.*cos(atan(2));
%vy=c.*sin(atan(2));
%v=[vx;vy];

%plot(xx,yy,'x','MarkerSize',10)

u=rand;

a=1;
b=1;
h=@(x)a./(1+exp(-b.*(x-(Yp0-.5))));
j=1;

for i=1:n
 
    %plot(xx,yy,'.','MarkerSize',20)
    %pause(0.02)
%System of ODE:s
odesystem1=@(t,y)[(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon))))))*k1*(At-y(1))-k2*y(1)*(Yt-y(2))-k3*y(1)*(Bt-y(3));
    k2*y(1)*(Yt-y(2))-k4*y(2)*Zt-k6*y(2);
    k3*y(1)*(Bt-y(3))-k5*y(3);
    gR*Rt*(1-(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon)))))))-gB*y(3)^2*(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon))))));
    h(y(2))*(1-y(5))];

[t,y]=ode15s(odesystem1,tspan,Y0);
S(i)=Y0(5);
%Y0=y(m,:);
Q(i)=Y0(2);
hold on
plot(t,y(:,2))
 %7*exp(-norm([x(1),x(2)]-[1 0])^2/3)

%if S(i)>u
%    v=T_rot(pi*(rand-0.5))*v;
%    Y0(5)=0;
%    u(j)=.4*rand;
%    j=j+1;
%end

r(i)=sqrt(xx^2+yy^2);

%tplot=tplot(end)+tspan;

xx=xx+v(1);
yy=yy+v(2);
if xx>2 
    xx=2;
    v(1)=-v(1);
end
if yy>2
    yy=2;
    v(2)=-v(2);
end
if yy<-2
    yy=-2;
    v(2)=-v(2);
end
if xx<-2
    xx=-2;
    v(1)=-v(1);
end
   
end
%subplot(2,2,[1,2])
%plot(linspace(0,50,n),Q)
%subplot(2,2,[3,4])
%plot(linspace(0,50,n),r)
%plot(xx,yy,'.','MarkerSize',20)
%%
clear
clc, clf
Yp0=3.5599;
Yp=linspace(0,1);
L=1;
k=20;
h=@(x)L./(1+exp(-k.*(x-.5)));
plot(Yp,h(Yp))

