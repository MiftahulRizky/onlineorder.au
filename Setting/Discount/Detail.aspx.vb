Imports System.Data
Partial Class Setting_Discount_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/blind", False)
            Exit Sub
        End If

        If Session("discountDetail") = "" Then
            Response.Redirect("~/setting/discount", False)
            Exit Sub
        End If

        lblId.Text = UCase(Session("discountDetail")).ToString()
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtDiscount.Text = "" Then
                Call MessageError(True, "")
                txtDiscount.BackColor = Drawing.Color.Red
                txtDiscount.Focus()
                Exit Sub
            End If

            If Not IsNumeric(txtDiscount.Text) Then
                Call MessageError(True, "")
                txtDiscount.BackColor = Drawing.Color.Red
                txtDiscount.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW DISCOUNT. STORE ID : " & ddlStoreId.SelectedValue)

                Response.Redirect("~/setting/discount", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/discount", False)
    End Sub

    Private Sub BindData(Id As String)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Discounts WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/discount", False)
                Exit Sub
            End If

            Call BindStore()
            Call BindPriceGroup()

            ddlStoreId.SelectedValue = myData.Tables(0).Rows(0).Item("StoreId").ToString() : ddlStoreId.Enabled = False
            ddlPriceGroupId.SelectedValue = myData.Tables(0).Rows(0).Item("PriceGroupId").ToString() : ddlPriceGroupId.Enabled = False
            txtDiscount.Text = myData.Tables(0).Rows(0).Item("Discount").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = myData.Tables(0).Rows(0).Item("Active").ToString()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindStore()
        ddlStoreId.Items.Clear()
        Try
            ddlStoreId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Stores WHERE Active=1 ORDER BY Name ASC")
            ddlStoreId.DataTextField = "NameText"
            ddlStoreId.DataValueField = "Id"
            ddlStoreId.DataBind()

            If ddlStoreId.Items.Count > 0 Then
                ddlStoreId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindPriceGroup()
        ddlPriceGroupId.Items.Clear()
        Try
            ddlPriceGroupId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Designs.Name) + ', ' + UPPER(PricesGroup.Name) AS NameText FROM PricesGroup INNER JOIN Designs ON PricesGroup.DesignId = Designs.Id ORDER BY Designs.Name, PricesGroup.Name ASC")
            ddlPriceGroupId.DataTextField = "NameText"
            ddlPriceGroupId.DataValueField = "Id"
            ddlPriceGroupId.DataBind()

            If ddlPriceGroupId.Items.Count > 0 Then
                ddlPriceGroupId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlStoreId.BackColor = Drawing.Color.Empty
        ddlPriceGroupId.BackColor = Drawing.Color.Empty
        txtDiscount.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
