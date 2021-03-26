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

L=0.75; %Ligand concentration

%Receptor constants
Kaon=0.5; %Dissociation constant of active (Tar) receptor
Kaoff=0.02; %Dissociation constant of inactive (Tar) receptor

%Total concentration (micromolar)
At=7.9;
Yt=9.7;
Bt=0.28;
Rt=0.16;
Zt=3.8;

%Initial values (arbitrary)
q=0.25; %experimental ratio
Ap0=q*At;
Yp0=q*Yt;
Bp0=q*Bt;
m0=5;
Y0=[Ap0;Yp0;Bp0;m0];
tspan=[0 0.1];

%System of ODE:s
odesystem=@(t,y)[(1/(1+exp(N*(1-y(4)/2+log((1+L/Kaoff)/(1+L/Kaon))))))*k1*(At-y(1))-k2*y(1)*(Yt-y(2))-k3*y(1)*(Bt-y(3));
    k2*y(1)*(Yt-y(2))-k4*y(2)*Zt-k6*y(2);
    k3*y(1)*(Bt-y(3))-k5*y(3);
    gR*Rt*(1-(1/(1+exp(N*(1-y(4)/2+log((1+L/Kaoff)/(1+L/Kaon)))))))-gB*y(3)^2*(1/(1+exp(N*(1-y(4)/2+log((1+L/Kaoff)/(1+L/Kaon))))))];

[t,y]=ode45(odesystem,tspan,Y0);
plot(t,y)
legend('CheA-P','CheY-P','CheB-P','M')
    