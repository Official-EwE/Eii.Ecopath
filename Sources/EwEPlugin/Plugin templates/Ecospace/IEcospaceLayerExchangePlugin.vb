'==============================================================================
'
' $Log: IEcospaceLayerExchangePlugin.vb,v $
' Revision 1.1  2008/10/28 02:45:19  jeroens
' Initial version
'
'==============================================================================

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface to allow plug-ins to access the content of individual Ecospace layers
''' during Ecospace execution.
''' </summary>
''' <remarks>
''' All layer objects exposed in the 
''' </remarks>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceLayerExchangePlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to allow external software to modify the content of a
    ''' cEcospaceLayer (EwECore.Ecospace.Basemap.cEcospaceLayer), 
    ''' called at the beginning of an Ecospace run.
    ''' </summary>
    ''' <param name="EcospaceLayer">The EcospaceLayer to modify.</param>
    ''' -----------------------------------------------------------------------
    Sub EcospaceStartRun(ByVal EcospaceLayer As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to allow external software to modify the content of a
    ''' cEcospaceLayer (EwECore.Ecospace.Basemap.cEcospaceLayer), 
    ''' called at end of a Ecospace run.
    ''' </summary>
    ''' <param name="EcospaceLayer">The EcospaceLayer to read.</param>
    ''' -----------------------------------------------------------------------
    Sub EcospaceEndRun(ByVal EcospaceLayer As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to allow external software to modify the content of a
    ''' cEcospaceLayer (EwECore.Ecospace.Basemap.cEcospaceLayer), 
    ''' called at the beginning of a time step.
    ''' </summary>
    ''' <param name="EcospaceLayer">The EcospaceLayer to modify.</param>
    ''' -----------------------------------------------------------------------
    Sub EcospaceBeginTimeStep(ByVal EcospaceLayer As Object, ByVal iTime As Integer)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to allow external software to modify the content of a
    ''' cEcospaceLayer (EwECore.Ecospace.Basemap.cEcospaceLayer), 
    ''' called at the end of time step.
    ''' </summary>
    ''' <param name="EcospaceLayer">The EcospaceLayer to read.</param>
    ''' -----------------------------------------------------------------------
    Sub EcospaceEndTimeStep(ByVal EcospaceLayer As Object, ByVal iTime As Integer)

End Interface
