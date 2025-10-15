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
                        If Matrix > 0 Then
                            totalCost = Convert.ToDecimal(Matrix) + Convert.ToDecimal(Charge)
                            Cost = "$" & totalCost.ToString("N2", enUS)
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
                Dim latesJoNumber As String = publicCfg.GetItemData("SELECT JoNumber FROM OrderHeaders WHERE Id = '" + headerid + "'")
                Dim latesJobHeader As DataSet = publicCfg.GetListData("SELECT * FROM JobHeaders WHERE HeaderId = '" + headerid + "' AND JoNumber = '" + latesJoNumber + "'")
                If latesJobHeader.Tables(0).Rows.Count < 1 then
                    Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Job data not found."}}
                End If

                JobId = latesJobHeader.Tables(0).Rows(0).Item("Id").ToString()
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
                Dim selectQuery As String = "SELECT * FROM view_details WHERE HeaderId = @HeaderId AND Active = @Active"
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
                    If Not String.IsNullOrEmpty(row("UniqueId").ToString()) Then
                        LinkBlind = "Linked"
                    End If
                    '#--------------------------|| TubeSkinSize ||--------------------------#
                    TubeSkinSize = GetTubeSkinSize(row)

                    '#--------------------------|| NumBoldNuts ||--------------------------#
                    NumBoldNuts = GetNumBoldNuts(row)

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
                    


                    ' === INSERT QUERY ===
                    fields = "JobId, ItemId, HeaderId, LinkBlind, BlindNo, Line, Qty, Location, Mounting, Width, WidthB, WidthMiddle, WidthBottom, [Drop], DropB, DropMiddle, DropRight, SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, PanelSize, NumOfPanel, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, StackPosition, TilterPosition, RollDirection, ControlPosition, ControlColour, ControlLength, ChainLength, MaterialChain, MotorStyle, MotorRemote, MotorRequired, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, TrackType, TrackColour, TrackLength, NumOfWand, WandPosition, WandColour, WandLength, CordColour, CordLength, AcornPlasticColour, Accessory, SideBySide, SlatSize, SlatQty, TubeSize, [Trim], Batten, BattenColour, BracketOption, BracketColour, BracketCover, BracketExtension, Fitting, FlatType, ChildSafe, Cleat, BottomHoldDown, HangerType, PelmetType, PelmetWidth, PelmetSize, PelmetReturn, PelmetReturnPosition, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom, BlindSize, Sloper, InsertInTrack, Notes, KitName, VenetianType, BracketType, TubeType, TubeSkinSize, NumBoldNuts, ControlType, ColourType, DesignName, BlindName, ChainName, ChainColour, CLength, BottomName, BottomType, BottomColour, FabricName, FabricType, FabricColour, FabricWidth, FabricGroups, OrderDelivery, PriceGroupName"

                    values = "@JobId, @ItemId, @HeaderId, @LinkBlind, @BlindNo, @Line, @Qty, @Location, @Mounting, @Width, @WidthB, @WidthMiddle, @WidthBottom, @Drop, @DropB, @DropMiddle, @DropRight, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @Layout, @LayoutSpecial, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @BottomTrackRecess, @Buildout, @BuildoutPosition, @PanelQty, @TrackQty, @PanelSize, @NumOfPanel, @HingeQtyPerPanel, @PanelQtyWithHinge, @LocationTPost1, @LocationTPost2, @LocationTPost3, @LocationTPost4, @LocationTPost5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @DoorCutOut, @SpecialShape, @TemplateProvided, @SquareMetre, @LinearMetre, @StackPosition, @TilterPosition, @RollDirection, @ControlPosition, @ControlColour, @ControlLength, @ChainLength, @MaterialChain, @MotorStyle, @MotorRemote, @MotorRequired, @MotorBattery, @MotorCharger, @Connector, @AdditionalMotor, @CableExitPoint, @TrackType, @TrackColour, @TrackLength, @NumOfWand, @WandPosition, @WandColour, @WandLength, @CordColour, @CordLength, @AcornPlasticColour, @Accessory, @SideBySide, @SlatSize, @SlatQty, @TubeSize, @Trim, @Batten, @BattenColour, @BracketOption, @BracketColour, @BracketCover, @BracketExtension, @Fitting, @FlatType, @ChildSafe, @Cleat, @BottomHoldDown, @HangerType, @PelmetType, @PelmetWidth, @PelmetSize, @PelmetReturn, @PelmetReturnPosition, @PelmetReturnSize, @PelmetReturnSize2, @CutOut_LeftTop, @CutOut_RightTop, @CutOut_LeftBottom, @CutOut_RightBottom, @LHSWidth_Top, @LHSHeight_Top, @RHSWidth_Top, @RHSHeight_Top, @LHSWidth_Bottom, @LHSHeight_Bottom, @RHSWidth_Bottom, @RHSHeight_Bottom, @BlindSize, @Sloper, @InsertInTrack, @Notes, @KitName, @VenetianType, @BracketType, @TubeType, @TubeSkinSize, @NumBoldNuts, @ControlType, @ColourType, @DesignName, @BlindName, @ChainName, @ChainColour, @CLength, @BottomName, @BottomType, @BottomColour, @FabricName, @FabricType, @FabricColour, @FabricWidth, @FabricGroups, @OrderDelivery, @PriceGroupName"

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
        Dim BracketType As String = row("BracketType").ToString()
        Dim TubeType As String = row("TubeType").ToString()
        Dim ControlPosition As String = row("ControlPosition").ToString()
        Dim TubeSize As String = row("TubeSize").ToString()
        Dim Width As Integer = CInt(row("Width").ToString())

        '#-----------------------|| JAI / LOV ||-----------------------#
        If InStr(KitName, "JAI") > 0 Or InStr(KitName, "LOV") > 0 Or InStr(TubeType, "JAI") > 0 Or InStr(TubeType, "LOV") > 0 Then
            '#-----------------------|| Single, Double, Linked, Double and Link ||-----------------------#
            If BracketType = "Single" Or BracketType = "Double" Or InStr(BracketType, "Linked") > 0 Or InStr(BracketType, "Double and Link") > 0 Then
                '#-----------------------|| Left or Right ||-----------------------#
                If ControlPosition = "Left" Or ControlPosition = "Right" Then
                    If TubeSize = "40" Then : result = Width + 28 : End IF
                    If TubeSize = "45" Or TubeSize = "50H" Then : result = Width + 32 : End IF
                End If
            End If
        End If
        '#-----------------------|| Spring System ||-----------------------#
        If InStr(KitName, "Spring System") > 0 Or InStr(TubeType, "Spring") > 0 Then
           '#-----------------------|| Null/Empty, N/A ||-----------------------#
            If String.IsNullOrEmpty(ControlPosition) Or ControlPosition = "N/A" Then
                If TubeSize = "40" Then : result = Width + 28 : End IF
                If TubeSize = "45" Or TubeSize = "50H" Then : result = Width + 32 : End IF
            End If
        End If
        Return result
    End Function

    Private Shared Function GetNumBoldNuts(row As DataRow) As Integer
        Dim result As Integer = 0
        Dim KitName As String = row("KitName").ToString()
        Dim BottomType As String = row("BottomType").ToString()
        Dim TubeSize As String = row("TubeSize").ToString()
        Dim Drop As Integer = CInt(row("Drop").ToString())
        Dim Trim As String = row("Trim").ToString()

        If BottomType = "Oval" Or BottomType = "Round" Or InStr(BottomType, "Flat") > 0 Then
            If Trim = "1F" Then
                If TubeSize = "40" Then : result = Drop + 200 : End IF
                If TubeSize = "43" Or TubeSize = "45" Or TubeSize = "45H" Or TubeSize = "50" Then 
                    result = Drop + 300
                End IF
                If TubeSize = "63" Then : result = Drop + 350 : End IF
            End If
        End If

        If Trim <> "1F" Then
            If TubeSize = "40" Then : result = Drop + 250 : End IF
            If TubeSize = "43" Or TubeSize = "45" Or TubeSize = "45H" Or TubeSize = "50" Then 
                result = Drop + 350
            End IF
            If TubeSize = "63" Then : result = Drop + 400 : End IF
        End If

        If InStr(KitName, "Spring System") > 0 Or InStr(KitName, "Spring") > 0 Then
            If Trim <> "1F" Then
                If TubeSize = "40" Then : result = Drop + 250 : End IF
                If TubeSize = "45" Or TubeSize = "45H" Then : result = Drop + 350 : End IF
            End If
        End If

        Return result
    End Function

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
                    Dim tableName As String = "JobSheets"
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
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "StackPosition", "ControlPosition", "ChainLength", "TrackColour", "SlatSize", "SlatQty", "BracketOption", "BracketColour", "BottomHoldDown", "HangerType", "Sloper", "InsertInTrack", "Notes", "KitName", "TubeType", "ControlType", "ChainName", "ChainColour", "CLength", "FabricName", "FabricType", "FabricColour", "FabricWidth"})

                                tableName = "JobSheet_Verticals"

                            Case "Aluminium Blinds"
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "ControlPosition", "WandLength", "BracketOption", "BottomHoldDown", "CutOut_LeftTop", "CutOut_RightTop", "CutOut_LeftBottom", "CutOut_RightBottom", "LHSWidth_Top", "LHSHeight_Top", "RHSWidth_Top", "RHSHeight_Top", "LHSWidth_Bottom", "LHSHeight_Bottom", "RHSWidth_Bottom", "RHSHeight_Bottom", "Notes", "KitName", "VenetianType", "ColourType"})

                                tableName = "JobSheet_Aluminium"

                            Case "Venetian Blinds"
                                fieldsToProcess.AddRange({"Line", "Qty", "Location", "Mounting", "Width", "Drop", "ControlPosition", "ControlLength", "WandLength", "BracketOption", "BottomHoldDown", "PelmetType", "PelmetWidth", "PelmetSize", "PelmetReturn", "PelmetReturnPosition", "PelmetReturnSize", "PelmetReturnSize2", "CutOut_LeftTop", "CutOut_RightTop", "CutOut_LeftBottom", "CutOut_RightBottom", "LHSWidth_Top", "LHSHeight_Top", "RHSWidth_Top", "RHSHeight_Top", "LHSWidth_Bottom", "LHSHeight_Bottom", "RHSWidth_Bottom", "RHSHeight_Bottom", "Notes", "KitName", "VenetianType", "ControlType", "ColourType"})

                                tableName = "JobSheet_Venetian"
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
        If Not String.IsNullOrEmpty(Notes1) Or Not String.IsNullOrEmpty(Notes2) Or Not String.IsNullOrEmpty(Notes3) Or Not String.IsNullOrEmpty(Notes4) Or Not String.IsNullOrEmpty(Notes5) Or Not String.IsNullOrEmpty(Notes6) Then hightColumnNotes = ""
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
                result+= "<th style=' text-align: left; width: 80px; font-size: 15px; padding-bottom: 5px;'>Job No</th>"
                result+= "<th style=' text-align: left; font-size: 15px; padding-bottom: 5px;'>: "& currentData("JoNumber").ToString() &"</th>"
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

            

            '#
            result+= trDetStart
                result+= tdTitleStart & "Controls (lift)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Controls (Tilt)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

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

            

            '#
            result+= trDetStart
                result+= tdTitleStart & "Controls (lift)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Controls (Tilt)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

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
                result+= tdDetStart & currentData("CLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CLength6").ToString() & tdDetEnd
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
                result+= tdDetStart & fs12Start & boldStart & currentData("ControlType1").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("ControlType2").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("ControlType3").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("ControlType4").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetStart & fs12Start & boldStart & currentData("ControlType5").ToString() & boldEnd & fsEnd & tdDetEnd
                result+= tdDetRight & fs12Start & boldStart & currentData("ControlType6").ToString() & boldEnd & fsEnd & tdDetEnd
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
            result += BlankLineEachRow(3)

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
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Spacer Size (mm)" & fsEnd & tdDetEnd
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

            '#
            result+= trDetStart
                result+= tdTitleStart & "Track Option" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
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
                result+= tdDetStart & currentData("CLength1").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength2").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength3").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength4").ToString() & tdDetEnd
                result+= tdDetStart & currentData("CLength5").ToString() & tdDetEnd
                result+= tdDetRight & currentData("CLength6").ToString() & tdDetEnd
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

            '#
            result+= trDetStart
                result+= tdTitleStart & "Fabric Qty (M)" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
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

            '#
            result+= trDetStart
                result+= tdTitleStart & "Left Hand Return" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & "Right Hand Return" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
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
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
            result+= trDetEnd

            '#
            result+= trDetStart
                result+= tdTitleStart & fs12Start & "Spacer Size (mm)" & fsEnd & tdDetEnd
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

            '#
            result+= trDetStart
                result+= tdTitleStart & "Track Option" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetStart & "" & tdDetEnd
                result+= tdDetRight & "" & tdDetEnd
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
