# run_enaR.r
# script to execute "full" ecological network analysis with enaR
# --_
# input - score file
# output - enaR analysis stored as CSV file
# ---
#Borrett,   Feb. 27, 2013
#Steenbeek, Mar. 17, 2013: Added filename placeholder for EwE
# ========================================================================



# file.name is the name of plain text SCOR formatted ENA model.  I am assuming it is in the current dirctory or the file name includes the path to the directory.
  
# prepare
rm(list=ls())
library(enaR)


# load model
file.name="%SCORFILE%"
m <- read.scor(file.name)          # read in model
m <- balance(m)                    # balances model if needed

# perform analyses
All <- list()
All$A <- enaStructure(m)           # structure analysis
All$F <- enaFlow(m)                # flow analysis
All$S <- enaStorage(m)
All$U <- enaUtility(mm,eigen.check=FALSE)
All$C <- enaControl(m)
All$mti <- mixedTrophicImpacts(m,eigen.check=FALSE)
All$E <- environ(m)

# writes output to a txt file.  
zz <- file("tmp.txt",open="wt")
sink(zz)
show(All)
sink()
unlink(fn)


