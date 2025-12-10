Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Mail
Partial Class Console_ReminderOrderDraft
    Inherits System.Web.UI.Page

    Dim publicCfg As New PublicConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        GetOrderDraft()

    End Sub

    Protected Sub GetOrderDraft()
        Try
            Dim orders As DataSet = publicCfg.GetListData("SELECT * FROM view_order_headers WHERE Active = '1' AND Status IN ( 'Draft', 'Unsubmitted' ) ORDER BY CreatedDate DESC")
            Dim dump As New Text.StringBuilder()
            
            For i As Integer = 0 To orders.Tables(0).Rows.Count - 1
                Dim row = orders.Tables(0).Rows(i)

                Dim Id As String = SafeGet(row, "Id")
                Dim OrderId As String = SafeGet(row, "OrderId")
                Dim OrderNumber As String = SafeGet(row, "OrderNumber")
                Dim OrderName As String = SafeGet(row, "OrderName")
                Dim OrderType As String = SafeGet(row, "OrderType")
                Dim Status As String = SafeGet(row, "Status")
                Dim Delivery As String = SafeGet(row, "Delivery")
                Dim CreatedBy As String = SafeGet(row, "CreatedBy")
                Dim CreatedDate As String = SafeGet(row, "CreatedDate")

                Dim ThisTime As DateTime
                If DateTime.TryParse(CreatedDate, ThisTime) Then
                    Dim CurrentDate As DateTime = DateTime.Now
                    Dim DaysDiff As Integer = (CurrentDate - ThisTime.Date).Days '#Hitung selisih hari

                    If DaysDiff < 3 Then
                        Dim Log_OrderDraft As DataSet = publicCfg.GetListData("SELECT * FROM Log_OrderDraft WHERE Id = '" & Id & "' AND OrderType = '" & OrderType & "'")
                        If Log_OrderDraft.Tables.Count > 0 AndAlso Log_OrderDraft.Tables(0).Rows.Count < 3 Then

                            Dim ResponseLog As String = InsertLogOrderDraft(Id, OrderId, OrderType, CreatedDate)

                            Dim statusColor As String = If(ResponseLog = "200", "green", "red")
                            Dim statusText As String = If(ResponseLog = "200", "Success", "Failed")

                            dump.AppendLine($"<div style='padding:6px 10px; border-radius:6px; margin-bottom:4px; font-weight:bold; color:{statusColor}; border:1px solid {statusColor};'>")
                            dump.AppendLine($"Status: {statusText} (Code: {ResponseLog})")
                            dump.AppendLine("</div>")


                            ' SendReminderMail(OrderNumber, OrderName, OrderType, Delivery, CreatedBy)
                        End If
                    End If
                Else
                    ltDump.Text = "<div style='color:red;'>Error: Tanggal CreatedDate tidak valid. </div>"
                End If
                

                ' dump.AppendLine("<div style='margin-bottom:10px;'>")
                ' dump.AppendLine("ID: " & Id & "<br>")
                ' dump.AppendLine("Order No: " & OrderNo & "<br>")
                ' dump.AppendLine("Status: " & Status  & "<br>")
                ' dump.AppendLine("CreatedBy: " & CreatedBy  & "<br>")
                ' dump.AppendLine("CreatedDate: " & CreatedDate  & "<br>")
                ' dump.AppendLine("</div>")
            Next

            ltDump.Text = dump.ToString()
        Catch ex As Exception
            ltDump.Text = "<div style='color:red;'>Error: " & ex.Message & "</div>"
        End Try
    End sub


    Public Function InsertLogOrderDraft(Id As String, OrderId As String, OrderType As String, CreatedDate As String) As String
        Try
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Log_OrderDraft (Id, OrderId, OrderType, DraftDate) VALUES (@Id, @OrderId, @OrderType, @DraftDate)")
                    myCmd.Parameters.AddWithValue("@Id", Id)
                    myCmd.Parameters.AddWithValue("@OrderId", OrderId)
                    myCmd.Parameters.AddWithValue("@OrderType", OrderType)
                    myCmd.Parameters.AddWithValue("@DraftDate", CreatedDate)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            Return "200"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function





    Private Function SafeGet(row As DataRow, column As String) As String
        If row.Table.Columns.Contains(column) AndAlso Not IsDBNull(row(column)) Then
            Return row(column).ToString()
        End If
        Return "<span style='color:red;'>(null)</span>"
    End Function


End Class
