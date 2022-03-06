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
	
	apt-get update
	apt-get install -y adb
	
	rm /tmp/$file
fi

rm install.sh