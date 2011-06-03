Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that performs a custom Ecopath 
''' Mass Balance calculation. If provided, this plug-in point will replace
''' the native Mass Balance calculation provided with EwE6.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcopathMassBalancePlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute a Mass balance calculation.
    ''' </summary>
    ''' <param name="EcoPathDataStructures">A reference to the Ecopath data 
    ''' structures as defined in the EwE project.</param>
    ''' <param name="eEstimateFor">Enumerated value, stating the purpose of the mass 
    ''' balance calculation.</param>
    ''' <param name="iResult">The result of the mass balance calculation. For 
    ''' possible values refer to the eStatusFlags enumerated type in the EwE project.
    ''' </param>
    ''' <returns>True if a MassBalance calculation has been performed succesfully.
    ''' This plug-in point is exclusive, meaning that only one IEcopathMassBalancePlugin 
    ''' plug-in is allowed to successdully perform this calculation.</returns>
    ''' -----------------------------------------------------------------------
    Function EcopathMassBalance(ByVal EcoPathDataStructures As Object, ByVal eEstimateFor As Integer, ByRef iResult As Integer) As Boolean

End Interface
