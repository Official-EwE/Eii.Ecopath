Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for overriding the Ecospace cost of sailing calculations.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceCalcCostOfSailingPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Datasource load ecospace scenario plugin point.
    ''' </summary>
    ''' <param name="EcospaceData">Ecospace data structure providing context
    ''' information for this calculation.</param>
    ''' <param name="Depth">Ecospace depth(row, col).</param>
    ''' <param name="Port">Ecospace ports(fleet, row, col). Fleets are indexed
    ''' one-based, the 0-fleet data contains the aggregation of ports for all 
    ''' fleets.</param>
    ''' <param name="Sail">Sailing cost buffer(fleet, row, col) to receive the
    ''' calculated cost of sailing.</param>
    ''' <remarks>This plugin point is exclusive; plug-ins of this type will be
    ''' invoked in an arbitrairy order until a plug-in returns True. If this
    ''' happens, Ecospace will not attempt to calculate cost of sailing.</remarks>
    ''' -----------------------------------------------------------------------
    Function CalculateCostOfSailing(ByVal EcospaceData As Object, _
                                    ByVal Depth(,) As Integer, _
                                    ByVal Port(,,) As Boolean, _
                                    ByVal Sail(,,) As Single) As Boolean

End Interface
