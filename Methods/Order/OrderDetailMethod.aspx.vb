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
    Public Shared Function BindOrderHeaderByID(ByVal headerid As String, ByVal ordertype As String) As Object
        Try
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
                    cmd.Parameters.AddWithValue("@Id", headerid)
                    cmd.Parameters.AddWithValue("@OrderType", ordertype)

                    conn.Open()

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim sumPrice As Decimal = Convert.ToDecimal(reader("SumPrice"))
                            Dim gst As Decimal = sumPrice * 0.1D
                            Dim finalTotal As Decimal = sumPrice + gst

                            Return New With {
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
                                .FinalTotal = finalTotal
                            }
                        End If
                    End Using
                End Using
            End Using

            Return Nothing

        Catch ex As Exception
            Return New With {
                .error = True,
                .message = ex.Message
            }
        End Try
    End Function
End Class
