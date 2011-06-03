Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is automatically invoked when
''' Ecopath has ran succesfully.
''' </summary>
''' <remarks>
''' This interfaces functionally replaces IEcopathRunCompletedPlugin without
''' breaking backward compatibility.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Interface IEcopathRunCompleted2Plugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute an Ecopath Run Completed plug-in.
    ''' </summary>
    ''' <param name="EcoPathDataStructures">A reference to the Ecopath data 
    ''' structures as defined in the EwE project.</param>
    ''' <param name="TaxonDataStructures">A reference to the taxon data 
    ''' structures as defined in the EwE project.</param>
    ''' <param name="StanzaDataStructures">A reference to the stanza data 
    ''' structures as defined in the EwE project.</param>
    ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
    ''' plug-ins can respond to this event.</remarks>
    ''' -----------------------------------------------------------------------
    Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object, ByRef TaxonDataStructures As Object, ByRef StanzaDataStructures As Object)

End Interface
