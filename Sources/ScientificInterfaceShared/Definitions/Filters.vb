Public Interface IContentFilter
    Event FilterChanged(sender As IContentFilter)
End Interface

Public Interface IGroupFilter
    Inherits IContentFilter
    ''' <summary>
    ''' One-based group index
    ''' </summary>
    Property Group As Integer
End Interface

Public Interface IFleetFilter
    Inherits IContentFilter
    ''' <summary>
    ''' Zero-based fleet index (0 to account for the 'all' fleet)
    ''' </summary>
    Property Fleet As Integer
End Interface

Public Interface IMonthFilter
    Inherits IContentFilter
    ''' <summary>
    ''' One-based month index 
    ''' </summary>
    Property Month As Integer
End Interface