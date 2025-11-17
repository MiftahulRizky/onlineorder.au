Imports System.Data
Imports System.Data.SqlClient

Public Class ShipmentConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(thisString, thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Function CreateOrderShipmentId() As String
        Dim result As String = String.Empty
        Try
            Dim idDetail As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM OrderShipments ORDER BY Id DESC", thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            idDetail = rdResult.Item("Id").ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
            If idDetail = "" Then : result = 1
            Else : result = CInt(idDetail) + 1
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function
End Class
