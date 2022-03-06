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

wget -O /tmp/$file https://github.com/nicko88/HTWebRemoteHost/releases/latest/download/$file
rm /opt/HTWebRemoteHost/HTWebRemoteHost
rm /opt/HTWebRemoteHost/libSystem.IO.Ports.Native.so
unzip /tmp/$file -d /opt/HTWebRemoteHost
chmod +x /opt/HTWebRemoteHost/HTWebRemoteHost
rm /tmp/$file
rm /opt/HTWebRemoteHost/update.sh
rm update.sh
service HTWebRemoteHost restart