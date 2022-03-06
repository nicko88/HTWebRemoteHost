#!/bin/bash

read -p "Are you sure you want to UNINSTALL HTWebRemoteHost? [y/n]" -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]
then
	service HTWebRemoteHost stop
	systemctl disable HTWebRemoteHost.service
	rm /lib/systemd/system/HTWebRemoteHost.service
	systemctl daemon-reload
	rm -rf /opt/HTWebRemoteHost
fi

rm uninstall.sh