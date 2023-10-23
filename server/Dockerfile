# COCOPRO Final - CS3505 
# April 2021


# parent image 

FROM ubuntu


# Set a default shell.

SHELL ["/bin/bash", "-c"]


# set up timezone environment 

ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"


# update from image and install compiler and git

RUN apt-get update -y && apt-get -y install \
	g++ \
	git \
	cmake \
	gdb \
	wget


# run makefile and start server 

RUN mkdir spreadsheets

RUN wget -O boost_1_69_0.tar.gz https://sourceforge.net/projects/boost/files/boost/1.69.0/boost_1_69_0.tar.gz/download
RUN tar xzvf boost_1_69_0.tar.gz

COPY . .
RUN make clean
RUN make