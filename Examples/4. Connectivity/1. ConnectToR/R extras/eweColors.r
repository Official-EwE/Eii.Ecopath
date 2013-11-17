##  ===============================================================================
##  This file is part of Ecopath with Ecosim (EwE)
##
##  EwE is free software: you can redistribute it and/or modify it under the terms
##  of the GNU General Public License version 2 as published by the Free Software 
##  Foundation.
##
##  EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
##  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
##  PURPOSE. See the GNU General Public License for more details.
##
##  You should have received a copy of the GNU General Public License along with EwE.
##  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
##
##  Copyright 2013- Ecopath International Initiative, Spain.
##  ===============================================================================
##

## Returns a vector of n rgb colour values using the EwE colour scheme
## n: number of colours in the vector
eweColors <- function(n) {
	if (n<=0) {
		n=100;
	}
	cols <- rep(rgb(0,0,0), n);
	for (i in 0:n) {
		cols[i]=eweColor(i, n);
	}
	return(cols);
}

## Return a rgb colour for a value using the EwE colour scheme
## v: value to provide color for
## vm: max value to scale to
eweColor <- function(v, vm) {
	if (vm==0) {
		vm=1;
	}
	v <- 1.0 - (v / vm);
	vm <- 1;
	m <- vm / 2;
	
	# Red
	if (v <= (2 * vm / 3)) {
		r <- 245.0 * ((2.5 * vm / 3.0 - v) / m);
	} else {
		r <- 255.0 * ((v - m) / m);
	};

	# Green
	if (v <= m) {
		g <- 300.0 * (v / m);
	} else {
		g <- 255.0 + 45.0 * ((v - m) / m);
	};
 
	# Blue
	if (v <= (vm / 3)) {
		b <- 5.0 * (v / m);
	} else {
		b <- 55.0 + 455.0 * ((v - m) / m);
	};

	# Truncate
	r <- min(255, max(0, r));
	g <- min(255, max(0, g));
	b <- min(255, max(0, b));
	if (b > 250) { 
		if (r > 250) { r <- 250; }
		if (g > 250) { g <- 250; }
		if (b > 255) { b <- 255; };
	};
	return(rgb(r, g, b, maxColorValue=255));
}