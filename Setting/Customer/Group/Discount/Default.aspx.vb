
Imports System.Data
Imports System.Globalization

Partial Class Setting_Customer_Group_Discount_Default
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" Then
            Response.Redirect("~/setting/customer/group", False)
            Exit Sub
        End If

        If Session("customerGroupDiscount") = "" Then
            Response.Redirect("~/setting/customer/group", False)
            Exit Sub
        End If

        lblId.Text = Session("customerGroupDiscount")
        If Not IsPostBack Then
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/group", False)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("customerGroupDiscountAdd") = lblId.Text
        Response.Redirect("~/setting/customer/group/discount/add", False)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)

    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)

    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            lblCustomerDiscount.Text = txtIdDelete.Text
            sdsPage.Delete()
            Response.Redirect("~/setting/customer/group/discount", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !!")
                mailCfg.MailError(Page.Title, "btnSubmitDelete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindData(Id As String)
        MessageError(False, String.Empty)
        Session("customerGroupDiscountAdd") = String.Empty
        Try
            Dim custGroupName As String = settingCfg.GetItemData("SELECT Name FROM CustomerGroups WHERE Id='" + Id + "'")
            hTitle.InnerText = "Customer Group : " & custGroupName

            Dim thisString As String = "SELECT * FROM CustomerDiscounts WHERE CustomerData = '" + Id + "' AND Active=1 ORDER BY CustomerBy ASC"

            gvList.DataSource = settingCfg.GetListData(thisString)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "PLEASE CONTACT IT AT REZA@BIGBLINDS.CO.ID !")
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Function TextType(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerDiscounts WHERE Id = '" + Id + "'")

            Dim type As String = thisData.Tables(0).Rows(0).Item("DiscountType").ToString()
            If type = "Product" Then
                Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
                Dim designName As String = settingCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                result = "Product : " & designName
            End If

            If type = "Fabric" Then
                Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
                Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()
                Dim fabricCustom As String = Convert.ToInt32(thisData.Tables(0).Rows(0).Item("CustomFabric"))
                Dim fabricName As String = settingCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")

                result = "Fabric : " & fabricName

                If fabricCustom = 1 Then
                    If Not String.IsNullOrEmpty(fabricColourId) Then
                        Dim ids As String() = fabricColourId.Split(","c).Where(Function(x) Not String.IsNullOrWhiteSpace(x)).ToArray()

                        Dim names As New List(Of String)

                        For Each thisId As String In ids
                            Dim name As String = settingCfg.GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" & thisId.Trim() & "'")
                            names.Add(name)
                        Next
                        Dim colourName As String = String.Join(",", names)
                        result = "Fabric : " & fabricName & " (" & colourName & ")"
                    End If
                End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function ValueDiscount(Data As Decimal) As String
        Dim result As String = String.Empty
        Try
            If Data > 0 Then
                result = Data.ToString("G29", enUS) & "%"
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function FinalDiscount(Type As String, Data As Boolean) As String
        Dim result As String = String.Empty
        Try
            If Type = "Fabric" Then
                result = "No"
                If Data = True Then : result = "Yes" : End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
