
''' <summary>
''' Interface for MSE initialization plugin points that are invoked once the MSE model has been loaded
''' </summary>
''' <remarks></remarks>
Public Interface IMSERunPlugin
    Inherits IPlugin

    ''' <summary>
    ''' An MSE run is about to start.
    ''' </summary>
    ''' <remarks>Called at the start of a run before the data has been initialized.</remarks>
    Sub MSERunStarted()

    ''' <summary>
    ''' An MSE run has completed.
    ''' </summary>
    ''' <remarks>Called at the end of a run after all the data has been populated and before the interface has been notified.</remarks>
    Sub MSERunCompleted()

    ''' <summary>
    ''' An MSE iteration has started.
    ''' </summary>
    ''' <remarks>The MSE has completed the initialization for an iteration and is about the run Ecosim.</remarks>
    Sub MSEIterationStarted()

    ''' <summary>
    ''' An MSE iteration has completed.
    ''' </summary>
    ''' <remarks>The MSE has populated data from the iteration but has not re-initialized for the next iteration.</remarks>
    Sub MSEIterationCompleted()

    ''' <summary>
    ''' The Stock Assessment model has been run.
    ''' </summary>
    ''' <param name="Biomass">Biomass(ngroups) calculated by Ecosim in the first month of the year.</param>
    ''' <remarks>
    ''' The MSE Stock Assessment model is call in the first month of a year by Ecosim. 
    ''' MSEDoAssessment is called after the model has been run and can be used to update variables in cMSEDataStructures that are used to set Quotas in cMSE.UpdateQuotas(biomass()).
    ''' Updates cMSEDataStructures.Bestimate(ngroups) estimated biomass for this year.
    ''' cMSEDataStructures.BestimateLast(ngroups) estimated biomass for the previous year.
    ''' cMSEDataStructures.KalmanGain(ngroups) for this year.
    ''' </remarks>
    Sub MSEDoAssessment(ByVal Biomass() As Single)

    ''' <summary>
    ''' Update cMSEDataStructures.QuotaTime(ngroups) with the quota for a year.
    ''' </summary>
    ''' <param name="Biomass">Biomass(ngroups) calculated by Ecosim in the first month of the year. </param>
    ''' <remarks>
    ''' MSEUpdateQuotas() is called after cMSEDataStructures.QuotaTime(ngroups) has been updated and can be used to 
    ''' </remarks>
    Sub MSEUpdateQuotas(ByVal Biomass() As Single)

    ''' <summary>
    ''' Regulate effort based on Quota and user selected controls.
    ''' </summary>
    ''' <param name="Biomass">Biomass(ngroups) for this time step.</param>
    ''' <param name="QMult">Density dependant catchability multiplier</param>
    ''' <param name="QYear">Catchability increase over time due to improved fishing efficiency.</param>
    ''' <param name="iTimeIndex">Cumulative time index.</param>
    ''' <remarks>Sets effort in cEcosimDataStructures.FishRateGear(nfleet,ngroups) base on Quota and user selected controls.</remarks>
    Sub MSERegulateEffort(ByVal Biomass() As Single, ByVal QMult() As Single, ByVal QYear() As Single, ByVal iTimeIndex As Integer)


End Interface