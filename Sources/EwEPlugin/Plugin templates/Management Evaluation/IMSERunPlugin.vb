
''' <summary>
''' Interface for MSE initialization plugin points that are invoked once the MSE model has been loaded
''' </summary>
''' <remarks></remarks>
Public Interface IMSERunPlugin
    Inherits IPlugin

    Sub MSERunStarted()

    Sub MSERunCompleted()

    Sub MSEIterationStarted()

    Sub MSEIterationCompleted()

    Sub MSEDoAssessment(ByVal Biomass() As Single)

    Sub MSEUpdateQuotas(ByVal Biomass() As Single)

    Sub MSERegulateEffort(ByVal Biomass() As Single, ByVal QMult() As Single, ByVal QYear() As Single, ByVal t As Integer)


End Interface