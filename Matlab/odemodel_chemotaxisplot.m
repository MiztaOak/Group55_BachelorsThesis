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

n=100;
%L=linspace(0.1015,0.1025,n)
%L=linspace(0.0875,0.13,n);


%Ett område på 10X10 med näringscentrum i [5,5]
%Önskar ett L i området [0.01 , 1.00]
hold on
%theta=linspace(0,2*pi);
%plot(10*cos(theta)+5,10*sin(theta)+5);
axis([-2,2,-2,2])
plot(0,0,'o')
%bias=1/(1+3/7*(Yp/Yt)^(5.5));
d=0.5;
L=@(var1,var2)0.0409+exp(-(sqrt((var1).^2+(var2).^2))./d);
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
%q=0.4168; %experimental ratio
q=0.1835;
Ap0=q*At;
Yp0=q*Yt;
Bp0=q*Bt;
m0=5;
Y0=[Ap0;Yp0;Bp0;m0];
m=1000;
tspan=linspace(0,200,m);
%bias=zeros(1,15);

xx=-2+(2+2)*rand;
yy=-2+(2+2)*rand;
u=[xx;yy];
c=0.03;
theta_n=2*pi*rand;
vx=c*sin(theta_n);
vy=c*cos(theta_n);
v=[vx;vy];
plot(xx,yy,'x','MarkerSize',10)

for i=1:50000
    
    plot(xx,yy,'.','MarkerSize',20)
    pause(0.01)
%System of ODE:s
odesystem1=@(t,y)[(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon))))))*k1*(At-y(1))-k2*y(1)*(Yt-y(2))-k3*y(1)*(Bt-y(3));
    k2*y(1)*(Yt-y(2))-k4*y(2)*Zt-k6*y(2);
    k3*y(1)*(Bt-y(3))-k5*y(3);
    gR*Rt*(1-(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon)))))))-gB*y(3)^2*(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon))))))];

[t,y]=ode15s(odesystem1,tspan,Y0);

summa=sum(y(:,2))/m;
kvot=summa/Ap0;
bias(i,1)=1/(1+10*kvot^(9.5));
%if L(xx,yy)<=0.1025
%    Y=max(y(:,2));
%else
%    Y=min(y(:,2));
%end

%a=2;
%b=2.5;
%bias(i,1)=1/(1+a*(Y/Yt)^b);

if bias(i,1)>rand %tumble
    v=T_rot(pi*(rand-0.5))*v;

end
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
    v(1)=-v(1)
end
   
end
%plot(xx,yy,'.','MarkerSize',20)