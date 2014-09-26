Imports EwEUtils.SpatialData

Public Class cSpatialDataConnection

    Property Dataset As ISpatialDataSet
    Property Converter As ISpatialDataConverter
    'ToDo: add diagnostics? Compatibility?

    Public Overridable Function IsConfigured() As Boolean

        Dim bIsConfigured As Boolean = False

        If (Me.Dataset IsNot Nothing) Then
            If (Me.Dataset.IsConfigured()) Then
                If Not String.IsNullOrWhiteSpace(Me.Dataset.ConversionFormat) Then
                    If (Me.Converter IsNot Nothing) Then
                        bIsConfigured = bIsConfigured Or Me.Converter.IsConfigured()
                    End If
                Else
                    bIsConfigured = True
                End If
            End If
        End If
        Return bIsConfigured

    End Function

End Class
