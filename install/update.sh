#!/bin/bash

if [[ $(getconf LONG_BIT) =~ "32" ]]
then
    file="HTWebRemoteHost_RasPi.zip"
elif [[ $(uname -m) =~ "aarch" ]]
then
    file="HTWebRemoteHost_RasPi64.zip"
else
    file="HTWebRemoteHost_Linux.zip"
fi

wget -O /tmp/$file https://github.com/nicko88/HTWebRemoteHost/releases/latest/download/$file
rm /opt/HTWebRemoteHost/HTWebRemoteHost
rm /opt/HTWebRemoteHost/libSystem.IO.Ports.Native.so
unzip /tmp/$file -d /opt/HTWebRemoteHost
chmod +x /opt/HTWebRemoteHost/HTWebRemoteHost
rm /tmp/$file
rm /opt/HTWebRemoteHost/update.sh
rm update.sh
service HTWebRemoteHost restart