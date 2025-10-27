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
Partial Class Methods_Setting_Customer_Detail
    Inherits System.Web.UI.Page

    Shared publicCfg As New PublicConfig()

    Public Class ServerSideParams
        Public Property rolename As String
        ' Public Property designid As String
        ' Public Property blindid As String
        
        '# Parameter DataTables untuk server-side processing
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

    Public Class DataTableResponse
        Public Property draw As Integer
        Public Property recordsTotal As Integer
        Public Property recordsFiltered As Integer
        Public Property data As List(Of ServerSideReturnRow)
    End Class

    Public Class ServerSideReturnRow
        Public Property No As String 
        Public Property Id As String 
        Public Property ExactId As String
        Public Property Name As String
        Public Property CustomerGroup As String 
        Public Property CustomerCashSale As String 
        Public Property CustomerOnStop As String
        Public Property CustomerMinSurcharge As String
        Public Property DataActive As String
    End Class

    ' Public Class FormData
    '     Public Property id As String
    '     Public Property soeid As String
    '     Public Property name As String
    '     Public Property designtype As String
    '     Public Property blindtype As String
    '     Public Property bracket As String
    '     Public Property tube As String
    '     Public Property control As String
    '     Public Property colour As String
    '     Public Property des As String
    '     Public Property active As String
    ' End Class

    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessResponse
        Public Property success As String
    End Class

End Class
