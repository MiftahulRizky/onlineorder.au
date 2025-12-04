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



Partial Class Methods_Order_DetailMethod
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()
    Shared printCfg As New PrintConfig()
    Shared jobsheet As New HalperJobSheetRenderer()
    Shared enUS As CultureInfo = New CultureInfo("en-US")
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    

    '#---------------------------------------|| Server side Order Detail Class || ---------------------------------------#
    Public Class OrdersParams
        Public Property headerid As String
        Public Property status As String
        Public Property userid As String
       
        Public Property draw As Integer
        Public Property start As Integer
        Public Property length As Integer
        Public Property order As List(Of OrderParam)
        Public Property columns As List(Of ColumnParam)
        Public Property search As SearchParam
    End Class

    Public Class OrderParam
        Public Property column As Integer
        Public Property dir As String ' "asc" or "desc"
    End Class

    Public Class ColumnParam
        Public Property data As String
        Public Property name As String
        Public Property searchable As Boolean
        Public Property orderable As Boolean
        Public Property search As SearchParam
    End Class

    Public Class SearchParam
        Public Property value As String
        Public Property regex As Boolean
    End Class

    ' --- Kelas Output WebMethod (untuk Respons DataTables) ---
    Public Class DataTableResponse
        Public Property draw As Integer
        Public Property recordsTotal As Integer
        Public Property recordsFiltered As Integer
        Public Property data As List(Of OrdersMatrixReturnRow)
    End Class

    Public Class OrdersMatrixReturnRow
        Public Property UserId As String 
        Public Property StatusHeader As String 
        Public Property HideNext As String 
        Public Property TextNext As String 


        Public Property No As String 
        Public Property Id As String 
        Public Property HeaderId As String 
        Public Property DesignId As String 
        Public Property Qty As String 
        Public Property Location As String 
        Public Property KitName As String 
        Public Property Matrix As String 
        Public Property Product As String 
        Public Property Cost As String 
        Public Property MarkUp As String 
    End Class
    '#---------------------------------------|| /Server side Order Detail Class || ---------------------------------------#

    '#---------------------------------------|| Server side Order Detail Pricing Class || ---------------------------------------#
    Public Class OrdersPricingParams
        Public Property id As String
       
        Public Property draw As Integer
        Public Property start As Integer
        Public Property length As Integer
        Public Property order As List(Of OrderParamPricing)
        Public Property columns As List(Of ColumnParamPricing)
        Public Property search As SearchParam
    End Class

    Public Class OrderParamPricing
        Public Property column As Integer
        Public Property dir As String ' "asc" or "desc"
    End Class

    Public Class ColumnParamPricing
        Public Property data As String
        Public Property name As String
        Public Property searchable As Boolean
        Public Property orderable As Boolean
        Public Property search As SearchParamPricing
    End Class

    Public Class SearchParamPricing
        Public Property value As String
        Public Property regex As Boolean
    End Class

    ' --- Kelas Output WebMethod (untuk Respons DataTables) ---
    Public Class DataTableResponsePricing
        Public Property draw As Integer
        Public Property recordsTotal As Integer
        Public Property recordsFiltered As Integer
        Public Property data As List(Of OrdersMatrixReturnRowPricing)
    End Class

    Public Class OrdersMatrixReturnRowPricing
        Public Property No As String 
        Public Property Id As String 
        Public Property HeaderId As String 
        Public Property ItemId As String 
        Public Property Qty As String 
        Public Property Description As String 
        Public Property Cost As String 
        Public Property FinalCost As String 
    End Class
    '#---------------------------------------|| /Server side Order Detail Pricing Class || ---------------------------------------#

    Public Class ParamUpdateStatusOrder
        Public Property id  As String
        Public Property status As String
        Public Property statusOld As String
        Public Property submitteddate As String
        Public Property completeddate As String
        Public Property canceleddate As String
        Public Property description As String

        Public Property username As String
    End Class





    Private Class SpacerInfo
        Public MaxWidth As Integer
        Public Spacer1Type As String
        Public CarriersQty As Integer
    End Class

    '#--- Kelas Output WebMethod ---#
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

    <WebMethod(EnableSession:=True)>
    Public Shared Sub SetSessionOpenEditOrderHeader(ByVal headerid As String)
        HttpContext.Current.Session("headerId") = headerid 
        HttpContext.Current.Session("headerAction") = "EditHeader"
    End Sub

    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SetSessionOpenPageInputItem(ByVal id As String, ByVal headerid As String, ByVal action As String, ByVal designid As String) As Object
        HttpContext.Current.Session("headerId") = headerid 
        HttpContext.Current.Session("itemAction") = action
        HttpContext.Current.Session("designId") = UCase(designid).ToString()

        If Not String.IsNullOrEmpty(id) And (action ="EditItem" Or action = "ViewItem" Or action = "NextItem") Then
            HttpContext.Current.Session("itemId") = id
        End If

        Dim page As String = publicCfg.GetDesignPage(designId)

        Return New SuccessResponse With {
            .Success = New SuccessDetail With { .message = page}
        }
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindDesignType() As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM Designs WHERE Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If datas IsNot Nothing AndAlso datas.Tables.Count > 0 Then
                For Each row As DataRow In datas.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Name").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderHeaderByID(ByVal headerid As String) As Object
        Try
            Dim datas As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id = '" + headerid + "'")

            Dim data As DataSet = DirectCast(datas, DataSet)

            Dim resultList As New List(Of Dictionary(Of String, String))()

            If data IsNot Nothing AndAlso data.Tables.Count > 0 Then
                For Each row As DataRow In data.Tables(0).Rows
                    Dim dict As New Dictionary(Of String, String)()
                    For Each col As DataColumn In data.Tables(0).Columns
                        dict(col.ColumnName) = row(col).ToString()
                    Next
                    resultList.Add(dict)
                Next
            End If

            Return resultList
        Catch ex As Exception
            ' Tangani error agar bisa dikenali di JavaScript
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetAmountPriceHeader(ByVal headerid As String, ByVal pricesaccess As String) As Object
        Dim sumPrice As Decimal = 0
        Dim gst As Decimal = 0
        Dim finaltotal As Decimal = 0
        Dim result As New Dictionary(Of String, String)
        
        Dim detaildata As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='"+headerid+"' AND Active='1'")


        If detaildata.Tables(0).Rows.Count > 0 And pricesaccess = "True" Then
            sumPrice = publicCfg.GetItemData_Decimal("SELECT SUM(TotalMatrix + TotalCharge) AS SumPrice FROM OrderDetails WHERE HeaderId = '" + headerid + "' AND Active=1")

            If sumPrice > 0 Then
                gst = sumPrice * 10 / 100
                finaltotal = sumPrice + gst
                result = New Dictionary(Of String, String) From {
                    {"amount", sumPrice.ToString("N2", enUS)},
                    {"gst", gst.ToString("N2", enUS)},
                    {"finaltotal", finaltotal.ToString("N2", enUS)}
                }
            End If
        End If

        Return result
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderDetails(params As OrdersParams) As DataTableResponse
        Dim response As New DataTableResponse()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of OrdersMatrixReturnRow)()
        Dim rolename As String = HttpContext.Current.Session("RoleName").ToString()
        ' Dim sessionCustomerId As String = HttpContext.Current.Session("CustomerId").ToString()
        ' Dim sessionUserId As String = sessionCustomerId
        Dim sessionUserId As String = HttpContext.Current.Session("userId").ToString()

        
        Try
            
            Using conn As New SqlConnection(myConn)
                conn.Open()

                
                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT( Id ) FROM view_details WHERE Active=@Active AND HeaderId=@HeaderId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@HeaderId", params.headerid)
                    countCmd.Parameters.AddWithValue("@Active", "1")
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT Id, HeaderId, DesignId, Qty, Location, DesignName, BlindName, KitName, BracketType, FabricType, BlindNo, UniqueId, Width, [Drop], Matrix, Charge, Markup")
                sqlBuilder.AppendLine("FROM view_details")
                sqlBuilder.AppendLine("WHERE Active=@Active AND HeaderId=@HeaderId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@HeaderId", params.headerid)
                cmd.Parameters.AddWithValue("@Active", "1")
                


                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND ( Id LIKE @SearchValue OR Location LIKE @SearchValue OR KitName LIKE @SearchValue OR FabricType LIKE @SearchValue OR Width LIKE @SearchValue )")
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue)
                End If

                sqlBuilder.Append(whereClause.ToString())
                
                ' --- Query untuk menghitung Filtered Records ---
                Dim filteredCountSql As String = "SELECT COUNT(T.Id) FROM (" & sqlBuilder.ToString() & ") AS T"
                Using filteredCountCmd As New SqlCommand(filteredCountSql, conn)
                    For Each p As SqlParameter In cmd.Parameters
                        filteredCountCmd.Parameters.Add(New SqlParameter(p.ParameterName, p.Value))
                    Next
                    filteredRecords = CInt(filteredCountCmd.ExecuteScalar())
                End Using
                

                ' ... kode sebelumnya ...
                Dim orderByClause As New System.Text.StringBuilder()
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    '# Notes: File di bawah ini untuk menambahkan order by ke query
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Id"}, _
                        {2, "Qty"}, _
                        {3, "Location"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY Id, BlindNo, DesignName ASC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY Id, BlindNo, DesignName ASC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim HideNext As String = "hidden"
                        Dim TextNext As String = "Add blind that is doubled to this blind"

                        Dim StatusHeader As String = params.status
                        Dim UserId As String = params.userid


                        Dim Id As String = reader("Id").ToString()
                        Dim HeaderId As String = reader("HeaderId").ToString()
                        Dim DesignId As String = reader("DesignId").ToString()
                        Dim Qty As String = reader("Qty").ToString()
                        Dim Location As String = reader("Location").ToString()
                        Dim DesignName As String = reader("DesignName").ToString()
                        Dim BlindName As String = reader("BlindName").ToString()
                        Dim KitName As String = reader("KitName").ToString()
                        Dim BracketType As String = reader("BracketType").ToString()
                        Dim FabricType As String = reader("FabricType").ToString()
                        Dim BlindNo As String = reader("BlindNo").ToString()
                        Dim UniqueId As String = reader("UniqueId").ToString()
                        Dim Width As String = reader("Width").ToString()
                        Dim Drop As String = reader("Drop").ToString()
                        Dim Matrix As String = reader("Matrix").ToString()
                        Dim Charge As String = reader("Charge").ToString()
                        Dim MarkUp As String = reader("MarkUp").ToString()

                        '#-------------------|| Cost ||-------------------#
                        Dim Cost As String = String.Empty
                        Dim totalCost As Decimal = 0.00

                        If DesignName = "Vertical Blinds" AndAlso BlindName = "Slat Only" Then
                            If Matrix = 0 Then
                                totalCost = Convert.ToDecimal(Charge)
                                Cost = "$" & totalCost.ToString("N2", enUS)
                            Else
                                totalCost = Convert.ToDecimal(Matrix) + Convert.ToDecimal(Charge)
                                Cost = "$" & totalCost.ToString("N2", enUS)
                            End If
                        Else
                            If Matrix > 0 Then
                                totalCost = Convert.ToDecimal(Matrix) + Convert.ToDecimal(Charge)
                                Cost = "$" & totalCost.ToString("N2", enUS)
                            End If
                        End If

                        '#-------------------|| Markup ||-------------------#
                        Dim FindMarkUp As String = String.Empty
                        If MarkUp > 0 Then
                            FindMarkUp = MarkUp & "%"
                        End If

                        '#-------------------|| Product ||-------------------#
                        Dim Product As String = KitName & " (" & width & " x " & drop & ")"
                    
                        If DesignName = "Aluminium Blinds" Or DesignName = "Venetian Blinds" Then
                            Product = KitName & " (" & Width & " x " & Drop & ")"
                        End If

                         If DesignName = "Roller Blinds" Then
                            Product = KitName & " #" & FabricType & " (" & Width & " x " & Drop & ")"

                            '#Linked 3 Blinds (Dep) & Linked 3 Blinds (Ind)
                            If BracketType = "Linked 3 Blinds (Dep)" Or BracketType = "Linked 3 Blinds (Ind)" Then
                                '#blind 1
                                If BlindNo = "Blind 1" Then
                                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    If Not getConnectedId2 = "" Then
                                        getConnectedId2 = " & ITEM ID " & getConnectedId2
                                    End If
                                    If Not getConnectedId = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                                    End If
                                End If

                                '#blind 2
                                If BlindNo = "Blind 2" Then
                                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    If Not getConnectedId2 = "" Then
                                        getConnectedId2 = " & ITEM ID " & getConnectedId2
                                    End If
                                    If Not getConnectedId = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                                    End If
                                End If

                                '#blind 3
                                If BlindNo = "Blind 3" Then
                                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    If Not getConnectedId2 = "" Then
                                        getConnectedId2 = " & ITEM ID " & getConnectedId2
                                    End If
                                    If Not getConnectedId = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                                    End If
                                End If
                            End If

                            '#Double and Link System Dep & Double and Link System Ind
                            If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then
                                '#blinds 1
                                If BlindNo = "Blind 1" Then
                                    Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + UniqueId + "' AND Active = 1")

                                    Dim spare As String = ""
                                    If Not blind3 = "" Then
                                        blind3 = "ITEM ID " & blind3
                                        spare = " & "
                                    End If
                                    If Not blind4 = "" Then
                                        blind4 = " & ITEM ID " & blind4
                                        spare = ", "
                                    End If
                                    If Not blind2 = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* LINKED ITEM ID " & blind2 & spare & blind3 & blind4 & "</small>"
                                    End If
                                End If

                                '#blinds 2
                                If BlindNo = "Blind 2" Then
                                    Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + UniqueId + "' AND Active = 1")

                                    Dim spare As String = ""
                                    If Not blind3 = "" Then
                                        blind3 = "ITEM ID " & blind3
                                        spare = " & "
                                    End If
                                    If Not blind4 = "" Then
                                        blind4 = " & ITEM ID " & blind4
                                        spare = ", "
                                    End If
                                    If Not blind1 = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & spare & blind3 & blind4 & "</small>"
                                    End If
                                End If

                                '#blinds 3
                                If BlindNo = "Blind 3" Then
                                    Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + UniqueId + "' AND Active = 1")

                                    Dim spare As String = ""
                                    If blind4 = "" Then
                                        spare = " & "
                                    End If
                                    If Not blind4 = "" Then
                                        blind4 = " & ITEM ID " & blind4
                                        spare = ", "
                                    End If
                                    If Not blind2 = "" Then
                                        blind2 = "ITEM ID " & blind2
                                    End If
                                    If Not blind1 = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & spare & blind2 & blind4 & "</small>"
                                    End If
                                End If

                                '#blinds 4
                                If BlindNo = "Blind 4" Then
                                    Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    Product += "<br />"
                                    Product += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & ", ITEM ID " & blind2 & " & " & "ITEM ID" & blind3 & "</small>"
                                End If
                            End If

                            '#Double, Linked 2 Blinds (Dep), Linked 2 Blinds (Ind)
                            If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Dep)" Or BracketType = "Linked 2 Blinds (Ind)" Then
                                If BlindNo = "Blind 1" Then
                                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    If Not getConnectedId = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* Complete set with ITEM ID " & getConnectedId & "</small>"
                                    End If
                                End If

                                If BlindNo = "Blind 2" Then
                                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + UniqueId + "' AND Active = 1")
                                    If Not getConnectedId = "" Then
                                        Product += "<br />"
                                        Product += "<small style='color:red;'>* Complete set with ITEM ID " & getConnectedId & "</small>"
                                    End If
                                End If
                            End If

                            If BracketType = "With Tube & Bottom Included" then
                                Product = "Roller Skin Only (+Tube & Bottom Inc) #" & FabricType & " (" & Width & " x " & Drop & ")"
                            End If


                            If BracketType = "With Bottom Included" then
                                Product = "Roller Skin Only (+Bottom Inc) #" & FabricType & " (" & Width & " x " & Drop & ")"
                            End If

                            If BracketType = "With Tube Included" then
                                Product = "Roller Skin Only (+Tube Inc) #" & FabricType & " (" & Width & " x " & Drop & ")"
                            End If

                         End If

                        If DesignName = "Vari Shades" Or DesignName = "Vertical Blinds" Then
                            '#Single & Complate
                            Product = KitName & " #" & FabricType & " (" & Width & " x " & Drop & ")"
                            If BlindName = "Slat Only" Then
                                Product = KitName & " #" & FabricType & " (Drop : " & Drop & "mm)"
                            End If
                            If BlindName = "Track Only" Then
                                Product = KitName & " (Width : " & Width & "mm)"
                            End If
                        End If

                        If DesignName = "Panorama PVC Shutters" Then
                            Product = DesignName & " " & KitName & " - " & Width & "mm x " & Drop & "mm"
                        End If

                        If DesignName = "Panel Glides" Then
                            Product = KitName & " #" & FabricType & " (" & Width & " x " & Drop & ")"
                        End If

                        If DesignName = "Roman Blinds" Then
                            Product = KitName & " #" & FabricType & " (" & Width & " x " & Drop & ")"
                        End If

                        If DesignName = "Cellora Blinds" Then
                            Product = KitName & " #" & FabricType & " (" & Width & " x " & Drop & ")"
                        End If

                        '#----------------|| Hidden Button Next ||----------------#
                        If DesignName = "Roller Blinds" Then

                            Dim TotalBlind As Integer = Convert.ToInt32(publicCfg.GetItemData("SELECT COUNT(Id) FROM OrderDetails WHERE UniqueId = '" + UniqueId + "' AND Active = 1"))
                            If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Ind)" Or BracketType = "Linked 2 Blinds (Dep)" Then
                                HideNext = ""
                                If TotalBlind >= 2 Then : HideNext = "hidden" : End If 
                            End If

                            If BracketType = "Linked 3 Blinds (Ind)" Or BracketType = "Linked 3 Blinds (Dep)" Then
                                HideNext = "hidden"
                                If BlindNo = "Blind 1" And TotalBlind < 2 Then
                                    HideNext = ""
                                End If
                                If BlindNo = "Blind 2" And TotalBlind < 3 Then
                                    HideNext = ""
                                End If
                            End If

                            If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then 'added 240925
                                HideNext = "hidden"
                                If BlindNo = "Blind 1" And TotalBlind < 2 Then
                                    HideNext = ""
                                End If
                                If BlindNo = "Blind 2" And TotalBlind < 3 Then
                                    HideNext = ""
                                End If
                                If BlindNo = "Blind 3" And TotalBlind < 4 Then
                                    HideNext = ""
                                End If
                            End If

                        End If
                        
                        If rolename = "PPIC & DE" And UCase(UserId).ToString() <> UCase(sessionUserId) Then
                            HideNext = "hidden"
                        End If

                        '#----------------|| TextNext ||----------------#
                        If BracketType = "Linked 2 Blinds (Ind)" Or BracketType = "Linked 2 Blinds (Dep)" Then
                            TextNext = "Add 2nd blind that is linked to this blind"
                        End If

                        If BracketType = "Linked 3 Blinds (Ind)" Or BracketType = "Linked 3 Blinds (Dep)" Then
                            TextNext = "Add 2nd blind that is linked to this blind"
                            If BlindNo = "Blind 2" Then
                                TextNext = "Add to complete blind"
                            End If
                        End If

                        If BracketType = "Double and Link System Dep" Or BracketType = "Double and Link System Ind" Then 'added 240925
                            TextNext = "Add a 2rd blind connected to this blind"
                            If BlindNo = "Blind 2" Then
                                TextNext = "Add a 3rd blind connected to this blind"
                            End If
                            If BlindNo = "Blind 3" Then
                                TextNext = "Add to complete blind"
                            End If
                        End If

                        Dim row As New OrdersMatrixReturnRow With {
                            .No = noCounter.ToString(),
                            .Id = Id,
                            .HeaderId = HeaderId,
                            .UserId = UserId,
                            .StatusHeader = StatusHeader,
                            .DesignId = DesignId,
                            .Qty = Qty,
                            .Location = Location,
                            .Product = Product,
                            .HideNext = HideNext,
                            .TextNext = TextNext,
                            .Cost = Cost,
                            .MarkUp = FindMarkUp
                        }
                        resultList.Add(row)
                        noCounter += 1
                    End While
                End Using

            End Using

            ' --- Siapkan Respons ---
            response.draw = params.draw
            response.recordsTotal = totalRecords
            response.recordsFiltered = filteredRecords
            response.data = resultList

            Return response

        Catch ex As Exception
            response.draw = If(params Is Nothing, 0, params.draw)
            response.recordsTotal = 0
            response.recordsFiltered = 0
            response.data = New List(Of OrdersMatrixReturnRow)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function



    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CreatePDFOrder(ByVal headerid As String, ByVal action As String) As Object
        Try
            Dim msg As String = ""
            Dim url As String = ""
            Dim fileDirectory As String = ""
            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + headerid + "' AND Active='1'")

            ' Dim request As HttpRequest = HttpContext.Current.Request
            ' Dim baseUrl As String = request.Url.Scheme & "://" & request.Url.Authority & request.ApplicationPath.TrimEnd("/"c)


            ' Return New ErrorResponse With {
            '     .[error] = New ErrorDetail With {
            '         .message = baseUrl
            '     }
            ' }

            '# --------------------------|| Check Order Detail ||-------------------------------
            If detailData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {
                    .[error] = New ErrorDetail With {
                        .message = "Please add item first."
                    }
                }
            End If

            '# --------------------------|| Check Order Header ||-------------------------------
            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id='" + headerid + "'")
            Dim status As String = headerData.Tables(0).Rows(0).Item("Status").ToString()
            if headerData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {
                    .[error] = New ErrorDetail With {
                        .message = "Order Header not found."
                    }
                }
            End If

            If action = "mail" Then
                If status = "Draft" Or Status = "Cenceled" Then
                    Return New ErrorResponse With {
                        .[error] = New ErrorDetail With {
                            .message = "You can't send an email for a draft or canceled order."
                        }
                    }
                End If
            End If

            '# --------------------------|| Prepare Generate PDF ||-------------------------------
            Dim orderNo As String = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim storeId As String = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
            Dim fileName As String = ("-ORDER-" & orderNo & "-" & storeId & ".pdf").Replace(" ", "")

            If action = "preview" Or action = "download" Then
                fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/preview")

                If action = "preview" Then
                    HttpContext.Current.Session("printPreview") = fileName
                    msg = "Print page is successfully prepared. <br> Click <b>OK</b> to open it."
                    url = "/order/preview"
                End If

            
                If action = "download" Then
                    msg = "Your download is ready. Click <b>OK</b> if download does not start automatically."
                    url = "/Methods/Order/Handler/DowloadPDFOrder.ashx?file=" & fileName & "&keyDownload=invoice"
                End If

                printCfg.CreatePDFOrder(headerid, fileDirectory, fileName)

            End If


            If action = "mail" Or action = "submit" Then
                fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/mail")
                
                If action = "submit" Then
                    msg = "This order was submitted successfully"
                End If

                If action = "mail" Then
                    msg = "This order was sent successfully"
                End If

                ' Ambil domain host saat ini
                Dim currentDomain As String = HttpContext.Current.Request.Url.Host.ToLower()

                ' Hanya kirim email jika domain sesuai
                printCfg.CreatePDFOrder(headerid, fileDirectory, fileName)
                If currentDomain.Contains("onlineorder.au") Then
                    publicCfg.MailOrder(headerid, fileDirectory)
                End If
            End If
            '# --------------------------|| Generate PDF Core ||-------------------------------


            ' Kembalikan respon sukses berupa pesan string
            Return New SuccessResponse With {
                .Success = New SuccessDetail With { .message = msg, .url = url }
            }

        Catch ex As Exception
            Return New ErrorResponse With {
                .[error] = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function SubmitOrder(ByVal headerid As String) As Object
        Try
            If String.IsNullOrEmpty(headerid) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "This order is missing !"
                    }
                }
            End If

            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + headerid + "' AND Active='1'")

            '# --------------------------|| Check Order Detail ||-------------------------------
            If detailData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {
                    .[error] = New ErrorDetail With {
                        .message = "Please add item first."
                    }
                }
            End If

            '#------------------------------------------------|| Prepare Submit ||-------------------------------------------------#
            '#-----------------------------------|| Set default values before submission ||----------------------------------------#
            
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Status='New Order',  SubmittedDate=GETDATE() WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", headerid)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return New SuccessResponse With {
                .Success = New SuccessDetail With { 
                    .message = "Order has been submitted successfully."
                }
            }
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With { .message = ex.Message}
            }
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function DeleteOrderHeader(ByVal id As String) As Object
        Try
         

        '#DELETE
        If String.IsNullOrEmpty(id) Then
             Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = "This order is missing !"
                }
            }
        End If

        If Not String.IsNullOrEmpty(id) Then
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderHeaders SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
        End If 

        Return New SuccessResponse With {
                .Success = New SuccessDetail With { 
                    .message = "Data has been deleted successfully, Click <b>OK</b> to redirect to order page.", 
                    .url = "/order" 
                }
            }
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CreatePDFQuote(ByVal headerid As String, ByVal username As String, ByVal action As String) As Object
        Try
            Dim msg As String = ""
            Dim url As String = ""
            Dim fileDirectory As String = ""
            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + headerid + "' AND Active='1'")

            '# --------------------------|| Check Order Detail ||-------------------------------
            If detailData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {
                    .[error] = New ErrorDetail With {
                        .message = "Please add item first."
                    }
                }
            End If

            '# --------------------------|| Check Order Header ||-------------------------------
            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id='" + headerid + "'")
            if headerData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {
                    .[error] = New ErrorDetail With {
                        .message = "Order Header not found."
                    }
                }
            End If

            '# --------------------------|| Prepare Generate PDF ||-------------------------------
            Dim orderNo As String = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim storeId As String = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
            Dim fileName As String = ("-QUOTE-ORDER-" & orderNo & "-" & storeId & ".pdf").Replace(" ", "")

            fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/quote")

            '# --------------------------|| Prepare Generate PDF ||-------------------------------

            If action = "reprint" or action = "preview" Then
                HttpContext.Current.Session("Reprint") = fileName
                msg = "Print page is successfully prepared. <br> Click <b>OK</b> to open it."
                url = "/order/printquote"
            End If

            If action = "download" Then
                msg = "Your download is ready. Click <b>OK</b> if download does not start automatically."
                url = "/Methods/Order/Handler/DowloadPDFOrder.ashx?file=" & fileName & "&keyDownload=quote"
            End If
            
            printCfg.CreatePDFQuote(headerid, username, fileDirectory, fileName)

            ' Kembalikan respon sukses berupa pesan string
            Return New SuccessResponse With {
                .Success = New SuccessDetail With { .message = msg, .url = url }
            }

        Catch ex As Exception
            Return New ErrorResponse With {
                .[error] = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function UpdateStatusOrder(data As ParamUpdateStatusOrder) As Object
        Try
        Dim msg As String

        '#-------------------------|| SET VALIDATE RULES ||-----------------------#
            '#-------------------------|| id ||-----------------------#
            If String.IsNullOrEmpty(data.id) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "this order is missing !",
                        .field = "#modalChangeStatus #id"
                    }
                }
            End If

            '#-------------------------|| status ||-----------------------#
            If String.IsNullOrEmpty(data.status) Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "status is required !",
                        .field = "#modalChangeStatus #status"
                    }
                }
            End If
            If data.status = data.statusOld Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "you don't choose different changes on status, don't do it with the same status!",
                        .field = "#modalChangeStatus #status"
                    }
                }
            End If


            If data.status = "New Order" Then
                If data.submittedDate = "" Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "submitted date is required !",
                            .field = "#modalChangeStatus #submitteddate"
                        }
                    }
                End If
            End If

            If data.status = "Completed" Then
                If data.completeddate = "" Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "shipped date is required !",
                            .field = "#modalChangeStatus #completeddate"
                        }
                    }
                End If
            End If

            If data.status = "Canceled" Then
                If data.canceleddate = "" Then
                    Return New ErrorResponse With {
                        .error = New ErrorDetail With {
                            .message = "canceled date is required !",
                            .field = "#modalChangeStatus #canceleddate"
                        }
                    }
                End If
            End If
            
            If data.description = "" AndAlso (data.status <> "Draft" AndAlso data.status <> "On Hold" AndAlso data.status <> "In Production") Then
                Return New ErrorResponse With {
                    .error = New ErrorDetail With {
                        .message = "description is required !",
                        .field = "#modalChangeStatus #description"
                    }
                }
            End If
        '#------------------------------------------------|| Prepare Submit ||-------------------------------------------------#
            '#-----------------------------------|| Set default values before submission ||----------------------------------------#
            

            Dim findDesc As String = data.description
            Select Case data.status
                Case "New Order"
                    findDesc = "Status changed to new order by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "In Production"
                    findDesc = "Your order is currently in the production process"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "On Hold"
                    findDesc = "Your order on hold by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "Completed"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
                Case "Canceled"
                    findDesc = "Your order has been canceled by <b>" & data.username & "</b>"
                    findDesc += "<br />"
                    findDesc += "Notes from the office:"
                    findDesc += "<br />"
                    findDesc += data.description
            End Select

            If Not String.IsNullOrEmpty(data.id) Then
                Dim query As String = "UPDATE OrderHeaders SET Status='Draft', StatusDescription=NULL, SubmittedDate=NULL, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                Select Case data.status
                    Case "New Order"
                        query = "UPDATE OrderHeaders SET Status='New Order', StatusDescription=@StatusDescription, SubmittedDate=@SubmittedDate, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                    Case "In Production"
                        query = "UPDATE OrderHeaders SET Status='In Production', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                    Case "On Hold"
                        query = "UPDATE OrderHeaders SET Status='On Hold', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=NULL WHERE Id=@Id"
                    Case "Completed"
                        query = "UPDATE OrderHeaders SET Status='Completed', StatusDescription=@StatusDescription, CanceledDate=NULL, CompletedDate=@CompletedDate WHERE Id=@Id"
                    Case "Canceled"
                        query = "UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=@CanceledDate WHERE Id=@Id"
                End Select

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(query, thisConn)
                        myCmd.Parameters.AddWithValue("@Id", data.id)
                        myCmd.Parameters.AddWithValue("@Status", data.status)
                        myCmd.Parameters.AddWithValue("@StatusDescription", findDesc)
                        myCmd.Parameters.AddWithValue("@SubmittedDate", data.submitteddate)
                        myCmd.Parameters.AddWithValue("@CompletedDate", data.completeddate)
                        myCmd.Parameters.AddWithValue("@CanceledDate", data.canceleddate)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                msg = "Status has been updated successfully."
             End If

            Return New SuccessResponse With {
                .Success = New SuccessDetail With { .message = msg}
            }
        Catch ex As Exception
            Return New ErrorResponse With {
                .error = New ErrorDetail With {
                    .message = ex.Message,
                    .field = ""
                }
            }
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function ReloadPricing(ByVal headerid As String) As Object
        Try
            Dim msg As String = "Reload pricing successfully."
            Dim url As String = ""
            Dim rolename As String = HttpContext.Current.Session("rolename").ToString()

            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id='" & headerid & "'")
            If headerData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Order Header not found."}}
            End If

            Dim status As String = headerData.Tables(0).Rows(0)("Status").ToString()
            If rolename <> "Administrator" AndAlso status <> "Draft" Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Permission denied : not administrator."}}
            End If

            If status = "Canceled" Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Permission denied : order has been canceled."}}
            End If

            ' Ambil semua detail sekaligus
            Dim query As String = "SELECT Id, BlindName, TubeType, FabricId, DesignId, DesignName, BottomHoldDown, FabricGroups FROM view_details WHERE HeaderId='" & headerid & "' AND Active='1' ORDER BY Id, BlindNo, DesignName ASC"
            Dim detailData As DataSet = publicCfg.GetListData(query)

            If detailData.Tables(0).Rows.Count < 1 Then
               Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Order Header not found."}}
            End If

            For Each row As DataRow In detailData.Tables(0).Rows
                Dim itemId = row("Id").ToString()
                Dim blindName = row("BlindName").ToString()
                Dim tubeType = row("TubeType").ToString()
                Dim fabricId = row("FabricId").ToString()
                Dim designId = row("DesignId").ToString()
                Dim designName = row("DesignName").ToString()
                Dim bottomHold = row("BottomHoldDown").ToString()
                Dim fabricGroups = row("FabricGroups").ToString()

                Dim fabricGroup = publicCfg.GetFabricGroup(fabricId)

                Dim priceGroupName = GetPriceGroupName(designName, blindName, tubeType, bottomHold, fabricGroup, fabricGroups)
                If Not String.IsNullOrEmpty(priceGroupName) Then
                    Dim priceGroupId = publicCfg.GetPriceGroupId(designId, priceGroupName)
                    If Not String.IsNullOrEmpty(priceGroupId) Then
                        publicCfg.UpdatePriceGroup(itemId, priceGroupId.ToUpper())
                    End If
                End If

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(headerid, itemId)
                publicCfg.HitungSurcharge(headerid, itemId)
            Next

            Return New SuccessResponse With {
                .Success = New SuccessDetail With {.message = msg, .url = url}
            }
        Catch ex As Exception
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ex.Message}}
        End Try
    End Function



    ' # Fungsi bantu untuk menentukan PriceGroupName
    Private Shared Function GetPriceGroupName(dname As String, bname As String, tube As String, bottomHold As String, fabricGroup As String, fabricGroups As String) As String
        Select Case dname
            Case "Vertical Blinds"
                If bname = "Track Only" Then Return bname & " - " & tube
                If bname = "Slat Only" AndAlso bottomHold = "Top Hanger Only" Then Return bname & " With Hanger - " & fabricGroup
                Return bname & " - " & fabricGroup
            Case "Veri Shades"
                If bname = "Single" Then Return "Veri Shades - " & fabricGroup
                If bname = "Slat Only" Then Return bname & " - " & fabricGroup
                Return bname
            Case "Venetian Blinds", "Aluminium Blinds"
                Return bname
            Case "Roller Blinds"
                If bname = "Skin Only" Then Return "Roller Skin Only - " & fabricGroup
                Return "Roller Blind - " & fabricGroup
            Case "Panorama Shutters"
                Return "Panorama - " & bname
            Case "Panel Glides"
                Return "Panel Glide - " & fabricGroups
            Case "Roman Blinds"
                Return "Roman Blind - " & fabricGroups
            Case Else
                Return ""
        End Select
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CopyItem(ByVal id As String) As Object
        Try
            


            If String.IsNullOrEmpty(id) Then
                Return New ErrorResponse With { .error = New ErrorDetail With {.message = "This order is missing !"}}
            End If

            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + id + "' AND Active='1'")

            If detailData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With { .[error] = New ErrorDetail With { .message = "Order ` not found."}}
            End If

            Dim DesignId As String = detailData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim BracketType As String = detailData.Tables(0).Rows(0).Item("BracketType").ToString()

            Dim NewItemId As string = publicCfg.CreateOrderItemId()
            Dim NewBlindNo As String = "Blind 1"
            Dim NewUniqueId As String = String.Empty
            If BracketType = "Double" Or BracketType = "Linked 2 Blinds (Ind)" Or BracketType = "Linked 2 Blinds (Dep)" Then
                NewUniqueId = GenerateUniqueId()
            End IF

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("INSERT INTO OrderDetails SELECT @IdNew, HeaderId, KitId, SoeKitId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, @UniqueId, BlindNo, Qty, Location, Mounting, Width, WidthB, WidthMiddle, WidthBottom, [Drop], DropB, DropMiddle, DropRight, SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, PanelSize, NumOfPanel, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, StackPosition, TilterPosition, RollDirection, ControlPosition, ControlColour, ControlLength, ChainLength, MaterialChain, MotorStyle, MotorRemote, MotorRequired, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, TrackType, TrackColour, TrackLength, NumOfWand, WandPosition,  WandColour, WandLength, CordColour, CordLength, AcornPlasticColour, Accessory, SideBySide, SlatSize, SlatQty, TubeSize, Trim, Batten, BattenColour,  BracketOption, BracketColour, BracketCover, BracketExtension, Fitting, FlatType, ChildSafe, Cleat, BottomHoldDown, HangerType, PelmetType, PelmetWidth, PelmetSize, PelmetReturn, PelmetReturnPosition, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom, BlindSize, Sloper, InsertInTrack, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active FROM OrderDetails WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", id)
                    myCmd.Parameters.AddWithValue("@IdNew", NewItemId)
                    myCmd.Parameters.AddWithValue("@UniqueId", NewUniqueId)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Return New SuccessResponse With {
                .Success = New SuccessDetail With { 
                    .message = "Data has been copied successfully, Click <b>OK</b> to reload item list."
                }
            }
        Catch ex As Exception
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ex.Message}}
        End Try
    End Function


    Private Shared Function GenerateUniqueId() As String
        Dim result As String = String.Empty

        Dim alphabets As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim small_alphabets As String = "abcdefghijklmnopqrstuvwxyz"
        Dim numbers As String = "1234567890"

        Dim characters As String = Convert.ToString(alphabets & small_alphabets) & numbers
        Dim length As Integer = Integer.Parse(20)
        Dim uniqueId As String = String.Empty
        For i As Integer = 0 To length - 1
            Dim character As String = String.Empty
            Do
                Dim index As Integer = New Random().Next(0, characters.Length)
                character = characters.ToCharArray()(index).ToString()
            Loop While uniqueId.IndexOf(character) <> -1
            uniqueId += character
        Next
        result = uniqueId

        Return result
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function DeleteItem(ByVal id As String) As Object
        Try
            

            If String.IsNullOrEmpty(id) Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "This order is missing !"}}
            End If

            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + id + "' AND Active='1'")
            If detailData.Tables(0).Rows.Count = 0 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "This order is missing !"}}
            End If

            Dim BracketName As String = detailData.Tables(0).Rows(0).Item("BracketType").ToString()
            Dim BlindNo As String = detailData.Tables(0).Rows(0).Item("BlindNo").ToString()
            Dim UniqueId As String = detailData.Tables(0).Rows(0).Item("UniqueId").ToString()

            UpdateDetail(id)
            DeleteDetail(id)

            Dim NewBlindNo As String = String.Empty
            If BracketName = "Double" Or BracketName = "Linked 2 Blinds (Dep)" Or BracketName = "Linked 2 Blinds (Ind)" Then
                If BlindNo = "Blind 1" Then
                    BlindNo = "Blind 2"
                    NewBlindNo = "Blind 1"
                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)
                End If
            End If

            If BracketName = "Linked 3 Blinds (Dep)" Or BracketName = "Linked 3 Blinds (Ind)" Then
                If BlindNo = "Blind 1" Then
                    BlindNo = "Blind 2"
                    NewBlindNo = "Blind 1"

                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)

                    BlindNo = "Blind 3"
                    NewBlindNo = "Blind 2"
                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)
                End If

                If BlindNo = "Blind 2" Then
                    BlindNo = "Blind 3"
                    NewBlindNo = "Blind 2"
                    UpdateDetailBlindNo(NewBlindNo, BlindNo, UniqueId)
                End If
            End If

            Return New SuccessResponse With {
                .Success = New SuccessDetail With { .message = "Data has been deleted successfully, Click <b>OK</b> to reload item list."}}
        Catch ex As Exception
            Return New ErrorResponse With { .error = New ErrorDetail With { .message = ex.Message}}
        End Try
    End Function

    Private Shared Sub UpdateDetail(ByVal id As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=NULL WHERE Id=@Id", thisConn)
                myCmd.Parameters.AddWithValue("@Id", id)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Private Shared Sub DeleteDetail(ByVal id As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@Id", thisConn)
                myCmd.Parameters.AddWithValue("@Id", id)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Private Shared Sub UpdateDetailBlindNo(ByVal newblindno As String, ByVal blindno As String, ByVal uniqueid As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As New SqlCommand("UPDATE OrderDetails SET BlindNo=@BlindNoNew WHERE UniqueId=@UniqueId AND BlindNo=@BlindNo AND Active=1", thisConn)
                myCmd.Parameters.AddWithValue("@BlindNoNew", newblindno)
                myCmd.Parameters.AddWithValue("@BlindNo", blindno)
                myCmd.Parameters.AddWithValue("@UniqueId", uniqueid)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindOrderPricingDetails(params As OrdersPricingParams) As DataTableResponsePricing
        Dim response As New DataTableResponsePricing()
        Dim totalRecords As Integer = 0
        Dim filteredRecords As Integer = 0
        Dim resultList As New List(Of OrdersMatrixReturnRowPricing)()

        
        Try
            Using conn As New SqlConnection(myConn)
                conn.Open()

                
                ' --- 1. Query untuk menghitung Total Records (tanpa filter DataTables, hanya filter awal Anda) ---
                Dim countSql As String = "SELECT COUNT( Id ) FROM OrderDetailsPrice WHERE ItemId=@ItemId"
                Using countCmd As New SqlCommand(countSql, conn)
                    countCmd.Parameters.AddWithValue("@ItemId", params.id)
                    totalRecords = CInt(countCmd.ExecuteScalar())
                End Using
                

                ' --- 2. Bangun Query Utama dengan Filtering, Ordering, dan Pagination ---
                Dim sqlBuilder As New System.Text.StringBuilder()
                sqlBuilder.AppendLine("SELECT *, FORMAT ( CASE WHEN Description LIKE '%Discount%' THEN - Cost ELSE Cost END, 'C', 'en-US' ) AS FormatCost, FORMAT ( CASE WHEN Description LIKE '%Discount%' THEN - FinalCost ELSE FinalCost END, 'C', 'en-US' ) AS FormatFinalCost")
                sqlBuilder.AppendLine("FROM OrderDetailsPrice")
                sqlBuilder.AppendLine("WHERE ItemId = @ItemId")

                Dim whereClause As New System.Text.StringBuilder()
                Dim cmd As New SqlCommand(sqlBuilder.ToString(), conn)
                cmd.Parameters.AddWithValue("@ItemId", params.id)
                


                ' --- Tambahkan Global Search DataTables (jika ada) ---
                If Not String.IsNullOrEmpty(params.search.value) Then
                    Dim searchValue As String = "%" & params.search.value.Trim() & "%"
                    whereClause.AppendLine(" AND ( Description LIKE @SearchValue )")
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue)
                End If

                sqlBuilder.Append(whereClause.ToString())
                
                ' --- Query untuk menghitung Filtered Records ---
                Dim filteredCountSql As String = "SELECT COUNT(T.Id) FROM (" & sqlBuilder.ToString() & ") AS T"
                Using filteredCountCmd As New SqlCommand(filteredCountSql, conn)
                    For Each p As SqlParameter In cmd.Parameters
                        filteredCountCmd.Parameters.Add(New SqlParameter(p.ParameterName, p.Value))
                    Next
                    filteredRecords = CInt(filteredCountCmd.ExecuteScalar())
                End Using
                

                ' ... kode sebelumnya ...
                Dim orderByClause As New System.Text.StringBuilder()
                If params.order IsNot Nothing AndAlso params.order.Count > 0 Then
                    '# Notes: File di bawah ini untuk menambahkan order by ke query
                    Dim columnMap As New Dictionary(Of Integer, String) From { _
                        {0, "No"}, _
                        {1, "Qty"} _
                    }
                    Dim orderColumnIndex As Integer = params.order(0).column
                    Dim orderDirection As String = params.order(0).dir.ToUpper()

                    If columnMap.ContainsKey(orderColumnIndex) AndAlso columnMap(orderColumnIndex) <> "No" Then
                        ' Perbaiki bagian ini:
                        orderByClause.AppendLine(" ORDER BY " & columnMap(orderColumnIndex) & " " & orderDirection)
                    Else
                        ' Default order jika kolom No atau kolom yang tidak bisa di-sort dipilih
                        orderByClause.AppendLine(" ORDER BY Description ASC")
                    End If
                Else
                    ' Default order jika tidak ada order dari DataTables
                    orderByClause.AppendLine(" ORDER BY Description ASC")
                End If
                sqlBuilder.Append(orderByClause.ToString())
                
                ' ... kode selanjutnya ...

                ' --- Tambahkan Pagination (OFFSET/FETCH NEXT untuk SQL Server 2012+) ---
                sqlBuilder.AppendLine(" OFFSET " & params.start.ToString() & " ROWS FETCH NEXT " & params.length.ToString() & " ROWS ONLY")

                cmd.CommandText = sqlBuilder.ToString()

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    Dim noCounter As Integer = params.start + 1 ' Mulai hitung dari offset

                    While reader.Read()
                        Dim Id As String = reader("Id").ToString()
                        Dim HeaderId As String = reader("HeaderId").ToString()
                        Dim ItemId As String = reader("ItemId").ToString()
                        Dim Qty As String = reader("Qty").ToString()
                        Dim Description As String = reader("Description").ToString()
                        Dim Cost As String = reader("FormatCost").ToString()
                        Dim FinalCost As String = reader("FormatFinalCost").ToString()


                        Dim row As New OrdersMatrixReturnRowPricing With {
                            .No = noCounter.ToString(),
                            .Id = Id,
                            .HeaderId = HeaderId,
                            .ItemId = ItemId,
                            .Qty = Qty,
                            .Description = Description,
                            .Cost = Cost,
                            .FinalCost = FinalCost
                        }
                        resultList.Add(row)
                        noCounter += 1
                    End While
                End Using

            End Using

            ' --- Siapkan Respons ---
            response.draw = params.draw
            response.recordsTotal = totalRecords
            response.recordsFiltered = filteredRecords
            response.data = resultList

            Return response

        Catch ex As Exception
            response.draw = If(params Is Nothing, 0, params.draw)
            response.recordsTotal = 0
            response.recordsFiltered = 0
            response.data = New List(Of OrdersMatrixReturnRowPricing)()
            ' Untuk debugging, bisa kirim error ke client, tapi jangan di production
            ' response.error = ex.Message
            Return response
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CheckOrder(ByVal headerid As String, ByVal status As String, ByVal userid As String) As Object
        Try
            Dim msg As String = String.Empty
            Dim url As String = String.Empty
            Dim Action As String = String.Empty
            Dim textSwall As String = String.Empty
            Dim RoleName As String = HttpContext.Current.Session("rolename").ToString()
            Dim sessionUserId As String = HttpContext.Current.Session("userid").ToString()

            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + headerid + "' AND Active=1 ORDER BY Id ASC")
            If detailData.Tables(0).Rows.Count < 1  Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Order Detail not found."}}
            End If

            For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                Dim Id As String = detailData.Tables(0).Rows(i).Item("Id").ToString()
                Dim UniqueId As String = detailData.Tables(0).Rows(i).Item("UniqueId").ToString()
                Dim DesignName As String = detailData.Tables(0).Rows(i).Item("DesignName").ToString()
                Dim BracketType As String = detailData.Tables(0).Rows(i).Item("BracketType").ToString()

                Dim TotalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM view_details WHERE UniqueId = '" + UniqueId + "' AND Active = 1")
                If DesignName = "Roller Blinds" Then

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

                If RoleName = "PPIC & DE" And Not userid = sessionUserId Then
                    Action = "No"
                End If

                textSwall = "You have an incomplete roller blinds order, which is on the ITEM ID "+ msg +" <br /><br />If you want to complete it, please click the <b>Next Item</b> button on the order line ID."
                
            Next

            Return New SuccessResponse With {
                .Success = New SuccessDetail With {.message = textSwall, .url = Action}
            }
        Catch ex As Exception
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ex.Message}}
        End Try
    End Function



    '#----------------------------------------------|| Create JobSheet ||----------------------------------------------#
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CreateJOBOrder(ByVal headerid As String, ByVal action As String) As Object
        Try
            ' Dim rolename As String = HttpContext.Current.Session("rolename").ToString()
            Dim msg As String = ""
            Dim url As String = ""
            Dim JobId As String = String.Empty
            Dim JoNumber As String = String.Empty
            Dim fileDirectory As String = String.Empty

            If action = "convert" then
                JobId = CreateJobId()
                JoNumber = CreateJobNumber()
                UpdateOrderHeader(headerid, JoNumber)
                CreateJobHeaders(JobId, headerid)

                '# Create Job Details
                Dim resultCreateJobDetails As String = CreateJobDetails(JobId, headerid)
                If resultCreateJobDetails <> "200" then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = resultCreateJobDetails}}
                End If

                 msg = "Jobsheet successfully created"
            End If

            If action = "reprint" or action = "preview" or action = "download" then
                '#----------------------------------------------|| cek jobheader ||----------------------------------------------#
                Dim latesJoNumber As String = publicCfg.GetItemData("SELECT JoNumber FROM OrderHeaders WHERE Id = '" + headerid + "'")
                Dim latesJobHeader As DataSet = publicCfg.GetListData("SELECT * FROM JobHeaders WHERE HeaderId = '" + headerid + "' AND JoNumber = '" + latesJoNumber + "'")
                If latesJobHeader.Tables(0).Rows.Count = 0 then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Job data not found."}}
                End If
                
                '#----------------------------------------------|| cek jobdetails & create ||----------------------------------------------#
                JobId = latesJobHeader.Tables(0).Rows(0).Item("Id").ToString()
                Dim checkJobDetail As DataSet = publicCfg.GetListData("SELECT * FROM JobDetails WHERE JobId = '" + JobId + "'")
                If checkJobDetail.Tables(0).Rows.Count = 0 then
                    Dim resultCreateJobDetails As String = CreateJobDetails(JobId, headerid)
                    If resultCreateJobDetails <> "200" then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = resultCreateJobDetails}}
                    End If
                End If

                '#----------------------------------------------|| create pdf ||----------------------------------------------#
                Dim orderNo As String = latesJobHeader.Tables(0).Rows(0).Item("OrderNo").ToString()
                Dim storeId As String = latesJobHeader.Tables(0).Rows(0).Item("StoreId").ToString()
                Dim fileName As String = ("-JOB-ORDER-" & orderNo & "-" & storeId & ".pdf").Replace(" ", "")
                fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/job")

                If action = "reprint" or action = "preview" then

                    Dim resultResetJobSheets As String = ResetJobSheets(JobId)
                    If resultResetJobSheets <> "200" then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = resultResetJobSheets}}
                    End If

                    Dim resultCreateJobSheets As String = CreateJobSheets(JobId)
                    If resultCreateJobSheets <> "200" then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = resultCreateJobSheets}}
                    End If

                    '#kirim file name pdf ke session
                    HttpContext.Current.Session("Reprint") = fileName

                    '#panggil fungsi create pdf
                    Dim  resultCreatePDFJobSheets As String = CreatePDFJobSheets(jobId, fileDirectory, fileName)
                    If resultCreatePDFJobSheets <> "200" then
                        Return New ErrorResponse With {.error = New ErrorDetail With {.message = resultCreatePDFJobSheets}}
                    End If

                    msg = "Print page is successfully prepared <br> click <b>OK</b> to open it."
                    url = "/order/jobsheets/"
                End If
                              

            End If

            

            Return New SuccessResponse With {.Success = New SuccessDetail With {.message = msg, .url = url}}
        Catch ex As Exception
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ex.Message}}
        End Try
    End Function

    Private Shared Function CreateJobId() As String
        Dim result As String = String.Empty
        Dim idDetail As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand("SELECT TOP 1 Id FROM JobHeaders ORDER BY Id DESC", thisConn)
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
        Return result
    End Function
    
    Private Shared Function CreateJobNumber() As String
        Dim result As String = String.Empty
        Dim jobId As Integer = 1
        Dim idDetail As String = String.Empty

        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            ' Mendapatkan JobNumber terbaru dari database
            Using myCmd As New SqlCommand("SELECT TOP 1 JoNumber FROM JobHeaders ORDER BY JoNumber DESC", thisConn)
                Using rdResult = myCmd.ExecuteReader
                    If rdResult.Read() Then
                        idDetail = rdResult.Item("JoNumber").ToString()
                    End If
                End Using
            End Using
            thisConn.Close()
        End Using

        If String.IsNullOrEmpty(idDetail) Then
            ' Jika tidak ada JobNumber, buat JobNumber baru "J000001"
            result = "J" & jobId.ToString("D6")
        Else
            ' Jika JobNumber ada, ambil angka dari JobNumber dan tambah 1
            jobId = Integer.Parse(idDetail.Substring(1)) + 1
            result = "J" & jobId.ToString("D6")
        End If

        Return result
    End Function

    Private Shared Sub UpdateOrderHeader(HeaderId As String, JoNumber As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET JoNumber=@JoNumber WHERE Id=@HeaderId")
                myCmd.Parameters.AddWithValue("@JoNumber", JoNumber)
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Private Shared Sub CreateJobHeaders(JobId As String, HeaderId As String)
        Using thisConn As SqlConnection = New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO JobHeaders (Id, HeaderId, JoNumber, UserId, StoreId, OrderNo, OrderCust, Delivery, Note, Address, Suburb, States, PostCode, Phone, Email, QuoteGST, QuoteDisc, QuoteInstall, QuoteMeasure, Status, StatusDescription, CreatedDate, SubmittedDate, CompletedDate, Active, OrderId, UserName, StoreName, StoreCompany, StoreType) SELECT @JobId, Id As HeaderId, JoNumber, UserId, StoreId, OrderNo, OrderCust, Delivery, Note, Address, Suburb, States, PostCode, Phone, Email, QuoteGST, QuoteDisc, QuoteInstall, QuoteMeasure, Status, StatusDescription, CreatedDate, SubmittedDate, CompletedDate, Active, OrderId, UserName, StoreName, StoreCompany, StoreType FROM view_headers WHERE Id=@HeaderId", thisConn)
                myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                myCmd.Parameters.AddWithValue("@JobId", JobId)
                myCmd.Connection = thisConn
                thisConn.Open()
                myCmd.ExecuteNonQuery()
                thisConn.Close()
            End Using
        End Using
    End Sub

    Private Shared Function CreateJobDetails(JobId As String, HeaderId As String) As String
        Try
            Dim dt As New DataTable()
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Dim selectQuery As String = "SELECT * FROM view_details WHERE HeaderId = @HeaderId AND Active = @Active ORDER BY Id ASC"
                Using da As New SqlDataAdapter(selectQuery, thisConn)
                    da.SelectCommand.Parameters.AddWithValue("@HeaderId", HeaderId)
                    da.SelectCommand.Parameters.AddWithValue("@Active", 1)
                    da.Fill(dt)
                End Using

            '    Return "view_details count : " & dt.Rows.Count
                ' Return "405 " & dt.Rows.Count

                thisConn.Open()
                Dim counter As Integer = 1
                For Each row As DataRow In dt.Rows
                    ' === VALIDASI CONTOH ===
                    Dim fields As String = String.Empty
                    Dim values As String = String.Empty
                    Dim TubeSkinSize As Integer = 0
                    Dim NumBoldNuts As Integer = 0
                    Dim LinkBlind As String = String.Empty
                    Dim BlindName As String = row("BlindName").ToString()
                    Dim KitName As String = row("KitName").ToString()
                    Dim BracketType As String = row("BracketType").ToString()
                    If BracketType.Contains("Linked") Then
                        LinkBlind = "Linked"
                    End If
                    '#--------------------------|| TubeSkinSize ||--------------------------#
                    TubeSkinSize = GetTubeSkinSize(row)

                    '#--------------------------|| NumBoldNuts ||--------------------------#
                    NumBoldNuts = GetNumBoldNuts(row)

                    If KitName.Contains("Roman") Then
                        If BlindName.Contains("Plantation") Then
                            BlindName = "Roman Plantation"
                        ElseIf BlindName.Contains("Sewless") Then
                            BlindName = "Roman Sewless"
                        End If
                    End If
                    
                    If KitName.Contains("Panel") Then
                        If BlindName.Contains("Plantation") Then
                            BlindName = "Panel Plantation"
                        ElseIf BlindName.Contains("Sewless") Then
                            BlindName = "Panel Sewless"
                        End If
                    End If

                    If BlindName.Contains("Aluminium") Then
                        BlindName = "Aluminium"
                    ElseIf BlindName.Contains("Mockwood") Then
                        BlindName = "Mockwood Venetian"
                    ElseIf BlindName.Contains("Timberstyle") Then
                        BlindName = "Timber Venetian"
                    ElseIf BlindName.Contains("Wooden") Then
                        BlindName = "Wooden Venetian"
                    ElseIf BlindName.Contains("Venetian") Then
                        BlindName = "Venetian"
                    End If

                    Dim lineString As String = "Line " & counter.ToString()

                    Dim Spacer As String = GetSpacer(row)
                    Dim CarrierQty As String = GetCarrier(row)
                    Dim FabricCutDrop As Integer = GetFabricCutDrop(row, CarrierQty)
                    


                    ' === INSERT QUERY ===
                    fields = "JobId, ItemId, HeaderId, LinkBlind, BlindNo, Line, Qty, Location, Mounting, Width, WidthB, WidthMiddle, WidthBottom, [Drop], DropB, DropMiddle, DropRight, SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, PanelSize, NumOfPanel, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, StackPosition, TilterPosition, RollDirection, ControlPosition, ControlColour, ControlLength, ChainLength, MaterialChain, MotorStyle, MotorRemote, MotorRequired, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, TrackType, TrackColour, TrackLength, NumOfWand, WandPosition, WandColour, WandLength, CordColour, CordLength, AcornPlasticColour, Accessory, SideBySide, SlatSize, SlatQty, TubeSize, [Trim], Batten, BattenColour, BracketOption, BracketColour, BracketCover, BracketExtension, Fitting, FlatType, ChildSafe, Cleat, BottomHoldDown, HangerType, PelmetType, PelmetWidth, PelmetSize, PelmetReturn, PelmetReturnPosition, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom, BlindSize, Sloper, InsertInTrack, Notes, KitName, VenetianType, BracketType, TubeType, TubeSkinSize, NumBoldNuts, Spacer, CarrierQty, FabricCutDrop,  ControlType, ColourType, DesignName, BlindName, ChainName, ChainColour, CLength, BottomName, BottomType, BottomColour, FabricName, FabricType, FabricColour, FabricWidth, FabricGroups, OrderDelivery, PriceGroupName"

                    values = "@JobId, @ItemId, @HeaderId, @LinkBlind, @BlindNo, @Line, @Qty, @Location, @Mounting, @Width, @WidthB, @WidthMiddle, @WidthBottom, @Drop, @DropB, @DropMiddle, @DropRight, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @Layout, @LayoutSpecial, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @BottomTrackRecess, @Buildout, @BuildoutPosition, @PanelQty, @TrackQty, @PanelSize, @NumOfPanel, @HingeQtyPerPanel, @PanelQtyWithHinge, @LocationTPost1, @LocationTPost2, @LocationTPost3, @LocationTPost4, @LocationTPost5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @DoorCutOut, @SpecialShape, @TemplateProvided, @SquareMetre, @LinearMetre, @StackPosition, @TilterPosition, @RollDirection, @ControlPosition, @ControlColour, @ControlLength, @ChainLength, @MaterialChain, @MotorStyle, @MotorRemote, @MotorRequired, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @TrackType, @TrackColour, @TrackLength, @NumOfWand, @WandPosition, @WandColour, @WandLength, @CordColour, @CordLength, @AcornPlasticColour, @Accessory, @SideBySide, @SlatSize, @SlatQty, @TubeSize, @Trim, @Batten, @BattenColour, @BracketOption, @BracketColour, @BracketCover, @BracketExtension, @Fitting, @FlatType, @ChildSafe, @Cleat, @BottomHoldDown, @HangerType, @PelmetType, @PelmetWidth, @PelmetSize, @PelmetReturn, @PelmetReturnPosition, @PelmetReturnSize, @PelmetReturnSize2, @CutOut_LeftTop, @CutOut_RightTop, @CutOut_LeftBottom, @CutOut_RightBottom, @LHSWidth_Top, @LHSHeight_Top, @RHSWidth_Top, @RHSHeight_Top, @LHSWidth_Bottom, @LHSHeight_Bottom, @RHSWidth_Bottom, @RHSHeight_Bottom, @BlindSize, @Sloper, @InsertInTrack, @Notes, @KitName, @VenetianType, @BracketType, @TubeType, @TubeSkinSize, @NumBoldNuts, @Spacer, @CarrierQty, @FabricCutDrop, @ControlType, @ColourType, @DesignName, @BlindName, @ChainName, @ChainColour, @CLength, @BottomName, @BottomType, @BottomColour, @FabricName, @FabricType, @FabricColour, @FabricWidth, @FabricGroups, @OrderDelivery, @PriceGroupName"

                    Dim insertQuery As String = "INSERT INTO JobDetails (" & fields & ") VALUES (" & values & ")"

                    Using insertCmd As New SqlCommand(insertQuery, thisConn)
                        insertCmd.Parameters.AddWithValue("@JobId", JobId)
                        insertCmd.Parameters.AddWithValue("@ItemId", row("Id"))
                        insertCmd.Parameters.AddWithValue("@HeaderId", row("HeaderId"))
                        insertCmd.Parameters.AddWithValue("@LinkBlind", LinkBlind)
                        insertCmd.Parameters.AddWithValue("@BlindNo", row("BlindNo"))
                        insertCmd.Parameters.AddWithValue("@Line", lineString)
                        insertCmd.Parameters.AddWithValue("@Qty", row("Qty"))
                        insertCmd.Parameters.AddWithValue("@Location", row("Location"))
                        insertCmd.Parameters.AddWithValue("@Mounting", row("Mounting"))
                        insertCmd.Parameters.AddWithValue("@Width", row("Width"))
                        insertCmd.Parameters.AddWithValue("@WidthB", row("WidthB"))
                        insertCmd.Parameters.AddWithValue("@WidthMiddle", row("WidthMiddle"))
                        insertCmd.Parameters.AddWithValue("@WidthBottom", row("WidthBottom"))
                        insertCmd.Parameters.AddWithValue("@Drop", row("Drop"))
                        insertCmd.Parameters.AddWithValue("@DropB", row("DropB"))
                        insertCmd.Parameters.AddWithValue("@DropMiddle", row("DropMiddle"))
                        insertCmd.Parameters.AddWithValue("@DropRight", row("DropRight"))
                        insertCmd.Parameters.AddWithValue("@SemiInsideMount", row("SemiInsideMount"))
                        insertCmd.Parameters.AddWithValue("@LouvreSize", row("LouvreSize"))
                        insertCmd.Parameters.AddWithValue("@LouvrePosition", row("LouvrePosition"))
                        insertCmd.Parameters.AddWithValue("@HingeColour", row("HingeColour"))
                        insertCmd.Parameters.AddWithValue("@MidrailHeight1", row("MidrailHeight1"))
                        insertCmd.Parameters.AddWithValue("@MidrailHeight2", row("MidrailHeight2"))
                        insertCmd.Parameters.AddWithValue("@MidrailCritical", row("MidrailCritical"))
                        insertCmd.Parameters.AddWithValue("@Layout", row("Layout"))
                        insertCmd.Parameters.AddWithValue("@LayoutSpecial", row("LayoutSpecial"))
                        insertCmd.Parameters.AddWithValue("@CustomHeaderLength", row("CustomHeaderLength"))
                        insertCmd.Parameters.AddWithValue("@FrameType", row("FrameType"))
                        insertCmd.Parameters.AddWithValue("@FrameLeft", row("FrameLeft"))
                        insertCmd.Parameters.AddWithValue("@FrameRight", row("FrameRight"))
                        insertCmd.Parameters.AddWithValue("@FrameTop", row("FrameTop"))
                        insertCmd.Parameters.AddWithValue("@FrameBottom", row("FrameBottom"))
                        insertCmd.Parameters.AddWithValue("@BottomTrackType", row("BottomTrackType"))
                        insertCmd.Parameters.AddWithValue("@BottomTrackRecess", row("BottomTrackRecess"))
                        insertCmd.Parameters.AddWithValue("@Buildout", row("Buildout"))
                        insertCmd.Parameters.AddWithValue("@BuildoutPosition", row("BuildoutPosition"))
                        insertCmd.Parameters.AddWithValue("@PanelQty", row("PanelQty"))
                        insertCmd.Parameters.AddWithValue("@TrackQty", row("TrackQty"))
                        insertCmd.Parameters.AddWithValue("@PanelSize", row("PanelSize"))
                        insertCmd.Parameters.AddWithValue("@NumOfPanel", row("NumOfPanel"))
                        insertCmd.Parameters.AddWithValue("@HingeQtyPerPanel", row("HingeQtyPerPanel"))
                        insertCmd.Parameters.AddWithValue("@PanelQtyWithHinge", row("PanelQtyWithHinge"))
                        insertCmd.Parameters.AddWithValue("@LocationTPost1", row("LocationTPost1"))
                        insertCmd.Parameters.AddWithValue("@LocationTPost2", row("LocationTPost2"))
                        insertCmd.Parameters.AddWithValue("@LocationTPost3", row("LocationTPost3"))
                        insertCmd.Parameters.AddWithValue("@LocationTPost4", row("LocationTPost4"))
                        insertCmd.Parameters.AddWithValue("@LocationTPost5", row("LocationTPost5"))
                        insertCmd.Parameters.AddWithValue("@HorizontalTPost", row("HorizontalTPost"))
                        insertCmd.Parameters.AddWithValue("@HorizontalTPostHeight", row("HorizontalTPostHeight"))
                        insertCmd.Parameters.AddWithValue("@JoinedPanels", row("JoinedPanels"))
                        insertCmd.Parameters.AddWithValue("@ReverseHinged", row("ReverseHinged"))
                        insertCmd.Parameters.AddWithValue("@PelmetFlat", row("PelmetFlat"))
                        insertCmd.Parameters.AddWithValue("@ExtraFascia", row("ExtraFascia"))
                        insertCmd.Parameters.AddWithValue("@HingesLoose", row("HingesLoose"))
                        insertCmd.Parameters.AddWithValue("@TiltrodType", row("TiltrodType"))
                        insertCmd.Parameters.AddWithValue("@TiltrodSplit", row("TiltrodSplit"))
                        insertCmd.Parameters.AddWithValue("@SplitHeight1", row("SplitHeight1"))
                        insertCmd.Parameters.AddWithValue("@SplitHeight2", row("SplitHeight2"))
                        insertCmd.Parameters.AddWithValue("@DoorCutOut", row("DoorCutOut"))
                        insertCmd.Parameters.AddWithValue("@SpecialShape", row("SpecialShape"))
                        insertCmd.Parameters.AddWithValue("@TemplateProvided", row("TemplateProvided"))
                        insertCmd.Parameters.AddWithValue("@SquareMetre", row("SquareMetre"))
                        insertCmd.Parameters.AddWithValue("@LinearMetre", row("LinearMetre"))
                        insertCmd.Parameters.AddWithValue("@StackPosition", row("StackPosition"))
                        insertCmd.Parameters.AddWithValue("@TilterPosition", row("TilterPosition"))
                        insertCmd.Parameters.AddWithValue("@RollDirection", row("RollDirection"))
                        insertCmd.Parameters.AddWithValue("@ControlPosition", row("ControlPosition"))
                        insertCmd.Parameters.AddWithValue("@ControlColour", row("ControlColour"))
                        insertCmd.Parameters.AddWithValue("@ControlLength", row("ControlLength"))
                        insertCmd.Parameters.AddWithValue("@ChainLength", row("ChainLength"))
                        insertCmd.Parameters.AddWithValue("@MaterialChain", row("MaterialChain"))
                        insertCmd.Parameters.AddWithValue("@MotorStyle", row("MotorStyle"))
                        insertCmd.Parameters.AddWithValue("@MotorRemote", row("MotorRemote"))
                        insertCmd.Parameters.AddWithValue("@MotorRequired", row("MotorRequired"))
                        insertCmd.Parameters.AddWithValue("@MotorBattery", row("MotorBattery"))
                        insertCmd.Parameters.AddWithValue("@MotorCharger", row("MotorCharger"))
                        insertCmd.Parameters.AddWithValue("@Connector", row("Connector"))
                        insertCmd.Parameters.AddWithValue("@AdditionalMotor", row("AdditionalMotor"))
                        insertCmd.Parameters.AddWithValue("@CableExitPoint", row("CableExitPoint"))
                        insertCmd.Parameters.AddWithValue("@TrackType", row("TrackType"))
                        insertCmd.Parameters.AddWithValue("@TrackColour", row("TrackColour"))
                        insertCmd.Parameters.AddWithValue("@TrackLength", row("TrackLength"))
                        insertCmd.Parameters.AddWithValue("@NumOfWand", row("NumOfWand"))
                        insertCmd.Parameters.AddWithValue("@WandPosition", row("WandPosition"))
                        insertCmd.Parameters.AddWithValue("@WandColour", row("WandColour"))
                        insertCmd.Parameters.AddWithValue("@WandLength", row("WandLength"))
                        insertCmd.Parameters.AddWithValue("@CordColour", row("CordColour"))
                        insertCmd.Parameters.AddWithValue("@CordLength", row("CordLength"))
                        insertCmd.Parameters.AddWithValue("@AcornPlasticColour", row("AcornPlasticColour"))
                        insertCmd.Parameters.AddWithValue("@Accessory", row("Accessory"))
                        insertCmd.Parameters.AddWithValue("@SideBySide", row("SideBySide"))
                        insertCmd.Parameters.AddWithValue("@SlatSize", row("SlatSize"))
                        insertCmd.Parameters.AddWithValue("@SlatQty", row("SlatQty"))
                        insertCmd.Parameters.AddWithValue("@TubeSize", row("TubeSize"))
                        insertCmd.Parameters.AddWithValue("@Trim", row("Trim"))
                        insertCmd.Parameters.AddWithValue("@Batten", row("Batten"))
                        insertCmd.Parameters.AddWithValue("@BattenColour", row("BattenColour"))
                        insertCmd.Parameters.AddWithValue("@BracketOption", row("BracketOption"))
                        insertCmd.Parameters.AddWithValue("@BracketColour", row("BracketColour"))
                        insertCmd.Parameters.AddWithValue("@BracketCover", row("BracketCover"))
                        insertCmd.Parameters.AddWithValue("@BracketExtension", row("BracketExtension"))
                        insertCmd.Parameters.AddWithValue("@Fitting", row("Fitting"))
                        insertCmd.Parameters.AddWithValue("@FlatType", row("FlatType"))
                        insertCmd.Parameters.AddWithValue("@ChildSafe", row("ChildSafe"))
                        insertCmd.Parameters.AddWithValue("@Cleat", row("Cleat"))
                        insertCmd.Parameters.AddWithValue("@BottomHoldDown", row("BottomHoldDown"))
                        insertCmd.Parameters.AddWithValue("@HangerType", row("HangerType"))
                        insertCmd.Parameters.AddWithValue("@PelmetType", row("PelmetType"))
                        insertCmd.Parameters.AddWithValue("@PelmetWidth", row("PelmetWidth"))
                        insertCmd.Parameters.AddWithValue("@PelmetSize", row("PelmetSize"))
                        insertCmd.Parameters.AddWithValue("@PelmetReturn", row("PelmetReturn"))
                        insertCmd.Parameters.AddWithValue("@PelmetReturnPosition", row("PelmetReturnPosition"))
                        insertCmd.Parameters.AddWithValue("@PelmetReturnSize", row("PelmetReturnSize"))
                        insertCmd.Parameters.AddWithValue("@PelmetReturnSize2", row("PelmetReturnSize2"))
                        insertCmd.Parameters.AddWithValue("@CutOut_LeftTop", row("CutOut_LeftTop"))
                        insertCmd.Parameters.AddWithValue("@CutOut_RightTop", row("CutOut_RightTop"))
                        insertCmd.Parameters.AddWithValue("@CutOut_LeftBottom", row("CutOut_LeftBottom"))
                        insertCmd.Parameters.AddWithValue("@CutOut_RightBottom", row("CutOut_RightBottom"))
                        insertCmd.Parameters.AddWithValue("@LHSWidth_Top", row("LHSWidth_Top"))
                        insertCmd.Parameters.AddWithValue("@LHSHeight_Top", row("LHSHeight_Top"))
                        insertCmd.Parameters.AddWithValue("@RHSWidth_Top", row("RHSWidth_Top"))
                        insertCmd.Parameters.AddWithValue("@RHSHeight_Top", row("RHSHeight_Top"))
                        insertCmd.Parameters.AddWithValue("@LHSWidth_Bottom", row("LHSWidth_Bottom"))
                        insertCmd.Parameters.AddWithValue("@LHSHeight_Bottom", row("LHSHeight_Bottom"))
                        insertCmd.Parameters.AddWithValue("@RHSWidth_Bottom", row("RHSWidth_Bottom"))
                        insertCmd.Parameters.AddWithValue("@RHSHeight_Bottom", row("RHSHeight_Bottom"))
                        insertCmd.Parameters.AddWithValue("@BlindSize", row("BlindSize"))
                        insertCmd.Parameters.AddWithValue("@Sloper", row("Sloper"))
                        insertCmd.Parameters.AddWithValue("@InsertInTrack", row("InsertInTrack"))
                        insertCmd.Parameters.AddWithValue("@Notes", row("Notes"))
                        insertCmd.Parameters.AddWithValue("@KitName", row("KitName"))
                        insertCmd.Parameters.AddWithValue("@VenetianType", row("BlindName"))
                        insertCmd.Parameters.AddWithValue("@BracketType", row("BracketType"))
                        insertCmd.Parameters.AddWithValue("@TubeType", row("TubeType"))
                        insertCmd.Parameters.AddWithValue("@TubeSkinSize", TubeSkinSize)
                        insertCmd.Parameters.AddWithValue("@NumBoldNuts", NumBoldNuts)
                        insertCmd.Parameters.AddWithValue("@Spacer", Spacer)
                        insertCmd.Parameters.AddWithValue("@CarrierQty", CarrierQty)
                        insertCmd.Parameters.AddWithValue("@FabricCutDrop", FabricCutDrop)
                        insertCmd.Parameters.AddWithValue("@ControlType", row("ControlType"))
                        insertCmd.Parameters.AddWithValue("@ColourType", row("ColourType"))
                        insertCmd.Parameters.AddWithValue("@DesignName", row("DesignName"))
                        insertCmd.Parameters.AddWithValue("@BlindName", BlindName)
                        insertCmd.Parameters.AddWithValue("@ChainName", row("ChainName"))
                        insertCmd.Parameters.AddWithValue("@ChainColour", row("ChainColour"))
                        insertCmd.Parameters.AddWithValue("@CLength", row("CLength"))
                        insertCmd.Parameters.AddWithValue("@BottomName", row("BottomName"))
                        insertCmd.Parameters.AddWithValue("@BottomType", row("BottomType"))
                        insertCmd.Parameters.AddWithValue("@BottomColour", row("BottomColour"))
                        insertCmd.Parameters.AddWithValue("@FabricName", row("FabricName"))
                        insertCmd.Parameters.AddWithValue("@FabricType", row("FabricType"))
                        insertCmd.Parameters.AddWithValue("@FabricColour", row("FabricColour"))
                        insertCmd.Parameters.AddWithValue("@FabricWidth", row("FabricWidth"))
                        insertCmd.Parameters.AddWithValue("@FabricGroups", row("FabricGroups"))
                        insertCmd.Parameters.AddWithValue("@OrderDelivery", row("OrderDelivery"))
                        insertCmd.Parameters.AddWithValue("@PriceGroupName", row("PriceGroupName"))
                        insertCmd.Connection = thisConn
                        insertCmd.ExecuteNonQuery()
                    End Using

                    counter += 1
                Next

                thisConn.Close()
            End Using

            Return "200"
        Catch ex As Exception
            Return "500: " & ex.Message
        End Try
    End Function

    Private Shared Function GetTubeSkinSize(row As DataRow) As Integer
        Dim result As Integer = 0
        Dim KitName As String = row("KitName").ToString()
        Dim BlindName As String = row("BlindName").ToString()
        Dim BracketType As String = row("BracketType").ToString()
        Dim TubeType As String = row("TubeType").ToString()
        Dim ControlPosition As String = row("ControlPosition").ToString()
        Dim TubeSize As String = row("TubeSize").ToString()
        Dim Width As Integer = CInt(row("Width").ToString())

        '#..........................................|| Blinds ||..........................................#
        If BlindName = "Roller Blind" Then
            '#-----------------------|| JAI / LOV ||-----------------------#
            If InStr(KitName, "JAI") > 0 Or InStr(KitName, "LOV") > 0 Or InStr(TubeType, "JAI") > 0 Or InStr(TubeType, "LOV") > 0 Then

                '#-----------------------|| Single, Double, Linked (Ind), Double and Link(Ind) ||-----------------------#
                If BracketType = "Single" Or BracketType = "Double" Or InStr(BracketType, "Ind") > 0 Then
                    If TubeSize = "40" Then : result = Width - 28 : End IF
                    If TubeSize = "45" Or TubeSize = "45H" Then : result = Width - 32 : End IF
                    If TubeSize = "50" Then : result = Width - 28 : End IF
                End If

                '#-----------------------|| Linked (Dep), Double and Link(Dep) ||-----------------------#
                If InStr(BracketType, "Dep") > 0 Then
                    If ControlPosition = "Left" Or ControlPosition = "Right" Then
                        If TubeSize = "40" Then : result = Width - 28 : End IF
                        If TubeSize = "45" Or TubeSize = "45H" Then : result = Width - 32 : End IF
                        If TubeSize = "50" Then : result = Width - 28 : End IF
                    End If
                    If String.IsNullOrEmpty(ControlPosition) Or ControlPosition = "N/A" Then
                        If TubeSize = "40" Then : result = Width - 22 : End IF
                        If TubeSize = "45" Or TubeSize = "45H" Then : result = Width - 26 : End IF
                        If TubeSize = "50" Then : result = Width - 22 : End IF
                    End If
                End If


            End If
            '#-----------------------|| Spring System ||-----------------------#
            If InStr(KitName, "Spring System") > 0 Or InStr(TubeType, "Spring") > 0 Then
            '#-----------------------|| Null/Empty, N/A ||-----------------------#
                If String.IsNullOrEmpty(ControlPosition) Or ControlPosition = "N/A" Then
                    If TubeSize = "40" Then : result = Width - 28 : End IF
                    If TubeSize = "45" Or TubeSize = "45H" Then : result = Width - 32 : End IF
                End If
            End If
        End If


        '#..........................................|| Motorised ||..........................................#
        If BlindName = "Motorised" Then
            '#-----------------------|| JAI / LOV ||-----------------------#
            If InStr(KitName, "Alpha RTS 45") > 0 Or InStr(KitName, "Alpha RTS 45H") > 0 Or InStr(KitName, "Alpha WF 45") > 0 Or InStr(KitName, "Alpha WF 45H") > 0 Or InStr(KitName, "Alpha WS 45") > 0 Or InStr(KitName, "Alpha WS 45H") > 0 Or InStr(KitName, "Somfy RTS 45") > 0 Or InStr(KitName, "Somfy RTS 45H") > 0 Or InStr(KitName, "Somfy WF 45") > 0 Or InStr(KitName, "Somfy WF 45H") > 0 Or InStr(KitName, "Somfy WS 45") > 0 Or InStr(KitName, "Somfy WS 45H") > 0 Then
                '#-----------------------|| Single, Double, Linked, Double and Link ||-----------------------#
                If BracketType = "Single" Or BracketType = "Double" Or InStr(BracketType, "Linked") > 0 Or InStr(BracketType, "Double and Link") > 0 Then
                    '#-----------------------|| Left or Right Or N/A ||-----------------------#
                    If ControlPosition = "Left" Or ControlPosition = "Right" Then
                        If TubeSize = "45" Or TubeSize = "45H" Then : result = Width - 34 : End IF
                    End If
                    If ControlPosition = "" Or ControlPosition = "N/A" Then
                        If TubeSize = "45" Or TubeSize = "45H" Then : result = Width - 24 : End IF
                    End If
                End If
            End If

            '#-----------------------|| Acmeda ||-----------------------#
            If InStr(KitName, "Somfy RTS 63 Acmeda") > 0 Or InStr(KitName, "Somfy WS 63 Acmeda") > 0 Then
                '#-----------------------|| Single, Double ||-----------------------#
                If BracketType = "Single" Or BracketType = "Double"  Then
                    '#-----------------------|| N/A ||-----------------------#
                    If ControlPosition <> "" Or Not ControlPosition <> "N/A" Then
                       result = Width - 40
                    End If
                End If

                '#-----------------------|| Linked 2 Dep ||-----------------------#
                If InStr(BracketType, "Linked 2 Blinds (Dep)") > 0 Then
                    '#-----------------------|| Left or Right Or N/A ||-----------------------#
                    If ControlPosition = "Left" Or ControlPosition = "Right" Then
                        result = Width - 39
                    End If
                    If ControlPosition = "" Or ControlPosition = "N/A" Then
                        result = Width - 28
                    End If
                End If

                '#-----------------------|| Linked 2 Ind ||-----------------------#
                If InStr(BracketType, "Linked 2 Blinds (Ind)") > 0 Then
                    '#-----------------------|| Left or Right Or N/A ||-----------------------#
                    If ControlPosition = "Left" Or ControlPosition = "Right" Or ControlPosition <> "" Or ControlPosition <> "N/A" Then
                        result = Width - 39
                    End If
                End If
            End If

        End If



        Return result
    End Function

    Private Shared Function GetNumBoldNuts(row As DataRow) As Integer
        Dim result As Integer = 0
        Dim kitName As String = row("KitName").ToString()
        Dim bottomType As String = row("BottomType").ToString()
        Dim tubeSize As String = row("TubeSize").ToString()
        Dim drop As Integer = CInt(row("Drop").ToString())
        Dim trim As String = row("Trim").ToString()

        Select Case tubeSize
            Case "40"
                If trim = "1F" AndAlso (bottomType = "Oval" OrElse (InStr(bottomType, "Flat") > 0 AndAlso bottomType <> "Flat Wrapped")) Then
                    result = drop + 200
                End If
                If trim = "1P" OrElse String.IsNullOrEmpty(bottomType) OrElse bottomType = "Flat Wrapped" OrElse bottomType = "Oval Bumper" Then
                    result = drop + 250
                End If
            Case "45","45H","50"
                If trim = "1F" AndAlso (bottomType = "Oval" OrElse (InStr(bottomType, "Flat") > 0 AndAlso bottomType <> "Flat Wrapped")) Then
                    result = drop + 300
                End If
                If trim = "1P" OrElse String.IsNullOrEmpty(bottomType) OrElse bottomType = "Flat Wrapped" OrElse bottomType = "Oval Bumper" Then
                    result = drop + 350
                End If
            Case "63"
                If trim = "1F" AndAlso (bottomType = "Oval" OrElse (InStr(bottomType, "Flat") > 0 AndAlso bottomType <> "Flat Wrapped")) Then
                    result = drop + 350
                End If
                If trim = "1P" OrElse String.IsNullOrEmpty(bottomType) OrElse bottomType = "Flat Wrapped" OrElse bottomType = "Oval Bumper" Then
                    result = drop + 400
                End If
        End Select

        Return result
    End Function


    
    Private Shared Function GetSpacer(row As DataRow) As String
        Dim result As String = "0"
        Dim SlatSize As String = row("SlatSize").ToString()
        Dim TubeType As String = row("TubeType").ToString()
        Dim DesignName As String = row("DesignName").ToString()
        Dim BlindName As String = row("BlindName").ToString()
        Dim Width As Integer = CInt(row("Width").ToString())

        If DesignName = "Vertical Blinds" Then
            If BlindName = "Complete" OR BlindName = "Track Only" Then
                Select Case SlatSize
                    Case "127", "127mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer127Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer127Metal
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        End If
                    Case "100", "100mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer100Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer100Metal
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        End If
                    Case "89", "89mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer89Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer89Metal
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        End If
                    Case "63", "63mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer63Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer63Metal
                                If Width <= item.MaxWidth Then
                                    result = item.Spacer1Type
                                    Exit For
                                End If
                            Next
                        End If
                End Select
            End If
        End If
        Return result
    End Function

    Private Shared Function GetCarrier(row As DataRow) As String
        Dim result As String = "0"
        Dim SlatSize As String = row("SlatSize").ToString()
        Dim TubeType As String = row("TubeType").ToString()
        Dim DesignName As String = row("DesignName").ToString()
        Dim BlindName As String = row("BlindName").ToString()
        Dim Width As Integer = CInt(row("Width").ToString())

        If DesignName = "Vertical Blinds" Then
            If BlindName = "Complete" OR BlindName = "Track Only" Then
                Select Case SlatSize
                    Case "127", "127mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer127Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer127Metal
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        End If
                    Case "100", "100mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer100Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer100Metal
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        End If
                    Case "89", "89mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer89Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer89Metal
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        End If
                    Case "63", "63mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer63Tiltrack
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer63Metal
                                If Width <= item.MaxWidth Then
                                    result = item.CarriersQty
                                    Exit For
                                End If
                            Next
                        End If
                End Select
            End If
        End If
        Return result
    End Function

    Private Shared Function GetFabricCutDrop(row As DataRow, CarrierQty As Integer) As Integer
        Dim result As Integer = 0
        Dim Drop As Integer = CInt(row("Drop").ToString())
        Dim Qty As Integer = CInt(row("Qty").ToString())
        Dim BlindName As String = row("BlindName").ToString()

        Dim tempValue As Double = (((Drop + 92) * CarrierQty * Qty) / 1000)
        result = CInt(Math.Floor(tempValue)) + If(tempValue Mod 1 > 0, 1, 0)

        If BlindName.Contains("Slat") Then
            result = 0
        End If

        Return result
    End Function

    Private Shared ReadOnly Spacer127Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 249, .Spacer1Type = "110", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 250, .Spacer1Type = "111", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 251, .Spacer1Type = "112", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "113", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 253, .Spacer1Type = "114", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 254, .Spacer1Type = "115", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 255, .Spacer1Type = "116", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 358, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 358, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 360, .Spacer1Type = "111", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 362, .Spacer1Type = "112", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 364, .Spacer1Type = "113", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 366, .Spacer1Type = "114", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 368, .Spacer1Type = "115", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 370, .Spacer1Type = "116", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 466, .Spacer1Type = "110", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 469, .Spacer1Type = "111", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 472, .Spacer1Type = "112", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "113", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 478, .Spacer1Type = "114", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 481, .Spacer1Type = "115", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 484, .Spacer1Type = "116", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 574, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 579, .Spacer1Type = "111", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 583, .Spacer1Type = "112", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 587, .Spacer1Type = "113", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 591, .Spacer1Type = "114", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "115", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 599, .Spacer1Type = "116", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 688, .Spacer1Type = "111", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 693, .Spacer1Type = "112", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "113", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 703, .Spacer1Type = "114", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 708, .Spacer1Type = "115", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 713, .Spacer1Type = "116", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 791, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 797, .Spacer1Type = "111", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 803, .Spacer1Type = "112", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 809, .Spacer1Type = "113", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 815, .Spacer1Type = "114", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 821, .Spacer1Type = "115", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 827, .Spacer1Type = "116", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 899, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 907, .Spacer1Type = "111", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 914, .Spacer1Type = "112", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "113", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 928, .Spacer1Type = "114", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 935, .Spacer1Type = "115", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 942, .Spacer1Type = "116", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1007, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1016, .Spacer1Type = "111", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1024, .Spacer1Type = "112", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1032, .Spacer1Type = "113", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1040, .Spacer1Type = "114", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1048, .Spacer1Type = "115", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1056, .Spacer1Type = "116", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1116, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1126, .Spacer1Type = "111", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1135, .Spacer1Type = "112", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1144, .Spacer1Type = "113", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "114", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1162, .Spacer1Type = "115", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1171, .Spacer1Type = "116", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1224, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1235, .Spacer1Type = "111", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1245, .Spacer1Type = "112", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1255, .Spacer1Type = "113", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1265, .Spacer1Type = "114", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1275, .Spacer1Type = "115", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1285, .Spacer1Type = "116", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1332, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1344, .Spacer1Type = "111", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1355, .Spacer1Type = "112", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1366, .Spacer1Type = "113", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1377, .Spacer1Type = "114", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1388, .Spacer1Type = "115", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1399, .Spacer1Type = "116", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1441, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1454, .Spacer1Type = "111", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1466, .Spacer1Type = "112", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1478, .Spacer1Type = "113", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1490, .Spacer1Type = "114", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1502, .Spacer1Type = "115", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1514, .Spacer1Type = "116", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1549, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1563, .Spacer1Type = "111", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1576, .Spacer1Type = "112", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1589, .Spacer1Type = "113", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1602, .Spacer1Type = "114", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "115", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1628, .Spacer1Type = "116", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1641, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1657, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1673, .Spacer1Type = "111", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1687, .Spacer1Type = "112", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1701, .Spacer1Type = "113", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1715, .Spacer1Type = "114", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1729, .Spacer1Type = "115", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1743, .Spacer1Type = "116", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1766, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1782, .Spacer1Type = "111", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1797, .Spacer1Type = "112", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1812, .Spacer1Type = "113", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1827, .Spacer1Type = "114", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "115", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1857, .Spacer1Type = "116", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1872, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1874, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1891, .Spacer1Type = "111", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1907, .Spacer1Type = "112", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1923, .Spacer1Type = "113", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "114", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1955, .Spacer1Type = "115", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1971, .Spacer1Type = "116", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1982, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2001, .Spacer1Type = "111", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2018, .Spacer1Type = "112", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2035, .Spacer1Type = "113", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2052, .Spacer1Type = "114", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2069, .Spacer1Type = "115", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2086, .Spacer1Type = "116", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2090, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2110, .Spacer1Type = "111", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2128, .Spacer1Type = "112", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2146, .Spacer1Type = "113", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2164, .Spacer1Type = "114", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2182, .Spacer1Type = "115", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2199, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "116", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2218, .Spacer1Type = "111", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2220, .Spacer1Type = "111", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2239, .Spacer1Type = "112", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2258, .Spacer1Type = "113", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2277, .Spacer1Type = "114", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2296, .Spacer1Type = "115", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2307, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2315, .Spacer1Type = "116", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2329, .Spacer1Type = "111", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2349, .Spacer1Type = "112", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2369, .Spacer1Type = "113", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2389, .Spacer1Type = "114", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2409, .Spacer1Type = "115", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2415, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2429, .Spacer1Type = "116", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2438, .Spacer1Type = "111", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2449, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2459, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2480, .Spacer1Type = "113", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2501, .Spacer1Type = "114", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2522, .Spacer1Type = "115", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2524, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2543, .Spacer1Type = "116", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2548, .Spacer1Type = "111", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2564, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2570, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2592, .Spacer1Type = "113", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2614, .Spacer1Type = "114", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2632, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2636, .Spacer1Type = "115", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "111", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2658, .Spacer1Type = "116", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2680, .Spacer1Type = "112", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2703, .Spacer1Type = "113", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2726, .Spacer1Type = "114", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2740, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2749, .Spacer1Type = "115", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2767, .Spacer1Type = "111", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "116", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2791, .Spacer1Type = "112", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2815, .Spacer1Type = "113", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2839, .Spacer1Type = "114", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2849, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2863, .Spacer1Type = "115", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2876, .Spacer1Type = "111", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2887, .Spacer1Type = "116", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2901, .Spacer1Type = "112", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2926, .Spacer1Type = "113", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2951, .Spacer1Type = "114", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2957, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2976, .Spacer1Type = "115", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2985, .Spacer1Type = "111", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3001, .Spacer1Type = "116", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3011, .Spacer1Type = "112", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3037, .Spacer1Type = "113", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3063, .Spacer1Type = "114", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3065, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3089, .Spacer1Type = "115", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3095, .Spacer1Type = "111", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3115, .Spacer1Type = "116", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3122, .Spacer1Type = "112", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3149, .Spacer1Type = "113", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3173, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3176, .Spacer1Type = "114", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3203, .Spacer1Type = "115", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3204, .Spacer1Type = "111", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3230, .Spacer1Type = "116", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3232, .Spacer1Type = "112", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3260, .Spacer1Type = "113", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3282, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3288, .Spacer1Type = "114", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3314, .Spacer1Type = "111", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3316, .Spacer1Type = "115", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3343, .Spacer1Type = "112", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3344, .Spacer1Type = "116", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3372, .Spacer1Type = "113", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3390, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3401, .Spacer1Type = "114", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3423, .Spacer1Type = "111", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3430, .Spacer1Type = "115", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3453, .Spacer1Type = "112", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3459, .Spacer1Type = "116", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3483, .Spacer1Type = "113", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3498, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3513, .Spacer1Type = "114", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3532, .Spacer1Type = "111", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3543, .Spacer1Type = "115", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3563, .Spacer1Type = "112", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3573, .Spacer1Type = "116", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3594, .Spacer1Type = "113", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3607, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "114", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3642, .Spacer1Type = "111", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3656, .Spacer1Type = "115", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3674, .Spacer1Type = "112", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3687, .Spacer1Type = "116", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3706, .Spacer1Type = "113", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3715, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3738, .Spacer1Type = "114", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3751, .Spacer1Type = "111", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3770, .Spacer1Type = "115", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3784, .Spacer1Type = "112", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3802, .Spacer1Type = "116", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3817, .Spacer1Type = "113", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3823, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3850, .Spacer1Type = "114", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3861, .Spacer1Type = "111", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3883, .Spacer1Type = "115", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3895, .Spacer1Type = "112", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3916, .Spacer1Type = "116", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3929, .Spacer1Type = "113", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3932, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3963, .Spacer1Type = "114", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3970, .Spacer1Type = "111", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3997, .Spacer1Type = "115", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4005, .Spacer1Type = "112", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4031, .Spacer1Type = "116", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "113", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4075, .Spacer1Type = "114", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4079, .Spacer1Type = "111", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4110, .Spacer1Type = "115", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4115, .Spacer1Type = "112", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4145, .Spacer1Type = "116", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4148, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4151, .Spacer1Type = "113", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4187, .Spacer1Type = "114", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4189, .Spacer1Type = "111", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4223, .Spacer1Type = "115", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4226, .Spacer1Type = "112", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4256, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4259, .Spacer1Type = "116", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4263, .Spacer1Type = "113", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4298, .Spacer1Type = "111", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4300, .Spacer1Type = "114", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4336, .Spacer1Type = "112", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4337, .Spacer1Type = "115", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4365, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4374, .Spacer1Type = "113", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4374, .Spacer1Type = "116", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4408, .Spacer1Type = "111", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4412, .Spacer1Type = "114", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4447, .Spacer1Type = "112", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4450, .Spacer1Type = "115", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4473, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4486, .Spacer1Type = "113", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4488, .Spacer1Type = "116", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4517, .Spacer1Type = "111", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4525, .Spacer1Type = "114", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4557, .Spacer1Type = "112", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4564, .Spacer1Type = "115", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4581, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4597, .Spacer1Type = "113", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4603, .Spacer1Type = "116", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4626, .Spacer1Type = "111", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4637, .Spacer1Type = "114", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4667, .Spacer1Type = "112", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4677, .Spacer1Type = "115", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4690, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4708, .Spacer1Type = "113", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4717, .Spacer1Type = "116", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4736, .Spacer1Type = "111", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4749, .Spacer1Type = "114", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4778, .Spacer1Type = "112", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4790, .Spacer1Type = "115", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4798, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4820, .Spacer1Type = "113", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4831, .Spacer1Type = "116", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4845, .Spacer1Type = "111", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4862, .Spacer1Type = "114", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4888, .Spacer1Type = "112", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4904, .Spacer1Type = "115", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4906, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4931, .Spacer1Type = "113", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4946, .Spacer1Type = "116", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4955, .Spacer1Type = "111", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4974, .Spacer1Type = "114", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "112", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5015, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5017, .Spacer1Type = "115", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5043, .Spacer1Type = "113", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5060, .Spacer1Type = "116", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5064, .Spacer1Type = "111", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5087, .Spacer1Type = "114", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5109, .Spacer1Type = "112", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5123, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5131, .Spacer1Type = "115", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5154, .Spacer1Type = "113", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5173, .Spacer1Type = "111", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5175, .Spacer1Type = "116", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5199, .Spacer1Type = "114", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5219, .Spacer1Type = "112", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5231, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5244, .Spacer1Type = "115", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5265, .Spacer1Type = "113", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5283, .Spacer1Type = "111", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "116", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5311, .Spacer1Type = "114", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5330, .Spacer1Type = "112", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5339, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5357, .Spacer1Type = "115", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5377, .Spacer1Type = "113", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5392, .Spacer1Type = "111", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5403, .Spacer1Type = "116", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5424, .Spacer1Type = "114", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5440, .Spacer1Type = "112", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5448, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5471, .Spacer1Type = "115", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5488, .Spacer1Type = "113", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5502, .Spacer1Type = "111", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5518, .Spacer1Type = "116", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5536, .Spacer1Type = "114", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5551, .Spacer1Type = "112", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5556, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5584, .Spacer1Type = "115", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "113", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "111", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5632, .Spacer1Type = "116", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5649, .Spacer1Type = "114", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5661, .Spacer1Type = "112", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5664, .Spacer1Type = "110", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5698, .Spacer1Type = "115", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5711, .Spacer1Type = "113", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5720, .Spacer1Type = "111", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5747, .Spacer1Type = "116", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5761, .Spacer1Type = "114", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5771, .Spacer1Type = "112", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5773, .Spacer1Type = "110", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5811, .Spacer1Type = "115", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5822, .Spacer1Type = "113", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5830, .Spacer1Type = "111", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5861, .Spacer1Type = "116", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5873, .Spacer1Type = "114", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5881, .Spacer1Type = "110", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 5882, .Spacer1Type = "112", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5924, .Spacer1Type = "115", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5934, .Spacer1Type = "113", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5939, .Spacer1Type = "111", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 5975, .Spacer1Type = "116", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5986, .Spacer1Type = "114", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5989, .Spacer1Type = "110", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 5992, .Spacer1Type = "112", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6038, .Spacer1Type = "115", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 6045, .Spacer1Type = "113", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6049, .Spacer1Type = "111", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6090, .Spacer1Type = "116", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 6097, .Spacer1Type = "110", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6098, .Spacer1Type = "114", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6103, .Spacer1Type = "112", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6151, .Spacer1Type = "115", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6157, .Spacer1Type = "113", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6158, .Spacer1Type = "111", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6204, .Spacer1Type = "116", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6206, .Spacer1Type = "110", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6211, .Spacer1Type = "114", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6213, .Spacer1Type = "112", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6265, .Spacer1Type = "115", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6267, .Spacer1Type = "111", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6268, .Spacer1Type = "113", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6314, .Spacer1Type = "110", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6319, .Spacer1Type = "116", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6323, .Spacer1Type = "112", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6323, .Spacer1Type = "114", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6377, .Spacer1Type = "111", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6378, .Spacer1Type = "115", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6379, .Spacer1Type = "113", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6422, .Spacer1Type = "110", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 6433, .Spacer1Type = "116", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6434, .Spacer1Type = "112", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6435, .Spacer1Type = "114", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6486, .Spacer1Type = "111", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 6491, .Spacer1Type = "113", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6491, .Spacer1Type = "115", .CarriersQty = 57}
    }

    Private Shared ReadOnly Spacer100Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 196, .Spacer1Type = "84", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 197, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 198, .Spacer1Type = "86", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 199, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 200, .Spacer1Type = "88", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 279, .Spacer1Type = "84", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 281, .Spacer1Type = "85", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 283, .Spacer1Type = "86", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 285, .Spacer1Type = "87", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 287, .Spacer1Type = "88", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 361, .Spacer1Type = "84", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 364, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 367, .Spacer1Type = "86", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 370, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 373, .Spacer1Type = "88", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 444, .Spacer1Type = "84", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 448, .Spacer1Type = "85", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 452, .Spacer1Type = "86", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 456, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 460, .Spacer1Type = "88", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 526, .Spacer1Type = "84", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 531, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 536, .Spacer1Type = "86", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 541, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 546, .Spacer1Type = "88", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 608, .Spacer1Type = "84", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 614, .Spacer1Type = "85", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 620, .Spacer1Type = "86", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 626, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 632, .Spacer1Type = "88", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 691, .Spacer1Type = "84", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 705, .Spacer1Type = "86", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 712, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 719, .Spacer1Type = "88", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 773, .Spacer1Type = "84", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 781, .Spacer1Type = "85", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 789, .Spacer1Type = "86", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 797, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 805, .Spacer1Type = "88", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 856, .Spacer1Type = "84", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 865, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 874, .Spacer1Type = "86", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 883, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 892, .Spacer1Type = "88", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 938, .Spacer1Type = "84", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 948, .Spacer1Type = "85", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 958, .Spacer1Type = "86", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 968, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 978, .Spacer1Type = "88", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1020, .Spacer1Type = "84", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1031, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1042, .Spacer1Type = "86", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1053, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1064, .Spacer1Type = "88", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1103, .Spacer1Type = "84", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1115, .Spacer1Type = "85", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1127, .Spacer1Type = "86", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1139, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1151, .Spacer1Type = "88", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1185, .Spacer1Type = "84", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1198, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1211, .Spacer1Type = "86", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1224, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1237, .Spacer1Type = "88", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1268, .Spacer1Type = "84", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1282, .Spacer1Type = "85", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1296, .Spacer1Type = "86", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1310, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1324, .Spacer1Type = "88", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1350, .Spacer1Type = "84", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1365, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1380, .Spacer1Type = "86", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1395, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1410, .Spacer1Type = "88", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1432, .Spacer1Type = "84", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1448, .Spacer1Type = "85", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1464, .Spacer1Type = "86", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1480, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1496, .Spacer1Type = "88", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1515, .Spacer1Type = "84", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1532, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1549, .Spacer1Type = "86", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1566, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1583, .Spacer1Type = "88", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1597, .Spacer1Type = "84", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "85", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "86", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1651, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1669, .Spacer1Type = "88", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1680, .Spacer1Type = "84", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1699, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1718, .Spacer1Type = "86", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1737, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1756, .Spacer1Type = "88", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1762, .Spacer1Type = "84", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1782, .Spacer1Type = "85", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1802, .Spacer1Type = "86", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1822, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "88", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1844, .Spacer1Type = "84", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1886, .Spacer1Type = "86", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1907, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1927, .Spacer1Type = "84", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1928, .Spacer1Type = "88", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1949, .Spacer1Type = "85", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1971, .Spacer1Type = "86", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1993, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2009, .Spacer1Type = "84", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2015, .Spacer1Type = "88", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2032, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "86", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2078, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2092, .Spacer1Type = "84", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2101, .Spacer1Type = "88", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2116, .Spacer1Type = "85", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2140, .Spacer1Type = "86", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2164, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2174, .Spacer1Type = "84", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2188, .Spacer1Type = "88", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2199, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2224, .Spacer1Type = "86", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2249, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2256, .Spacer1Type = "84", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2274, .Spacer1Type = "88", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2282, .Spacer1Type = "85", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2308, .Spacer1Type = "86", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2334, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2339, .Spacer1Type = "84", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2360, .Spacer1Type = "88", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2366, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2393, .Spacer1Type = "86", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2420, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2421, .Spacer1Type = "84", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2447, .Spacer1Type = "88", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2449, .Spacer1Type = "85", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2477, .Spacer1Type = "86", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2504, .Spacer1Type = "84", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "88", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2562, .Spacer1Type = "86", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2586, .Spacer1Type = "84", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2591, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2616, .Spacer1Type = "85", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2620, .Spacer1Type = "88", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2646, .Spacer1Type = "86", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2668, .Spacer1Type = "84", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2676, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2699, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2706, .Spacer1Type = "88", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2730, .Spacer1Type = "86", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2751, .Spacer1Type = "84", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2783, .Spacer1Type = "85", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2792, .Spacer1Type = "88", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2815, .Spacer1Type = "86", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "84", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2847, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2866, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2879, .Spacer1Type = "88", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2899, .Spacer1Type = "86", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2916, .Spacer1Type = "84", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2932, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "85", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2965, .Spacer1Type = "88", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2984, .Spacer1Type = "86", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2998, .Spacer1Type = "84", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3018, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3033, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3052, .Spacer1Type = "88", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3068, .Spacer1Type = "86", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3080, .Spacer1Type = "84", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3103, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3116, .Spacer1Type = "85", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3138, .Spacer1Type = "88", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3152, .Spacer1Type = "86", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3163, .Spacer1Type = "84", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3188, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3200, .Spacer1Type = "85", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3224, .Spacer1Type = "88", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3237, .Spacer1Type = "86", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3245, .Spacer1Type = "84", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3274, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3283, .Spacer1Type = "85", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3311, .Spacer1Type = "88", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3321, .Spacer1Type = "86", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3328, .Spacer1Type = "84", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3359, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3367, .Spacer1Type = "85", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3397, .Spacer1Type = "88", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3406, .Spacer1Type = "86", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3410, .Spacer1Type = "84", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3450, .Spacer1Type = "85", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3484, .Spacer1Type = "88", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3490, .Spacer1Type = "86", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3492, .Spacer1Type = "84", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3530, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3533, .Spacer1Type = "85", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3570, .Spacer1Type = "88", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3574, .Spacer1Type = "86", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3575, .Spacer1Type = "84", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3615, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3617, .Spacer1Type = "85", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3656, .Spacer1Type = "88", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3657, .Spacer1Type = "84", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3659, .Spacer1Type = "86", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "85", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3701, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3740, .Spacer1Type = "84", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3743, .Spacer1Type = "86", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3743, .Spacer1Type = "88", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3784, .Spacer1Type = "85", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3786, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3822, .Spacer1Type = "84", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3828, .Spacer1Type = "86", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3829, .Spacer1Type = "88", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3867, .Spacer1Type = "85", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3872, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3904, .Spacer1Type = "84", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3912, .Spacer1Type = "86", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3916, .Spacer1Type = "88", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3950, .Spacer1Type = "85", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3957, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3987, .Spacer1Type = "84", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3996, .Spacer1Type = "86", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4002, .Spacer1Type = "88", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4034, .Spacer1Type = "85", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4042, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4069, .Spacer1Type = "84", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4081, .Spacer1Type = "86", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4088, .Spacer1Type = "88", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4117, .Spacer1Type = "85", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4128, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4152, .Spacer1Type = "84", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4165, .Spacer1Type = "86", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4175, .Spacer1Type = "88", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4201, .Spacer1Type = "85", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4213, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4234, .Spacer1Type = "84", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4250, .Spacer1Type = "86", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4261, .Spacer1Type = "88", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4284, .Spacer1Type = "85", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4299, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4316, .Spacer1Type = "84", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4334, .Spacer1Type = "86", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4348, .Spacer1Type = "88", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4367, .Spacer1Type = "85", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4384, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4399, .Spacer1Type = "84", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "86", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4434, .Spacer1Type = "88", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4451, .Spacer1Type = "85", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4469, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4481, .Spacer1Type = "84", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4503, .Spacer1Type = "86", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4520, .Spacer1Type = "88", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4534, .Spacer1Type = "85", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4555, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4564, .Spacer1Type = "84", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4587, .Spacer1Type = "86", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4607, .Spacer1Type = "88", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4618, .Spacer1Type = "85", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4640, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4646, .Spacer1Type = "84", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4672, .Spacer1Type = "86", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4693, .Spacer1Type = "88", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4701, .Spacer1Type = "85", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4726, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4728, .Spacer1Type = "84", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4756, .Spacer1Type = "86", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4780, .Spacer1Type = "88", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4784, .Spacer1Type = "85", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "84", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4840, .Spacer1Type = "86", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4866, .Spacer1Type = "88", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4868, .Spacer1Type = "85", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4893, .Spacer1Type = "84", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4896, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4925, .Spacer1Type = "86", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4951, .Spacer1Type = "85", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4952, .Spacer1Type = "88", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4976, .Spacer1Type = "84", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4982, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5009, .Spacer1Type = "86", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5035, .Spacer1Type = "85", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5039, .Spacer1Type = "88", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5058, .Spacer1Type = "84", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5067, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5094, .Spacer1Type = "86", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5118, .Spacer1Type = "85", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5125, .Spacer1Type = "88", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5140, .Spacer1Type = "84", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5153, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5178, .Spacer1Type = "86", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5201, .Spacer1Type = "85", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5212, .Spacer1Type = "88", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5223, .Spacer1Type = "84", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5238, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5262, .Spacer1Type = "86", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5285, .Spacer1Type = "85", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5298, .Spacer1Type = "88", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "84", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5323, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5347, .Spacer1Type = "86", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5368, .Spacer1Type = "85", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5384, .Spacer1Type = "88", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "84", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5409, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5431, .Spacer1Type = "86", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5452, .Spacer1Type = "85", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5470, .Spacer1Type = "84", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5471, .Spacer1Type = "88", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5494, .Spacer1Type = "87", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5516, .Spacer1Type = "86", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "85", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5552, .Spacer1Type = "84", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5557, .Spacer1Type = "88", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5580, .Spacer1Type = "87", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "86", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5618, .Spacer1Type = "85", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5635, .Spacer1Type = "84", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5644, .Spacer1Type = "88", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5665, .Spacer1Type = "87", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5684, .Spacer1Type = "86", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5702, .Spacer1Type = "85", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5717, .Spacer1Type = "84", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5730, .Spacer1Type = "88", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5750, .Spacer1Type = "87", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5769, .Spacer1Type = "86", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5785, .Spacer1Type = "85", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5800, .Spacer1Type = "84", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5816, .Spacer1Type = "88", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5836, .Spacer1Type = "87", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5853, .Spacer1Type = "86", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5869, .Spacer1Type = "85", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5882, .Spacer1Type = "84", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5903, .Spacer1Type = "88", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5921, .Spacer1Type = "87", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5938, .Spacer1Type = "86", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5952, .Spacer1Type = "85", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5964, .Spacer1Type = "84", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5989, .Spacer1Type = "88", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 6007, .Spacer1Type = "87", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 6022, .Spacer1Type = "86", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6035, .Spacer1Type = "85", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6047, .Spacer1Type = "84", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6076, .Spacer1Type = "88", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 6092, .Spacer1Type = "87", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6106, .Spacer1Type = "86", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6119, .Spacer1Type = "85", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6129, .Spacer1Type = "84", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6162, .Spacer1Type = "88", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6177, .Spacer1Type = "87", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6191, .Spacer1Type = "86", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6202, .Spacer1Type = "85", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6212, .Spacer1Type = "84", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6248, .Spacer1Type = "88", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6263, .Spacer1Type = "87", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6275, .Spacer1Type = "86", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6286, .Spacer1Type = "85", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6294, .Spacer1Type = "84", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6335, .Spacer1Type = "88", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6348, .Spacer1Type = "87", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6360, .Spacer1Type = "86", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6369, .Spacer1Type = "85", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6376, .Spacer1Type = "84", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 6421, .Spacer1Type = "88", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6434, .Spacer1Type = "87", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6444, .Spacer1Type = "86", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6452, .Spacer1Type = "85", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 6459, .Spacer1Type = "84", .CarriersQty = 78}
    }

    Private Shared ReadOnly Spacer89Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 103, .Spacer1Type = "76", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 175, .Spacer1Type = "74", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 176, .Spacer1Type = "75", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 177, .Spacer1Type = "76", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 178, .Spacer1Type = "77", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 180, .Spacer1Type = "78", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 248, .Spacer1Type = "74", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 250, .Spacer1Type = "75", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "76", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 254, .Spacer1Type = "77", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 256, .Spacer1Type = "78", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 320, .Spacer1Type = "74", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 323, .Spacer1Type = "75", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 326, .Spacer1Type = "76", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 329, .Spacer1Type = "77", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 333, .Spacer1Type = "78", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 393, .Spacer1Type = "74", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 397, .Spacer1Type = "75", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 401, .Spacer1Type = "76", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 405, .Spacer1Type = "77", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 409, .Spacer1Type = "78", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 465, .Spacer1Type = "74", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 470, .Spacer1Type = "75", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "76", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 480, .Spacer1Type = "77", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 486, .Spacer1Type = "78", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 537, .Spacer1Type = "74", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 543, .Spacer1Type = "75", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 549, .Spacer1Type = "76", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 555, .Spacer1Type = "77", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 562, .Spacer1Type = "78", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "74", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 617, .Spacer1Type = "75", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 624, .Spacer1Type = "76", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 631, .Spacer1Type = "77", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 639, .Spacer1Type = "78", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "74", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 690, .Spacer1Type = "75", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "76", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 706, .Spacer1Type = "77", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 715, .Spacer1Type = "78", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "74", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 764, .Spacer1Type = "75", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 773, .Spacer1Type = "76", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 782, .Spacer1Type = "77", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 792, .Spacer1Type = "78", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 827, .Spacer1Type = "74", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 837, .Spacer1Type = "75", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 847, .Spacer1Type = "76", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 857, .Spacer1Type = "77", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 868, .Spacer1Type = "78", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 899, .Spacer1Type = "74", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 910, .Spacer1Type = "75", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "76", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 932, .Spacer1Type = "77", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 945, .Spacer1Type = "78", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 972, .Spacer1Type = "74", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 984, .Spacer1Type = "75", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 996, .Spacer1Type = "76", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1008, .Spacer1Type = "77", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1021, .Spacer1Type = "78", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1044, .Spacer1Type = "74", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1057, .Spacer1Type = "75", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1070, .Spacer1Type = "76", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1083, .Spacer1Type = "77", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1098, .Spacer1Type = "78", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1117, .Spacer1Type = "74", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1131, .Spacer1Type = "75", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "76", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1159, .Spacer1Type = "77", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1174, .Spacer1Type = "78", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1189, .Spacer1Type = "74", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1204, .Spacer1Type = "75", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1219, .Spacer1Type = "76", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1234, .Spacer1Type = "77", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1251, .Spacer1Type = "78", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1261, .Spacer1Type = "74", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1277, .Spacer1Type = "75", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1293, .Spacer1Type = "76", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1309, .Spacer1Type = "77", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1327, .Spacer1Type = "78", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1334, .Spacer1Type = "74", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1351, .Spacer1Type = "75", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1368, .Spacer1Type = "76", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1385, .Spacer1Type = "77", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1404, .Spacer1Type = "78", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1406, .Spacer1Type = "74", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1424, .Spacer1Type = "75", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1442, .Spacer1Type = "76", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1460, .Spacer1Type = "77", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1479, .Spacer1Type = "74", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1480, .Spacer1Type = "78", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1498, .Spacer1Type = "75", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1517, .Spacer1Type = "76", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1536, .Spacer1Type = "77", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1551, .Spacer1Type = "74", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1557, .Spacer1Type = "78", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1571, .Spacer1Type = "75", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1591, .Spacer1Type = "76", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1611, .Spacer1Type = "77", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1623, .Spacer1Type = "74", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "78", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1644, .Spacer1Type = "75", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1665, .Spacer1Type = "76", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1686, .Spacer1Type = "77", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1696, .Spacer1Type = "74", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1710, .Spacer1Type = "78", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1718, .Spacer1Type = "75", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1740, .Spacer1Type = "76", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1762, .Spacer1Type = "77", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1768, .Spacer1Type = "74", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1786, .Spacer1Type = "78", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1791, .Spacer1Type = "75", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1814, .Spacer1Type = "76", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1837, .Spacer1Type = "77", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1841, .Spacer1Type = "74", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1863, .Spacer1Type = "78", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "75", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1889, .Spacer1Type = "76", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1913, .Spacer1Type = "74", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1913, .Spacer1Type = "77", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1938, .Spacer1Type = "75", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "78", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1963, .Spacer1Type = "76", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1985, .Spacer1Type = "74", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1988, .Spacer1Type = "77", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2011, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2016, .Spacer1Type = "78", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2037, .Spacer1Type = "76", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2058, .Spacer1Type = "74", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2063, .Spacer1Type = "77", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2085, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2092, .Spacer1Type = "78", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2112, .Spacer1Type = "76", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2130, .Spacer1Type = "74", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2139, .Spacer1Type = "77", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2158, .Spacer1Type = "75", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2169, .Spacer1Type = "78", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2186, .Spacer1Type = "76", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2203, .Spacer1Type = "74", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2214, .Spacer1Type = "77", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2232, .Spacer1Type = "75", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2245, .Spacer1Type = "78", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2261, .Spacer1Type = "76", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2275, .Spacer1Type = "74", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2290, .Spacer1Type = "77", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2305, .Spacer1Type = "75", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2322, .Spacer1Type = "78", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2335, .Spacer1Type = "76", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2347, .Spacer1Type = "74", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2365, .Spacer1Type = "77", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2378, .Spacer1Type = "75", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2398, .Spacer1Type = "78", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2409, .Spacer1Type = "76", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2420, .Spacer1Type = "74", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2440, .Spacer1Type = "77", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2452, .Spacer1Type = "75", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2475, .Spacer1Type = "78", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2484, .Spacer1Type = "76", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2492, .Spacer1Type = "74", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "77", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2525, .Spacer1Type = "75", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2551, .Spacer1Type = "78", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2558, .Spacer1Type = "76", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2565, .Spacer1Type = "74", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2591, .Spacer1Type = "77", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2599, .Spacer1Type = "75", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2628, .Spacer1Type = "78", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2633, .Spacer1Type = "76", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2637, .Spacer1Type = "74", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2667, .Spacer1Type = "77", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2672, .Spacer1Type = "75", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2704, .Spacer1Type = "78", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2707, .Spacer1Type = "76", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2709, .Spacer1Type = "74", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2742, .Spacer1Type = "77", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "75", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "76", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "78", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2782, .Spacer1Type = "74", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2817, .Spacer1Type = "77", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2819, .Spacer1Type = "75", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2854, .Spacer1Type = "74", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2856, .Spacer1Type = "76", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2857, .Spacer1Type = "78", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2892, .Spacer1Type = "75", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2893, .Spacer1Type = "77", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2927, .Spacer1Type = "74", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2930, .Spacer1Type = "76", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2934, .Spacer1Type = "78", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2966, .Spacer1Type = "75", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2968, .Spacer1Type = "77", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2999, .Spacer1Type = "74", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3005, .Spacer1Type = "76", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3010, .Spacer1Type = "78", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3039, .Spacer1Type = "75", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3044, .Spacer1Type = "77", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3071, .Spacer1Type = "74", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3079, .Spacer1Type = "76", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3087, .Spacer1Type = "78", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3112, .Spacer1Type = "75", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3119, .Spacer1Type = "77", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3144, .Spacer1Type = "74", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3153, .Spacer1Type = "76", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3163, .Spacer1Type = "78", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3186, .Spacer1Type = "75", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3194, .Spacer1Type = "77", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3216, .Spacer1Type = "74", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3228, .Spacer1Type = "76", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3240, .Spacer1Type = "78", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3259, .Spacer1Type = "75", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3270, .Spacer1Type = "77", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3289, .Spacer1Type = "74", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3302, .Spacer1Type = "76", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3316, .Spacer1Type = "78", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3333, .Spacer1Type = "75", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3345, .Spacer1Type = "77", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3361, .Spacer1Type = "74", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3377, .Spacer1Type = "76", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3393, .Spacer1Type = "78", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3406, .Spacer1Type = "75", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3421, .Spacer1Type = "77", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3433, .Spacer1Type = "74", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3451, .Spacer1Type = "76", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3469, .Spacer1Type = "78", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3479, .Spacer1Type = "75", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3496, .Spacer1Type = "77", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3506, .Spacer1Type = "74", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3525, .Spacer1Type = "76", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3546, .Spacer1Type = "78", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3553, .Spacer1Type = "75", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3571, .Spacer1Type = "77", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3578, .Spacer1Type = "74", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3600, .Spacer1Type = "76", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3622, .Spacer1Type = "78", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3626, .Spacer1Type = "75", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3647, .Spacer1Type = "77", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3651, .Spacer1Type = "74", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3674, .Spacer1Type = "76", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3699, .Spacer1Type = "78", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "75", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3722, .Spacer1Type = "77", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3723, .Spacer1Type = "74", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3749, .Spacer1Type = "76", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3773, .Spacer1Type = "75", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "78", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3795, .Spacer1Type = "74", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3798, .Spacer1Type = "77", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3823, .Spacer1Type = "76", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3846, .Spacer1Type = "75", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3852, .Spacer1Type = "78", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3868, .Spacer1Type = "74", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3873, .Spacer1Type = "77", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3897, .Spacer1Type = "76", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3920, .Spacer1Type = "75", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3928, .Spacer1Type = "78", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3940, .Spacer1Type = "74", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3948, .Spacer1Type = "77", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3972, .Spacer1Type = "76", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3993, .Spacer1Type = "75", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4005, .Spacer1Type = "78", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4013, .Spacer1Type = "74", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4024, .Spacer1Type = "77", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4046, .Spacer1Type = "76", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4067, .Spacer1Type = "75", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4081, .Spacer1Type = "78", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4085, .Spacer1Type = "74", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4099, .Spacer1Type = "77", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4121, .Spacer1Type = "76", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4140, .Spacer1Type = "75", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4157, .Spacer1Type = "74", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4158, .Spacer1Type = "78", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4175, .Spacer1Type = "77", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4195, .Spacer1Type = "76", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4213, .Spacer1Type = "75", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4230, .Spacer1Type = "74", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4234, .Spacer1Type = "78", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4250, .Spacer1Type = "77", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4269, .Spacer1Type = "76", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4287, .Spacer1Type = "75", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4302, .Spacer1Type = "74", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4311, .Spacer1Type = "78", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4325, .Spacer1Type = "77", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4344, .Spacer1Type = "76", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4360, .Spacer1Type = "75", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "74", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4387, .Spacer1Type = "78", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4401, .Spacer1Type = "77", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "76", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4434, .Spacer1Type = "75", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4447, .Spacer1Type = "74", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4464, .Spacer1Type = "78", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4476, .Spacer1Type = "77", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4493, .Spacer1Type = "76", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4507, .Spacer1Type = "75", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4519, .Spacer1Type = "74", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4540, .Spacer1Type = "78", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4552, .Spacer1Type = "77", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4567, .Spacer1Type = "76", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4580, .Spacer1Type = "75", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4592, .Spacer1Type = "74", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "78", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4627, .Spacer1Type = "77", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4641, .Spacer1Type = "76", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4654, .Spacer1Type = "75", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4664, .Spacer1Type = "74", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4693, .Spacer1Type = "78", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4702, .Spacer1Type = "77", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4716, .Spacer1Type = "76", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4727, .Spacer1Type = "75", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4737, .Spacer1Type = "74", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4770, .Spacer1Type = "78", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4778, .Spacer1Type = "77", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4790, .Spacer1Type = "76", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4801, .Spacer1Type = "75", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4809, .Spacer1Type = "74", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4846, .Spacer1Type = "78", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4853, .Spacer1Type = "77", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4865, .Spacer1Type = "76", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4874, .Spacer1Type = "75", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4881, .Spacer1Type = "74", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4923, .Spacer1Type = "78", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4929, .Spacer1Type = "77", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4939, .Spacer1Type = "76", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4947, .Spacer1Type = "75", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4954, .Spacer1Type = "74", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "78", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5004, .Spacer1Type = "77", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5013, .Spacer1Type = "76", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5021, .Spacer1Type = "75", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5026, .Spacer1Type = "74", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5076, .Spacer1Type = "78", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5079, .Spacer1Type = "77", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5088, .Spacer1Type = "76", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5094, .Spacer1Type = "75", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5099, .Spacer1Type = "74", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5152, .Spacer1Type = "78", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5155, .Spacer1Type = "77", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5162, .Spacer1Type = "76", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5168, .Spacer1Type = "75", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5171, .Spacer1Type = "74", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5229, .Spacer1Type = "78", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5230, .Spacer1Type = "77", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5237, .Spacer1Type = "76", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5241, .Spacer1Type = "75", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5243, .Spacer1Type = "74", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "78", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5306, .Spacer1Type = "77", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5311, .Spacer1Type = "76", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5314, .Spacer1Type = "75", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5316, .Spacer1Type = "74", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5381, .Spacer1Type = "77", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5382, .Spacer1Type = "78", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5385, .Spacer1Type = "76", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "74", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "75", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5456, .Spacer1Type = "77", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5458, .Spacer1Type = "78", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5460, .Spacer1Type = "76", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "74", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "75", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5532, .Spacer1Type = "77", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5533, .Spacer1Type = "74", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5534, .Spacer1Type = "76", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "75", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "78", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5605, .Spacer1Type = "74", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5607, .Spacer1Type = "77", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5608, .Spacer1Type = "75", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5609, .Spacer1Type = "76", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "78", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5678, .Spacer1Type = "74", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5681, .Spacer1Type = "75", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5683, .Spacer1Type = "76", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5683, .Spacer1Type = "77", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5688, .Spacer1Type = "78", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5750, .Spacer1Type = "74", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5755, .Spacer1Type = "75", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5757, .Spacer1Type = "76", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5764, .Spacer1Type = "78", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5823, .Spacer1Type = "74", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5828, .Spacer1Type = "75", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5832, .Spacer1Type = "76", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5895, .Spacer1Type = "74", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5902, .Spacer1Type = "75", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5906, .Spacer1Type = "76", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5975, .Spacer1Type = "75", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5981, .Spacer1Type = "76", .CarriersQty = 81}
    }

    Private Shared ReadOnly Spacer63Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 77, .Spacer1Type = "51", .CarriersQty = 1},
        New SpacerInfo With {.MaxWidth = 126, .Spacer1Type = "51", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 176, .Spacer1Type = "51", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 225, .Spacer1Type = "51", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 275, .Spacer1Type = "51", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 324, .Spacer1Type = "51", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 373, .Spacer1Type = "51", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 423, .Spacer1Type = "51", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 472, .Spacer1Type = "51", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 522, .Spacer1Type = "51", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 571, .Spacer1Type = "51", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 620, .Spacer1Type = "51", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 670, .Spacer1Type = "51", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 719, .Spacer1Type = "51", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 769, .Spacer1Type = "51", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 818, .Spacer1Type = "51", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 867, .Spacer1Type = "51", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 917, .Spacer1Type = "51", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 966, .Spacer1Type = "51", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1016, .Spacer1Type = "51", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1065, .Spacer1Type = "51", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1114, .Spacer1Type = "51", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1164, .Spacer1Type = "51", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1213, .Spacer1Type = "51", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1263, .Spacer1Type = "51", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1312, .Spacer1Type = "51", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1361, .Spacer1Type = "51", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1411, .Spacer1Type = "51", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 1460, .Spacer1Type = "51", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 1510, .Spacer1Type = "51", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 1559, .Spacer1Type = "51", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 1608, .Spacer1Type = "51", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 1658, .Spacer1Type = "51", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 1707, .Spacer1Type = "51", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 1757, .Spacer1Type = "51", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 1806, .Spacer1Type = "51", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 1855, .Spacer1Type = "51", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "51", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 1954, .Spacer1Type = "51", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2004, .Spacer1Type = "51", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2053, .Spacer1Type = "51", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 2102, .Spacer1Type = "51", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 2152, .Spacer1Type = "51", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 2201, .Spacer1Type = "51", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 2251, .Spacer1Type = "51", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 2300, .Spacer1Type = "51", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 2349, .Spacer1Type = "51", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 2399, .Spacer1Type = "51", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 2448, .Spacer1Type = "51", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 2498, .Spacer1Type = "51", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 2547, .Spacer1Type = "51", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 2596, .Spacer1Type = "51", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 2646, .Spacer1Type = "51", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 2695, .Spacer1Type = "51", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "51", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 2794, .Spacer1Type = "51", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 2843, .Spacer1Type = "51", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 2893, .Spacer1Type = "51", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 2942, .Spacer1Type = "51", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 2992, .Spacer1Type = "51", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 3041, .Spacer1Type = "51", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 3090, .Spacer1Type = "51", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 3140, .Spacer1Type = "51", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 3189, .Spacer1Type = "51", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 3239, .Spacer1Type = "51", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 3288, .Spacer1Type = "51", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 3337, .Spacer1Type = "51", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 3387, .Spacer1Type = "51", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 3436, .Spacer1Type = "51", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 3486, .Spacer1Type = "51", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "51", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 3584, .Spacer1Type = "51", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 3634, .Spacer1Type = "51", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 3683, .Spacer1Type = "51", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 3733, .Spacer1Type = "51", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 3782, .Spacer1Type = "51", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 3831, .Spacer1Type = "51", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 3881, .Spacer1Type = "51", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 3930, .Spacer1Type = "51", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 3980, .Spacer1Type = "51", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 4029, .Spacer1Type = "51", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 4078, .Spacer1Type = "51", .CarriersQty = 82},
        New SpacerInfo With {.MaxWidth = 4128, .Spacer1Type = "51", .CarriersQty = 83},
        New SpacerInfo With {.MaxWidth = 4177, .Spacer1Type = "51", .CarriersQty = 84},
        New SpacerInfo With {.MaxWidth = 4227, .Spacer1Type = "51", .CarriersQty = 85},
        New SpacerInfo With {.MaxWidth = 4276, .Spacer1Type = "51", .CarriersQty = 86},
        New SpacerInfo With {.MaxWidth = 4325, .Spacer1Type = "51", .CarriersQty = 87},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "51", .CarriersQty = 88}
    }

    Private Shared ReadOnly Spacer127Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 555, .Spacer1Type = "106", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 575, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 585, .Spacer1Type = "112", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "114", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "117", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 661, .Spacer1Type = "106", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 685, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 697, .Spacer1Type = "112", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 709, .Spacer1Type = "114", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 727, .Spacer1Type = "117", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 767, .Spacer1Type = "106", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 795, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 809, .Spacer1Type = "112", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 823, .Spacer1Type = "114", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 844, .Spacer1Type = "117", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 873, .Spacer1Type = "106", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 905, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "112", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 937, .Spacer1Type = "114", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 961, .Spacer1Type = "117", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 979, .Spacer1Type = "106", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1015, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1033, .Spacer1Type = "112", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1051, .Spacer1Type = "114", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1078, .Spacer1Type = "117", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1085, .Spacer1Type = "106", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1125, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "112", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1165, .Spacer1Type = "114", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1191, .Spacer1Type = "106", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1195, .Spacer1Type = "117", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1235, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1257, .Spacer1Type = "112", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1279, .Spacer1Type = "114", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1297, .Spacer1Type = "106", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1312, .Spacer1Type = "117", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1345, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1369, .Spacer1Type = "112", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1393, .Spacer1Type = "114", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1403, .Spacer1Type = "106", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1429, .Spacer1Type = "117", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1455, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1481, .Spacer1Type = "112", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1507, .Spacer1Type = "114", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1509, .Spacer1Type = "106", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1546, .Spacer1Type = "117", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1565, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1593, .Spacer1Type = "112", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "106", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1621, .Spacer1Type = "114", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1663, .Spacer1Type = "117", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1675, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1705, .Spacer1Type = "112", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1721, .Spacer1Type = "106", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1735, .Spacer1Type = "114", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1780, .Spacer1Type = "117", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1785, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1817, .Spacer1Type = "112", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1827, .Spacer1Type = "106", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1849, .Spacer1Type = "114", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1895, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1897, .Spacer1Type = "117", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1929, .Spacer1Type = "112", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1933, .Spacer1Type = "106", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1963, .Spacer1Type = "114", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2005, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2014, .Spacer1Type = "117", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2039, .Spacer1Type = "106", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2041, .Spacer1Type = "112", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2077, .Spacer1Type = "114", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2115, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2131, .Spacer1Type = "117", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2145, .Spacer1Type = "106", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2153, .Spacer1Type = "112", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2191, .Spacer1Type = "114", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2225, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2248, .Spacer1Type = "117", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2251, .Spacer1Type = "106", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2265, .Spacer1Type = "112", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2305, .Spacer1Type = "114", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2335, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2357, .Spacer1Type = "106", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2365, .Spacer1Type = "117", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2377, .Spacer1Type = "112", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2419, .Spacer1Type = "114", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2445, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2463, .Spacer1Type = "106", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2482, .Spacer1Type = "117", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2489, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "114", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2555, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2569, .Spacer1Type = "106", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2599, .Spacer1Type = "117", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2601, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2647, .Spacer1Type = "114", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2675, .Spacer1Type = "106", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2713, .Spacer1Type = "112", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2716, .Spacer1Type = "117", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "114", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2775, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "106", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2825, .Spacer1Type = "112", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "117", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2875, .Spacer1Type = "114", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2885, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2887, .Spacer1Type = "106", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2937, .Spacer1Type = "112", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "117", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2989, .Spacer1Type = "114", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2993, .Spacer1Type = "106", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2995, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3049, .Spacer1Type = "112", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3067, .Spacer1Type = "117", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3099, .Spacer1Type = "106", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3103, .Spacer1Type = "114", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3105, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3161, .Spacer1Type = "112", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3184, .Spacer1Type = "117", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3205, .Spacer1Type = "106", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3215, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3217, .Spacer1Type = "114", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3273, .Spacer1Type = "112", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3301, .Spacer1Type = "117", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3311, .Spacer1Type = "106", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3325, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3331, .Spacer1Type = "114", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3385, .Spacer1Type = "112", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3417, .Spacer1Type = "106", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3418, .Spacer1Type = "117", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3435, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "114", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3497, .Spacer1Type = "112", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3523, .Spacer1Type = "106", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "117", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3545, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3559, .Spacer1Type = "114", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3609, .Spacer1Type = "112", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3629, .Spacer1Type = "106", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3652, .Spacer1Type = "117", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3655, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3673, .Spacer1Type = "114", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3721, .Spacer1Type = "112", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3735, .Spacer1Type = "106", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3765, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3769, .Spacer1Type = "117", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3787, .Spacer1Type = "114", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3833, .Spacer1Type = "112", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3841, .Spacer1Type = "106", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3875, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3886, .Spacer1Type = "117", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3901, .Spacer1Type = "114", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3945, .Spacer1Type = "112", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3947, .Spacer1Type = "106", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3985, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4003, .Spacer1Type = "117", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 4015, .Spacer1Type = "114", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4053, .Spacer1Type = "106", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4057, .Spacer1Type = "112", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4095, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4120, .Spacer1Type = "117", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4129, .Spacer1Type = "114", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4159, .Spacer1Type = "106", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4169, .Spacer1Type = "112", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4205, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4237, .Spacer1Type = "117", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4243, .Spacer1Type = "114", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4265, .Spacer1Type = "106", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4281, .Spacer1Type = "112", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4315, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4354, .Spacer1Type = "117", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4357, .Spacer1Type = "114", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4371, .Spacer1Type = "106", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4393, .Spacer1Type = "112", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4425, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4471, .Spacer1Type = "114", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4471, .Spacer1Type = "117", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4477, .Spacer1Type = "106", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4505, .Spacer1Type = "112", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4535, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4583, .Spacer1Type = "106", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4585, .Spacer1Type = "114", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4588, .Spacer1Type = "117", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "112", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4645, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4689, .Spacer1Type = "106", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4699, .Spacer1Type = "114", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4705, .Spacer1Type = "117", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4729, .Spacer1Type = "112", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4755, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4795, .Spacer1Type = "106", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4813, .Spacer1Type = "114", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4822, .Spacer1Type = "117", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4841, .Spacer1Type = "112", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4865, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4901, .Spacer1Type = "106", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4927, .Spacer1Type = "114", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4939, .Spacer1Type = "117", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4953, .Spacer1Type = "112", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4975, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5007, .Spacer1Type = "106", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5041, .Spacer1Type = "114", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5056, .Spacer1Type = "117", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 5065, .Spacer1Type = "112", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5085, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5113, .Spacer1Type = "106", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5155, .Spacer1Type = "114", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5173, .Spacer1Type = "117", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5177, .Spacer1Type = "112", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5195, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5219, .Spacer1Type = "106", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5269, .Spacer1Type = "114", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "112", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5290, .Spacer1Type = "117", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5325, .Spacer1Type = "106", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5383, .Spacer1Type = "114", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5401, .Spacer1Type = "112", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5407, .Spacer1Type = "117", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5415, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5431, .Spacer1Type = "106", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5497, .Spacer1Type = "114", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5513, .Spacer1Type = "112", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5524, .Spacer1Type = "117", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5525, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "114", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5625, .Spacer1Type = "112", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5635, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5641, .Spacer1Type = "117", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5725, .Spacer1Type = "114", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5737, .Spacer1Type = "112", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5758, .Spacer1Type = "117", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5839, .Spacer1Type = "114", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5875, .Spacer1Type = "117", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5992, .Spacer1Type = "117", .CarriersQty = 51}
    }

    Private Shared ReadOnly Spacer100Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 425, .Spacer1Type = "80", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 435, .Spacer1Type = "82", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 460, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 505, .Spacer1Type = "80", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 517, .Spacer1Type = "82", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 547, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 585, .Spacer1Type = "80", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 599, .Spacer1Type = "82", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 634, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 665, .Spacer1Type = "80", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 681, .Spacer1Type = "82", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 721, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 745, .Spacer1Type = "80", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 763, .Spacer1Type = "82", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 808, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 825, .Spacer1Type = "80", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 845, .Spacer1Type = "82", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 895, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 905, .Spacer1Type = "80", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 927, .Spacer1Type = "82", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 982, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 985, .Spacer1Type = "80", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1009, .Spacer1Type = "82", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1065, .Spacer1Type = "80", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1069, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1091, .Spacer1Type = "82", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "80", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1156, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1173, .Spacer1Type = "82", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1225, .Spacer1Type = "80", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1243, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1255, .Spacer1Type = "82", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1305, .Spacer1Type = "80", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1330, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1337, .Spacer1Type = "82", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1385, .Spacer1Type = "80", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1417, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1419, .Spacer1Type = "82", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1465, .Spacer1Type = "80", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1501, .Spacer1Type = "82", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1504, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1545, .Spacer1Type = "80", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1583, .Spacer1Type = "82", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1591, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1625, .Spacer1Type = "80", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1665, .Spacer1Type = "82", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1678, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1705, .Spacer1Type = "80", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1747, .Spacer1Type = "82", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1765, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1785, .Spacer1Type = "80", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1829, .Spacer1Type = "82", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1852, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "80", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1911, .Spacer1Type = "82", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1945, .Spacer1Type = "80", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1993, .Spacer1Type = "82", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2025, .Spacer1Type = "80", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2026, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2075, .Spacer1Type = "82", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2105, .Spacer1Type = "80", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2113, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2157, .Spacer1Type = "82", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2185, .Spacer1Type = "80", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2239, .Spacer1Type = "82", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2265, .Spacer1Type = "80", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2287, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2321, .Spacer1Type = "82", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2345, .Spacer1Type = "80", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2374, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "82", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2425, .Spacer1Type = "80", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2461, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2485, .Spacer1Type = "82", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "80", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2548, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2567, .Spacer1Type = "82", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2585, .Spacer1Type = "80", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2635, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2649, .Spacer1Type = "82", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "80", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2722, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2731, .Spacer1Type = "82", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "80", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2809, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2813, .Spacer1Type = "82", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2825, .Spacer1Type = "80", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2895, .Spacer1Type = "82", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2896, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2905, .Spacer1Type = "80", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2977, .Spacer1Type = "82", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2983, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2985, .Spacer1Type = "80", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3059, .Spacer1Type = "82", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3065, .Spacer1Type = "80", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3070, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3141, .Spacer1Type = "82", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3145, .Spacer1Type = "80", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3157, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3223, .Spacer1Type = "82", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3225, .Spacer1Type = "80", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3244, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3305, .Spacer1Type = "80", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3305, .Spacer1Type = "82", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3331, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3385, .Spacer1Type = "80", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3387, .Spacer1Type = "82", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3418, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3465, .Spacer1Type = "80", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3469, .Spacer1Type = "82", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3505, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3545, .Spacer1Type = "80", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3551, .Spacer1Type = "82", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3592, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "80", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3633, .Spacer1Type = "82", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3679, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3705, .Spacer1Type = "80", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3715, .Spacer1Type = "82", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3766, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3785, .Spacer1Type = "80", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3797, .Spacer1Type = "82", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3853, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3865, .Spacer1Type = "80", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3879, .Spacer1Type = "82", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3940, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3945, .Spacer1Type = "80", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3961, .Spacer1Type = "82", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4025, .Spacer1Type = "80", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4025, .Spacer1Type = "80", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4027, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4043, .Spacer1Type = "82", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4105, .Spacer1Type = "80", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4114, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "82", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "82", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4201, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4207, .Spacer1Type = "82", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4265, .Spacer1Type = "80", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4288, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4345, .Spacer1Type = "80", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4371, .Spacer1Type = "82", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4425, .Spacer1Type = "80", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4453, .Spacer1Type = "82", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4462, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4505, .Spacer1Type = "80", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4535, .Spacer1Type = "82", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4585, .Spacer1Type = "80", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "82", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4636, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4665, .Spacer1Type = "80", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4699, .Spacer1Type = "82", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4723, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4745, .Spacer1Type = "80", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4781, .Spacer1Type = "82", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4810, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4825, .Spacer1Type = "80", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4863, .Spacer1Type = "82", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4897, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4905, .Spacer1Type = "80", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4945, .Spacer1Type = "82", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4984, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4985, .Spacer1Type = "80", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5027, .Spacer1Type = "82", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5065, .Spacer1Type = "80", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5071, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5109, .Spacer1Type = "82", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5145, .Spacer1Type = "80", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5158, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5191, .Spacer1Type = "82", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5245, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5273, .Spacer1Type = "82", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5332, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5419, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5506, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5593, .Spacer1Type = "87", .CarriersQty = 64}
    }

    Private Shared ReadOnly Spacer89Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 390, .Spacer1Type = "73", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 400, .Spacer1Type = "75", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 463, .Spacer1Type = "73", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "75", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 536, .Spacer1Type = "73", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 550, .Spacer1Type = "75", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 609, .Spacer1Type = "73", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 625, .Spacer1Type = "75", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "73", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 700, .Spacer1Type = "75", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "73", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 775, .Spacer1Type = "75", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 828, .Spacer1Type = "73", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 850, .Spacer1Type = "75", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 901, .Spacer1Type = "73", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 925, .Spacer1Type = "75", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 974, .Spacer1Type = "73", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1000, .Spacer1Type = "75", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1047, .Spacer1Type = "73", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1075, .Spacer1Type = "75", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1120, .Spacer1Type = "73", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1150, .Spacer1Type = "75", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1193, .Spacer1Type = "73", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1225, .Spacer1Type = "75", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1266, .Spacer1Type = "73", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1300, .Spacer1Type = "75", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1339, .Spacer1Type = "73", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1375, .Spacer1Type = "75", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1412, .Spacer1Type = "73", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1450, .Spacer1Type = "75", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1485, .Spacer1Type = "73", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1525, .Spacer1Type = "75", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1558, .Spacer1Type = "73", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1600, .Spacer1Type = "75", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1631, .Spacer1Type = "73", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1675, .Spacer1Type = "75", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1704, .Spacer1Type = "73", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1750, .Spacer1Type = "75", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1777, .Spacer1Type = "73", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1825, .Spacer1Type = "75", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1850, .Spacer1Type = "73", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1900, .Spacer1Type = "75", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1923, .Spacer1Type = "73", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1975, .Spacer1Type = "75", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1996, .Spacer1Type = "73", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2050, .Spacer1Type = "75", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2069, .Spacer1Type = "73", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2125, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2142, .Spacer1Type = "73", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "75", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2215, .Spacer1Type = "73", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2275, .Spacer1Type = "75", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2288, .Spacer1Type = "73", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2350, .Spacer1Type = "75", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2361, .Spacer1Type = "73", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2425, .Spacer1Type = "75", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2434, .Spacer1Type = "73", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2500, .Spacer1Type = "75", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2507, .Spacer1Type = "73", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2575, .Spacer1Type = "75", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2580, .Spacer1Type = "73", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2650, .Spacer1Type = "75", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2653, .Spacer1Type = "73", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2725, .Spacer1Type = "75", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2726, .Spacer1Type = "73", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2799, .Spacer1Type = "73", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2800, .Spacer1Type = "75", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2872, .Spacer1Type = "73", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2875, .Spacer1Type = "75", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2945, .Spacer1Type = "73", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "75", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3018, .Spacer1Type = "73", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3025, .Spacer1Type = "75", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3091, .Spacer1Type = "73", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3100, .Spacer1Type = "75", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3164, .Spacer1Type = "73", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3175, .Spacer1Type = "75", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3237, .Spacer1Type = "73", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3250, .Spacer1Type = "75", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3310, .Spacer1Type = "73", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3325, .Spacer1Type = "75", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3383, .Spacer1Type = "73", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3400, .Spacer1Type = "75", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3456, .Spacer1Type = "73", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3475, .Spacer1Type = "75", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3529, .Spacer1Type = "73", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "75", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3602, .Spacer1Type = "73", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "75", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3675, .Spacer1Type = "73", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3675, .Spacer1Type = "73", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "75", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3748, .Spacer1Type = "73", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "75", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "75", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3850, .Spacer1Type = "75", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3894, .Spacer1Type = "73", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3967, .Spacer1Type = "73", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4000, .Spacer1Type = "75", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "73", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4075, .Spacer1Type = "75", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4113, .Spacer1Type = "73", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4150, .Spacer1Type = "75", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4186, .Spacer1Type = "73", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4225, .Spacer1Type = "75", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4259, .Spacer1Type = "73", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4300, .Spacer1Type = "75", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4332, .Spacer1Type = "73", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "75", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4405, .Spacer1Type = "73", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4450, .Spacer1Type = "75", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4478, .Spacer1Type = "73", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4525, .Spacer1Type = "75", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4551, .Spacer1Type = "73", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4600, .Spacer1Type = "75", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4624, .Spacer1Type = "73", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4675, .Spacer1Type = "75", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4697, .Spacer1Type = "73", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4750, .Spacer1Type = "75", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4825, .Spacer1Type = "75", .CarriersQty = 64}
    }

    Private Shared ReadOnly Spacer63Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 260, .Spacer1Type = "47", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 307, .Spacer1Type = "47", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 354, .Spacer1Type = "47", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 401, .Spacer1Type = "47", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 448, .Spacer1Type = "47", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 495, .Spacer1Type = "47", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 542, .Spacer1Type = "47", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 589, .Spacer1Type = "47", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 636, .Spacer1Type = "47", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "47", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 730, .Spacer1Type = "47", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 777, .Spacer1Type = "47", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 824, .Spacer1Type = "47", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 871, .Spacer1Type = "47", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 918, .Spacer1Type = "47", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 965, .Spacer1Type = "47", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1012, .Spacer1Type = "47", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1059, .Spacer1Type = "47", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1106, .Spacer1Type = "47", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "47", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1200, .Spacer1Type = "47", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1247, .Spacer1Type = "47", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1294, .Spacer1Type = "47", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1341, .Spacer1Type = "47", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 1388, .Spacer1Type = "47", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 1435, .Spacer1Type = "47", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 1482, .Spacer1Type = "47", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 1529, .Spacer1Type = "47", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 1576, .Spacer1Type = "47", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 1623, .Spacer1Type = "47", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 1670, .Spacer1Type = "47", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 1717, .Spacer1Type = "47", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 1764, .Spacer1Type = "47", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 1811, .Spacer1Type = "47", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 1858, .Spacer1Type = "47", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "47", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 1952, .Spacer1Type = "47", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 1999, .Spacer1Type = "47", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 2046, .Spacer1Type = "47", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 2093, .Spacer1Type = "47", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 2140, .Spacer1Type = "47", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 2187, .Spacer1Type = "47", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 2234, .Spacer1Type = "47", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 2281, .Spacer1Type = "47", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 2328, .Spacer1Type = "47", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 2375, .Spacer1Type = "47", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 2422, .Spacer1Type = "47", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 2469, .Spacer1Type = "47", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "47", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 2563, .Spacer1Type = "47", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 2610, .Spacer1Type = "47", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "47", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 2704, .Spacer1Type = "47", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 2751, .Spacer1Type = "47", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 2798, .Spacer1Type = "47", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 2845, .Spacer1Type = "47", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 2892, .Spacer1Type = "47", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 2939, .Spacer1Type = "47", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 2986, .Spacer1Type = "47", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 3033, .Spacer1Type = "47", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 3080, .Spacer1Type = "47", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 3127, .Spacer1Type = "47", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 3174, .Spacer1Type = "47", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 3221, .Spacer1Type = "47", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 3268, .Spacer1Type = "47", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 3315, .Spacer1Type = "47", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 3362, .Spacer1Type = "47", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 3409, .Spacer1Type = "47", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 3456, .Spacer1Type = "47", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 3503, .Spacer1Type = "47", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "47", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 3597, .Spacer1Type = "47", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 3644, .Spacer1Type = "47", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 3691, .Spacer1Type = "47", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 3738, .Spacer1Type = "47", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 3785, .Spacer1Type = "47", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 3832, .Spacer1Type = "47", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 3879, .Spacer1Type = "47", .CarriersQty = 82},
        New SpacerInfo With {.MaxWidth = 3926, .Spacer1Type = "47", .CarriersQty = 83},
        New SpacerInfo With {.MaxWidth = 3973, .Spacer1Type = "47", .CarriersQty = 84},
        New SpacerInfo With {.MaxWidth = 4020, .Spacer1Type = "47", .CarriersQty = 85},
        New SpacerInfo With {.MaxWidth = 4067, .Spacer1Type = "47", .CarriersQty = 86},
        New SpacerInfo With {.MaxWidth = 4114, .Spacer1Type = "47", .CarriersQty = 87}
    }

    Private Shared Function ResetJobSheets(JobId As String) As String
        Try
            Dim tableNames As String() = {
                "JobSheets",
                "JobSheet_RollerBlinds",
                "JobSheet_Verishades",
                "JobSheet_Verticals",
                "JobSheet_Aluminium",
                "JobSheet_Venetian"
            }

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                For Each tableName As String In tableNames
                    ' Pertama cek apakah ada data dengan JobId
                    Dim checkQuery As String = "SELECT COUNT(*) FROM " & tableName & " WHERE JobId = @JobId"
                    Using checkCmd As New SqlCommand(checkQuery, thisConn)
                        checkCmd.Parameters.Clear()
                        checkCmd.Parameters.AddWithValue("@JobId", JobId)

                        Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                        ' Jika ada datanya, hapus
                        If count > 0 Then
                            Dim deleteQuery As String = "DELETE FROM " & tableName & " WHERE JobId = @JobId"
                            Using deleteCmd As New SqlCommand(deleteQuery, thisConn)
                                deleteCmd.Parameters.Clear()
                                deleteCmd.Parameters.AddWithValue("@JobId", JobId)
                                deleteCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End Using
                Next

                thisConn.Close()
            End Using

            Return "200"
        Catch ex As Exception
            Return "500 : " & ex.Message
        End Try
    End Function

    Private Shared Function CreateJobSheets(JobId As String) As String
        Try
            Dim JobHeaderData As DataSet = publicCfg.GetListData("SELECT * FROM JobHeaders WHERE Id='" & JobId & "'")
            If JobHeaderData Is Nothing OrElse JobHeaderData.Tables(0).Rows.Count = 0 Then : Return "403" : End If

            Dim HeaderId As String = JobHeaderData.Tables(0).Rows(0).Item("HeaderId").ToString()
            Dim JoNumber As String = JobHeaderData.Tables(0).Rows(0).Item("JoNumber").ToString()
            Dim StoreName As String = JobHeaderData.Tables(0).Rows(0).Item("StoreName").ToString()
            Dim OrderNo As String = JobHeaderData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim OrderCust As String = JobHeaderData.Tables(0).Rows(0).Item("OrderCust").ToString()
            Dim Delivery As String = JobHeaderData.Tables(0).Rows(0).Item("Delivery").ToString() '#As ZoneID
            Dim UserName As String = JobHeaderData.Tables(0).Rows(0).Item("UserName").ToString()
            Dim CreatedDateStr As String = JobHeaderData.Tables(0).Rows(0).Item("CreatedDate").ToString()
            Dim SubmittedDateStr As String = JobHeaderData.Tables(0).Rows(0).Item("SubmittedDate").ToString()

            Dim CreatedDate As DateTime
            Dim SubmittedDate As DateTime

            If Not DateTime.TryParse(CreatedDateStr, CreatedDate) Then : CreatedDate = DateTime.MinValue : End If
            If Not DateTime.TryParse(SubmittedDateStr, SubmittedDate) Then : SubmittedDate = DateTime.MinValue : End If

            ' Ambil data JobDetails
            Dim JobDetailData As DataSet = publicCfg.GetListData("SELECT * FROM JobDetails WHERE JobId='" & JobId & "' ORDER BY BlindName, DesignName, Id")
            Dim allDetails As DataTable = JobDetailData.Tables(0)

            If allDetails.Rows.Count = 0 Then
                Return "404 : Tidak ada data JobDetails"
            End If

            '#hitung total qty
            Dim totalQty As Integer = allDetails.AsEnumerable().Sum(Function(r) If(IsDBNull(r("Qty")), 0, Convert.ToInt32(r("Qty"))))


            ' Group hanya berdasarkan BlindName
            Dim grouped = allDetails.AsEnumerable().GroupBy(Function(r) r.Field(Of String)("BlindName"))

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                For Each group In grouped
                    Dim tableName As String = String.Empty
                    Dim detailRows = group.ToList()
                    Dim blindName As String = group.Key
                    Dim designName As String = If(detailRows.Any(), detailRows(0).Field(Of String)("DesignName"), "")

                    Dim totalBlinds = detailRows.Count
                    Dim totalPages = CInt(Math.Ceiling(totalBlinds / 6.0))


                    Dim amountBlind As Integer = totalQty
                    ' Dim amountBlind As Integer = detailRows.Sum(Function(r) If(IsDBNull(r("Qty")), 0, Convert.ToInt32(r("Qty"))))
                    ' Dim amountBlind As Integer = detailRows.Count
                    ' Return "400 :" & amountBlind

                    For pageIndex As Integer = 0 To totalPages - 1
                        Dim pageOf As Integer = pageIndex + 1
                        Dim chunk = detailRows.Skip(pageIndex * 6).Take(6).ToList()

                        '#Indeks 0 sampai 5 untuk 6 record
                        Dim fieldsToProcess As New List(Of String)
                        Select Case designName
                            Case "Roller Blinds"
                                fieldsToProcess.AddRange({"Line", "BlindNo", "LinkBlind", "Qty", "Location", "Mounting", "Width", "Drop", "RollDirection", "ControlPosition", "ControlLength", "MotorStyle", "MotorRemote", "MotorCharger", "Connector", "Accessory", "TubeSize", "Trim", "ChildSafe", "Notes", "KitName", "BracketType", "TubeType", "TubeSkinSize", "NumBoldNuts",  "ControlType",  "ColourType", "ChainName", "ChainColour", "ChainLength","BottomName", "BottomType", "BottomColour","FabricName", "FabricType", "FabricColour", "FabricWidth"})

                                tableName = "JobSheet_RollerBlinds"

                            Case "Veri Shades"
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "StackPosition", "TrackType", "TrackColour", "WandColour", "WandLength", "Notes", "KitName", "BracketType", "FabricName", "FabricType", "FabricColour"})

                                tableName = "JobSheet_VeriShades"
                            Case "Vertical Blinds"
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "StackPosition", "ControlPosition", "ChainLength", "TrackColour", "WandColour", "WandLength", "SlatSize", "SlatQty", "BracketOption", "BracketColour", "BottomHoldDown", "HangerType", "Sloper", "InsertInTrack", "Notes", "KitName", "TubeType", "Spacer", "CarrierQty", "FabricCutDrop", "ControlType", "ChainName", "ChainColour", "CLength", "FabricName", "FabricType", "FabricColour", "FabricWidth"})

                                tableName = "JobSheet_Verticals"

                            Case "Aluminium Blinds"
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "ControlPosition", "WandLength", "BracketOption", "BottomHoldDown", "CutOut_LeftTop", "CutOut_RightTop", "CutOut_LeftBottom", "CutOut_RightBottom", "LHSWidth_Top", "LHSHeight_Top", "RHSWidth_Top", "RHSHeight_Top", "LHSWidth_Bottom", "LHSHeight_Bottom", "RHSWidth_Bottom", "RHSHeight_Bottom", "Notes", "KitName", "VenetianType", "ColourType"})

                                tableName = "JobSheet_Aluminium"

                            Case "Venetian Blinds"
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "ControlPosition", "ControlLength", "WandLength", "BracketOption", "BottomHoldDown", "PelmetType", "PelmetWidth", "PelmetSize", "PelmetReturn", "PelmetReturnPosition", "PelmetReturnSize", "PelmetReturnSize2", "CutOut_LeftTop", "CutOut_RightTop", "CutOut_LeftBottom", "CutOut_RightBottom", "LHSWidth_Top", "LHSHeight_Top", "RHSWidth_Top", "RHSHeight_Top", "LHSWidth_Bottom", "LHSHeight_Bottom", "RHSWidth_Bottom", "RHSHeight_Bottom", "Notes", "KitName", "VenetianType", "ControlType", "ColourType"})

                                tableName = "JobSheet_Venetian"
                            Case Else
                                fieldsToProcess.AddRange({"Line", "BlindNo", "LinkBlind", "Qty","Location","Mounting","Width","Drop","SemiInsideMount","LouvreSize","LouvrePosition","HingeColour","MidrailHeight1","MidrailHeight2","MidrailCritical","Layout","LayoutSpecial","CustomHeaderLength","FrameType","FrameLeft","FrameRight","FrameTop","FrameBottom","BottomTrackType","BottomTrackRecess","Buildout","BuildoutPosition","PanelQty","TrackQty","PanelSize","NumOfPanel","HingeQtyPerPanel","PanelQtyWithHinge","LocationTPost1","LocationTPost2","LocationTPost3","LocationTPost4","LocationTPost5","HorizontalTPost","HorizontalTPostHeight","JoinedPanels","ReverseHinged","PelmetFlat","ExtraFascia","HingesLoose","TiltrodType","TiltrodSplit","SplitHeight1","SplitHeight2","DoorCutOut","SpecialShape","TemplateProvided","SquareMetre","LinearMetre","StackPosition","TilterPosition","RollDirection","ControlPosition","ControlColour","ControlLength","ChainLength","MaterialChain","MotorStyle","MotorRemote","MotorRequired","MotorBattery","MotorCharger","Connector","AdditionalMotor","CableExitPoint","TrackType","TrackColour","TrackLength","NumOfWand","WandPosition","WandColour","WandLength","CordColour","CordLength","AcornPlasticColour","Accessory","SideBySide","SlatSize","SlatQty","TubeSize","Trim","Batten","BattenColour","BracketOption","BracketColour","BracketCover","BracketExtension","Fitting","FlatType","ChildSafe","Cleat","BottomHoldDown","HangerType","PelmetType","PelmetWidth","PelmetSize","PelmetReturn","PelmetReturnPosition","PelmetReturnSize","PelmetReturnSize2","CutOut_LeftTop","CutOut_RightTop","CutOut_LeftBottom","CutOut_RightBottom","LHSWidth_Top","LHSHeight_Top","RHSWidth_Top","RHSHeight_Top","LHSWidth_Bottom","LHSHeight_Bottom","RHSWidth_Bottom","RHSHeight_Bottom","BlindSize","Sloper","InsertInTrack","Notes","KitName","VenetianType","BracketType","TubeType","TubeSkinSize","NumBoldNuts","Spacer", "CarrierQty", "FabricCutDrop","ControlType","ColourType","ChainName","ChainColour","CLength","BottomName","BottomType","BottomColour","FabricName","FabricType","FabricColour","FabricWidth"})

                                tableName = "JobSheets"
                        End Select

                        ' Bangun bagian kolom dan parameter dari query INSERT secara dinamis
                        Dim dynamicColumns As New List(Of String)
                        Dim dynamicPlaceholders As New List(Of String)

                        For Each fieldName In fieldsToProcess
                            For i As Integer = 1 To 6
                                dynamicColumns.Add(fieldName & i.ToString())
                                dynamicPlaceholders.Add("@" & fieldName & i.ToString())
                            Next
                        Next

                        ' Gabungkan semua kolom dan placeholder
                        Dim commonColumns As String = "JobId, PageOf, AmountOfPage, JoNumber, HeaderId, DesignName, BlindName, AmountBlind, StoreName, OrderNo, OrderCust, ZoneId, UserName, OrderCreated, ShipDate"
                        Dim commonPlaceholders As String = "@JobId, @PageOf, @AmountOfPage, @JoNumber, @HeaderId, @DesignName, @BlindName, @AmountBlind, @StoreName, @OrderNo, @OrderCust, @ZoneId, @UserName, @OrderCreated, @ShipDate"

                        Dim insertColumns As String = commonColumns & ", " & String.Join(", ", dynamicColumns)
                        Dim insertPlaceholders As String = commonPlaceholders & ", " & String.Join(", ", dynamicPlaceholders)

                        Dim insertQuery As String = "INSERT INTO " & tableName & " (" & insertColumns & ") VALUES (" & insertPlaceholders & ")"

                        Using myCmd As New SqlCommand(insertQuery, thisConn)
                            myCmd.Parameters.AddWithValue("@JobId", JobId)
                            myCmd.Parameters.AddWithValue("@PageOf", pageOf)
                            myCmd.Parameters.AddWithValue("@AmountOfPage", totalPages)
                            myCmd.Parameters.AddWithValue("@JoNumber", JoNumber)
                            myCmd.Parameters.AddWithValue("@HeaderId", HeaderId)
                            myCmd.Parameters.AddWithValue("@DesignName", designName)
                            myCmd.Parameters.AddWithValue("@BlindName", blindName)
                            myCmd.Parameters.AddWithValue("@AmountBlind", amountBlind)
                            myCmd.Parameters.AddWithValue("@StoreName", StoreName)
                            myCmd.Parameters.AddWithValue("@OrderNo", OrderNo)
                            myCmd.Parameters.AddWithValue("@OrderCust", OrderCust)
                            myCmd.Parameters.AddWithValue("@ZoneId", Delivery)
                            myCmd.Parameters.AddWithValue("@UserName", UserName)
                            myCmd.Parameters.AddWithValue("@OrderCreated", CreatedDate)
                            myCmd.Parameters.AddWithValue("@ShipDate", SubmittedDate)

                            ' Tambahkan parameter untuk setiap field secara dinamis
                            For Each fieldName In fieldsToProcess
                                For i As Integer = 0 To 5 ' Indeks 0 sampai 5 untuk 6 record
                                    Dim paramName As String = "@" & fieldName & (i + 1).ToString()
                                    Dim value As Object = DBNull.Value
                                    Dim globalLineNumber As Integer = (pageIndex * 6) + i + 1

                                '     If fieldName = "Line" Then
                                '         ' --- LOGIKA BARU UNTUK KOLOM "LINE" DIMULAI DI SINI ---
                                '         If i < chunk.Count Then
                                '             ' Hitung nomor baris global untuk BlindName ini
                                '             value = "Line " & globalLineNumber.ToString()
                                '         End If
                                '         ' --- LOGIKA BARU UNTUK KOLOM "LINE" BERAKHIR DI SINI ---
                                '    Else
                                   If fieldName = "Notes" Then
                                        If i < chunk.Count Then
                                            If chunk(i).Table.Columns.Contains(fieldName) AndAlso Not IsDBNull(chunk(i)(fieldName)) Then
                                                value = "(Item " & globalLineNumber.ToString() & ") -" & CType(chunk(i)(fieldName), Object) & " |"
                                            End If
                                        End If
                                    ElseIf fieldName = "CLength" Then
                                        If i < chunk.Count Then
                                            If chunk(i).Table.Columns.Contains(fieldName) AndAlso Not IsDBNull(chunk(i)(fieldName)) Then
                                                value = CType(chunk(i)(fieldName), Object) & " + joiner"
                                            End If
                                        End If
                                    Else
                                        ' Logika umum untuk kolom lain
                                        If i < chunk.Count Then
                                            If chunk(i).Table.Columns.Contains(fieldName) AndAlso Not IsDBNull(chunk(i)(fieldName)) Then
                                                value = CType(chunk(i)(fieldName), Object)
                                            End If
                                        End If
                                    End If
                                    myCmd.Parameters.AddWithValue(paramName, value)
                                Next
                            Next

                            myCmd.ExecuteNonQuery()
                        End Using
                        ' --- End Penyederhanaan ---

                    Next ' End For pageIndex
                Next ' End For group
            End Using ' End Using thisConn

            Return "200"

        Catch ex As Exception
            Console.WriteLine("Error in CreateJobSheets: " & ex.Message)
            Return "403: Error in CreateJobSheets: " & ex.Message
        End Try
    End Function

    Private Shared Function CreatePDFJobSheets(JobId As String, fileDirectory As String, fileName As String) As String
        Try
            
             '{"TableNameOnDatabase", AddressOf PrintFunctionName},
            Dim jobSheetSources As New Dictionary(Of String, Func(Of DataRow, String)) From {
                {"JobSheets", AddressOf JobSheets},
                {"JobSheet_RollerBlinds", AddressOf JobSheetRollerBlinds},
                {"JobSheet_Verishades", AddressOf JobSheetVerishades},
                {"JobSheet_Verticals", AddressOf JobSheetVerticals},
                {"JobSheet_Aluminium", AddressOf JobSheetAluminium},
                {"JobSheet_Venetian", AddressOf JobSheetVenetian}
            }


            Using stream As FileStream = New FileStream(fileDirectory + "/" + fileName, FileMode.Create)
                Dim pdfDoc As Document = New Document(PageSize.A4, 20, 20, 20, 20)
                Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
                pdfDoc.Open()              

                For Each kvp In jobSheetSources
                    '#for break page /Table
                    pdfDoc.NewPage()
                    Dim tableName As String = kvp.Key
                    Dim printFunction As Func(Of DataRow, String) = kvp.Value

                    Dim ds As DataSet = publicCfg.GetListData("SELECT * FROM " & tableName & " WHERE JobId='" & JobId & "' ORDER BY DesignName, BlindName ASC")
                    If ds.Tables(0).Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            Dim currentData As DataRow = ds.Tables(0).Rows(i)
                            Dim result As String = String.Empty
                            ' result += jobsheet.PrintHeader(currentData)
                            result += PrintHeader(currentData)
                            result += printFunction(currentData)

                            Dim sr As New StringReader(result)
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)

                            If i < ds.Tables(0).Rows.Count - 1 Then
                                pdfDoc.NewPage()
                            End If
                        Next
                    End If
                Next              

                pdfDoc.Close()
                stream.Close()
            End Using 

            Return "200"
        Catch ex As Exception
            Return "500: " & ex.Message
        End Try
    End Function

    '#---------------------------------------|| Styling PDF JobSheets || ---------------------------------------#
    '#---------------------------------------|| Templats || ---------------------------------------#
    Private Shared tableDetStart AS String = "<table style='width: 100%; border-collapse: collapse; font-size:13px;'>"
    Private Shared boldStart As String = "<b>"
    Private Shared boldEnd As String = "</b>"
    Private Shared trDetStart As String = "<tr style='text-align:left;'>"
    Private Shared tdTitleStart As String ="<td style='width:100px; padding:5px 2px; border-top:1px solid black; border-right:1px solid black;'>"
    Private Shared tdDetStart  As String ="<td style='width:100px; padding:5px 2px; border-top:1px solid black; border-right:1px solid black;'>"
    Private Shared tdDetFooterStart  As String ="<td style='width:100px; padding:5px 2px; text-align: center;'>"
    Private Shared tdDetTransStart  As String ="<td style='width:100px; padding:5px 2px; border-top:1px solid black; border-right:1px solid black; color:white;'>"
    Private Shared tdDetRight  As String ="<td style='width:100px padding:5px 2px; border-top:1px solid black;'>"
    Private Shared tdDetEnd As String = "</td>"
    Private Shared trDetEnd As String = "</tr>"
    Private Shared fs12Start As String = "<span style='font-size:12px;'>"
    Private Shared fs11Start As String = "<span style='font-size:11px;'>"
    Private Shared fs10Start As String = "<span style='font-size:10px;'>"
    Private Shared fsEnd As String = "</span>"
    Private Shared tableDetEnd As String = "</table>"

    '#------------------------------------------|| Rendering Print ||------------------------------------------#
    Private Shared Function JobSheets(currentData As DataRow) As String
        Dim result As String = String.Empty
        
        ' Select Case currentData("BlindName").ToString()
        '     Case "Cassette"
        '         result += PrintRollerCassette(currentData)
        ' End Select
        result+= SubstituteFabric()
        result+= LineOptions(currentData)

        Return result
    End Function

    Private Shared Function JobSheetRollerBlinds(currentData As DataRow) As String
        Dim result As String = String.Empty
        
        Select Case currentData("BlindName").ToString()
            Case "Cassette"
                result += PrintRollerCassette(currentData)
            Case "Motorised"
                result += PrintRollerMotorised(currentData)
            Case "Roller Blind"
                result += PrintRollerBlind(currentData)
            Case "Skin Only"
                result += PrintRollerSkin(currentData)
        End Select

        Return result
    End Function

    Private Shared Function JobSheetVerishades(currentData As DataRow) As String
        Dim result As String = String.Empty
        
        Select Case currentData("BlindName").ToString()
            Case "Single"
                result += PrintVerishadeSingle(currentData)
            Case "Slat Only"
                result += PrintVerishadeSlat(currentData)
            Case "Track Only"
                result += PrintVerishadeTrack(currentData)
        End Select

        Return result
    End Function

    Private Shared Function JobSheetVerticals(currentData As DataRow) As String
        Dim result As String = String.Empty
        
        Select Case currentData("BlindName").ToString()
            Case "Complete"
                result += PrintVerticalComplete(currentData)
            Case "Slat Only"
                result += PrintVerticalSlat(currentData)
            Case "Track Only"
                result += PrintVerticalTrack(currentData)
        End Select

        Return result
    End Function

    Private Shared Function JobSheetAluminium(currentData As DataRow) As String
        Dim result As String = String.Empty
        result += PrintAluminium(currentData)
        Return result
    End Function
    
    Private Shared Function JobSheetVenetian(currentData As DataRow) As String
        Dim result As String = String.Empty
         Select Case currentData("BlindName").ToString()
            Case "Mockwood Venetian"
                result += PrintMockwoodVenetian(currentData)
            Case "Timber Venetian"
                result += PrintTimberVenetian(currentData)
            Case "Wooden Venetian"
                result += PrintWoodenVenetian(currentData)
        End Select
        Return result
    End Function


    '#------------------------------------------|| Print Header ||------------------------------------------#
    Private Shared Function PrintHeader(currentData As DataRow) As String
        Dim result As String = String.Empty
        Dim ReportType As String = String.Empty
        Dim ReportIcon As String = String.Empty
        Dim GoWith As String = String.Empty
        Dim goWithList As New List(Of String)()
        Dim JobId As String = currentData("JobId").ToString()

        ' Dim BlindNameList As DataSet = Nothing
        '#-------------------------------|| Find Go With ||-------------------------------#
        '#All
        Dim all As DataSet = publicCfg.GetListData("SELECT DesignName, BlindName FROM Jobsheets WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To all.Tables(0).Rows.Count - 1
            Dim designName As String = all.Tables(0).Rows(i).Item("DesignName").ToString()
            Select Case designName
            Case "Cellora Blinds"
                goWithList.Add("Cel")
            Case "Roman Blinds"
                goWithList.Add("Rom")
            Case "Panel Glides"
                goWithList.Add("PG")
            End Select
        Next

        '#Roller Blinds
        Dim rollerList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_RollerBlinds WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To rollerList.Tables(0).Rows.Count - 1
            Dim blindName As String = rollerList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case blindName
                Case "Roller Blind"
                        goWithList.Add("H")
                Case "Motorised"
                    goWithList.Add("Motorised")
                Case "Cassette"
                        goWithList.Add("Hc")
                Case "Skin Only"
                    goWithList.Add("Hs")
            End Select
        Next

        '#Aluminium Blinds
        Dim alumList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Aluminium WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To alumList.Tables(0).Rows.Count - 1
            goWithList.Add("Alu")
        Next

        '#Venetian Blinds
        Dim venList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Venetian WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To venList.Tables(0).Rows.Count - 1
            Dim BlindName As String = venList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case BlindName
                Case "Mockwood Venetian"
                    goWithList.Add("MV")
                Case "Timber Venetian"
                    goWithList.Add("TV")
                Case "Wooden Venetian"
                    goWithList.Add("WV")
            End Select
        Next

        '#Vertical Blinds
        Dim verList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Verticals WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To verList.Tables(0).Rows.Count - 1
            Dim blindName As String = verList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case blindName
                Case "Complete"
                    goWithList.Add("VD")
                Case "Slat Only"
                    goWithList.Add("VDs")
                Case "Track Only"
                    goWithList.Add("VDt")
            End Select
        Next

        '#Vertical Blinds
        Dim veriList As DataSet = publicCfg.GetListData("SELECT BlindName FROM Jobsheet_Verishades WHERE JobId = '" & JobId & "'")
        For i As Integer = 0 To veriList.Tables(0).Rows.Count - 1
            Dim blindName As String = veriList.Tables(0).Rows(i).Item("BlindName").ToString()
            Select Case blindName
                Case "Single"
                    goWithList.Add("VR")
                Case "Slat Only"
                    goWithList.Add("VRs")
                Case "Track Only"
                    goWithList.Add("VRt")
            End Select
        Next


        '#-------------------------------|| ReportIcon & ReportType ||-------------------------------#
        Select Case currentData("DesignName").ToString()
            '#--------Roller Blinds-----------
            Case "Roller Blinds"
                ReportIcon = "H"
                ReportType = "Holland"

                Select Case  currentData("BlindName").ToString()
                    Case "Roller Blind"
                        ReportType = "Holland Blinds"
                        ReportIcon = "H"

                    Case "Motorised"
                        ReportType = "Holland Motorised"
                        ReportIcon = "HM"

                    Case "Cassette"
                        ReportType = "Holland Cassette"
                        ReportIcon = "Hc"

                    Case "Skin Only"
                        ReportType = "Holland Skin"
                        ReportIcon = "Hs"
                End Select

            '#--------Aluminium Blinds-----------
            Case "Aluminium Blinds"
                ReportIcon = "V-alu"
                ReportType = "Venetian Aluminium"

            '#--------Venetian Blinds-----------
            Case "Venetian Blinds"

                ReportIcon = "V"
                ReportType = "Venetian"

                Select Case  currentData("BlindName").ToString()
                    Case "Mockwood Venetian"
                        ReportType = "Mockwood Venetian"
                        ReportIcon = "MV"

                    Case "Timber Venetian"
                        ReportType = "Timber Venetian"
                        ReportIcon = "TV"

                    Case "Wooden Venetian"
                        ReportType = "Wooden Venetian"
                        ReportIcon = "WV"
                End Select

            '#--------Vertical Blinds-----------
            Case "Vertical Blinds"
                ReportIcon = "V"
                ReportType = "Vertical"

                Select Case  currentData("BlindName").ToString()
                    Case "Complete"
                        ReportType = "Vertical"
                        ReportIcon = "VD"

                    Case "Slat Only"
                        ReportType = "Vertical Slat"
                        ReportIcon = "VDs"

                    Case "Track Only"
                        ReportType = "Vertical Track"
                        ReportIcon = "VDt"
                End Select


            ' '#--------Veri Shades-----------
            Case "Veri Shades"

                ReportIcon = "VR"
                ReportType = "Verishade"
                
                Select Case  currentData("BlindName").ToString()
                    Case "Single"
                        ReportIcon = "VR"
                        ReportType = "Verishade"
                    Case "Slat Only"
                        ReportIcon = "VRs"
                        ReportType = "Verishade Slat"
                    Case "Track Only"
                        ReportIcon = "VRt"
                        ReportType = "Verishade Track"
                End Select
            Case "Cellora Blinds"
                ReportIcon = "CL"
                ReportType = "Cellora"
            Case "Roman Blinds"
                ReportIcon = "R"
                ReportType = "Roman"
            Case "Panel Glides"
                ReportIcon = "PG"
                ReportType = "Panel Glide"
            Case Else
                ReportIcon = "NO"
                ReportType = "Nulable"
        End Select
        
        If goWithList.Count > 0 Then
            GoWith = String.Join(" / ", goWithList)
        End If


        Dim OrderCreated As String = Convert.ToDateTime(currentData("OrderCreated")).ToString("dd/MM/yyyy")
        Dim JobCreated As String = Convert.ToDateTime(currentData("JobCreated")).ToString("dd/MM/yyyy")
        Dim ShipDate As String = Convert.ToDateTime(currentData("ShipDate")).ToString("dd MMM yy").ToUpper()
        Dim Notes1 As String = currentData("Notes1").ToString()
        Dim Notes2 As String = currentData("Notes2").ToString()
        Dim Notes3 As String = currentData("Notes3").ToString()
        Dim Notes4 As String = currentData("Notes4").ToString()
        Dim Notes5 As String = currentData("Notes5").ToString()
        Dim Notes6 As String = currentData("Notes6").ToString()
        Dim hightColumnNotes As String = "height: 30px;"
        If Not (String.IsNullOrEmpty(Notes1) Or String.IsNullOrEmpty(Notes2) Or String.IsNullOrEmpty(Notes3) Or String.IsNullOrEmpty(Notes4) Or String.IsNullOrEmpty(Notes5) Or String.IsNullOrEmpty(Notes6)) Then hightColumnNotes = ""
        '#header
        result+= "<table style='width: 100%; border-collapse: collapse;'>"
            '#Go With, Icon, & Job No
            result+= "<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 15px; padding-bottom: 5px;'>Go With</th>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 15px; padding-bottom: 5px;'>: "& GoWith &" /</th>"

                '#Heading Center This Only
                result+= "<th style='font-family: Impact, sans-serif; text-align: center; font-size: 35px; width: auto;' rowspan='6'>" & ReportIcon & "</th>"

                '#heading Right
                ' result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Job No</th>"
                ' result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("JoNumber").ToString() &"</th>"
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'></th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'></th>"
            result+= "</tr>"

            '#Order/Job Date & Reff
            result+="<tr>"   
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px; font-size: 12px; padding-bottom: 5px;'>Order/Job Date</td>"
                result+= "<td style=' text-align: left; width: 350px; font-size: 12px; padding-bottom: 5px;'>: "& OrderCreated &" / "& JobCreated &"</td>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 13px; padding-bottom: 5px;'>Reff</th>"
                result+= "<th style=' text-align: left; font-size: 13px; padding-bottom: 5px;'>: "& currentData("OrderCust").ToString() &"</th>"
            result+="</tr>"

            '#ID Unique & Design Type
            result+="<tr>"
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px;font-size: 13px; padding-bottom: 5px;'>ID Unique</td>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 13px; padding-bottom: 5px;'>: "& currentData("HeaderId").ToString() &"</th>"

                '#Heading Right
                result+= "<td style=' text-align: left; width: 80px; font-size: 12px; padding-bottom: 5px;'>Design Type</td>"
                result+= "<td style=' text-align: left; font-size: 12px; padding-bottom: 5px;'>: " & ReportType & "</td>"
            result+="</tr>"

            '#Store & Due Date
            result+="<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 15px; padding-bottom: 5px;'>Store</th>"
                result+= "<th style=' text-align: left; width: 350px;font-size: 15px; padding-bottom: 5px;'>: "& currentData("StoreName").ToString() &"</th>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Due date</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "&  ShipDate &"</th>"
            result+="</tr>"

            '#Order No & Zone
            result+="<tr>"
                '#Heading Left
                result+= "<th style=' text-align: left; width: 100px;font-size: 12px; padding-bottom: 5px;'>Order No</th>"
                result+= "<th style=' text-align: left; width: 350px; font-size: 12px; padding-bottom: 5px;'>: "& currentData("OrderNo").ToString() &"</th>"

                '#Heading Right
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Zone</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("ZoneId").ToString() &"</th>"
            result+="</tr>"

            '#Total Order Blind & Entered By
            result+="<tr>"
                '#Heading Left
                result+= "<td style=' text-align: left; width: 100px; font-size: 12px;'>Total Order Blind</td>"
                result+= "<td style=' text-align: left; width: 350px;  font-size: 12px;'>: "& currentData("AmountBlind").ToString() &"</td>"

                '#Heading Right
                result+= "<td style=' text-align: left; width: 80px; font-size: 12px; padding-bottom: 5px;'>Entered By</td>"
                result+= "<td style=' text-align: left; font-size: 12px; padding-bottom: 5px;'>: "& currentData("UserName").ToString() &"</td>"
            result+="</tr>"
            
            '#Information Notes
            result+="<tr>"
                result+= "<td colspan='5' style='font-size: 14px; border-top:1px solid black; "& hightColumnNotes &" vertical-align: top; padding: 0;'>"
                    result+= Notes1 & Notes2 & Notes3 & Notes4 & Notes5 & Notes6
                result+="</td>"
            result+="</tr>"
        result+= "</table>"
        Return result
    End Function

    '#------------------------------------------|| Print Detail - Aluminium Blinds||------------------------------------------#
    Private Shared Function PrintAluminium(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))

        '#line options
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#VenetianType
            result+= trDetStart
                result+= tdTitleStart & "Ven Type" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("VenetianType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#ColourType
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & currentData("ColourType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ColourType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControlPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#WandLength
            result+= trDetStart
                result+= tdTitleStart & "Wand Length" & tdDetEnd
                result+= tdDetStart & currentData("WandLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("WandLength6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketType
            result+= trDetStart
                result+= tdTitleStart & "Bracket Type" & tdDetEnd
                result+= tdDetStart & currentData("BracketOption1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BracketOption6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Cutouts
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Cut Outs" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Cut Outs ||-------------------------------------#
            '#CutOut_LeftTop
            result+= trDetStart
                result+= tdTitleStart & "Top Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightTop
            result+= trDetStart
                result+= tdTitleStart & "Top Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_LeftBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Top ||-------------------------------------#
            '#LHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Width" & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Height" & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Width" & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Height" & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#----------------------------------|| Bottom ||----------------------------------#
            '#LHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & "Holdown Colour" & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Line Blank
            result+= BlankLineEachRow(5)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Alu: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "H/RAILCUT" & tdDetEnd
                result+= tdDetFooterStart &  "TAPE CUT" & tdDetEnd
                result+= tdDetFooterStart &  "H/RAIL ASSEMBLING" & tdDetEnd
                result+= tdDetFooterStart &  "SLAT CUT" & tdDetEnd
                result+= tdDetFooterStart &  "BLIND FUNCH" & tdDetEnd
                result+= tdDetFooterStart &  "FINISHING" & tdDetEnd
                result+= tdDetFooterStart &  "VALANCE" & tdDetEnd
                result+= tdDetFooterStart &  "PACKING" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>____________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    '#------------------------------------------|| Print Detail - Venetian Blinds||------------------------------------------#
    Private Shared Function PrintMockwoodVenetian(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))

        Dim ControlPositions As New List(Of String())
        For i As Integer = 1 To 6
            Dim val As String = ""
            If Not IsDBNull(currentData("ControlPosition" & i)) Then
                val = currentData("ControlPosition" & i).ToString()
            End If
            Dim parts As String() = If(val.Contains("|"), val.Split("|"c), New String() {val})
            ControlPositions.Add(parts)
        Next

        




        '#line options
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#VenetianType
            result+= trDetStart
                result+= tdTitleStart & "Ven Type" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("VenetianType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControlType
            result+= trDetStart
                result+= tdTitleStart & "Slat Type" & tdDetEnd
                result+= tdDetStart & currentData("ControlType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ColourType
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & currentData("ColourType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ColourType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            

            result += trDetStart
            result += tdTitleStart & "Controls (lift)" & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 0, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 1, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 2, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 3, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 4, 0) & tdDetEnd
            result += tdDetRight & GetPart(ControlPositions, 5, 0) & tdDetEnd
            result += trDetEnd

            result += trDetStart
            result += tdTitleStart & "Controls (Tilt)" & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 0, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 1, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 2, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 3, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 4, 1) & tdDetEnd
            result += tdDetRight & GetPart(ControlPositions, 5, 1) & tdDetEnd
            result += trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Pelmet (76mm)" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetWidth
            result+= trDetStart
                result+= tdTitleStart & "Pelmet Length" & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetWidth6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetReturnSize
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Left Length Return" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetReturnSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetReturnSize2
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Right Length Return" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize21").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize22").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize23").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize24").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize25").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetReturnSize26").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Hold Down Brckts" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Cutouts
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Cut Outs" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Cut Outs ||-------------------------------------#
            '#CutOut_LeftTop
            result+= trDetStart
                result+= tdTitleStart & "Top Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightTop
            result+= trDetStart
                result+= tdTitleStart & "Top Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_LeftBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Top ||-------------------------------------#
            '#LHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Width" & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Height" & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Width" & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Height" & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#----------------------------------|| Bottom ||----------------------------------#
            '#LHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd


            '#Line Blank
            result+= BlankLineEachRow(5)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Ven: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    Private Shared Function PrintTimberVenetian(currentData As DataRow) As String
        Dim result As String = String.Empty

         Dim initUseBottom As String() = {
            currentData("BottomHoldDown1").ToString(),
            currentData("BottomHoldDown2").ToString(),
            currentData("BottomHoldDown3").ToString(),
            currentData("BottomHoldDown4").ToString(),
            currentData("BottomHoldDown5").ToString(),
            currentData("BottomHoldDown6").ToString()
        }

        For i As Integer = 0 To initUseBottom.Length - 1
           If Not String.IsNullOrEmpty(initUseBottom(i).ToString()) Then
                initUseBottom(i) = "Yes"
            Else
                initUseBottom(i) = String.Empty
            End If
        Next

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))

        '#line options
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#VenetianType
            result+= trDetStart
                result+= tdTitleStart & "Ven Type" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("VenetianType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControlType
            result+= trDetStart
                result+= tdTitleStart & "Slat Type" & tdDetEnd
                result+= tdDetStart & currentData("ControlType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ColourType
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & currentData("ColourType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ColourType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#ControlPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Cord Length" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

           '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & "Use Hold Down" & tdDetEnd
                result+= tdDetStart & initUseBottom(0) & tdDetEnd
                result+= tdDetStart & initUseBottom(1) & tdDetEnd
                result+= tdDetStart & initUseBottom(2) & tdDetEnd
                result+= tdDetStart & initUseBottom(3) & tdDetEnd
                result+= tdDetStart & initUseBottom(4) & tdDetEnd
                result+= tdDetRight & initUseBottom(5) & tdDetEnd
            result+= trDetEnd

           '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Hold Down Colour" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetType
            result+= trDetStart
                result+= tdTitleStart & "Pelmet Type" & tdDetEnd
                result+= tdDetStart & currentData("PelmetType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetSize
            result+= trDetStart
                result+= tdTitleStart & "Fascia Size" & tdDetEnd
                result+= tdDetStart & currentData("PelmetSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetWidth
            result+= trDetStart
                result+= tdTitleStart & "Fascia Width" & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetWidth6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Left Fascia Return" & fsEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Right Fascia Return" & fsEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Cutouts
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Cut Outs" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Cut Outs ||-------------------------------------#
            '#CutOut_LeftTop
            result+= trDetStart
                result+= tdTitleStart & "Top Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightTop
            result+= trDetStart
                result+= tdTitleStart & "Top Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_LeftBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Top ||-------------------------------------#
            '#LHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Width" & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Height" & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Width" & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Height" & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#----------------------------------|| Bottom ||----------------------------------#
            '#LHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd


            '#Line Blank
            result+= BlankLineEachRow(5)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Ven: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    Private Shared Function PrintWoodenVenetian(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))

        Dim ControlPositions As New List(Of String())
        For i As Integer = 1 To 6
            Dim val As String = ""
            If Not IsDBNull(currentData("ControlPosition" & i)) Then
                val = currentData("ControlPosition" & i).ToString()
            End If
            Dim parts As String() = If(val.Contains("|"), val.Split("|"c), New String() {val})
            ControlPositions.Add(parts)
        Next
        
        '#line options
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#VenetianType
            result+= trDetStart
                result+= tdTitleStart & "Ven Type" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("VenetianType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("VenetianType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ColourType
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & currentData("ColourType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ColourType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            

            result += trDetStart
            result += tdTitleStart & "Controls (lift)" & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 0, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 1, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 2, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 3, 0) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 4, 0) & tdDetEnd
            result += tdDetRight & GetPart(ControlPositions, 5, 0) & tdDetEnd
            result += trDetEnd

            result += trDetStart
            result += tdTitleStart & "Controls (Tilt)" & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 0, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 1, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 2, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 3, 1) & tdDetEnd
            result += tdDetStart & GetPart(ControlPositions, 4, 1) & tdDetEnd
            result += tdDetRight & GetPart(ControlPositions, 5, 1) & tdDetEnd
            result += trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Pelmet (76mm)" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetWidth
            result+= trDetStart
                result+= tdTitleStart & "Pelmet Length" & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetWidth5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetWidth6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetReturnSize
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Left Length Return" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetReturnSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#PelmetReturnSize2
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Right Length Return" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize21").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize22").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize23").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize24").ToString() & tdDetEnd
                result+= tdDetStart & currentData("PelmetReturnSize25").ToString() & tdDetEnd
                result+= tdDetRight & currentData("PelmetReturnSize26").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Hold Down Brckts" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Cutouts
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Cut Outs" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & "" & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Cut Outs ||-------------------------------------#
            '#CutOut_LeftTop
            result+= trDetStart
                result+= tdTitleStart & "Top Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightTop
            result+= trDetStart
                result+= tdTitleStart & "Top Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightTop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightTop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_LeftBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Left" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_LeftBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_LeftBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CutOut_RightBottom
            result+= trDetStart
                result+= tdTitleStart & "Bottom Right" & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CutOut_RightBottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CutOut_RightBottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#---------------------------------|| Top ||-------------------------------------#
            '#LHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Width" & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top LHS Height" & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Width" & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Top
            result+= trDetStart
                result+= tdTitleStart & "Top RHS Height" & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Top5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Top6").ToString() & tdDetEnd
            result+= trDetEnd

            '#----------------------------------|| Bottom ||----------------------------------#
            '#LHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#LHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom LHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSWidth_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Width" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSWidth_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSWidth_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RHSHeight_Bottom
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Bottom RHS Height" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RHSHeight_Bottom5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RHSHeight_Bottom6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd


            '#Line Blank
            result+= BlankLineEachRow(5)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Ven: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function
    
    Private Shared Function GetPart(list As List(Of String()), controlIndex As Integer, partIndex As Integer) As String
        If controlIndex < 0 OrElse controlIndex >= list.Count Then
            Return "" ' index kontrol tidak valid
        End If
        Dim arr As String() = list(controlIndex)
        If arr Is Nothing Then Return ""
        If partIndex < 0 OrElse partIndex >= arr.Length Then
            Return "" ' index part tidak ada
        End If
        Return arr(partIndex)
    End Function

    
    
    '#------------------------------------------|| Print Detail - Roller Blinds||------------------------------------------#
    Private Shared Function PrintRollerCassette(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim bracketTypes As String() = {
            currentData("BracketType1").ToString(),
            currentData("BracketType2").ToString(),
            currentData("BracketType3").ToString(),
            currentData("BracketType4").ToString(),
            currentData("BracketType5").ToString(),
            currentData("BracketType6").ToString()
        }

        For i As Integer = 0 To bracketTypes.Length - 1
            Select Case bracketTypes(i)
                Case "Headbox & Side Channels"
                    bracketTypes(i) = "Hb & SC"
                Case "Headbox Only"
                    bracketTypes(i) = "Hb Only"
            End Select
        Next

        Dim BracketType1 As String = bracketTypes(0)
        Dim BracketType2 As String = bracketTypes(1)
        Dim BracketType3 As String = bracketTypes(2)
        Dim BracketType4 As String = bracketTypes(3)
        Dim BracketType5 As String = bracketTypes(4)
        Dim BracketType6 As String = bracketTypes(5)

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))


        '#line options
        result+= SubstituteFabric()
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Fabrics
            result+= trDetStart
                result+= tdTitleStart & "Fabric" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#FabricColour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricColour6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#SkinWidth
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Skin Width" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#SkinDrop
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Skin Drop" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts1").ToString()), "0", currentData("NumBoldNuts1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts2").ToString()), "0", currentData("NumBoldNuts2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts3").ToString()), "0", currentData("NumBoldNuts3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts4").ToString()), "0", currentData("NumBoldNuts4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts5").ToString()), "0", currentData("NumBoldNuts5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts6").ToString()), "0", currentData("NumBoldNuts6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Trim
            result+= trDetStart
                result+= tdTitleStart & "Trim" & tdDetEnd
                result+= tdDetStart & currentData("Trim1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Trim6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RollDirection
            result+= trDetStart
                result+= tdTitleStart & "Roll Direction" & tdDetEnd
                result+= tdDetStart & currentData("RollDirection1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RollDirection6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControlType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Control Type" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ControlType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ControlType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ControlType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ControlType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ControlType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("ControlType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControlPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChainColour
            result+= trDetStart
                result+= tdTitleStart & "Chain Colour" & tdDetEnd
                result+= tdDetStart & currentData("ChainColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChainColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChildSafe
            result+= trDetStart
                result+= tdTitleStart & "Delux Child Safe" & tdDetEnd
                result+= tdDetStart & currentData("ChildSafe1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChildSafe2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChildSafe3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChildSafe4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChildSafe5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChildSafe6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CLength
            result+= trDetStart
                result+= tdTitleStart & "Control Length" & tdDetEnd
                result+= tdDetStart & currentData("ChainLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChainLength6").ToString() & tdDetEnd
            result+= trDetEnd

            '#TubeSize
            result+= trDetStart
                result+= tdTitleStart & "Tube Size" & tdDetEnd
                result+= tdDetStart & currentData("TubeSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TubeSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Shape" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BottomType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#BottomColour
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BottomColour6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TubeType
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("TubeType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#BracketType
            result+= trDetStart
                result+= tdTitleStart & "Cassette" & tdDetEnd
                result+= tdDetStart & boldStart & BracketType1 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & BracketType2 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & BracketType3 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & BracketType4 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & BracketType5 & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & BracketType6 & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ColourType
            result+= trDetStart
                result+= tdTitleStart & "Cassette Colour" & tdDetEnd
                result+= tdDetStart & currentData("ColourType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ColourType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "FIXING" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "LOCATION" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd
            
            '#Blank Line
            result+= BlankLineEachRow(8)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                ' result+= "<td style='width:100px;'>" & "<td style='border: 1px solid black;'>|0|0|</td>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd


        Return result
    End Function

    Private Shared Function PrintRollerMotorised(currentData As DataRow) As String
        Dim result As String = String.Empty
       
        Dim ControlType As String() = {
            currentData("ControlType1").ToString(),
            currentData("ControlType2").ToString(),
            currentData("ControlType3").ToString(),
            currentData("ControlType4").ToString(),
            currentData("ControlType5").ToString(),
            currentData("ControlType6").ToString()
        }
        For i As Integer = 0 To ControlType.Length - 1
            Select Case ControlType(i)
                Case "Alpha RTS"
                    ControlType(i) = "AP RTS"
                Case "Alpha WF"
                    ControlType(i) = "AP WF"
                Case "Alpha WS"
                    ControlType(i) = "AP WS"
                Case "Somfy RTS"
                    ControlType(i) = "SF RTS"
                Case "Somfy WF"
                    ControlType(i) = "SF WF"
                Case "Somfy WS"
                    ControlType(i) = "SF WS"
            End Select
        Next

        Dim Motorised1 As String = ControlType(0) & " " & currentData("TubeSize1").ToString() & " (" & currentData("ColourType1").ToString() & ")"
        Dim Motorised2 As String = ControlType(1) & " " & currentData("TubeSize2").ToString() & " (" & currentData("ColourType2").ToString() & ")"
        Dim Motorised3 As String = ControlType(2) & " " & currentData("TubeSize3").ToString() & " (" & currentData("ColourType3").ToString() & ")"
        Dim Motorised4 As String = ControlType(3) & " " & currentData("TubeSize4").ToString() & " (" & currentData("ColourType4").ToString() & ")"
        Dim Motorised5 As String = ControlType(4) & " " & currentData("TubeSize5").ToString() & " (" & currentData("ColourType5").ToString() & ")"
        Dim Motorised6 As String = ControlType(5) & " " & currentData("TubeSize6").ToString() & " (" & currentData("ColourType6").ToString() & ")"

        Dim initBracketType As String() = {
            currentData("BracketType1").ToString(),
            currentData("BracketType2").ToString(),
            currentData("BracketType3").ToString(),
            currentData("BracketType4").ToString(),
            currentData("BracketType5").ToString(),
            currentData("BracketType6").ToString()
        }
        For i As Integer = 0 To initBracketType.Length - 1
            Select Case initBracketType(i)
                Case "Single"
                    initBracketType(i) = ""
                Case "Linked 2 Blinds (Dep)"
                    initBracketType(i) = "L2B1C"
                Case "Linked 2 Blinds (Ind)"
                    initBracketType(i) = "L2B2C"
                Case "Linked 3 Blinds (Dep)"
                    initBracketType(i) = "L3B1C"
                Case "Linked 3 Blinds (Ind)"
                    initBracketType(i) = "L3B2C"
                Case "Double"
                    initBracketType(i) = "D"
                Case "Double and Link System Dep"
                    initBracketType(i) = "DL4B2C"
                Case "Double and Link System Ind"
                    initBracketType(i) = "DL4B4C"
            End Select
        Next

         Dim initTubeType As String() = {
            currentData("TubeType1").ToString(),
            currentData("TubeType2").ToString(),
            currentData("TubeType3").ToString(),
            currentData("TubeType4").ToString(),
            currentData("TubeType5").ToString(),
            currentData("TubeType6").ToString()
        }

         For i As Integer = 0 To initTubeType.Length - 1
            If InStr(initTubeType(i), "JAI") > 0 Then : initTubeType(i) = "MJH" : End If
            If InStr(initTubeType(i), "Acmeda") > 0 Then : initTubeType(i) = "MAC" : End If
            If InStr(initTubeType(i), "LOV") > 0 Then : initTubeType(i) = "MLOV" : End If
        Next

        Dim Bracket1 As String = initTubeType(0) & " " & initBracketType(0)
        Dim Bracket2 As String = initTubeType(1) & " " & initBracketType(1)
        Dim Bracket3 As String = initTubeType(2) & " " & initBracketType(2)
        Dim Bracket4 As String = initTubeType(3) & " " & initBracketType(3)
        Dim Bracket5 As String = initTubeType(4) & " " & initBracketType(4)
        Dim Bracket6 As String = initTubeType(5) & " " & initBracketType(5)

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))


       
        
        '#line options
        result+= SubstituteFabric()
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd
        
            '#Fabrics
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Fabric" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#FabricColour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricColour6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TubeSkinSize
            result+= trDetStart
                result+= tdTitleStart & fs11Start & boldStart &  "Tube & Skin Width" & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & boldEnd & tdDetEnd
            result+= trDetEnd

            '#NumBoldNuts
            result+= trDetStart
                result+= tdTitleStart & "Skin Drop" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts1").ToString()), "0", currentData("NumBoldNuts1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts2").ToString()), "0", currentData("NumBoldNuts2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts3").ToString()), "0", currentData("NumBoldNuts3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts4").ToString()), "0", currentData("NumBoldNuts4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts5").ToString()), "0", currentData("NumBoldNuts5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("NumBoldNuts6").ToString()), "0", currentData("NumBoldNuts6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#TubeSize
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Tube" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeSize5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("TubeSize6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TubeType
            result+= trDetStart
                result+= tdTitleStart & "Control Type" & tdDetEnd
                result+= tdDetStart & currentData("TubeType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TubeType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#MotorStyle
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Motor Style" & boldEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("MotorStyle1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("MotorStyle2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("MotorStyle3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("MotorStyle4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & currentData("MotorStyle5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs11Start & boldStart & currentData("MotorStyle6").ToString() & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#ColourType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Ctrl Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ColourType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ColourType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ColourType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ColourType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("ColourType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("ColourType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#RollDirection
            result+= trDetStart
                result+= tdTitleStart & "Roll Direction" & tdDetEnd
                result+= tdDetStart & currentData("RollDirection1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RollDirection6").ToString() & tdDetEnd
            result+= trDetEnd

            '#MotorRemote
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Remote/Switch" & boldEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs10Start & boldStart & currentData("MotorRemote5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs10Start & boldStart & currentData("MotorRemote6").ToString() & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#MotorCharger
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Charger" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorCharger1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorCharger2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorCharger3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorCharger4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("MotorCharger5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("MotorCharger6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Connector
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Flush Connect" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("Connector1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("Connector2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("Connector3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("Connector4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("Connector5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("Connector6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#ControlPosition
            result+= trDetStart
                result+= tdTitleStart & "Motor Side" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Bracket" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & Bracket1 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & Bracket2 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & Bracket3 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & Bracket4 & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & Bracket5 & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & Bracket6 & boldEnd & tdDetEnd
            result+= trDetEnd

            '#LinkBlind
            result+= trDetStart
                result+= tdTitleStart & "Link Blinds" & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LinkBlind6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Bkt Cover Colour" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Trim
            result+= trDetStart
                result+= tdTitleStart & "Trim" & tdDetEnd
                result+= tdDetStart & currentData("Trim1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Trim6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Shape" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BottomType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#BottomColour
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BottomColour6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Accessory
            result+= trDetStart
                result+= tdTitleStart & "Accessory" & tdDetEnd
                result+= tdDetStart & currentData("Accessory1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Accessory6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Blind Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Blind Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

             '#KitName
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Motorised" & boldEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & Motorised1 & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & Motorised2 & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & Motorised3 & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & Motorised4 & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & boldStart & Motorised5 & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs11Start & boldStart & Motorised6 & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#line Blank
            result += BlankLineEachRow(4)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                ' result+= "<td style='width:100px;'>" & "<td style='border: 1px solid black;'>|0|0|</td>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    Private Shared Function PrintRollerBlind(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))
        
        result+= SubstituteFabric()
        result+= LineOptions(currentData)

        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#FabricType Or Fabric
            result+= trDetStart
                result+= tdTitleStart & "Fabric" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#FabricColour Or Colour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricColour6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TubeSkinSize Or Tube Width
            result+= trDetStart
                result+= tdTitleStart & "Tube Width" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#TubeSkinSize Or Skin Width
            result+= trDetStart
                result+= tdTitleStart & "Skin Width" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize1").ToString()), "0", currentData("TubeSkinSize1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize2").ToString()), "0", currentData("TubeSkinSize2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize3").ToString()), "0", currentData("TubeSkinSize3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize4").ToString()), "0", currentData("TubeSkinSize4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("TubeSkinSize5").ToString()), "0", currentData("TubeSkinSize5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("TubeSkinSize6").ToString()), "0", currentData("TubeSkinSize6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#NumBoldNuts Or Skin Drop
            result+= trDetStart
                result+= tdTitleStart & "Skin Drop" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts1").ToString()), "0", currentData("NumBoldNuts1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts2").ToString()), "0", currentData("NumBoldNuts2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts3").ToString()), "0", currentData("NumBoldNuts3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts4").ToString()), "0", currentData("NumBoldNuts4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("NumBoldNuts5").ToString()), "0", currentData("NumBoldNuts5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("NumBoldNuts6").ToString()), "0", currentData("NumBoldNuts6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Trim Or Trims
            result+= trDetStart
                result+= tdTitleStart & "Trim" & tdDetEnd
                result+= tdDetStart & currentData("Trim1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Trim6").ToString() & tdDetEnd
            result+= trDetEnd

            '#RollDirection
            result+= trDetStart
                result+= tdTitleStart & "Roll Direction" & tdDetEnd
                result+= tdDetStart & currentData("RollDirection1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("RollDirection5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("RollDirection6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControllType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Control Type" & boldEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("TubeType1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("TubeType2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("TubeType3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("TubeType4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("TubeType5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs12Start & boldStart & currentData("TubeType6").ToString() & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

            '#ControllColour
            result+= trDetStart
                result+= tdTitleStart & "Control Colour" & tdDetEnd
                result+= tdDetStart & currentData("ColourType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ColourType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ColourType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControllPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChainColour
            result+= trDetStart
                result+= tdTitleStart & "Chain Colour" & tdDetEnd
                result+= tdDetStart & currentData("ChainColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChainColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChildSafe
            result+= trDetStart
                result+= tdTitleStart & "Delux Child Safe" & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe1").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe2").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe3").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe4").ToString() & fsEnd & tdDetEnd
                result+= tdDetStart & fs11Start & currentData("ChildSafe5").ToString() & fsEnd & tdDetEnd
                result+= tdDetRight & fs11Start & currentData("ChildSafe6").ToString() & fsEnd & tdDetEnd
            result+= trDetEnd

            '#ControlLength
            result+= trDetStart
                result+= tdTitleStart & "Control Length" & tdDetEnd
                result+= tdDetStart & currentData("ChainLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChainLength6").ToString() & tdDetEnd
            result+= trDetEnd

            '#TubeSize
            result+= trDetStart
                result+= tdTitleStart & "Tube Size" & tdDetEnd
                result+= tdDetStart & currentData("TubeSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TubeSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TubeSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomRailShape
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Shape" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BottomType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#BottomRailColour
            result+= trDetStart
                result+= tdTitleStart & boldStart & "BRail Colour" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BottomColour5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BottomColour6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Accessory
            result+= trDetStart
                result+= tdTitleStart & "Accessories" & tdDetEnd
                result+= tdDetStart & currentData("Accessory1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Accessory5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Accessory6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Spring Assist
            result+= trDetStart
                result+= tdTitleStart & "Spring Assist" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#BracketType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Bracket" & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BracketType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BracketType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BracketType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BracketType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("BracketType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("BracketType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#LinkBlind
            result+= trDetStart
                result+= tdTitleStart & "Link Blinds" & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("LinkBlind5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("LinkBlind6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Bkt Cover Colour
            result+= trDetStart
                result+= tdTitleStart & "Bkt Cover Colour" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

             '#Spring Type
            result+= trDetStart
                result+= tdTitleStart & "Spring Type" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "LOCATION" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#line Blank
            result += BlankLineEachRow(1)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                ' result+= "<td style='width:100px;'>" & "<td style='border: 1px solid black;'>|0|0|</td>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    Private Shared Function PrintRollerSkin(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initControlType As String() = {
            currentData("Qty1").ToString(),
            currentData("Qty2").ToString(),
            currentData("Qty3").ToString(),
            currentData("Qty4").ToString(),
            currentData("Qty5").ToString(),
            currentData("Qty6").ToString()
        }
        For i As Integer = 0 To initControlType.Length - 1
            If Not String.IsNullOrEmpty(initControlType(i).ToString()) Then
                initControlType(i) = "Skin Only"
            Else
                initControlType(i) = String.Empty
            End If
        Next

        Dim skinInfo As String = fs10Start & "<b>skin comes with top splin and bottom spline only (pocket if 1P trim) </b>" & fsEnd

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))

        result+= SubstituteFabric()
        result+= LineOptions(currentData)

        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#FabricType Or Fabric
            result+= trDetStart
                result+= tdTitleStart & "Fabric" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricType5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricType6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#FabricColour Or Colour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour1").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour2").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour3").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour4").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & fs11Start & currentData("FabricColour5").ToString() & fsEnd & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & fs11Start & currentData("FabricColour6").ToString() & fsEnd & boldEnd & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Trim Or Trims
            result+= trDetStart
                result+= tdTitleStart & "Trim <br></br>" & skinInfo & tdDetEnd
                result+= tdDetStart & currentData("Trim1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Trim5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Trim6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControllType
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Control Type" & boldEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & initControlType(0) & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & initControlType(1) & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & initControlType(2) & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & initControlType(3) & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & initControlType(4) & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs12Start & boldStart & initControlType(5) & boldEnd & fsEnd & tdDetEnd
            result+= trDetEnd

             '#line Blank
            result += BlankLineEachRow(20)

        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                ' result+= "<td style='width:100px;'>" & "<td style='border: 1px solid black;'>|0|0|</td>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
                result+= tdDetFooterStart & "<span style='border:1px solid black; background-color:#b4b4b4; color:#b4b4b4; font-size:13px;'>|0|0|</span>" & tdDetEnd
            result+= trDetEnd
            '#Total Rollers
            result+= trDetStart
                result+= "<td style='width:100px; padding:5px 0px;'>" & "<span>Total Rollers: </span><span style='color:white;'>------</span><span style='font-weight:bold;'>" & TotalBlind & "</span>" &  tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Tube" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting Fabric" & tdDetEnd
                result+= tdDetFooterStart &  "Sewing" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling, Packing" & tdDetEnd
                result+= tdDetFooterStart &  "QC" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    '#------------------------------------------|| Print Detail - Verishade||------------------------------------------#
    Private Shared Function PrintVerishadeSingle(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initBlindType As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initBlindType.Length - 1
            If Not String.IsNullOrEmpty(initBlindType(i)) And currentData("BlindName").ToString() = "Single" Then
                initBlindType(i) = "Complete Blind"
            End If
        Next

        Dim initBlindName As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initBlindName.Length - 1
            If Not String.IsNullOrEmpty(initBlindName(i)) Then
                initBlindName(i) = currentData("BlindName").ToString()
            End If
        Next

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))
        
        '#Line Option
        result += SubstituteFabric()
        result += LineOptions(currentData)
        
        '#Table Data
        result+= tableDetStart
           '#initBlindType
            result+= trDetStart
                result+= tdTitleStart & "Blind Type" & tdDetEnd
                result+= tdDetStart & initBlindType(0) & tdDetEnd
                result+= tdDetStart & initBlindType(1) & tdDetEnd
                result+= tdDetStart & initBlindType(2) & tdDetEnd
                result+= tdDetStart & initBlindType(3) & tdDetEnd
                result+= tdDetStart & initBlindType(4) & tdDetEnd
                result+= tdDetRight & initBlindType(5) & tdDetEnd
            result+= trDetEnd

            '#initBlindName
            result+= trDetStart
                result+= tdTitleStart & "Type" & tdDetEnd
                result+= tdDetStart & initBlindName(0) & tdDetEnd
                result+= tdDetStart & initBlindName(1) & tdDetEnd
                result+= tdDetStart & initBlindName(2) & tdDetEnd
                result+= tdDetStart & initBlindName(3) & tdDetEnd
                result+= tdDetStart & initBlindName(4) & tdDetEnd
                result+= tdDetRight & initBlindName(5) & tdDetEnd
            result+= trDetEnd

            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Blind Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Blind Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#TrackType
            result+= trDetStart
                result+= tdTitleStart & "Track Type" & tdDetEnd
                result+= tdDetStart & currentData("TrackType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#TrackColour
            result+= trDetStart
                result+= tdTitleStart & "Track Colour" & tdDetEnd
                result+= tdDetStart & currentData("TrackColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Carrier Qty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Spacer (mm)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#StackPosition
            result+= trDetStart
                result+= tdTitleStart & "Staking" & tdDetEnd
                result+= tdDetStart & currentData("StackPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("StackPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#WandColour & WandLength
            result+= trDetStart
                result+= tdTitleStart & fs10Start & "Wand Colour & Size" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("WandColour1").ToString() & " - " & currentData("WandLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour2").ToString() & " - " & currentData("WandLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour3").ToString() & " - " & currentData("WandLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour4").ToString() & " - " & currentData("WandLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour5").ToString() & " - " & currentData("WandLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("WandColour6").ToString() & " - " & currentData("WandLength6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fitting" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Fabrics
            result+= trDetStart
                result+= tdTitleStart & "Fabric" & tdDetEnd
                result+= tdDetStart & currentData("FabricType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#FabricColour
            result+= trDetStart
                result+= tdTitleStart & "Colour" & tdDetEnd
                result+= tdDetStart & currentData("FabricColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Slat Size (mm)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Slat Qty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "End Slats" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Total Slats" & boldEnd & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Tape Colour" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Fabric Qty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Slat Type" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd


            '#blank line
            result += BlankLineEachRow(5)
        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= tdDetFooterStart &  "" & tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd


        Return result
    End Function

    Private Shared Function PrintVerishadeSlat(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initBlindType As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initBlindType.Length - 1
            If Not String.IsNullOrEmpty(initBlindType(i)) And currentData("BlindName").ToString() = "Single" Then
                initBlindType(i) = "Complete Blind"
            End If
        Next

        Dim initBlindName As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initBlindName.Length - 1
            If Not String.IsNullOrEmpty(initBlindName(i)) Then
                initBlindName(i) = currentData("BlindName").ToString()
            End If
        Next

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))
        
        '#Line Option
        result += SubstituteFabric()
        result += LineOptions(currentData)
        
        '#Table Data
        result+= tableDetStart
           '#initBlindType
            result+= trDetStart
                result+= tdTitleStart & "Blind Type" & tdDetEnd
                result+= tdDetStart & initBlindType(0) & tdDetEnd
                result+= tdDetStart & initBlindType(1) & tdDetEnd
                result+= tdDetStart & initBlindType(2) & tdDetEnd
                result+= tdDetStart & initBlindType(3) & tdDetEnd
                result+= tdDetStart & initBlindType(4) & tdDetEnd
                result+= tdDetRight & initBlindType(5) & tdDetEnd
            result+= trDetEnd

            '#initBlindName
            result+= trDetStart
                result+= tdTitleStart & "Type" & tdDetEnd
                result+= tdDetStart & initBlindName(0) & tdDetEnd
                result+= tdDetStart & initBlindName(1) & tdDetEnd
                result+= tdDetStart & initBlindName(2) & tdDetEnd
                result+= tdDetStart & initBlindName(3) & tdDetEnd
                result+= tdDetStart & initBlindName(4) & tdDetEnd
                result+= tdDetRight & initBlindName(5) & tdDetEnd
            result+= trDetEnd

            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd


            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Blind Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Fabrics
            result+= trDetStart
                result+= tdTitleStart & "Fabric" & tdDetEnd
                result+= tdDetStart & currentData("FabricType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#FabricColour
            result+= trDetStart
                result+= tdTitleStart & "Fabric Colour" & tdDetEnd
                result+= tdDetStart & currentData("FabricColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Slat Size (mm)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Slat Qty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "End Slats" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & boldStart & "Total Slats" & boldEnd & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Tape Colour" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Fabric Qty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Slat Type" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd


            '#blank line
            result += BlankLineEachRow(5)
        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= tdDetFooterStart &  "" & tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd


        Return result
    End Function

    Private Shared Function PrintVerishadeTrack(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initBlindType As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initBlindType.Length - 1
            If Not String.IsNullOrEmpty(initBlindType(i)) And currentData("BlindName").ToString() = "Single" Then
                initBlindType(i) = "Complete Blind"
            End If
        Next

        Dim initBlindName As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initBlindName.Length - 1
            If Not String.IsNullOrEmpty(initBlindName(i)) Then
                initBlindName(i) = currentData("BlindName").ToString()
            End If
        Next

        Dim TotalBlind As Integer = If(IsDBNull(currentData("Qty1")), 0, Convert.ToInt32(currentData("Qty1"))) + If(IsDBNull(currentData("Qty2")), 0, Convert.ToInt32(currentData("Qty2"))) + If(IsDBNull(currentData("Qty3")), 0, Convert.ToInt32(currentData("Qty3"))) + If(IsDBNull(currentData("Qty4")), 0, Convert.ToInt32(currentData("Qty4"))) + If(IsDBNull(currentData("Qty5")), 0, Convert.ToInt32(currentData("Qty5"))) + If(IsDBNull(currentData("Qty6")), 0, Convert.ToInt32(currentData("Qty6")))
        
        '#Line Option
        result += SubstituteFabric()
        result += LineOptions(currentData)
        
        '#Table Data
        result+= tableDetStart
           '#initBlindType
            result+= trDetStart
                result+= tdTitleStart & "Blind Type" & tdDetEnd
                result+= tdDetStart & initBlindType(0) & tdDetEnd
                result+= tdDetStart & initBlindType(1) & tdDetEnd
                result+= tdDetStart & initBlindType(2) & tdDetEnd
                result+= tdDetStart & initBlindType(3) & tdDetEnd
                result+= tdDetStart & initBlindType(4) & tdDetEnd
                result+= tdDetRight & initBlindType(5) & tdDetEnd
            result+= trDetEnd

            '#initBlindName
            result+= trDetStart
                result+= tdTitleStart & "Type" & tdDetEnd
                result+= tdDetStart & initBlindName(0) & tdDetEnd
                result+= tdDetStart & initBlindName(1) & tdDetEnd
                result+= tdDetStart & initBlindName(2) & tdDetEnd
                result+= tdDetStart & initBlindName(3) & tdDetEnd
                result+= tdDetStart & initBlindName(4) & tdDetEnd
                result+= tdDetRight & initBlindName(5) & tdDetEnd
            result+= trDetEnd

            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Blind Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Blind Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#TrackType
            result+= trDetStart
                result+= tdTitleStart & "Track Type" & tdDetEnd
                result+= tdDetStart & currentData("TrackType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#TrackColour
            result+= trDetStart
                result+= tdTitleStart & "Track Colour" & tdDetEnd
                result+= tdDetStart & currentData("TrackColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Carrier Qty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Spacer (mm)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#StackPosition
            result+= trDetStart
                result+= tdTitleStart & "Staking" & tdDetEnd
                result+= tdDetStart & currentData("StackPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("StackPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#WandColour & WandLength
            result+= trDetStart
                result+= tdTitleStart & fs10Start & "Wand Colour & Size" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("WandColour1").ToString() & " - " & currentData("WandLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour2").ToString() & " - " & currentData("WandLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour3").ToString() & " - " & currentData("WandLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour4").ToString() & " - " & currentData("WandLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("WandColour5").ToString() & " - " & currentData("WandLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("WandColour6").ToString() & " - " & currentData("WandLength6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fitting" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd



            '#blank line
            result += BlankLineEachRow(5)
        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Total Rollers
            result+= trDetStart
                result+= tdDetFooterStart &  "" & tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd


        Return result
    End Function

    '#------------------------------------------|| Print Detail - Vertical||------------------------------------------#
    Private Shared Function PrintVerticalComplete(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initVenType As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initVenType.Length - 1
            If Not String.IsNullOrEmpty(initVenType(i).ToString()) Then
                initVenType(i) = "Blind"
            Else
                initVenType(i) = String.Empty
            End If
        Next

        Dim initTrackOption As String() = {
            currentData("InsertInTrack1").ToString(),
            currentData("InsertInTrack2").ToString(),
            currentData("InsertInTrack3").ToString(),
            currentData("InsertInTrack4").ToString(),
            currentData("InsertInTrack5").ToString(),
            currentData("InsertInTrack6").ToString()
        }
        For i As Integer = 0 To initTrackOption.Length - 1
            If Not String.IsNullOrEmpty(initTrackOption(i).ToString()) Then
                initTrackOption(i) = "Insert"
            Else
                initTrackOption(i) = "Plain"
            End If
        Next

        Dim initSlover As String() = {
            currentData("Sloper1").ToString(),
            currentData("Sloper2").ToString(),
            currentData("Sloper3").ToString(),
            currentData("Sloper4").ToString(),
            currentData("Sloper5").ToString(),
            currentData("Sloper6").ToString()
        }
        For i As Integer = 0 To initSlover.Length - 1
            If initSlover(i).ToString() = "1" OR initSlover(i).ToString() = "True" Then
                initSlover(i) = "Yes"
            Else
                initSlover(i) = String.Empty
            End If
        Next

        Dim initChainType As String() = {
            currentData("ChainLength1").ToString(),
            currentData("ChainLength2").ToString(),
            currentData("ChainLength3").ToString(),
            currentData("ChainLength4").ToString(),
            currentData("ChainLength5").ToString(),
            currentData("ChainLength6").ToString()
        }
        For i As Integer = 0 To initChainType.Length - 1
            If Not String.IsNullOrEmpty(initChainType(i).ToString()) Then
                initChainType(i) = initChainType(i).ToString() & " + joiner"
            Else
                initChainType(i) = String.Empty
            End If
        Next


        
        '#Line Option
        result += LineOptions(currentData)
        
        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#initVenType
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Vertical Blind Type" & fsEnd & tdDetEnd
                result+= tdDetStart & initVenType(0) & tdDetEnd
                result+= tdDetStart & initVenType(1) & tdDetEnd
                result+= tdDetStart & initVenType(2) & tdDetEnd
                result+= tdDetStart & initVenType(3) & tdDetEnd
                result+= tdDetStart & initVenType(4) & tdDetEnd
                result+= tdDetRight & initVenType(5) & tdDetEnd
            result+= trDetEnd

            '#TubeType
            result+= trDetStart
                result+= tdTitleStart & "Track Type" & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("TubeType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TrackColour
            result+= trDetStart
                result+= tdTitleStart & "Track Colour" & tdDetEnd
                result+= tdDetStart & currentData("TrackColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#SlatSize
            result+= trDetStart
                result+= tdTitleStart & "Slat Size(mm)" & tdDetEnd
                result+= tdDetStart & currentData("SlatSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SlatSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Carries Qty" & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CarrierQty6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Spacer Size (mm)" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("Spacer1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Spacer6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Track Option" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line1").ToString()), "", initTrackOption(0)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line2").ToString()), "", initTrackOption(1)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line3").ToString()), "", initTrackOption(2)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line4").ToString()), "", initTrackOption(3)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line5").ToString()), "", initTrackOption(4)) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Line6").ToString()), "", initTrackOption(5)) & tdDetEnd
            result+= trDetEnd

            '#ControlType
            result+= trDetStart
                result+= tdTitleStart & "Chain/Wand" & tdDetEnd
                result+= tdDetStart & currentData("ControlType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CLength
            result+= trDetStart
                result+= tdTitleStart & "Chain Type" & tdDetEnd
                result+= tdDetStart & initChainType(0) & tdDetEnd
                result+= tdDetStart & initChainType(1) & tdDetEnd
                result+= tdDetStart & initChainType(2) & tdDetEnd
                result+= tdDetStart & initChainType(3) & tdDetEnd
                result+= tdDetStart & initChainType(4) & tdDetEnd
                result+= tdDetRight & initChainType(5) & tdDetEnd
            result+= trDetEnd

            '#ChainColour
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Chain/Wand Colour" & fsEnd & tdDetEnd
                For i As Integer = 1 To 6
                    Dim controlType As String = currentData("ControlType" & i).ToString()
                    Dim colourValue As String = ""

                    If controlType = "Chain" Then
                        colourValue = currentData("ChainColour" & i).ToString()
                    Else IF controlType = "Wand" Then
                        colourValue = currentData("WandColour" & i).ToString()
                    Else
                        colourValue = ""
                    End If

                    ' Gunakan tdDetRight untuk kolom terakhir
                    If i = 6 Then
                        result += tdDetRight & colourValue & tdDetEnd
                    Else
                        result += tdDetStart & colourValue & tdDetEnd
                    End If
                Next
            result+= trDetEnd

            '#StackPosition
            result+= trDetStart
                result+= tdTitleStart & "Stacking" & tdDetEnd
                result+= tdDetStart & currentData("StackPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("StackPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControlPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChainLength
            result+= trDetStart
                result+= tdTitleStart & "Control Length" & tdDetEnd
                For i As Integer = 1 To 6
                    Dim controlType As String = currentData("ControlType" & i).ToString()
                    Dim colourValue As String = ""

                    If controlType = "Chain" Then
                        colourValue = currentData("ChainLength" & i).ToString()
                    Else IF controlType = "Wand" Then
                        colourValue = currentData("WandLength" & i).ToString()
                    Else
                        colourValue = ""
                    End If

                    ' Gunakan tdDetRight untuk kolom terakhir
                    If i = 6 Then
                        result += tdDetRight & colourValue & tdDetEnd
                    Else
                        result += tdDetStart & colourValue & tdDetEnd
                    End If
                Next
            result+= trDetEnd

            '#FabricType
            result+= trDetStart
                result+= tdTitleStart & "Fabric Material" & tdDetEnd
                result+= tdDetStart & currentData("FabricType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#FabricColour
            result+= trDetStart
                result+= tdTitleStart & "Fabric Colour" & tdDetEnd
                result+= tdDetStart & currentData("FabricColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#FabricCutDrop
            result+= trDetStart
                result+= tdTitleStart & "Fabric Qty (M)" & tdDetEnd
                result+= tdDetStart & currentData("FabricCutDrop1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricCutDrop2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricCutDrop3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricCutDrop4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricCutDrop5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricCutDrop6").ToString() & tdDetEnd
            result+= trDetEnd

            '#HangerType
            result+= trDetStart
                result+= tdTitleStart & "Hanger Type" & tdDetEnd
                result+= tdDetStart & currentData("HangerType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("HangerType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & "Bottom" & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketColour
            result+= trDetStart
                result+= tdTitleStart & "Bottom Colour" & tdDetEnd
                result+= tdDetStart & currentData("BracketColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BracketColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketOption
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & currentData("BracketOption1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BracketOption6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Sloper
            result+= trDetStart
                result+= tdTitleStart & "Sloper" & tdDetEnd
                result+= tdDetStart & initSlover(0) & tdDetEnd
                result+= tdDetStart & initSlover(1) & tdDetEnd
                result+= tdDetStart & initSlover(2) & tdDetEnd
                result+= tdDetStart & initSlover(3) & tdDetEnd
                result+= tdDetStart & initSlover(4) & tdDetEnd
                result+= tdDetRight & initSlover(5) & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            result+= BlankLineEachRow(4)


        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd

            '#Total Rollers
            result+= trDetStart
                result+= tdDetFooterStart &  "" & tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    Private Shared Function PrintVerticalSlat(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initVenType As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initVenType.Length - 1
            If Not String.IsNullOrEmpty(initVenType(i).ToString()) Then
                initVenType(i) = "Blind"
            Else
                initVenType(i) = String.Empty
            End If
        Next


        
        '#Line Option
        result += LineOptions(currentData)
        
        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#SlatQty
            result+= trDetStart
                result+= tdTitleStart & "Slat Qty" & tdDetEnd
                result+= tdDetStart & boldStart & currentData("SlatQty1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("SlatQty2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("SlatQty3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("SlatQty4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("SlatQty5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("SlatQty6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

             '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

             '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop Exact (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#FabricType
            result+= trDetStart
                result+= tdTitleStart & "Fabric Material" & tdDetEnd
                result+= tdDetStart & currentData("FabricType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#FabricColour
            result+= trDetStart
                result+= tdTitleStart & "Fabric Colour" & tdDetEnd
                result+= tdDetStart & currentData("FabricColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("FabricColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("FabricColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#SlatSize
            result+= trDetStart
                result+= tdTitleStart & "Slat Size(mm)" & tdDetEnd
                result+= tdDetStart & currentData("SlatSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SlatSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#HangerType
            result+= trDetStart
                result+= tdTitleStart & "Hanger Type" & tdDetEnd
                result+= tdDetStart & currentData("HangerType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("HangerType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Accessory" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

             '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & "Bottom" & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            '#TubeType
            result+= trDetStart
                result+= tdTitleStart & "Track Type" & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("TubeType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            result+= BlankLineEachRow(15)

            
        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            
            '#Total Rollers
            result+= trDetStart
                result+= tdDetFooterStart &  "" & tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    Private Shared Function PrintVerticalTrack(currentData As DataRow) As String
        Dim result As String = String.Empty

        Dim initVenType As String() = {
            currentData("Line1").ToString(),
            currentData("Line2").ToString(),
            currentData("Line3").ToString(),
            currentData("Line4").ToString(),
            currentData("Line5").ToString(),
            currentData("Line6").ToString()
        }
        For i As Integer = 0 To initVenType.Length - 1
            If Not String.IsNullOrEmpty(initVenType(i).ToString()) Then
                initVenType(i) = "Track Only"
            Else
                initVenType(i) = String.Empty
            End If
        Next
        

        Dim initTrackOption As String() = {
            currentData("InsertInTrack1").ToString(),
            currentData("InsertInTrack2").ToString(),
            currentData("InsertInTrack3").ToString(),
            currentData("InsertInTrack4").ToString(),
            currentData("InsertInTrack5").ToString(),
            currentData("InsertInTrack6").ToString()
        }
        For i As Integer = 0 To initTrackOption.Length - 1
            If Not String.IsNullOrEmpty(initTrackOption(i).ToString()) Then
                initTrackOption(i) = "Insert"
            Else
                initTrackOption(i) = "Plain"
            End If
        Next

        
        '#Line Option
        result += LineOptions(currentData)
        
        '#Table Data
        result+= tableDetStart
            '#QTY
            result+= trDetStart
                result+= tdTitleStart & "Qty" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty1").ToString()), "0", currentData("Qty1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty2").ToString()), "0", currentData("Qty2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty3").ToString()), "0", currentData("Qty3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty4").ToString()), "0", currentData("Qty4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Qty5").ToString()), "0", currentData("Qty5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Qty6").ToString()), "0", currentData("Qty6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#initVenType
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Vertical Blind Type" & fsEnd & tdDetEnd
                result+= tdDetStart & initVenType(0) & tdDetEnd
                result+= tdDetStart & initVenType(1) & tdDetEnd
                result+= tdDetStart & initVenType(2) & tdDetEnd
                result+= tdDetStart & initVenType(3) & tdDetEnd
                result+= tdDetStart & initVenType(4) & tdDetEnd
                result+= tdDetRight & initVenType(5) & tdDetEnd
            result+= trDetEnd

            '#TubeType
            result+= trDetStart
                result+= tdTitleStart & "Track Type" & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType1").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType2").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType3").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType4").ToString() & boldEnd & tdDetEnd
                result+= tdDetStart & boldStart & currentData("TubeType5").ToString() & boldEnd & tdDetEnd
                result+= tdDetRight & boldStart & currentData("TubeType6").ToString() & boldEnd & tdDetEnd
            result+= trDetEnd

            '#TrackColour
            result+= trDetStart
                result+= tdTitleStart & "Track Colour" & tdDetEnd
                result+= tdDetStart & currentData("TrackColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("TrackColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("TrackColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#SlatSize
            result+= trDetStart
                result+= tdTitleStart & "Slat Size(mm)" & tdDetEnd
                result+= tdDetStart & currentData("SlatSize1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("SlatSize5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("SlatSize6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Carries Qty" & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CarrierQty5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CarrierQty6").ToString() & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Spacer Size (mm)" & fsEnd & tdDetEnd
                result+= tdDetStart & currentData("Spacer1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Spacer5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Spacer6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Width
            result+= trDetStart
                result+= tdTitleStart & "Width (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width1").ToString()), "0", currentData("Width1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width2").ToString()), "0", currentData("Width2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width3").ToString()), "0", currentData("Width3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width4").ToString()), "0", currentData("Width4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Width5").ToString()), "0", currentData("Width5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Width6").ToString()), "0", currentData("Width6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#Drop
            result+= trDetStart
                result+= tdTitleStart & "Drop (mm)" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop1").ToString()), "0", currentData("Drop1").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop2").ToString()), "0", currentData("Drop2").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop3").ToString()), "0", currentData("Drop3").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop4").ToString()), "0", currentData("Drop4").ToString()) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Drop5").ToString()), "0", currentData("Drop5").ToString()) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Drop6").ToString()), "0", currentData("Drop6").ToString()) & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Track Option" & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line1").ToString()), "", initTrackOption(0)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line2").ToString()), "", initTrackOption(1)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line3").ToString()), "", initTrackOption(2)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line4").ToString()), "", initTrackOption(3)) & tdDetEnd
                result+= tdDetStart & If(String.IsNullOrEmpty(currentData("Line5").ToString()), "", initTrackOption(4)) & tdDetEnd
                result+= tdDetRight & If(String.IsNullOrEmpty(currentData("Line6").ToString()), "", initTrackOption(5)) & tdDetEnd
            result+= trDetEnd

            '#ControlType
            result+= trDetStart
                result+= tdTitleStart & "Chain/Wand" & tdDetEnd
                result+= tdDetStart & currentData("ControlType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlType6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChainColour
            result+= trDetStart
                result+= tdTitleStart & fs11Start & "Chain/Wand Colour" & fsEnd & tdDetEnd 
                result+= tdDetStart & currentData("ChainColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChainColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#CLength
            result+= trDetStart
                result+= tdTitleStart & "Chain Type" & tdDetEnd
                result+= tdDetStart & currentData("CLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CLength6").ToString() & tdDetEnd
            result+= trDetEnd

            '#StackPosition
            result+= trDetStart
                result+= tdTitleStart & "Stacking" & tdDetEnd
                result+= tdDetStart & currentData("StackPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("StackPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("StackPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ControlPosition
            result+= trDetStart
                result+= tdTitleStart & "Control Position" & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ControlPosition5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ControlPosition6").ToString() & tdDetEnd
            result+= trDetEnd

            '#ChainLength
            result+= trDetStart
                result+= tdTitleStart & "Control Length" & tdDetEnd
                result+= tdDetStart & currentData("ChainLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("ChainLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("ChainLength6").ToString() & tdDetEnd
            result+= trDetEnd

             '#HangerType
            result+= trDetStart
                result+= tdTitleStart & "Hanger Type" & tdDetEnd
                result+= tdDetStart & currentData("HangerType1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("HangerType5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("HangerType6").ToString() & tdDetEnd
            result+= trDetEnd

           '#BottomHoldDown
            result+= trDetStart
                result+= tdTitleStart & "Bottom" & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BottomHoldDown5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BottomHoldDown6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketColour
            result+= trDetStart
                result+= tdTitleStart & "Bottom Colour" & tdDetEnd
                result+= tdDetStart & currentData("BracketColour1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketColour5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BracketColour6").ToString() & tdDetEnd
            result+= trDetEnd

            '#BracketOption
            result+= trDetStart
                result+= tdTitleStart & "Bracket" & tdDetEnd
                result+= tdDetStart & currentData("BracketOption1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("BracketOption5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("BracketOption6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Sloper
            result+= trDetStart
                result+= tdTitleStart & "Sloper" & tdDetEnd
                result+= tdDetStart & currentData("Sloper1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Sloper2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Sloper3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Sloper4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Sloper5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Sloper6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Location
            result+= trDetStart
                result+= tdTitleStart & "Location" & tdDetEnd
                result+= tdDetStart & currentData("Location1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Location5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Location6").ToString() & tdDetEnd
            result+= trDetEnd

            '#Mounting
            result+= trDetStart
                result+= tdTitleStart & "Fixing" & tdDetEnd
                result+= tdDetStart & currentData("Mounting1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("Mounting5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("Mounting6").ToString() & tdDetEnd
            result+= trDetEnd

            result+= BlankLineEachRow(2)


        result+= tableDetEnd

        '#Footer
        result+= "<table style='width: 100%; font-size:11px; border-collapse: collapse;'>"
            '#Offcut Fabric Used
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Offcut Fabric Used" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#Recut Made
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "Recut Made" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd
            '#If ys, how  many times
            result+= trDetStart
                result+= "<td style='width:100px;'>" & "If yes, how  many times" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
                result+= tdDetFooterStart & "Yes / No" & tdDetEnd
            result+= trDetEnd

            '#Total Rollers
            result+= trDetStart
                result+= tdDetFooterStart &  "" & tdDetEnd
                result+= tdDetFooterStart &  "Issued By" & tdDetEnd
                result+= tdDetFooterStart &  "Cutting" & tdDetEnd
                result+= tdDetFooterStart &  "Assembling" & tdDetEnd
                result+= tdDetFooterStart &  "Blind Punch" & tdDetEnd
                result+= tdDetFooterStart &  "Finishing" & tdDetEnd
                result+= tdDetFooterStart &  "Packing" & tdDetEnd
            result+= trDetEnd
            '#Page
            result+= trDetStart
                result+= "<td rowspan='2' style='width:100px; padding:5px 2px; text-align:center;'>" &  "<div style='font-size:12px;'>Page </div><div style='padding-top:8px; font-size:12px;'>" & currentData("PageOf").ToString() &" OF "& currentData("AmountOfPage").ToString() & "</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
                result+= tdDetFooterStart &  "<div style='color:white;'>01010</div>" & tdDetEnd
            result+= trDetEnd
            '#Of
            result+= trDetStart
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
                result+= tdDetFooterStart & "<div>________________</div> <div style='padding-top:5px; text-align:left;'>Date:</div> <div style='text-align:left;'>Time:</div>" & tdDetEnd
            result+= trDetEnd
        result+= tableDetEnd

        Return result
    End Function

    


    '#------------------------------------------|| Additional Printing ||------------------------------------------#
    Private Shared Function SubstituteFabric() As String
        Dim result As String = String.Empty
        result+= "<table style='width: 100%; border-top:1px solid black; font-size:11px; border-collapse: collapse;'>"
            result+= "<tr>"
                result+= "<td style='width:100px; padding:5px 0;'>Substitute Fabric:</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
                result+= "<td>Yes/No</td>"
            result+= "</tr>"
        result+= "</table>"
        Return result
    End Function

    Private Shared Function LineOptions(currentData) As String
        Dim result As String = String.Empty
        Dim Line1 As String = "Line 0"
        Dim Line2 As String = "Line 0"
        Dim Line3 As String = "Line 0"
        Dim Line4 As String = "Line 0"
        Dim Line5 As String = "Line 0"
        Dim Line6 As String = "Line 0"
        If Not String.IsNullOrEmpty(currentData("Line1").ToString()) Then Line1 = currentData("Line1").ToString()
        If Not String.IsNullOrEmpty(currentData("Line2").ToString()) Then Line2 = currentData("Line2").ToString()
        If Not String.IsNullOrEmpty(currentData("Line3").ToString()) Then Line3 = currentData("Line3").ToString()
        If Not String.IsNullOrEmpty(currentData("Line4").ToString()) Then Line4 = currentData("Line4").ToString()
        If Not String.IsNullOrEmpty(currentData("Line5").ToString()) Then Line5 = currentData("Line5").ToString()
        If Not String.IsNullOrEmpty(currentData("Line6").ToString()) Then Line6 = currentData("Line6").ToString()
        '#Line Option
        result+= "<table style='width: 100%; border-top:1px solid black; font-size:10px; border-collapse: collapse;'>"
            '#Line Option
            result+= "<tr style='text-align: center;'>"
                result+= "<td style='width:100px;'></td>"
                result+= "<td style='width:100px; padding:5px 0;'>"& Line1 &"</td>"
                result+= "<td width:100px>"& Line2 &"</td>"
                result+= "<td width:100px>"& Line3 &"</td>"
                result+= "<td width:100px>"& Line4 &"</td>"
                result+= "<td width:100px>"& Line5 &"</td>"
                result+= "<td width:100px>"& Line6 &"</td>"
            result+= "</tr>"
        result+= "</table>"
        Return result
    End Function

    Private Shared Function BlankLineEachRow(qty) As String
        Dim result As String = String.Empty
        For i As Integer = 1 To qty
            result+= trDetStart
                result+= tdDetTransStart & "Empty" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd
        Next
        Return result
    End Function

    	






   




End Class
