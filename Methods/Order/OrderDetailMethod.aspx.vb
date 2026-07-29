Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services
Imports OfficeOpenXml
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Globalization
Imports System.Linq ' Pastikan ini ada di bagian atas file Anda untuk LINQ
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Http
Imports System.Text
Partial Class Methods_Order_OrderDetailMethod
    Inherits System.Web.UI.Page
    Shared orderCfg As New OrderConfig()
    Shared publicCfg As New PublicConfig()
    Shared printCfg As New PrintConfig()
    Shared jobsheet As New HalperJobSheetRenderer()
    Shared enUS As CultureInfo = New CultureInfo("en-US")
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    '#--- Response Message ---#
    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessDetail
        Public Property message As String
        Public Property url As String
    End Class

    Public Class SuccessResponse
        Public Property success As SuccessDetail
    End Class

    Public Class ParamBindOrderHeaderByID
        Public Property headerid As String
        Public Property ordertype As String

        Public Property loginid As String
        Public Property rolename As String
    End Class

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetItemData(ByVal query As String) As Object
        Try
            Dim Item As String = publicCfg.GetItemData(query)
            Return Item
        Catch ex As Exception
            Return "ERROR: " & ex.Message ' biar kelihatan errornya
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderHeaderByID(ByVal data As ParamBindOrderHeaderByID) As Object
        Try
            Dim HeaderData As Object
            Dim DetailData As Object
            Dim query As String = <sql>
            SELECT 
                h.Id,
                h.CustomerName,
                h.CustomerId,
                h.OrderId,
                h.JoNumberId,
                h.OrderType,
                h.OrderNumber,
                h.OrderName,
                h.CreatedDate,
                h.CreatedBy,
                cl.FullName AS CreatedByName,
                h.OrderNote,
                h.StatusAdditional,
                h.Status,
                h.Delivery,
                h.SubmittedDate,
                h.JobDate,
                h.CompletedDate,
                h.CanceledDate,

                ISNULL(SUM(odp.Cost * odp.Qty), 0) 
                - ISNULL(SUM((odp.Discount + odp.DiscountB + odp.DiscountC) * odp.Qty), 0) 
                AS SumPrice

            FROM view_order_headers h
            LEFT JOIN CustomerLogins cl ON h.CreatedBy = cl.Id
            LEFT JOIN OrderDetails od ON od.HeaderId = h.Id AND od.Active = 1
            LEFT JOIN OrderDetailsPrice odp ON odp.ItemId = od.Id AND odp.HeaderId = h.Id

            WHERE h.Id = @Id 
            AND h.OrderType = @OrderType 
            AND h.Active = 1

            GROUP BY 
                h.Id, h.CustomerName, h.CustomerId, h.OrderId,
                h.JoNumberId, h.OrderType, h.OrderNumber, h.OrderName,
                h.CreatedDate, h.CreatedBy, cl.FullName,
                h.OrderNote, h.StatusAdditional, h.Status, h.Delivery,
                h.SubmittedDate, h.JobDate, h.CompletedDate, h.CanceledDate
            </sql>.Value


            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id", data.headerid)
                    cmd.Parameters.AddWithValue("@OrderType", data.ordertype)

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim sumPrice As Decimal = Convert.ToDecimal(reader("SumPrice"))
                            Dim gst As Decimal = sumPrice * 0.1D
                            Dim finalTotal As Decimal = sumPrice + gst

                            Dim Id As String = reader("Id").ToString()
                            Dim Status As String = reader("Status").ToString()
                            Dim ResCheckOrder As Object = CekOrder(Id, Status, data.loginid, data.rolename)

                            HeaderData = New With {
                                .Id = reader("Id").ToString(),
                                .CustomerName = reader("CustomerName").ToString(),
                                .CustomerId = reader("CustomerId").ToString(),
                                .OrderId = reader("OrderId").ToString(),
                                .JoNumberId = reader("JoNumberId").ToString(),
                                .OrderType = reader("OrderType").ToString(),
                                .OrderNumber = reader("OrderNumber").ToString(),
                                .OrderName = reader("OrderName").ToString(),
                                .CreatedDate = reader("CreatedDate").ToString(),
                                .CreatedByName = reader("CreatedByName").ToString(),
                                .OrderNote = reader("OrderNote").ToString(),
                                .StatusAdditional = reader("StatusAdditional").ToString(),
                                .Status = reader("Status").ToString(),
                                .Delivery = reader("Delivery").ToString(),
                                .SubmittedDate = reader("SubmittedDate").ToString(),
                                .JobDate = reader("JobDate").ToString(),
                                .CompletedDate = reader("CompletedDate").ToString(),
                                .CanceledDate = reader("CanceledDate").ToString(),
                                .SumPrice = sumPrice,
                                .Gst = gst,
                                .FinalTotal = finalTotal,
                                .ResCheckOrder = ResCheckOrder
                            }
                        End If
                    End Using
                End Using
            End Using

            Return New With {
                .header = HeaderData,
                .detail = DetailData
            }

            Return Nothing

        Catch ex As Exception
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function

    Private Shared Function CekOrder(ByVal headerid As String, ByVal status As String, ByVal loginid As String, ByVal rolename As String) As Object
        Try
            Dim msg As String = String.Empty
            Dim url As String = String.Empty
            Dim Action As String = String.Empty
            Dim textSwall As String = String.Empty
            ' Dim RoleName As String = HttpContext.Current.Session("RoleName").ToString()
            Dim CustomerContactId As String = HttpContext.Current.Session("CustomerContactId").ToString()

            Dim detailData As DataSet = publicCfg.GetListData("SELECT Id, UniqueId, DesignName, BracketType, FabricGroups FROM view_details WHERE HeaderId='" + headerid + "' AND Active=1 ORDER BY Id ASC")
            If detailData.Tables(0).Rows.Count < 1  Then Return Nothing

            For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                Dim Id As String = detailData.Tables(0).Rows(i).Item("Id").ToString()
                Dim UniqueId As String = detailData.Tables(0).Rows(i).Item("UniqueId").ToString()
                Dim DesignName As String = detailData.Tables(0).Rows(i).Item("DesignName").ToString()
                Dim BracketType As String = detailData.Tables(0).Rows(i).Item("BracketType").ToString()
                Dim FabricGroups As String = detailData.Tables(0).Rows(i).Item("FabricGroups").ToString()

                If FabricGroups = "POA" Then
                    Dim Poa As Integer = publicCfg.GetItemData(String.Format("SELECT Poa FROM OrderDetailsPrice where HeaderId={0} AND ItemId={1}", headerid, Id))
                    If Poa = 0 Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status='Pending Price Approval' WHERE Id = @Id")
                                myCmd.Parameters.AddWithValue("@Id", headerid)
                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                    End If
                    If Poa > 0  Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status='Draft' WHERE Id = @Id")
                                myCmd.Parameters.AddWithValue("@Id", headerid)
                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                    End If
                End If

                Dim TotalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM view_details WHERE UniqueId = '" + UniqueId + "' AND Active = 1")
                If DesignName = "Roller Blinds" or DesignName = "Global Roller Blinds" Then

                    If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Dep)" Or BracketType = "Linked 2 Blinds (Ind)" Then
                        If TotalBlind < 2 Then
                            Action = "Yes"
                            msg += "<b> " & Id & "</b>,"
                        End If
                    End If

                    If BracketType = "Linked 3 Blinds (Dep)" Or BracketType = "Linked 3 Blinds (Ind)" Then
                        If TotalBlind < 3 Then
                            Action = "Yes"
                            msg += "<b> " & Id & "</b>,"
                        End If
                    End If

                    If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then
                        If TotalBlind < 4 Then
                            Action = "Yes"
                            msg += "<b> " & Id & "</b>,"
                        End If
                    End If

                End If

                If Not status = "Draft" Then
                    Action = "No"
                End If

                If rolename = "PPIC & DE" And Not loginid = CustomerContactId Then
                    Action = "No"
                End If

                textSwall = "You have an incomplete roller blinds order, which is on the ITEM ID "+ msg +" <br /><br />If you want to complete it, please click the <b>Next Item</b> button on the order line ID."
                
            Next

            Return New With {.Action = Action, .Message = textSwall}
        Catch ex As Exception
            Return New With {.error = ex.Message}
        End Try
    End Function
End Class
