#!/bin/bash

case $(uname -m) in
	*"arm"*)
	file="HTWebRemoteHost_RasPi.zip"
	;;
	
	*"aarch"*)
	file="HTWebRemoteHost_RasPi64.zip"
	;;
	
	*"x86_64"*)
	file="HTWebRemoteHost_Linux.zip"
	;;
esac

read -p "Are you sure you want to INSTALL HTWebRemoteHost? [y/n]" -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]
then
	mkdir /opt/HTWebRemoteHost
	wget -O /tmp/$file https://github.com/nicko88/HTWebRemoteHost/releases/latest/download/$file
	unzip /tmp/$file -d /opt/HTWebRemoteHost
	chmod +x /opt/HTWebRemoteHost/HTWebRemoteHost
	wget -O /lib/systemd/system/HTWebRemoteHost.service https://raw.githubusercontent.com/nicko88/HTWebRemoteHost/master/install/HTWebRemoteHost.service
	systemctl daemon-reload
	systemctl enable HTWebRemoteHost.service
	service HTWebRemoteHost start
	
	rm /tmp/$file
fi

read -p "Do you want to INSTALL Android Debug Bridge (adb) for Nvidia Shield control? [y/n]" -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]
then
	apt-get update
	apt-get install -y adb
	
	codename=$(grep -Po 'VERSION="[0-9]+ \(\K[^)]+' /etc/os-release)
	echo "deb [trusted=yes] http://deb.debian.org/debian ${codename}-backports main" > /etc/apt/sources.list.d/backports.list
	
	apt-get update
	apt-get install -y adb/"${codename}-backports"
	apt-get autoremove -y
	
	rm /etc/apt/sources.list.d/backports.list
fi

rm install.sh