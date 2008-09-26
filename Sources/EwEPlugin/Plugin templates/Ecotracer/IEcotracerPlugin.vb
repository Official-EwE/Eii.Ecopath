'==============================================================================
'
' $Log: IEcotracerPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2007/11/25 19:45:34  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing plugin points that are invoked from the EwE
''' Ecosim model.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcotracerPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Datasource load ecotracer scenario plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source from which
    ''' data is being loaded.</param>
    ''' <remarks>This plugin point is non-exclusive; each implementation 
    ''' of this plugin point will be called.</remarks>
    ''' -----------------------------------------------------------------------
    Sub LoadEcotracerScenario(ByVal dataSource As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Datasource save ecotracer scenario plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source to which
    ''' data is being saved.</param>
    ''' <remarks>This plugin point is non-exclusive; each implementation 
    ''' of this plugin point will be called.</remarks>
    ''' -----------------------------------------------------------------------
    Sub SaveEcotracerScenario(ByVal dataSource As Object)

End Interface
