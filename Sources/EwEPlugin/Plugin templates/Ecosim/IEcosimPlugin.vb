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
