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

n=1000;
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
d=1;
L=@(var1,var2).05+.05*exp(-(sqrt((var1).^2+(var2).^2))./d);
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


%h=@(x).3+(.7./(1+(10./x).^3));
%h=@(x).1+1./(1+(3./x).^3);
h=@(x)2./(1+(6./x).^3);

%A=linspace(0,9.7*2);
%plot(A,h(A))

%Initial values (arbitrary)
%q=0.4168; %experimental ratio
q=0.1835;
Ap0=q*At;
Yp0=q*Yt;
Bp0=q*Bt;
m0=5;
%S0=0.5;
Y0=[Ap0;Yp0;Bp0;m0];
m=100;
tspan=linspace(0,0.1,m);
tplot=tspan;
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
Q(1)=Yp0;

for i=1:n
    
    plot(xx,yy,'.','MarkerSize',20)
    pause(0.02)
%System of ODE:s
odesystem1=@(t,y)[(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon))))))*k1*(At-y(1))-k2*y(1)*(Yt-y(2))-k3*y(1)*(Bt-y(3));
    k2*y(1)*(Yt-y(2))-k4*y(2)*Zt-k6*y(2);
    k3*y(1)*(Bt-y(3))-k5*y(3);
    gR*Rt*(1-(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon)))))))-gB*y(3)^2*(1/(1+exp(N*(1-y(4)/2+log((1+L(xx,yy)/Kaoff)/(1+L(xx,yy)/Kaon))))))];
    %h(y(2))*(1-y(5))];

[t,y]=ode15s(odesystem1,tspan,Y0);
Y0=y(m,:);

hold on


r(i)=sqrt(xx^2+yy^2);

tplot=tplot(end)+tspan;

S(i+1)=h(y(m,2));
Q(i+1)=y(m,2);
R(i)=(y(m,2)/Yp0)^2;
if Q(i+1)>Q(i)
    X=.7;
    
else
    X=.3;
end
    
if X>rand
    v=T_rot(pi*(rand-0.5))*v;
end
%if S(i)>rand %tumble
 %   v=T_rot(pi*(rand-0.5))*v;
    %disp('Tumble')
%end

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
%plot(linspace(0,500,n),Q/Yp0)
%subplot(2,2,[3,4])
%plot(linspace(0,500,n),r)
%%
d2=1;
L2=@(r).05+.05*exp(-r./d2);
r=linspace(0,sqrt(8));
plot(r,L2(r))
%plot(xx,yy,'.','MarkerSize',20)
%%
%%
clc, clf
h=@(x)2./(1+(6./x).^3);
%h=@(x).5+.5./(1+(10./x).^3);
%h=@(x).3+(.7./(1+(10./x).^3));
A=linspace(0,6);
plot(A,h(A))
plot(linspace(0,500,1000),Q)
%%
clear
clc, clf
X=linspace(0,1);
h1=@(x)log(x+1)/log(2);
h2=@(x)sqrt(x);
h3=@(x)x.^2;
h4=@(x)sin(pi*x/2);
h5=@(x)atan(pi*x/2);
h6=@(x)x.^(1/3);
h7=@(x)x.^(1/4);
h8=@(x)2./(1+(1./x));
h9=@(x)2./(1+(1./x.^(2)));
h10=@(x)2./(1+(1./x.^3));
h11=@(x)(3/2)./(1+(1/2)*(1./x.^3));
h12=@(x)(6/5)./(1+(1/5)*(1./x.^3));
h13=@(x)(10/9)./(1+(1/9)*(1./x.^3));


%S_dot=@(S)h(y_p)*(1-S);
 
subplot(2,2,1)
plot(X,h1(X),X,h4(X),X,h5(X))
legend('ln(x+1)/ln(2)','sin(pi*x/2)','arctan(x)','Location','southeast')
subplot(2,2,2)
plot(X,h2(X),X,h3(X),X,h6(X),X,h7(X))
legend('sqrt(x)','x^2','x^{1/3}','x^{1/4}','Location','southeast')
subplot(2,2,3)
plot(X,h8(X),X,h9(X),X,h10(X))
legend('2x/(1+x)','2x/(1+x^2)','2x/(1+x^3)','Location','southeast')
subplot(2,2,4)
plot(X,h11(X),X,h12(X),X,h13(X))
legend('h11','h12','h13','Location','southeast')


