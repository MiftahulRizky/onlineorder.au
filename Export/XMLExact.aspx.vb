Imports System.Data
Imports System.IO
Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class Export_XMLExact
    Inherits Page

    Dim exactCfg As New ExactConfig
    Dim publicCfg As New PublicConfig
    Public myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class OrderSendingItem
        Public Property name As String
        Public Property status As Boolean
    End Class

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim status As String = "In Production"
            Dim jobDate As String = Now.ToString("yyyy-MM-dd")
            If Not String.IsNullOrEmpty(Request.QueryString("status")) Then
                status = Request.QueryString("status").ToString()
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("jobdate")) Then
                jobDate = DateTime.Parse(Request.QueryString("jobdate")).ToString("yyyy-MM-dd")
            End If

            Process(status, jobDate)
        End If
    End Sub

    Private Sub Process(status As String, jobDate As String)
        Dim headerData As DataSet = exactCfg.GetListData("SELECT * FROM OrderHeaders_Shutters WHERE Status = '" + status + "' AND OrderType IN ('Panorama', 'Evolve', 'Shutters') AND CONVERT(DATE, JobDate) = '" + jobDate + "'")
        If headerData.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To headerData.Tables(0).Rows.Count - 1
                Dim id As String = headerData.Tables(0).Rows(i).Item("Id").ToString()
                Dim orderId As String = headerData.Tables(0).Rows(i).Item("OrderId").ToString()

                Dim fileName As String = String.Format("Order-{0}.xml", orderId)
                Dim filePath As String = Server.MapPath("~/file/inv/")

                Dim finalPath As String = Path.Combine(filePath, fileName)

                exactCfg.CreateXML(id, fileName, filePath)
                exactCfg.Connect(finalPath)
            Next
        End If

        Dim headerBlindsData As DataSet = exactCfg.GetListData("SELECT * FROM OrderHeaders WHERE Status = '" + status + "' AND  CONVERT(DATE, JobDate) = '" + jobDate + "'")
        If headerBlindsData.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To headerBlindsData.Tables(0).Rows.Count - 1
                Dim id As String = headerBlindsData.Tables(0).Rows(i).Item("Id").ToString()
                Dim orderId As String = "SP-" & id

                Dim fileName As String = String.Format("-SPOD-Order-Furnishings-{0}.xml", orderId)
                Dim filePath As String = Server.MapPath("~/file/inv/")

                Dim finalPath As String = Path.Combine(filePath, fileName)
                
                If IsOrderSendingActive(id, "Exact") = False Then

                    Dim Res As Object = UpdateOrderSending(id, "Exact", True)
                    If Res.error then
                        Continue For
                    End If

                    exactCfg.CreateXMLB(id, fileName, filePath)
                    exactCfg.Connect(finalPath)
                End If
            Next
        End If

        Response.Redirect("~/export/sunlight", False)
    End Sub

    Private Function UpdateOrderSending(id As String, name As String, status As Boolean) As Object
        Try
            Using thisConn As New SqlConnection(myConn)

                Dim currentJson As String = Nothing

                Using getCmd As New SqlCommand("SELECT OrderSending FROM OrderHeaders WHERE Id = @Id",thisConn)
                    getCmd.Parameters.AddWithValue("@Id", id)
                    thisConn.Open()

                    Dim result = getCmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        currentJson = result.ToString()
                    End If
                End Using

                '#Get Old data
                Dim sendingList As New List(Of OrderSendingItem)
                If Not String.IsNullOrWhiteSpace(currentJson) Then
                    sendingList = JsonConvert.DeserializeObject(Of List(Of OrderSendingItem))(currentJson)
                    If sendingList Is Nothing Then
                        sendingList = New List(Of OrderSendingItem)
                    End If
                End If

                '#Find By Name
                Dim existingItem = sendingList.FirstOrDefault(
                    Function(x) x.name.Equals(name, StringComparison.OrdinalIgnoreCase)
                )

                If existingItem IsNot Nothing Then
                    '#Update
                    existingItem.status = status
                Else
                    '#Insert
                    sendingList.Add(New OrderSendingItem With {
                        .name = name,
                        .status = status
                    })
                End If

                Dim newJson As String = JsonConvert.SerializeObject(sendingList)
                Using updateCmd As New SqlCommand("UPDATE OrderHeaders SET OrderSending = @OrderSending WHERE Id = @Id",thisConn)
                    updateCmd.Parameters.AddWithValue("@Id", id)
                    updateCmd.Parameters.AddWithValue("@OrderSending", newJson)
                    updateCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using
           
            Return New With { .error = false, .message = "Order has been updated successfully."}
        Catch ex As Exception
            Return New With { .error = true, .message = String.Format("UpdateOrderSending: {0}", ex.Message)}
        End Try
    End Function

    Private Function IsOrderSendingActive(headerid As Integer, sendingName As String) As Boolean

        Using conn As New SqlConnection(myConn)
            Using cmd As New SqlCommand("SELECT OrderSending FROM OrderHeaders WHERE Id = @Id", conn)

                cmd.Parameters.AddWithValue("@Id", headerid)
                conn.Open()

                Dim result = cmd.ExecuteScalar()

                If result Is Nothing OrElse result Is DBNull.Value Then
                    Return False
                End If

                Dim orderSending As String = result.ToString()

                If String.IsNullOrWhiteSpace(orderSending) Then
                    Return False
                End If

                Dim sendingList = JsonConvert.DeserializeObject(Of List(Of OrderSendingItem))(orderSending)

                If sendingList Is Nothing Then
                    Return False
                End If

                Dim item = sendingList.FirstOrDefault(
                    Function(x) x.name.Equals(sendingName, StringComparison.OrdinalIgnoreCase)
                )

                If item Is Nothing Then
                    Return False
                End If

                Return item.status

            End Using
        End Using

    End Function
End Class
