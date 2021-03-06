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

n=30;
L=[linspace(.05,.1,10),linspace(.1,.05,10),linspace(.05,.1,10)];
%L=[.05,.055,.06,.065,.07,.065,.06,.055,.05];
%L=[.1025; 0.235; 0.1];
%L=0.115.*ones(1,n);


%Ett omr�de p� 10X10 med n�ringscentrum i [5,5]
%�nskar ett L i omr�det [0.01 , 1.00]
%hold on
%plot(plot::Circle2d(1, [5,5])
%plot(5,5,o)
%bias=1/(1+3/7*(Yp/Yt)^(5.5));



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
%q=0.4168; %k=1
%q=0.232; %k=8
q=0.1835; %k=2
Ap0=q*At;
Yp0=q*Yt;
Bp0=q*Bt;
m0=5;
Y0=[Ap0;Yp0;Bp0;m0];
m=100;
tspan=linspace(0,.1,m);
tplot=tspan;

%tspan(:,1)=linspace(0,20,1000);
%tspan(:,2)=linspace(20,600,1000);
%tspan(:,3)=linspace(600,700,1000);

Y=zeros(1,1);
%bias=zeros(1,15);

for i=1:n
%System of ODE:s
odesystem1=@(t,y)[(1/(1+exp(N*(1-(y(4)/2)+log((1+(L(i)/Kaoff))/(1+(L(i)/Kaon)))))))*k1*(At-y(1))-k2*y(1)*(Yt-y(2))-k3*y(1)*(Bt-y(3));
    k2*y(1)*(Yt-y(2))-k4*y(2)*Zt-k6*y(2);
    k3*y(1)*(Bt-y(3))-k5*y(3);
    gR*Rt*(1-(1/(1+exp(N*(1-y(4)/2+log((1+L(i)/Kaoff)/(1+L(i)/Kaon)))))))-gB*y(3)^2*(1/(1+exp(N*(1-y(4)/2+log((1+L(i)/Kaoff)/(1+L(i)/Kaon))))))];

%[t,y]=ode15s(odesystem1,tspan(:,i),Y0);
[t,y]=ode15s(odesystem1,tspan,Y0);
hold on
%plot(tplot,y(:,1))
plot(tplot,y(:,2))
%plot(tplot,y(:,3))
%plot(tplot,y(:,4))
tplot=tplot(end)+tspan;
Y0=y(m,:);
%T(:,i)=t;
G(:,i)=sum(y(:,2))/m;

%plot(t,4.042*ones(length(t),1),'black')
%plot(0.55*ones(100,1),linspace(0,10,100),'black')

%k=1;
%while t(k)<=0.55
%    Y=y(k,2);
%    k=k+1;
%end
%U(i)=Y;
%a=1000;
%b=7;
%bias(1,i)=1/(1+a*(Y/Yt)^b);
%else
%    Y(1,i)=min(y(:,2));
%    bias(1,i)=1/(1+3/7*(Y(1,i)/Yt)^(5.5));
%end


%bias=@(x)1/(1+3/7*(x./Yp0).^(5.5));
%bias(1,i)=Y(1,i)/Yp0;
%R=bias(y(:,2));
%plot(t,R)

end
%plot(T(:,1),G(:,1))
%plot(t2,y2(:,2))
%figure
%plot(U,bias)
%legend('CheA-P','CheY-P','CheB-P','M')