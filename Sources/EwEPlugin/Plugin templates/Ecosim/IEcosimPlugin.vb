'==============================================================================
'
' $Log: IEcosimPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/03/14 00:52:00  jeroens
' - Restricted to data access interfaces now plugin-points are implemented as objects instead of function calls.
'
' Revision 1.2  2007/01/14 21:03:14  jeroens
' Discontinued iDatasourcePlugin
'
' Revision 1.1  2006/08/30 20:52:35  jeroens
' * Moved and/or created
'
' Revision 1.1  2006/08/20 21:20:06  jeroens
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
Public Interface IEcosimPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Datasource load ecosim scenario plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source from which
    ''' data is being loaded.</param>
    ''' <remarks>This plugin point is non-exclusive; each implementation 
    ''' of this plugin point will be called.</remarks>
    ''' -----------------------------------------------------------------------
    Sub LoadEcosimScenario(ByVal dataSource As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Datasource save ecosim scenario plugin point.
    ''' </summary>
    ''' <param name="dataSource">A reference to the EwE data source to which
    ''' data is being saved.</param>
    ''' <remarks>This plugin point is non-exclusive; each implementation 
    ''' of this plugin point will be called.</remarks>
    ''' -----------------------------------------------------------------------
    Sub SaveEcosimScenario(ByVal dataSource As Object)

End Interface
