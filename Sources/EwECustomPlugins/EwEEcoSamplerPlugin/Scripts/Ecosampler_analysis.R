#Packages
library(readr)
library(dplyr)
library(tidyr)
library(purrr)
#Getting the name of all csv files
files = list.files(pattern="*.csv")
file <- list.files("~/escritorio/Ecosampler8", pattern = "\\NA_Annual_IndicesWithoutPPR.csv", recursive = T, full.names = T)
#Reading the csv files and joining all together in a big dataframe
data <- file %>%
  map(read_csv) %>%    # read in all the files individually, using 
  # the function read_csv() from the readr package
  reduce(rbind)        # reduce with rbind into one dataframe
data <- lapply(dir(),read.csv)
#Taking a look on our dataframe
newdata<-data
#Removing some columns (ecological indicators that we are not interested)
newdata$`Asc flow`<-NULL
newdata$`Asc import`<-NULL
newdata$`Asc export`<-NULL
newdata$`Asc resp`<-NULL
newdata$`Ovh import`<-NULL
newdata$`Ovh flow`<-NULL
newdata$`Ovh export`<-NULL
newdata$`Ovh resp`<-NULL
newdata$Export<-NULL
newdata$Resp<-NULL
newdata$`Prim prod`<-NULL
newdata$Prod<-NULL
newdata$Biomass<-NULL
newdata$Catch<-NULL
newdata$`Prop flow det` <-NULL
newdata$Ascendency<-NULL
newdata$AMI<-NULL
newdata$Entropy<-NULL
newdata$TLc<-NULL
newdata$`Shannon diversity index`<-NULL
newdata$`FiB index`<-NULL
newdata$`Det. TE (weighted)`<-NULL
newdata$`PP TE (weighted)`<-NULL
newdata$`Total TE (weigthed)` <-NULL


#Calculate vectors for throughput

ThroughputMin<-tapply(newdata$Throughput,factor(newdata$Year),min)
ThroughputMax<-tapply(newdata$Throughput,factor(newdata$Year),max)
ThroughputMean<-tapply(newdata$Throughput,factor(newdata$Year),mean)

f1<-function(x){quantile(x,probs=c(0.05))}
f2<-function(x){quantile(x,probs=c(0.95))}


Throughput05<-tapply(newdata$Throughput,factor(newdata$Year),f1)
Throughput95<-tapply(newdata$Throughput,factor(newdata$Year),f2)

#Create a table

tabla<-data.frame(Year=as.integer(names(ThroughputMin)),
                  Min=as.vector(ThroughputMin),
                  Q5=as.vector(Throughput05),
                  Mean=as.vector(ThroughputMean),
                  Q95=as.vector(Throughput95),
                  Max=as.vector(ThroughputMax))

#Save the table on the desktop

write.csv(tabla, '~/escritorio/throughtput.csv',row.names = FALSE)


